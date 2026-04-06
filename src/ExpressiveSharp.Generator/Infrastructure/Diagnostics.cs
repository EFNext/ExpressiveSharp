using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Infrastructure;

static internal class Diagnostics
{
    // ── Errors ──────────────────────────────────────────────────────────────

    public readonly static DiagnosticDescriptor RequiresBodyDefinition = new DiagnosticDescriptor(
        id: "EXP0001",
        title: "Method or property should expose a body definition",
        messageFormat: "Method or property '{0}' should expose a body definition (e.g. an expression-bodied member or a block-bodied method) to be used as the source for the generated expression tree.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor MissingParameterlessConstructor = new DiagnosticDescriptor(
        id: "EXP0002",
        title: "Target class is missing a parameterless constructor",
        messageFormat: "Class '{0}' must have a parameterless constructor to be used with an [Expressive] constructor. The generated projection uses 'new {0}() {{ ... }}' (object-initializer syntax), which requires an accessible parameterless constructor.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor NoSourceAvailableForDelegatedConstructor = new DiagnosticDescriptor(
        id: "EXP0003",
        title: "Delegated constructor cannot be analyzed for projection",
        messageFormat: "The delegated constructor '{0}' in type '{1}' has no source available and cannot be analyzed. Base/this initializer in member '{2}' will not be projected.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor BlockBodyRequiresOptIn = new DiagnosticDescriptor(
        id: "EXP0004",
        title: "Block-bodied [Expressive] member requires AllowBlockBody",
        messageFormat: "Member '{0}' uses a block body ({{ }}) which requires [Expressive(AllowBlockBody = true)]. Block bodies support local variables, if/else, and foreach loops, but not all constructs are translatable by every LINQ provider. Use an expression-bodied member (=>) for full compatibility, or opt in with AllowBlockBody = true.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor SideEffectInBlockBody = new DiagnosticDescriptor(
        id: "EXP0005",
        title: "Statement with side effects in block-bodied method",
        messageFormat: "{0}",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // ── Warnings ────────────────────────────────────────────────────────────

    public readonly static DiagnosticDescriptor UnsupportedStatementInBlockBody = new DiagnosticDescriptor(
        id: "EXP0006",
        title: "Unsupported statement in block-bodied method",
        messageFormat: "Method '{0}' contains an unsupported statement: {1}",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedInitializer = new DiagnosticDescriptor(
        id: "EXP0007",
        title: "Unsupported initializer in object creation",
        messageFormat: "Object initializer contains an unsupported element ({0}). Only property and field assignments are supported in expression trees.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedOperation = new DiagnosticDescriptor(
        id: "EXP0008",
        title: "Unsupported expression operation",
        messageFormat: "Expression contains an unsupported operation ({0}). A default value will be used instead.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedOperator = new DiagnosticDescriptor(
        id: "EXP0009",
        title: "Unsupported operator in expression",
        messageFormat: "Operator '{0}' is not supported in expression trees. A default value will be used instead.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor InterceptorEmissionFailed = new DiagnosticDescriptor(
        id: "EXP0010",
        title: "Interceptor emission failed unexpectedly",
        messageFormat: "Failed to generate interceptor for call site: {0}. The original delegate stub will be used at runtime.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnresolvablePatternMember = new DiagnosticDescriptor(
        id: "EXP0011",
        title: "Unresolvable member in pattern",
        messageFormat: "Pattern sub-expression for member '{0}' could not be resolved and was skipped. The pattern may not match correctly.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    // NOTE: EXP0013 (MemberCouldBeExpressive) is now emitted by MissingExpressiveAnalyzer
    // in ExpressiveSharp.CodeFixers, so VS can pair it with the code fix provider.

    // ── Info ────────────────────────────────────────────────────────────────

    public readonly static DiagnosticDescriptor FactoryMethodShouldBeConstructor = new DiagnosticDescriptor(
        id: "EXP0012",
        title: "[Expressive] factory method can be converted to a constructor",
        messageFormat: "Factory method '{0}' creates and returns an instance of the containing class via object initializer. Consider converting it to an [Expressive] constructor.",
        category: "Design",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    // ── [ExpressiveFor] Diagnostics ─────────────────────────────────────────

    public readonly static DiagnosticDescriptor ExpressiveForTargetTypeNotFound = new DiagnosticDescriptor(
        id: "EXP0014",
        title: "[ExpressiveFor] target type not found",
        messageFormat: "[ExpressiveFor] target type '{0}' could not be resolved",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor ExpressiveForMemberNotFound = new DiagnosticDescriptor(
        id: "EXP0015",
        title: "[ExpressiveFor] target member not found",
        messageFormat: "No member '{0}' found on type '{1}' matching the stub's parameter signature",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor ExpressiveForStubMustBeStatic = new DiagnosticDescriptor(
        id: "EXP0016",
        title: "[ExpressiveFor] stub must be static",
        messageFormat: "[ExpressiveFor] stub method '{0}' must be static",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor ExpressiveForReturnTypeMismatch = new DiagnosticDescriptor(
        id: "EXP0017",
        title: "[ExpressiveFor] return type mismatch",
        messageFormat: "[ExpressiveFor] return type mismatch for '{0}': target returns '{1}' but stub returns '{2}'",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor IgnoredOperation = new DiagnosticDescriptor(
        id: "EXP0018",
        title: "Unsupported operation ignored",
        messageFormat: "Expression contains an unsupported operation ({0}). The operation will be ignored and the surrounding expression emitted without it.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor ExpressiveForConflictsWithExpressive = new DiagnosticDescriptor(
        id: "EXP0019",
        title: "[ExpressiveFor] conflicts with [Expressive]",
        messageFormat: "Target member '{0}' on type '{1}' already has [Expressive]; remove [ExpressiveFor] or [Expressive]",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor ExpressiveForDuplicateMapping = new DiagnosticDescriptor(
        id: "EXP0020",
        title: "Duplicate [ExpressiveFor] mapping",
        messageFormat: "Duplicate [ExpressiveFor] mapping for member '{0}' on type '{1}'; only one stub per target member is allowed",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
