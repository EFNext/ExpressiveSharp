using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace ExpressiveSharp.MongoDB.Infrastructure;

/// <summary>
/// Mongo <see cref="IClassMapConvention"/> that unmaps every property marked with
/// <see cref="ExpressiveAttribute"/> from its containing class map. This is the Mongo
/// counterpart of the EF Core <c>ExpressivePropertiesNotMappedConvention</c>: without it,
/// a <c>[Expressive(Projectable = true)]</c> property would be serialized to its BSON
/// document as a real field (because the property has a writable accessor), and the
/// backing field's default value would leak into storage.
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

    public void Apply(BsonClassMap classMap)
    {
        if (classMap is null) throw new ArgumentNullException(nameof(classMap));

        // Walk the class map's already-auto-mapped members and remove any whose underlying
        // PropertyInfo carries [Expressive]. Using DeclaredMemberMaps (not AllMemberMaps) to
        // only touch members declared on this specific class; inherited ones will be unmapped
        // when the base class's map is built.
        foreach (var memberMap in classMap.DeclaredMemberMaps.ToArray())
        {
            if (memberMap.MemberInfo is not PropertyInfo property) continue;
            if (property.GetCustomAttribute<ExpressiveAttribute>(inherit: false) is null) continue;

            classMap.UnmapMember(memberMap.MemberInfo);
        }
    }

    // ── Registration ────────────────────────────────────────────────────────

    private static int _registered;

    /// <summary>
    /// Registers this convention once, idempotently, against the global
    /// <see cref="ConventionRegistry"/>. Subsequent calls are no-ops.
    /// </summary>
    /// <remarks>
    /// The filter predicate returns <c>true</c> for every type — the convention's
    /// <see cref="Apply(BsonClassMap)"/> is a no-op for classes without <c>[Expressive]</c>
    /// properties, so applying it globally is harmless and avoids subtle ordering issues
    /// where a type-level predicate is evaluated before attribute metadata is visible.
    /// </remarks>
    public static void EnsureRegistered()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1) return;

        var pack = new ConventionPack { new ExpressiveMongoIgnoreConvention() };
        ConventionRegistry.Register(ConventionPackName, pack, _ => true);
    }
}
