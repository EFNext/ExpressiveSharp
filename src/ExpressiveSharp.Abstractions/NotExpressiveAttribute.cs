namespace ExpressiveSharp;

/// <summary>
/// Suppresses ExpressiveSharp diagnostics that suggest decorating this member with
/// <see cref="ExpressiveAttribute"/> or wrapping a query in <c>.AsExpressive()</c>.
/// </summary>
/// <remarks>
/// Use this attribute on members whose bodies look expandable but should intentionally
/// remain runtime-evaluated — for example, when the body relies on side effects, captures
/// state that would not survive translation, or is deliberately client-side.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor,
    Inherited = true, AllowMultiple = false)]
public sealed class NotExpressiveAttribute : Attribute
{
}
