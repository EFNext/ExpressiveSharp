using System.Linq.Expressions;

namespace ExpressiveSharp.Services
{
    public sealed class ExpressionArgumentReplacer : ExpressionVisitor
    {
        public Dictionary<ParameterExpression, Expression> ParameterArgumentMapping { get; } = new();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (ParameterArgumentMapping.TryGetValue(node, out var mappedArgument))
            {
                return mappedArgument;
            }

            return base.VisitParameter(node);
        }
    }
}
