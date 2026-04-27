using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Interpretation;

// Kept central so the interpreter (validates + emits) and the registry extractor (builds
// the runtime lookup entry) can never disagree about which target member a stub maps to —
// divergence produces silently-missing bodies or orphaned registry entries.
static internal class ExpressiveForSignatureMatcher
{
    // Four-quadrant matrix for method-target ↔ method-stub matching:
    //   static target + static stub: stub params = target params
    //   instance target + static stub: stub params = [receiver] + target params (receiver = targetType)
    //   instance target + instance stub on targetType: stub params = target params (`this` is receiver)
    //   static target + instance stub: never matches (no way to receive a non-null instance)
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

    // Property-target ↔ method-stub matching (same matrix as MatchesMethodSignature, but
    // target has zero parameters):
    //   static property + static stub (0 params): match.
    //   instance property + static stub (1 param of targetType): match.
    //   instance property + instance stub on targetType (0 params): match (`this` is receiver).
    //   static property + instance stub: never matches.
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

    // Property-target ↔ property-stub: parameterless stub, `this` as receiver.
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
