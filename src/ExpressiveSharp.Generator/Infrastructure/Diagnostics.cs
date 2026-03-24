using Microsoft.CodeAnalysis;

namespace ExpressiveSharp.Generator.Infrastructure;

static internal class Diagnostics
{
    public readonly static DiagnosticDescriptor UnsupportedStatementInBlockBody = new DiagnosticDescriptor(
        id: "EXP0003",
        title: "Unsupported statement in block-bodied method",
        messageFormat: "Method '{0}' contains an unsupported statement: {1}",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor SideEffectInBlockBody = new DiagnosticDescriptor(
        id: "EXP0004",
        title: "Statement with side effects in block-bodied method",
        messageFormat: "{0}",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor PotentialSideEffectInBlockBody = new DiagnosticDescriptor(
        id: "EXP0005",
        title: "Potential side effect in block-bodied method",
        messageFormat: "{0}",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor RequiresBodyDefinition = new DiagnosticDescriptor(
        id: "EXP0006",
        title: "Method or property should expose a body definition",
        messageFormat: "Method or property '{0}' should expose a body definition (e.g. an expression-bodied member or a block-bodied method) to be used as the source for the generated expression tree.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedPatternInExpression = new DiagnosticDescriptor(
        id: "EXP0007",
        title: "Unsupported pattern in expressive expression",
        messageFormat: "The pattern '{0}' cannot be rewritten into an expression tree. Simplify the pattern or restructure the expressive member body.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor MissingParameterlessConstructor = new DiagnosticDescriptor(
        id: "EXP0008",
        title: "Target class is missing a parameterless constructor",
        messageFormat: "Class '{0}' must have a parameterless constructor to be used with an [Expressive] constructor. The generated projection uses 'new {0}() {{ ... }}' (object-initializer syntax), which requires an accessible parameterless constructor.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor NoSourceAvailableForDelegatedConstructor = new DiagnosticDescriptor(
        id: "EXP0009",
        title: "Delegated constructor cannot be analyzed for projection",
        messageFormat: "The delegated constructor '{0}' in type '{1}' has no source available and cannot be analyzed. Base/this initializer in member '{2}' will not be projected.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor FeatureDisabled = new DiagnosticDescriptor(
        id: "EXP0013",
        title: "Expression feature is disabled",
        messageFormat: "Feature '{0}' is disabled for member '{1}'. Remove ExpressionFeature.{0} from the Disable property to enable this feature.",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnrecognizedLoopPattern = new DiagnosticDescriptor(
        id: "EXP0014",
        title: "Unrecognized loop pattern",
        messageFormat: "The loop in method '{0}' could not be converted to a LINQ expression: {1}",
        category: "Design",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor FactoryMethodShouldBeConstructor = new DiagnosticDescriptor(
        id: "EXP0012",
        title: "[Expressive] factory method can be converted to a constructor",
        messageFormat: "Factory method '{0}' creates and returns an instance of the containing class via object initializer. Consider converting it to an [Expressive] constructor.",
        category: "Design",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedInitializer = new DiagnosticDescriptor(
        id: "EXP0015",
        title: "Unsupported initializer in object creation",
        messageFormat: "Object initializer contains an unsupported element ({0}). Only property and field assignments are supported in expression trees.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedOperation = new DiagnosticDescriptor(
        id: "EXP0016",
        title: "Unsupported expression operation",
        messageFormat: "Expression contains an unsupported operation ({0}). A default value will be used instead.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public readonly static DiagnosticDescriptor UnsupportedOperator = new DiagnosticDescriptor(
        id: "EXP0017",
        title: "Unsupported operator in expression",
        messageFormat: "Operator '{0}' is not supported in expression trees. A default value will be used instead.",
        category: "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
