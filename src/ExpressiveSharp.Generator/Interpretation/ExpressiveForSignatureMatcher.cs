using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Interpretation;

/// <summary>
/// Shared signature-matching rules for <c>[ExpressiveFor]</c> stubs.
/// Kept central so the interpreter (which validates + emits) and the registry extractor
/// (which builds the runtime lookup entry) can never disagree about which target member
/// a stub maps to — a divergence here produces either silently-missing bodies or
/// orphaned registry entries.
/// </summary>
static internal class ExpressiveForSignatureMatcher
{
    /// <summary>
    /// Four-quadrant matrix for method-target ↔ method-stub matching:
    /// <list type="bullet">
    ///   <item>static target + static stub: stub params = target params</item>
    ///   <item>instance target + static stub: stub params = [receiver] + target params (receiver type = targetType)</item>
    ///   <item>instance target + instance stub on targetType: stub params = target params (<c>this</c> is receiver)</item>
    ///   <item>static target + instance stub: never matches (no way to receive a non-null instance)</item>
    /// </list>
    /// </summary>
    public static bool MatchesMethodSignature(
        IMethodSymbol target,
        INamedTypeSymbol targetType,
        bool stubIsStatic,
        INamedTypeSymbol stubContainingType,
        ImmutableArray<IParameterSymbol> stubParameters)
    {
        if (target.IsStatic && !stubIsStatic)
            return false;

        int expectedStubParamCount;
        int offset;
        if (!target.IsStatic && stubIsStatic)
        {
            // Static stub over instance target: first stub param is the explicit receiver.
            expectedStubParamCount = target.Parameters.Length + 1;
            offset = 1;
        }
        else
        {
            // Either both static, or both instance (receiver implicit via `this`).
            expectedStubParamCount = target.Parameters.Length;
            offset = 0;
        }

        if (stubParameters.Length != expectedStubParamCount)
            return false;

        if (!target.IsStatic && stubIsStatic &&
            !SymbolEqualityComparer.Default.Equals(stubParameters[0].Type, targetType))
            return false;

        if (!target.IsStatic && !stubIsStatic &&
            !SymbolEqualityComparer.Default.Equals(stubContainingType, targetType))
            return false;

        for (var i = 0; i < target.Parameters.Length; i++)
        {
            if (!SymbolEqualityComparer.Default.Equals(
                target.Parameters[i].Type, stubParameters[i + offset].Type))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Matching rules for property-target ↔ method-stub (the parameter-count encoding is the
    /// same matrix but restricted to zero target parameters):
    /// <list type="bullet">
    ///   <item>static property + static stub (0 params): match.</item>
    ///   <item>instance property + static stub (1 param of targetType): match.</item>
    ///   <item>instance property + instance stub on targetType (0 params): match (<c>this</c> is receiver).</item>
    ///   <item>static property + instance stub: never matches.</item>
    /// </list>
    /// </summary>
    public static bool MatchesPropertyFromMethodStub(
        IPropertySymbol target,
        INamedTypeSymbol targetType,
        bool stubIsStatic,
        INamedTypeSymbol stubContainingType,
        ImmutableArray<IParameterSymbol> stubParameters)
    {
        // Indexers have parameters and cannot be expressive targets.
        if (target.Parameters.Length > 0)
            return false;

        if (stubIsStatic)
        {
            if (target.IsStatic && stubParameters.Length == 0)
                return true;
            if (!target.IsStatic && stubParameters.Length == 1 &&
                SymbolEqualityComparer.Default.Equals(stubParameters[0].Type, targetType))
                return true;
            return false;
        }

        return !target.IsStatic && stubParameters.Length == 0 &&
               SymbolEqualityComparer.Default.Equals(stubContainingType, targetType);
    }

    /// <summary>
    /// Matching rules for property-target ↔ property-stub (parameterless stub, <c>this</c> as receiver).
    /// </summary>
    public static bool MatchesPropertyFromPropertyStub(
        IPropertySymbol target,
        INamedTypeSymbol targetType,
        bool stubIsStatic,
        INamedTypeSymbol stubContainingType)
    {
        if (target.Parameters.Length > 0)
            return false;

        if (stubIsStatic && target.IsStatic)
            return true;

        return !stubIsStatic && !target.IsStatic &&
               SymbolEqualityComparer.Default.Equals(stubContainingType, targetType);
    }
}
