using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveSharp.Services;

public interface IExpressiveResolver
{
    LambdaExpression FindGeneratedExpression(MemberInfo expressiveMemberInfo,
        ExpressiveAttribute? expressiveAttribute = null);

    /// <summary>
    /// Searches all loaded assembly registries for an [ExpressiveFor] mapping targeting
    /// the given member. Returns null if no mapping is found.
    /// </summary>
    LambdaExpression? FindExternalExpression(MemberInfo memberInfo);
}
