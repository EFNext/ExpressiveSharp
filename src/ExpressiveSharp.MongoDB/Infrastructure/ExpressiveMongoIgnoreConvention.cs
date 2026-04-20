using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace ExpressiveSharp.MongoDB.Infrastructure;

/// <summary>
/// Mongo <see cref="IClassMapConvention"/> that unmaps every property marked with
/// <see cref="ExpressiveAttribute"/> (and every property synthesized by
/// <c>[ExpressiveFor(..., Synthesize = true)]</c>) from its containing class map. This is
/// the Mongo counterpart of the EF Core <c>ExpressivePropertiesNotMappedConvention</c>:
/// without it, a synthesized property would be serialized to its BSON document as a real
/// field (because the generated property has a writable accessor) and the backing field's
/// default value would leak into storage.
/// </summary>
/// <remarks>
/// <para>
/// The convention fires when a class map is built — typically the first time a given
/// document type participates in a Mongo query or serialization. It runs once per type
/// (Mongo caches the resulting class map).
/// </para>
/// <para>
/// Read-only computed <c>[Expressive]</c> properties are also unmapped defensively.
/// Mongo would skip them anyway in most cases (there's no setter), but matching the EF
/// convention's behavior keeps the two providers consistent.
/// </para>
/// </remarks>
public sealed class ExpressiveMongoIgnoreConvention : ConventionBase, IClassMapConvention
{
    /// <summary>
    /// The name under which this convention's pack is registered in the global
    /// <see cref="ConventionRegistry"/>. Exposed so callers can inspect or unregister it.
    /// </summary>
    public const string ConventionPackName = "ExpressiveSharp.MongoDB";

    public ExpressiveMongoIgnoreConvention() : base("ExpressiveSharpIgnore") { }

    /// <summary>
    /// Runs during class-map construction. Inspects the class's CLR properties directly
    /// (instead of <see cref="BsonClassMap.DeclaredMemberMaps"/>, which may not be populated
    /// yet at this stage) and unmaps any that carry <see cref="ExpressiveAttribute"/>.
    /// </summary>
    public void Apply(BsonClassMap classMap)
    {
        ArgumentNullException.ThrowIfNull(classMap);

        foreach (var property in classMap.ClassType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            // Only unmap properties declared on this exact type; inherited ones are handled
            // by the base class's own class-map convention pass.
            if (property.DeclaringType != classMap.ClassType)
            {
                continue;
            }
            if (property.GetCustomAttribute<ExpressiveAttribute>(inherit: false) is null)
            {
                continue;
            }

            classMap.UnmapProperty(property.Name);
        }
    }

    // ── Registration ────────────────────────────────────────────────────────

    private static int _registered;

    /// <summary>
    /// Registers this convention once, idempotently, against the global
    /// <see cref="ConventionRegistry"/>. Subsequent calls are no-ops.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Ordering matters.</b> MongoDB builds and caches a <see cref="BsonClassMap"/> for a
    /// document type on the first call to <c>IMongoDatabase.GetCollection&lt;T&gt;()</c>. A
    /// convention registered <i>after</i> that call does not apply to the cached map; the
    /// <c>[Expressive]</c> properties will still be serialized to BSON.
    /// </para>
    /// <para>
    /// Call this method once at application startup, before any <c>GetCollection&lt;T&gt;</c>
    /// call for a type that has <c>[Expressive]</c> properties. Alternatively, wrap the
    /// collection in <see cref="ExpressiveMongoCollection{TDocument}"/> or call
    /// <c>collection.AsExpressive()</c> before any other collection handle is obtained; both
    /// of those paths call this method.
    /// </para>
    /// <para>
    /// The filter predicate returns <c>true</c> for every type — the convention's
    /// <see cref="Apply(BsonClassMap)"/> is a no-op for classes without <c>[Expressive]</c>
    /// properties, so applying it globally is harmless and avoids subtle ordering issues
    /// where a type-level predicate is evaluated before attribute metadata is visible.
    /// </para>
    /// </remarks>
    public static void EnsureRegistered()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1) return;

        var pack = new ConventionPack { new ExpressiveMongoIgnoreConvention() };
        ConventionRegistry.Register(ConventionPackName, pack, _ => true);
    }
}
