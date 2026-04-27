using System.Linq.Expressions;
using System.Reflection;
using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.WindowFunctions;

namespace ExpressiveSharp.EntityFrameworkCore.RelationalExtensions.Transformers;

/// <summary>
/// Rewrites <c>Queryable.Select(source, (elem, index) => body)</c> into a 1-arg Select where
/// references to <c>index</c> become <c>WindowFunction.RowNumber() - 1</c>. The emitted
/// <c>ROW_NUMBER() OVER()</c> has no ordering — non-deterministic unless the query has an explicit OrderBy.
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
            if (node.Method.DeclaringType != typeof(Queryable)
                || node.Method.Name != "Select"
                || node.Arguments.Count != 2)
                return base.VisitMethodCall(node);

            if (UnwrapQuote(node.Arguments[1]) is not LambdaExpression lambda
                || lambda.Parameters.Count != 2
                || lambda.Parameters[1].Type != typeof(int))
                return base.VisitMethodCall(node);

            var elemParam = lambda.Parameters[0];
            var indexParam = lambda.Parameters[1];

            // (int)((long)WindowFunction.RowNumber() - 1L)
            var rowNumberCall = Expression.Call(RowNumberMethod);
            var subtractOne = Expression.Subtract(rowNumberCall, Expression.Constant(1L));
            var castToInt = Expression.Convert(subtractOne, typeof(int));

            var rewrittenBody = new ParameterReplacer(indexParam, castToInt).Visit(lambda.Body);
            var newLambda = Expression.Lambda(rewrittenBody, elemParam);

            var sourceType = elemParam.Type;
            var resultType = lambda.ReturnType;
            var selectMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Select"
                    && m.GetGenericArguments().Length == 2
                    && m.GetParameters()[1].ParameterType.GetGenericArguments()[0]
                        .GetGenericArguments().Length == 2) // Func<T, TResult> has 2 type args
                .MakeGenericMethod(sourceType, resultType);

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
