using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExpressiveSharp.Diagnostics;
using ExpressiveSharp.Extensions;

namespace ExpressiveSharp.Services
{
    public sealed class ExpressiveResolver : IExpressiveResolver
    {
        // Sentinel delegate for assemblies without a registry; ConcurrentDictionary forbids null values.
        private readonly static Func<MemberInfo, LambdaExpression> _nullRegistry = static _ => null!;
        private readonly static ConcurrentDictionary<Assembly, Func<MemberInfo, LambdaExpression>> _assemblyRegistries = new();

        private readonly static ConcurrentDictionary<MemberInfo, LambdaExpression> _expressionCache = new();

        /// <summary>
        /// Clears all process-level caches. For test harnesses and the docs prerenderer, where many
        /// short-lived snippet assemblies are loaded in sequence.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal static void ResetAllCaches()
        {
            _assemblyRegistries.Clear();
            _expressionCache.Clear();
            _reflectionCache.Clear();
            Volatile.Write(ref _lastScannedAssemblyCount, 0);
            _assemblyScanFilter = null;
        }

        /// <summary>
        /// Invalidates cached expression trees on hot-reload. Preserves <c>_assemblyScanFilter</c>
        /// (wiping it would silently disable a user-configured restriction) and <c>_typeNameCache</c>
        /// (does not go stale on non-rude edits).
        /// </summary>
        internal static void ClearCachesForMetadataUpdate()
        {
            _expressionCache.Clear();
            _reflectionCache.Clear();
            _assemblyRegistries.Clear();
            Volatile.Write(ref _lastScannedAssemblyCount, 0);
        }

        internal static bool IsExpressionCached(MemberInfo mi) => _expressionCache.ContainsKey(mi);

        internal static Func<Assembly, bool>? GetAssemblyScanFilter() => _assemblyScanFilter;

        private static Func<Assembly, bool>? _assemblyScanFilter;

        /// <summary>
        /// Restricts <see cref="EnsureAllRegistriesLoaded"/> to assemblies matching the given filter.
        /// Pass <c>null</c> to remove the filter.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        internal static void SetAssemblyScanFilter(Func<Assembly, bool>? filter)
        {
            _assemblyScanFilter = filter;
            Volatile.Write(ref _lastScannedAssemblyCount, 0);
        }

        private readonly static ConditionalWeakTable<Type, string> _typeNameCache = new();

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
        /// Returns a delegate calling <c>TryGet(MemberInfo)</c> on the assembly's generated
        /// <c>ExpressionRegistry</c>, or null if the source generator did not run for this assembly.
        /// </summary>
        private static Func<MemberInfo, LambdaExpression>? GetAssemblyRegistry(Assembly assembly)
        {
            var registry = _assemblyRegistries.GetOrAdd(assembly, static asm =>
            {
                var registryType = asm.GetType("ExpressiveSharp.Generated.ExpressionRegistry");
                var tryGetMethod = registryType?.GetMethod("TryGet", BindingFlags.Static | BindingFlags.Public);

                if (tryGetMethod is null)
                    return _nullRegistry;

                return (Func<MemberInfo, LambdaExpression>)Delegate.CreateDelegate(typeof(Func<MemberInfo, LambdaExpression>), tryGetMethod);
            });

            return ReferenceEquals(registry, _nullRegistry) ? null : registry;
        }

        public LambdaExpression FindGeneratedExpression(MemberInfo expressiveMemberInfo,
            ExpressiveAttribute? expressiveAttribute = null)
        {
            if (ExpressiveDiagnostics.CacheHits.Enabled || ExpressiveDiagnostics.CacheMisses.Enabled)
            {
                if (_expressionCache.TryGetValue(expressiveMemberInfo, out var cached))
                {
                    ExpressiveDiagnostics.CacheHits.Add(1);
                    return cached;
                }
                ExpressiveDiagnostics.CacheMisses.Add(1);
            }

            return _expressionCache.GetOrAdd(expressiveMemberInfo,
                static (mi, _) => ResolveExpressionCore(mi), (object?)null);
        }

        /// <inheritdoc/>
        public LambdaExpression? FindExternalExpression(MemberInfo memberInfo)
        {
            // Handles the case where only [ExpressiveFor] is used (no [Expressive] members)
            // and no assembly registry has been lazily loaded yet.
            EnsureAllRegistriesLoaded();

            LambdaExpression? found = null;
            Assembly? foundAssembly = null;

            foreach (var kvp in _assemblyRegistries)
            {
                if (ReferenceEquals(kvp.Value, _nullRegistry))
                    continue;

                LambdaExpression? result;
                try
                {
                    result = kvp.Value(memberInfo);
                }
                catch (TypeInitializationException ex)
                {
                    // Registry's static ctor failed — mark inert so we don't re-throw on every lookup.
                    ExpressiveEventSource.Log.RegistryInitializationFailed(kvp.Key, ex);
                    _assemblyRegistries[kvp.Key] = _nullRegistry;
                    continue;
                }

                if (result is null)
                    continue;

                if (found is not null)
                {
                    ExpressiveEventSource.Log.MultipleExpressiveForMappings(memberInfo, foundAssembly!, kvp.Key);
                    throw new InvalidOperationException(
                        $"Multiple [ExpressiveFor] mappings found for '{memberInfo}' " +
                        $"in assemblies '{foundAssembly!.GetName().Name}' and '{kvp.Key.GetName().Name}'.");
                }

                found = result;
                foundAssembly = kvp.Key;
            }

            return found;
        }

