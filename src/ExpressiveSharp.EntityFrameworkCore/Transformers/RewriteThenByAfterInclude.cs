using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ExpressiveSharp.EntityFrameworkCore.Transformers;

/// <summary>
/// Rewrites <c>ThenBy</c>/<c>ThenByDescending</c> applied after <c>Include</c>/<c>ThenInclude</c>
/// into the equivalent tree EF Core can translate: the ordering is applied to the ordered source
/// beneath the include chain. The C# type system cannot express this shape directly, so the
/// generated interceptors produce a cast node (<see cref="ExpressiveQueryableExtensions.AsOrdered{T}"/>)
/// that this transformer resolves before EF Core sees the query.
/// </summary>
public sealed class RewriteThenByAfterInclude : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression) => Visit(expression);

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType != typeof(Queryable)
            || node.Method.Name is not (nameof(Queryable.ThenBy) or nameof(Queryable.ThenByDescending)))
            return base.VisitMethodCall(node);

        var arguments = new Expression[node.Arguments.Count];
        for (var i = 0; i < node.Arguments.Count; i++)
            arguments[i] = Visit(node.Arguments[i]);

        var source = arguments[0];
        if (source is UnaryExpression { NodeType: ExpressionType.Convert } convert
            && typeof(IOrderedQueryable).IsAssignableFrom(convert.Type))
        {
            source = convert.Operand;
        }

        var includes = new List<MethodCallExpression>();
        var baseSource = source;
        while (baseSource is MethodCallExpression call && IsIncludeCall(call))
        {
            includes.Add(call);
            baseSource = call.Arguments[0];
        }

        if (includes.Count == 0)
        {
            return node.Update(node.Object, arguments);
        }

        if (!typeof(IOrderedQueryable).IsAssignableFrom(baseSource.Type))
        {
            throw new InvalidOperationException(
                $"'{node.Method.Name}' was called after 'Include'/'ThenInclude' on a source that " +
                "is not ordered. Call 'OrderBy'/'OrderByDescending' before 'Include', or apply " +
                "the complete ordering after the includes.");
        }

        arguments[0] = baseSource;
        Expression current = Expression.Call(node.Method, arguments);
        for (var i = includes.Count - 1; i >= 0; i--)
        {
            var includeArguments = includes[i].Arguments.ToArray();
            includeArguments[0] = current;
            current = includes[i].Update(null, includeArguments);
        }

        return current;
    }

    private static bool IsIncludeCall(MethodCallExpression call)
        => call.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions)
           && call.Method.Name is nameof(EntityFrameworkQueryableExtensions.Include)
               or nameof(EntityFrameworkQueryableExtensions.ThenInclude);
}
