using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExpressiveSharp.Extensions;

namespace ExpressiveSharp.Services
{
    public sealed class ExpressiveResolver : IExpressiveResolver
    {
        // We never store null in the dictionary; assemblies without a registry use a sentinel delegate.
        private readonly static Func<MemberInfo, LambdaExpression> _nullRegistry = static _ => null!;
        private readonly static ConcurrentDictionary<Assembly, Func<MemberInfo, LambdaExpression>> _assemblyRegistries = new();

        /// <summary>
        /// Caches the fully-resolved <see cref="LambdaExpression"/> per <see cref="MemberInfo"/> so that
        /// EF Core never repeats reflection work for the same member across queries.
        /// </summary>
        private readonly static ConcurrentDictionary<MemberInfo, LambdaExpression> _expressionCache = new();

        /// <summary>
        /// Caches <see cref="Type"/> → C#-formatted name strings, since the same parameter types
        /// appear repeatedly across different expressive members.
        /// </summary>
        private readonly static ConditionalWeakTable<Type, string> _typeNameCache = new();

        /// <summary>
        /// O(1) hash-table lookup replacing the original 16 sequential <c>if</c> checks.
        /// Rearranging the entries has no effect on lookup cost (hash-based), but the most common
        /// EF Core types (<c>int</c>, <c>string</c>, <c>bool</c>) are listed first for readability.
        /// </summary>
        private readonly static Dictionary<Type, string> _csharpKeywords = new(16)
        {
            [typeof(int)]     = "int",
            [typeof(string)]  = "string",
            [typeof(bool)]    = "bool",
            [typeof(long)]    = "long",
            [typeof(double)]  = "double",
            [typeof(decimal)] = "decimal",
            [typeof(float)]   = "float",
            [typeof(byte)]    = "byte",
            [typeof(sbyte)]   = "sbyte",
            [typeof(char)]    = "char",
            [typeof(uint)]    = "uint",
            [typeof(ulong)]   = "ulong",
            [typeof(short)]   = "short",
            [typeof(ushort)]  = "ushort",
            [typeof(object)]  = "object",
        };

        /// <summary>
        /// Looks up the generated <c>ExpressionRegistry</c> class in an assembly (once, then caches it).
        /// Returns a delegate that calls <c>TryGet(MemberInfo)</c> on the registry, or null if the registry
        /// is not present in that assembly (e.g. if the source generator was not run against it).
        /// </summary>
        private static Func<MemberInfo, LambdaExpression>? GetAssemblyRegistry(Assembly assembly)
        {
            var registry = _assemblyRegistries.GetOrAdd(assembly, static asm =>
            {
                var registryType = asm.GetType("ExpressiveSharp.Generated.ExpressionRegistry");
                var tryGetMethod = registryType?.GetMethod("TryGet", BindingFlags.Static | BindingFlags.Public);

                if (tryGetMethod is null)
                {
                    // Use sentinel to indicate "no registry for this assembly"
                    return _nullRegistry;
                }

                return (Func<MemberInfo, LambdaExpression>)Delegate.CreateDelegate(typeof(Func<MemberInfo, LambdaExpression>), tryGetMethod);
            });

            // Translate sentinel back to null for callers, preserving existing behavior.
            return ReferenceEquals(registry, _nullRegistry) ? null : registry;
        }

        public LambdaExpression FindGeneratedExpression(MemberInfo expressiveMemberInfo,
            ExpressiveAttribute? expressiveAttribute = null)
            => _expressionCache.GetOrAdd(expressiveMemberInfo, static (mi, _) => ResolveExpressionCore(mi),
                (object?)null);

        /// <inheritdoc/>
        public LambdaExpression? FindExternalExpression(MemberInfo memberInfo)
        {
            // Ensure all loaded assemblies with registries have been discovered.
            // This handles the edge case where only [ExpressiveFor] is used (no [Expressive] members)
            // and no assembly registry has been lazily loaded yet.
            EnsureAllRegistriesLoaded();

            LambdaExpression? found = null;
            Assembly? foundAssembly = null;

            foreach (var kvp in _assemblyRegistries)
            {
                if (ReferenceEquals(kvp.Value, _nullRegistry))
                    continue;

                var result = kvp.Value(memberInfo);
                if (result is null)
                    continue;

                if (found is not null)
                    throw new InvalidOperationException(
                        $"Multiple [ExpressiveFor] mappings found for '{memberInfo}' " +
                        $"in assemblies '{foundAssembly!.GetName().Name}' and '{kvp.Key.GetName().Name}'.");

                found = result;
                foundAssembly = kvp.Key;
            }

            return found;
        }