        private static int _lastScannedAssemblyCount;
        private static readonly object _scanLock = new();

        /// <summary>
        /// Rescans when new assemblies have been loaded since the previous scan
        /// (matters for runtime-compiled assemblies).
        /// </summary>
        private static void EnsureAllRegistriesLoaded()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies.Length == Volatile.Read(ref _lastScannedAssemblyCount)) return;

            lock (_scanLock)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (assemblies.Length == _lastScannedAssemblyCount) return;

                var filter = _assemblyScanFilter;
                foreach (var assembly in assemblies)
                {
                    if (assembly.IsDynamic)
                        continue;

                    if (filter is not null && !filter(assembly))
                        continue;

                    GetAssemblyRegistry(assembly);
                }

                Volatile.Write(ref _lastScannedAssemblyCount, assemblies.Length);
            }
        }

        /// <summary>
        /// Call this for assemblies that may contain [ExpressiveFor] stubs but no [Expressive]
        /// members (which would otherwise trigger lazy loading).
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

            // Fast path: per-assembly generated static registry.
            var registry = GetAssemblyRegistry(declaringType.Assembly);
            var registeredExpr = registry?.Invoke(expressiveMemberInfo);

            // Slow path: reflection fallback for open-generic class members and generic methods not yet in the registry.
            return registeredExpr ?? FindGeneratedExpressionViaReflection(expressiveMemberInfo);
        }

        // Sentinel for "no generated type found" — ConcurrentDictionary forbids null values.
        private readonly static LambdaExpression _reflectionNullSentinel =
            Expression.Lambda(Expression.Empty());

        private readonly static ConcurrentDictionary<MemberInfo, LambdaExpression> _reflectionCache = new();

        /// <summary>
        /// Reflection-based slow path, bypassing the static registry. Useful for members not yet
        /// in the registry (e.g. open-generic types). Result is cached.
        /// </summary>
        public static LambdaExpression? FindGeneratedExpressionViaReflection(MemberInfo expressiveMemberInfo)
        {
            var result = _reflectionCache.GetOrAdd(expressiveMemberInfo, static mi =>
            {
                var built = BuildReflectionExpression(mi);
                if (ExpressiveDiagnostics.ReflectionFallback.Enabled)
                {
                    ExpressiveDiagnostics.ReflectionFallback.Add(1,
                        new KeyValuePair<string, object?>("member", mi.ToString()));
                }
                return built ?? _reflectionNullSentinel;
            });
            return ReferenceEquals(result, _reflectionNullSentinel) ? null : result;
        }

        /// <summary>
        /// One-time reflection work. <c>MethodInfo.Invoke</c> is fine here because the result is
        /// cached — invocation cost is paid only on cache misses.
        /// </summary>
        private static LambdaExpression? BuildReflectionExpression(MemberInfo expressiveMemberInfo)
        {
            var declaringType = expressiveMemberInfo.DeclaringType
                ?? throw new InvalidOperationException("Expected a valid type here");

            var originalDeclaringType = declaringType;

            // Use the generic type definition to match the generated name.
            if (declaringType.IsGenericType && !declaringType.IsGenericTypeDefinition)
            {
                declaringType = declaringType.GetGenericTypeDefinition();
            }

            string[]? parameterTypeNames = null;
            var memberLookupName = expressiveMemberInfo.Name;

            if (expressiveMemberInfo is MethodInfo method)
            {
                // Use the generic definition so type parameters (TEntity, etc.) are used instead of closed-generic arguments.
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
                // Constructors are registered under the synthetic name "_ctor".
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

        private static string[] NestedTypePathToNames(Type[] types)
        {
            var names = new string[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                names[i] = types[i].Name;
            }

            return names;
        }

        private static string GetFullTypeName(Type type)
            => _typeNameCache.GetValue(type, static t => ComputeFullTypeName(t));

        private static string ComputeFullTypeName(Type type)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return $"{GetFullTypeName(underlyingType)}?";
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType == null)
                {
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

            // Map primitives to C# keywords to match Roslyn's output.
            var typeKeyword = GetCSharpKeyword(type);
            if (typeKeyword != null)
            {
                return typeKeyword;
            }

            if (type.IsGenericType)
            {
                var genericTypeDef = type.GetGenericTypeDefinition();
                var genericArgs = type.GetGenericArguments();
                var baseName = genericTypeDef.FullName ?? genericTypeDef.Name;

                // Strip the `n arity suffix.
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
                // + → . for nested types to match Roslyn's format.
                return type.FullName.Replace('+', '.');
            }

            return type.Name;
        }

        private static string? GetCSharpKeyword(Type type) => _csharpKeywords.GetValueOrDefault(type);
    }
}
