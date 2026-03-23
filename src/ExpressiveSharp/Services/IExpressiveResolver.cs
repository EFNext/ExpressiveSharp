using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveSharp.Services;

public interface IExpressiveResolver
{
    LambdaExpression FindGeneratedExpression(MemberInfo expressiveMemberInfo,
        ExpressiveAttribute? expressiveAttribute = null);
}