        private static volatile bool _allRegistriesScanned;
        private static readonly object _scanLock = new();

        /// <summary>
        /// Scans all loaded assemblies once to discover expression registries.
        /// This is a one-time cost on the first <see cref="FindExternalExpression"/> call.
        /// </summary>
        private static void EnsureAllRegistriesLoaded()
        {
            if (_allRegistriesScanned) return;

            lock (_scanLock)
            {
                if (_allRegistriesScanned) return;

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.IsDynamic)
                        continue;

                    GetAssemblyRegistry(assembly);
                }

                _allRegistriesScanned = true;
            }
        }

        /// <summary>
        /// Ensures the registry for the given assembly is loaded into <see cref="_assemblyRegistries"/>.
        /// Call this for assemblies that may contain [ExpressiveFor] stubs but no [Expressive] members
        /// (which would otherwise trigger lazy loading).
        /// </summary>
        public static void EnsureRegistryLoaded(Assembly assembly)
        {
            GetAssemblyRegistry(assembly);
        }

        private static LambdaExpression ResolveExpressionCore(MemberInfo expressiveMemberInfo)
        {
            var expression = GetExpressionFromGeneratedType(expressiveMemberInfo);

            if (expression is not null)
            {
                return expression;
            }

            var declaringType = expressiveMemberInfo.DeclaringType ?? throw new InvalidOperationException("Expected a valid type here");
            var nestedPath = declaringType.GetNestedTypePath();
            var ns = declaringType.Namespace;
            var offset = ns is not null ? 1 : 0;
            var parts = new string[nestedPath.Length + offset + 1];
            if (ns is not null)
                parts[0] = ns;
            for (var i = 0; i < nestedPath.Length; i++)
                parts[i + offset] = nestedPath[i].Name;
            parts[^1] = expressiveMemberInfo.Name;
            var fullName = string.Join(".", parts);

            throw new InvalidOperationException($"Unable to resolve generated expression for {fullName}.");
        }

        private static LambdaExpression? GetExpressionFromGeneratedType(MemberInfo expressiveMemberInfo)
        {
            var declaringType = expressiveMemberInfo.DeclaringType ?? throw new InvalidOperationException("Expected a valid type here");

            // Fast path: check the per-assembly static registry (generated by source generator).
            // The first call per assembly does a reflection lookup to find the registry class and
            // caches it as a delegate; subsequent calls use the cached delegate for an O(1) dictionary lookup.
            var registry = GetAssemblyRegistry(declaringType.Assembly);
            var registeredExpr = registry?.Invoke(expressiveMemberInfo);

            return registeredExpr ??
                   // Slow path: reflection fallback for open-generic class members and generic methods
                   // that are not yet in the registry.
                   FindGeneratedExpressionViaReflection(expressiveMemberInfo);
        }

        /// <summary>
        /// Sentinel stored in <see cref="_reflectionCache"/> to represent
        /// "no generated type found for this member", distinguishing it from a not-yet-populated entry.
        /// <see cref="ConcurrentDictionary{TKey,TValue}"/> does not allow null values, so a sentinel is required.
        /// </summary>
        private readonly static LambdaExpression _reflectionNullSentinel =
            Expression.Lambda(Expression.Empty());

        /// <summary>
        /// Caches the fully-resolved <see cref="LambdaExpression"/> per <see cref="MemberInfo"/>
        /// for the reflection-based slow path.
        /// On the first call per member the reflection work (<c>Assembly.GetType</c>, <c>GetMethod</c>,
        /// <c>MakeGenericType</c>, <c>MakeGenericMethod</c>) is performed once and the resulting
        /// expression tree is stored here; subsequent calls return the cached reference directly,
        /// eliminating expression-tree re-construction on every access.
        /// This is especially important for constructors whose object-initializer trees are
        /// significantly more expensive to build than simple method-body trees.
        /// </summary>
        private readonly static ConcurrentDictionary<MemberInfo, LambdaExpression> _reflectionCache = new();

        /// <summary>
        /// Resolves the <see cref="LambdaExpression"/> for an <c>[Expressive]</c> member using the
        /// reflection-based slow path only, bypassing the static registry.
        /// The result is cached after the first call, so subsequent calls return the cached expression
        /// without any reflection or expression-tree construction overhead.
        /// Useful for members not yet in the registry (e.g. open-generic types).
        /// </summary>
        public static LambdaExpression? FindGeneratedExpressionViaReflection(MemberInfo expressiveMemberInfo)
        {
            var result = _reflectionCache.GetOrAdd(expressiveMemberInfo,
                static mi => BuildReflectionExpression(mi) ?? _reflectionNullSentinel);
            return ReferenceEquals(result, _reflectionNullSentinel) ? null : result;
        }

        /// <summary>
        /// Performs the one-time reflection work for a member: locates the generated expression
        /// accessor (inline or external-class path), invokes it, and returns the resulting
        /// <see cref="LambdaExpression"/>. Returns <c>null</c> if no generated type is found.
        /// <para>
        /// Using <c>MethodInfo.Invoke</c> rather than a compiled delegate is appropriate here because
        /// the result is cached in <see cref="_reflectionCache"/> — the invocation cost is paid only
        /// on cache misses, and subsequent EF Core queries reuse the cached expression. Under
        /// contention the value factory may be invoked more than once, but only a single expression
        /// instance is ultimately stored per member.
        /// </para>
        /// </summary>
        private static LambdaExpression? BuildReflectionExpression(MemberInfo expressiveMemberInfo)
        {
            var declaringType = expressiveMemberInfo.DeclaringType
                ?? throw new InvalidOperationException("Expected a valid type here");

            var originalDeclaringType = declaringType;

            // For generic types, use the generic type definition to match the generated name.
            if (declaringType.IsGenericType && !declaringType.IsGenericTypeDefinition)
            {
                declaringType = declaringType.GetGenericTypeDefinition();
            }

            // Build parameter type name array with a plain for-loop — avoids IEnumerator + delegate allocations.
            string[]? parameterTypeNames = null;
            var memberLookupName = expressiveMemberInfo.Name;

            if (expressiveMemberInfo is MethodInfo method)
            {
                // For generic methods, use the generic definition so type parameters (TEntity, etc.)
                // are used instead of the concrete closed-generic arguments.
                var methodToInspect = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
                var parameters = methodToInspect.GetParameters();

                if (parameters.Length > 0)
                {
                    parameterTypeNames = new string[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        parameterTypeNames[i] = GetFullTypeName(parameters[i].ParameterType);
                    }
                }
            }
            else if (expressiveMemberInfo is ConstructorInfo ctor)
            {
                // Constructors are stored under the synthetic name "_ctor".
                memberLookupName = "_ctor";
                var parameters = ctor.GetParameters();

                if (parameters.Length > 0)
                {
                    parameterTypeNames = new string[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        parameterTypeNames[i] = GetFullTypeName(parameters[i].ParameterType);
                    }
                }
            }

            // Build the generated class name (without member) and method suffix separately.
            var nestedTypeNames = NestedTypePathToNames(declaringType.GetNestedTypePath());
            var generatedContainingTypeName = ExpressionClassNameGenerator.GenerateClassFullName(
                declaringType.Namespace,
                nestedTypeNames);

            var methodSuffix = ExpressionClassNameGenerator.GenerateMethodSuffix(
                memberLookupName,
                parameterTypeNames);
            var expressionMethodName = methodSuffix + "_Expression";

            var expressionFactoryType = declaringType.Assembly.GetType(generatedContainingTypeName);

            if (expressionFactoryType is null)
            {
                return null;
            }

            if (expressionFactoryType.IsGenericTypeDefinition)
            {
                expressionFactoryType = expressionFactoryType.MakeGenericType(originalDeclaringType.GenericTypeArguments);
            }

            var expressionFactoryMethod = expressionFactoryType.GetMethod(expressionMethodName, BindingFlags.Static | BindingFlags.NonPublic);

            if (expressionFactoryMethod is null)
            {
                return null;
            }

            if (expressiveMemberInfo is MethodInfo mi && mi.GetGenericArguments() is { Length: > 0 } methodGenericArgs)
            {
                expressionFactoryMethod = expressionFactoryMethod.MakeGenericMethod(methodGenericArgs);
            }

            var expression = expressionFactoryMethod.Invoke(null, null) as LambdaExpression;
            if (expression is null)
                return null;

            // Apply declared transformers from the generated class (if any)
            var transformerMethodName = methodSuffix + "_Transformers";
            var transformersMethod = expressionFactoryType.GetMethod(transformerMethodName, BindingFlags.Static | BindingFlags.NonPublic);
            if (transformersMethod?.Invoke(null, null) is IExpressionTreeTransformer[] transformers)
            {
                Expression result = expression;
                foreach (var transformer in transformers)
                {
                    result = transformer.Transform(result);
                }
                return result as LambdaExpression ?? expression;
            }

            return expression;
        }

        /// <summary>
        /// Projects an array of <see cref="Type"/> objects — in practice always the
        /// <c>Type[]</c> returned by <see cref="ExpressiveSharp.Extensions.TypeExtensions.GetNestedTypePath"/> — to a <c>string[]</c>
        /// of simple type names without allocating a LINQ enumerator or intermediate delegate.
        /// </summary>
        private static string[] NestedTypePathToNames(Type[] types)
        {
            var names = new string[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                names[i] = types[i].Name;
            }

            return names;
        }

        /// <summary>
        /// Returns the C#-formatted full name of <paramref name="type"/>.
        /// Results are memoised in <see cref="_typeNameCache"/>; the same <see cref="Type"/> object
        /// is encountered repeatedly across expressive members (e.g. <c>int</c>, <c>string</c>).
        /// </summary>
        private static string GetFullTypeName(Type type)
            => _typeNameCache.GetValue(type, static t => ComputeFullTypeName(t));

        private static string ComputeFullTypeName(Type type)
        {
            // Handle generic type parameters (e.g., T, TEntity)
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            // Handle nullable value types (e.g., int? -> int?)
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return $"{GetFullTypeName(underlyingType)}?";
            }

            // Handle array types
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType == null)
                {
                    // Fallback for edge cases where GetElementType() might return null
                    return type.Name;
                }

                var rank = type.GetArrayRank();
                var elementTypeName = GetFullTypeName(elementType);

                if (rank == 1)
                {
                    return $"{elementTypeName}[]";
                }
                else
                {
                    var commas = new string(',', rank - 1);
                    return $"{elementTypeName}[{commas}]";
                }
            }

            // Map primitive types to their C# keyword equivalents to match Roslyn's output
            var typeKeyword = GetCSharpKeyword(type);
            if (typeKeyword != null)
            {
                return typeKeyword;
            }

            // For generic types, construct the full name matching Roslyn's format
            if (type.IsGenericType)
            {
                var genericTypeDef = type.GetGenericTypeDefinition();
                var genericArgs = type.GetGenericArguments();
                var baseName = genericTypeDef.FullName ?? genericTypeDef.Name;

                // Remove the `n suffix (e.g., `1, `2)
                var backtickIndex = baseName.IndexOf('`');
                if (backtickIndex > 0)
                {
                    baseName = baseName.Substring(0, backtickIndex);
                }

                var args = string.Join(", ", genericArgs.Select(GetFullTypeName));
                return $"{baseName}<{args}>";
            }

            if (type.FullName != null)
            {
                // Replace + with . for nested types to match Roslyn's format
                return type.FullName.Replace('+', '.');
            }

            return type.Name;
        }

        /// <summary>
        /// O(1) dictionary lookup — replaces the original 16 sequential <c>if</c> checks.
        /// Note: reordering the entries in <see cref="_csharpKeywords"/> has <em>no effect</em> on
        /// performance because <see cref="Dictionary{TKey,TValue}"/> uses hashing, not linear scan.
        /// (Reordering only mattered with the old <c>if</c>-chain, where placing <c>int</c> / <c>string</c>
        /// / <c>bool</c> first would have reduced average comparisons from ~8 to ~1.)
        /// </summary>
        private static string? GetCSharpKeyword(Type type) => _csharpKeywords.GetValueOrDefault(type);
    }
}
