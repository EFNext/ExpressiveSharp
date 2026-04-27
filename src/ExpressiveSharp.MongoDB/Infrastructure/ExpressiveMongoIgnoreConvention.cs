using System.Reflection;
using ExpressiveSharp.Mapping;
using ExpressiveSharp.Services;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace ExpressiveSharp.MongoDB.Infrastructure;

/// <summary>
/// Mongo <see cref="IClassMapConvention"/> that unmaps every property marked with
/// <see cref="ExpressiveAttribute"/> (and every property synthesized by
/// <c>[ExpressiveProperty]</c>) from its containing class map. Without it,
/// a synthesized property would be serialized to its BSON document as a real field
/// (because the generated property has a writable accessor) and the backing field's
/// default value would leak into storage. Mongo counterpart of the EF Core
/// <c>ExpressivePropertiesNotMappedConvention</c>.
/// </summary>
public sealed class ExpressiveMongoIgnoreConvention : ConventionBase, IClassMapConvention
{
    public const string ConventionPackName = "ExpressiveSharp.MongoDB";

    // Detects properties synthesized via [ExpressiveProperty] / [ExpressiveFor] — these
    // carry no CLR marker attribute, so we identify them through the generated registry.
    private static readonly IExpressiveResolver _resolver = new ExpressiveResolver();

    public ExpressiveMongoIgnoreConvention() : base("ExpressiveSharpIgnore") { }

    public void Apply(BsonClassMap classMap)
    {
        ArgumentNullException.ThrowIfNull(classMap);

        // Inspect CLR properties directly instead of BsonClassMap.DeclaredMemberMaps,
        // which may not be populated yet at this stage.
        foreach (var property in classMap.ClassType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            // Inherited members are handled by the base class's own class-map convention pass.
            if (property.DeclaringType != classMap.ClassType)
            {
                continue;
            }

            if (property.GetCustomAttribute<ExpressiveAttribute>(inherit: false) is null
                && property.GetCustomAttribute<ExpressiveForAttribute>(inherit: false) is null
                && _resolver.FindExternalExpression(property) is null)
            {
                continue;
            }

            classMap.UnmapProperty(property.Name);
        }
    }

    private static int _registered;

    /// <summary>
    /// Registers this convention once, idempotently, against the global
    /// <see cref="ConventionRegistry"/>. Subsequent calls are no-ops.
    /// </summary>
    /// <remarks>
    /// <b>Ordering matters.</b> MongoDB builds and caches a <see cref="BsonClassMap"/> for a
    /// document type on the first call to <c>IMongoDatabase.GetCollection&lt;T&gt;()</c>. A
    /// convention registered <i>after</i> that call does not apply to the cached map.
    /// Wrap the collection in <see cref="ExpressiveMongoCollection{TDocument}"/> or call
    /// <c>collection.AsExpressive()</c> before any other collection handle is obtained — both
    /// paths call this method.
    /// </remarks>
    public static void EnsureRegistered()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1) return;

        var pack = new ConventionPack { new ExpressiveMongoIgnoreConvention() };
        // Filter returns true for every type — Apply is a no-op for classes without
        // [Expressive] properties, and a type-level filter risks evaluating before
        // attribute metadata is visible.
        ConventionRegistry.Register(ConventionPackName, pack, _ => true);
    }
}
