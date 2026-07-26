using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Infrastructure.Internal;

/// <summary>
/// Temporarily replaces <see cref="WindowFunctionSqlExpression"/> nodes with
/// <see cref="SqlFragmentExpression"/> placeholders so the provider's
/// <see cref="Microsoft.EntityFrameworkCore.Query.SqlNullabilityProcessor"/> can traverse the tree
/// without throwing on unknown expression types. Originals are restored after nullability processing.
/// </summary>
internal static class WindowFunctionSqlExpressionWrapper
{
    private const string PlaceholderPrefix = "__wf_placeholder_";

    public static Expression WrapAll(Expression expression, out Dictionary<string, WindowFunctionSqlExpression> stash)
    {
        var visitor = new WrapVisitor();
        var result = visitor.Visit(expression);
        stash = visitor.Stash;
        return result;
    }

    public static Expression UnwrapAll(Expression expression, Dictionary<string, WindowFunctionSqlExpression> stash)
        => new UnwrapVisitor(stash).Visit(expression);

    private sealed class WrapVisitor : ExpressionVisitor
    {
        public Dictionary<string, WindowFunctionSqlExpression> Stash { get; } = new();
        private int _counter;

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(node))]
        public override Expression? Visit(Expression? node)
        {
            if (node is WindowFunctionSqlExpression windowFunc)
            {
                var key = $"{PlaceholderPrefix}{_counter++}";
                Stash[key] = windowFunc;
                return new SqlFragmentExpression(key);
            }

            return base.Visit(node);
        }
    }

    private sealed class UnwrapVisitor : ExpressionVisitor
    {
        private readonly Dictionary<string, WindowFunctionSqlExpression> _stash;

        public UnwrapVisitor(Dictionary<string, WindowFunctionSqlExpression> stash) => _stash = stash;

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(node))]
        public override Expression? Visit(Expression? node)
        {
            if (node is SqlFragmentExpression fragment
                && fragment.Sql.StartsWith(PlaceholderPrefix, StringComparison.Ordinal)
                && _stash.TryGetValue(fragment.Sql, out var original))
            {
                return original;
            }

            return base.Visit(node);
        }
    }
}
