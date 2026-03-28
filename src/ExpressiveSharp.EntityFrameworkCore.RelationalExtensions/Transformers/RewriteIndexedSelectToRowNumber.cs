using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Transformers;

/// <summary>
/// Rewrites <c>Queryable.Select(source, (elem, index) => body)</c> into
/// <c>Queryable.Select(source, elem => body')</c> where references to the
/// <c>int index</c> parameter are replaced with <c>WindowFunction.RowNumber() - 1</c>.
/// <para>
/// The resulting <c>WindowFunction.RowNumber()</c> call produces
/// <c>ROW_NUMBER() OVER()</c> with no ordering — row numbering is non-deterministic
/// unless the query includes an explicit <c>OrderBy</c>.
/// </para>
/// </summary>
public sealed class RewriteIndexedSelectToRowNumber : IExpressionTreeTransformer
{
    private static readonly MethodInfo RowNumberMethod =
        typeof(WindowFunction).GetMethod(nameof(WindowFunction.RowNumber), Type.EmptyTypes)!;

    public Expression Transform(Expression expression)
        => new IndexedSelectRewriter().Visit(expression);

    private sealed class IndexedSelectRewriter : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Look for Queryable.Select(source, Expression<Func<T, int, TResult>>)
            if (node.Method.DeclaringType != typeof(Queryable)
                || node.Method.Name != "Select"
                || node.Arguments.Count != 2)
                return base.VisitMethodCall(node);

            // The second argument must be a quoted lambda: Expression<Func<T, int, TResult>>
            if (UnwrapQuote(node.Arguments[1]) is not LambdaExpression lambda
                || lambda.Parameters.Count != 2
                || lambda.Parameters[1].Type != typeof(int))
                return base.VisitMethodCall(node);

            var elemParam = lambda.Parameters[0];
            var indexParam = lambda.Parameters[1];

            // Build: (long)WindowFunction.RowNumber() - 1L
            var rowNumberCall = Expression.Call(RowNumberMethod);
            var subtractOne = Expression.Subtract(rowNumberCall, Expression.Constant(1L));
            var castToInt = Expression.Convert(subtractOne, typeof(int));

            // Replace index parameter references with the ROW_NUMBER expression
            var rewrittenBody = new ParameterReplacer(indexParam, castToInt).Visit(lambda.Body);

            // Build new 1-parameter lambda
            var newLambda = Expression.Lambda(rewrittenBody, elemParam);

            // Find the 1-param Queryable.Select<T, TResult> overload
            var sourceType = elemParam.Type;
            var resultType = lambda.ReturnType;
            var selectMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Select"
                    && m.GetGenericArguments().Length == 2
                    && m.GetParameters()[1].ParameterType.GetGenericArguments()[0]
                        .GetGenericArguments().Length == 2) // Func<T, TResult> has 2 type args
                .MakeGenericMethod(sourceType, resultType);

            // Visit the source in case it also needs transformation
            var visitedSource = Visit(node.Arguments[0]);

            return Expression.Call(selectMethod, visitedSource, Expression.Quote(newLambda));
        }

        private static Expression? UnwrapQuote(Expression expression)
        {
            if (expression is UnaryExpression { NodeType: ExpressionType.Quote } unary)
                return unary.Operand;
            return expression as LambdaExpression;
        }
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly Expression _replacement;

        public ParameterReplacer(ParameterExpression oldParam, Expression replacement)
        {
            _oldParam = oldParam;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _oldParam ? _replacement : node;
    }
}
