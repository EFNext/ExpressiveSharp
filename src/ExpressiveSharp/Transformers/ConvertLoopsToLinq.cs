using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveSharp.Transformers;

/// <summary>
/// Converts <see cref="LoopExpression"/> nodes (produced by the emitter for foreach/for/while loops)
/// into equivalent LINQ method calls (<c>Sum</c>, <c>Count</c>, <c>Any</c>, <c>All</c>, etc.)
/// that LINQ providers like EF Core can translate to SQL.
/// </summary>
/// <remarks>
/// <para>
/// Recognized patterns:
/// <list type="table">
/// <item><term>Sum</term><description><c>acc = acc + expr(x)</c> → <c>collection.Sum(x => expr)</c></description></item>
/// <item><term>Count</term><description><c>acc = acc + 1</c> → <c>collection.Count()</c></description></item>
/// <item><term>Count with predicate</term><description><c>if (cond) acc = acc + 1</c> → <c>collection.Count(x => cond)</c></description></item>
/// <item><term>Sum with predicate</term><description><c>if (cond) acc = acc + expr(x)</c> → <c>collection.Where(x => cond).Sum(x => expr)</c></description></item>
/// <item><term>Any</term><description><c>if (cond) found = true</c> → <c>collection.Any(x => cond)</c></description></item>
/// <item><term>All</term><description><c>if (!cond) all = false</c> → <c>collection.All(x => cond)</c></description></item>
/// </list>
/// </para>
/// <para>
/// Throws <see cref="InvalidOperationException"/> if a loop is encountered that cannot be rewritten.
/// </para>
/// </remarks>
public sealed class ConvertLoopsToLinq : ExpressionVisitor, IExpressionTreeTransformer
{
    public Expression Transform(Expression expression) => Visit(expression);

    protected override Expression VisitBlock(BlockExpression node)
    {
        // First, recursively visit sub-expressions
        var visited = (BlockExpression)base.VisitBlock(node);

        // Look for the foreach accumulator pattern:
        // Block([accumulator], [Assign(accumulator, init), innerBlock, accumulator])
        if (TryMatchForEachAccumulator(visited, out var result))
            return result;

        return visited;
    }

    protected override Expression VisitLoop(LoopExpression node)
    {
        // Foreach loops are handled at the block level (TryMatchForEachAccumulator).
        // If a LoopExpression reaches here, it's a for/while loop that wasn't part
        // of a recognized foreach accumulator pattern — fail clearly.
        var hasEnumeratorPattern = node.Body is ConditionalExpression cond
            && cond.Test is MethodCallExpression call
            && call.Method.Name == "MoveNext";

        if (!hasEnumeratorPattern)
        {
            throw new InvalidOperationException(
                "Expression contains a for/while loop that cannot be converted to LINQ. " +
                "Only foreach loops with recognized accumulator patterns " +
                "(Sum, Count, Any, All, Min, Max, Select) are supported. " +
                "Rewrite as a foreach loop or use an expression-bodied member.");
        }

        return base.VisitLoop(node);
    }

    private bool TryMatchForEachAccumulator(BlockExpression outer, out Expression result)
    {
        result = null!;

        // Must have: exactly 1 variable (the accumulator), 3 expressions
        // [Assign(acc, init), innerBlock, acc]
        if (outer.Variables.Count != 1 || outer.Expressions.Count != 3)
            return false;

        var accumulator = outer.Variables[0];

        // First expression: Assign(accumulator, initialValue)
        if (outer.Expressions[0] is not BinaryExpression { NodeType: ExpressionType.Assign } initAssign
            || initAssign.Left != accumulator)
            return false;

        var initValue = initAssign.Right;

        // Last expression: must be the accumulator itself (return value)
        if (outer.Expressions[2] != accumulator)
            return false;

        // Middle expression: inner block with enumerator + iteration variable
        if (outer.Expressions[1] is not BlockExpression innerBlock
            || innerBlock.Variables.Count != 2)
            return false;

        // Find the enumerator and iteration variable
        // Inner block: [Assign(enumerator, Call(collection, GetEnumerator)), Loop(...)]
        if (innerBlock.Expressions.Count < 2)
            return false;

        if (innerBlock.Expressions[0] is not BinaryExpression { NodeType: ExpressionType.Assign } enumAssign)
            return false;

        var enumeratorVar = enumAssign.Left as ParameterExpression;
        if (enumeratorVar is null)
            return false;

        // Extract collection from: Call(collection, GetEnumerator)
        if (enumAssign.Right is not MethodCallExpression getEnumCall
            || getEnumCall.Method.Name != "GetEnumerator")
            return false;

        var collection = getEnumCall.Object;
        if (collection is null)
            return false;

        // Find the Loop expression
        if (innerBlock.Expressions[1] is not LoopExpression loop)
            return false;

        // Loop body: IfThenElse(Call(enumerator, MoveNext), bodyBlock, Break)
        if (loop.Body is not ConditionalExpression ifThenElse
            || ifThenElse.Test is not MethodCallExpression moveNextCall
            || moveNextCall.Method.Name != "MoveNext")
            return false;

        var loopBody = ifThenElse.IfTrue;

        // The loop body block: Block(Assign(iterVar, Property(enum, Current)), <userBody>)
        if (loopBody is not BlockExpression bodyBlock || bodyBlock.Expressions.Count < 2)
            return false;

        // First expression in body: Assign(iterVar, Property(enumerator, Current))
        if (bodyBlock.Expressions[0] is not BinaryExpression { NodeType: ExpressionType.Assign } currentAssign)
            return false;

        var iterVar = currentAssign.Left as ParameterExpression;
        if (iterVar is null)
            return false;

        // The user's loop body is the second expression (or remaining expressions in the block)
        var userBody = bodyBlock.Expressions.Count == 2
            ? bodyBlock.Expressions[1]
            : Expression.Block(bodyBlock.Expressions.Skip(1));

        // Determine the element type from the iteration variable
        var elementType = iterVar.Type;

        // Now match the user body against known patterns
        if (TryMatchCountPattern(userBody, accumulator, initValue, collection, iterVar, elementType, out result))
            return true;

        if (TryMatchSumPattern(userBody, accumulator, initValue, collection, iterVar, elementType, out result))
            return true;

        if (TryMatchAnyPattern(userBody, accumulator, initValue, collection, iterVar, elementType, out result))
            return true;

        if (TryMatchAllPattern(userBody, accumulator, initValue, collection, iterVar, elementType, out result))
            return true;

        if (TryMatchMinMaxPattern(userBody, accumulator, collection, iterVar, elementType, out result))
            return true;

        if (TryMatchSelectPattern(userBody, accumulator, initValue, collection, iterVar, elementType, out result))
            return true;

        // Conditional variants: IfThen(condition, innerBody)
        if (userBody is ConditionalExpression { IfFalse: DefaultExpression } conditional)
        {
            var condition = conditional.Test;
            var innerBody = conditional.IfTrue;

            if (TryMatchConditionalCount(innerBody, accumulator, condition, collection, iterVar, elementType, out result))
                return true;

            if (TryMatchConditionalSum(innerBody, accumulator, condition, collection, iterVar, elementType, out result))
                return true;

            if (TryMatchConditionalSelect(innerBody, accumulator, condition, collection, iterVar, elementType, out result))
                return true;
        }

        throw new InvalidOperationException(
            $"Cannot convert foreach loop to LINQ. The loop body pattern is not recognized. " +
            $"Accumulator: '{accumulator.Name}', element type: '{elementType.Name}'. " +
            $"Supported patterns: Sum, Count, Any, All, Min, Max, Select, and their conditional variants.");
    }

    // ── Count: acc = acc + 1, init = 0 ──────────────────────────────────────

    private bool TryMatchCountPattern(
        Expression body, ParameterExpression accumulator, Expression initValue,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // init must be 0
        if (!IsConstant(initValue, 0))
            return false;

        // body: Assign(acc, Add(acc, 1))
        if (body is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || assign.Right is not BinaryExpression { NodeType: ExpressionType.Add } add
            || add.Left != accumulator
            || !IsConstant(add.Right, 1))
            return false;

        // → collection.Count()
        result = Expression.Call(
            GetEnumerableMethod("Count", elementType, parameterless: true),
            collection);
        return true;
    }

    // ── Sum: acc = acc + expr(x), init = 0 ──────────────────────────────────

    private bool TryMatchSumPattern(
        Expression body, ParameterExpression accumulator, Expression initValue,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // init must be 0 or 0.0
        if (!IsNumericZero(initValue))
            return false;

        // body: Assign(acc, Add(acc, expr))
        if (body is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || assign.Right is not BinaryExpression { NodeType: ExpressionType.Add } add
            || add.Left != accumulator)
            return false;

        var selectorBody = add.Right;

        // If selector is just the iteration variable itself, use parameterless Sum
        if (selectorBody == iterVar)
        {
            result = Expression.Call(
                GetEnumerableMethod("Sum", elementType, parameterless: true),
                collection);
            return true;
        }

        // Otherwise: collection.Sum(x => selectorBody)
        var selectorLambda = Expression.Lambda(selectorBody, iterVar);
        result = Expression.Call(
            GetEnumerableSumWithSelector(elementType, selectorBody.Type),
            collection,
            selectorLambda);
        return true;
    }

    // ── Any: if (cond) found = true, init = false ───────────────────────────

    private bool TryMatchAnyPattern(
        Expression body, ParameterExpression accumulator, Expression initValue,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // init must be false
        if (!IsConstant(initValue, false))
            return false;

        // body: IfThen(condition, Assign(acc, true))
        if (body is not ConditionalExpression conditional
            || conditional.IfTrue is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || !IsConstant(assign.Right, true))
            return false;

        var condition = conditional.Test;
        var predicateLambda = Expression.Lambda(condition, iterVar);

        result = Expression.Call(
            GetEnumerableMethod("Any", elementType, hasLambda: true),
            collection,
            predicateLambda);
        return true;
    }

    // ── All: if (!cond) all = false, init = true ────────────────────────────

    private bool TryMatchAllPattern(
        Expression body, ParameterExpression accumulator, Expression initValue,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // init must be true
        if (!IsConstant(initValue, true))
            return false;

        // body: IfThen(Not(condition), Assign(acc, false))
        if (body is not ConditionalExpression conditional)
            return false;

        // The test is the negated condition: Not(originalCondition)
        Expression originalCondition;
        if (conditional.Test is UnaryExpression { NodeType: ExpressionType.Not } not)
        {
            originalCondition = not.Operand;
        }
        else
        {
            // The condition might be directly negated in a different way
            return false;
        }

        if (conditional.IfTrue is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || !IsConstant(assign.Right, false))
            return false;

        var predicateLambda = Expression.Lambda(originalCondition, iterVar);

        result = Expression.Call(
            GetEnumerableMethod("All", elementType, hasLambda: true),
            collection,
            predicateLambda);
        return true;
    }

    // ── Conditional Count: if (pred) acc = acc + 1 ──────────────────────────

    private bool TryMatchConditionalCount(
        Expression innerBody, ParameterExpression accumulator, Expression condition,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // innerBody: Assign(acc, Add(acc, 1))
        if (innerBody is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || assign.Right is not BinaryExpression { NodeType: ExpressionType.Add } add
            || add.Left != accumulator
            || !IsConstant(add.Right, 1))
            return false;

        var predicateLambda = Expression.Lambda(condition, iterVar);

        result = Expression.Call(
            GetEnumerableMethod("Count", elementType, hasLambda: true),
            collection,
            predicateLambda);
        return true;
    }

    // ── Conditional Sum: if (pred) acc = acc + expr ─────────────────────────

    private bool TryMatchConditionalSum(
        Expression innerBody, ParameterExpression accumulator, Expression condition,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // innerBody: Assign(acc, Add(acc, expr))
        if (innerBody is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || assign.Right is not BinaryExpression { NodeType: ExpressionType.Add } add
            || add.Left != accumulator)
            return false;

        var selectorBody = add.Right;

        // → collection.Where(x => condition).Sum(x => selectorBody)
        var predicateLambda = Expression.Lambda(condition, iterVar);
        var whereCall = Expression.Call(
            GetEnumerableMethod("Where", elementType, hasLambda: true),
            collection,
            predicateLambda);

        if (selectorBody == iterVar)
        {
            result = Expression.Call(
                GetEnumerableMethod("Sum", elementType, parameterless: true),
                whereCall);
        }
        else
        {
            var selectorLambda = Expression.Lambda(selectorBody, iterVar);
            result = Expression.Call(
                GetEnumerableSumWithSelector(elementType, selectorBody.Type),
                whereCall,
                selectorLambda);
        }

        return true;
    }

    // ── Min/Max: acc = Math.Min(acc, expr(x)) or Math.Max(acc, expr(x)) ────

    private bool TryMatchMinMaxPattern(
        Expression body, ParameterExpression accumulator,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // body: Assign(acc, Call(Math.Min/Max, acc, expr))
        if (body is not BinaryExpression { NodeType: ExpressionType.Assign } assign
            || assign.Left != accumulator
            || assign.Right is not MethodCallExpression mathCall
            || mathCall.Method.DeclaringType != typeof(Math)
            || (mathCall.Method.Name != "Min" && mathCall.Method.Name != "Max")
            || mathCall.Arguments.Count != 2
            || mathCall.Arguments[0] != accumulator)
            return false;

        var linqMethodName = mathCall.Method.Name; // "Min" or "Max"
        var selectorBody = mathCall.Arguments[1];

        if (selectorBody == iterVar)
        {
            result = Expression.Call(
                GetEnumerableMethod(linqMethodName, elementType, parameterless: true),
                collection);
        }
        else
        {
            var selectorLambda = Expression.Lambda(selectorBody, iterVar);
            var method = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == linqMethodName && m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 2
                    && m.GetGenericArguments().Length == 2);
            result = Expression.Call(
                method.MakeGenericMethod(elementType, selectorBody.Type),
                collection,
                selectorLambda);
        }

        return true;
    }

    // ── Select: acc.Add(expr(x)), acc = new List<T>() ───────────────────────

    private bool TryMatchSelectPattern(
        Expression body, ParameterExpression accumulator, Expression initValue,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // init must be new List<TResult>()
        if (initValue is not NewExpression newExpr
            || !initValue.Type.IsGenericType
            || initValue.Type.GetGenericTypeDefinition() != typeof(List<>))
            return false;

        // body: Call(accumulator, Add, selectorExpr)
        if (body is not MethodCallExpression addCall
            || addCall.Method.Name != "Add"
            || addCall.Object != accumulator
            || addCall.Arguments.Count != 1)
            return false;

        var selectorBody = addCall.Arguments[0];
        var resultElementType = selectorBody.Type;

        // → collection.Select(x => expr).ToList()
        var selectorLambda = Expression.Lambda(selectorBody, iterVar);

        var selectMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Select" && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 2
                && m.GetGenericArguments().Length == 2);
        var selectCall = Expression.Call(
            selectMethod.MakeGenericMethod(elementType, resultElementType),
            collection,
            selectorLambda);

        var toListMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "ToList" && m.IsGenericMethodDefinition);
        result = Expression.Call(
            toListMethod.MakeGenericMethod(resultElementType),
            selectCall);

        return true;
    }

    // ── Conditional Select: if (pred) acc.Add(expr(x)) ──────────────────────

    private bool TryMatchConditionalSelect(
        Expression innerBody, ParameterExpression accumulator, Expression condition,
        Expression collection, ParameterExpression iterVar, Type elementType,
        out Expression result)
    {
        result = null!;

        // innerBody: Call(accumulator, Add, selectorExpr)
        if (innerBody is not MethodCallExpression addCall
            || addCall.Method.Name != "Add"
            || addCall.Object != accumulator
            || addCall.Arguments.Count != 1)
            return false;

        var selectorBody = addCall.Arguments[0];
        var resultElementType = selectorBody.Type;

        // → collection.Where(x => condition).Select(x => expr).ToList()
        var predicateLambda = Expression.Lambda(condition, iterVar);
        var whereMethod = GetEnumerableMethod("Where", elementType, hasLambda: true);
        var whereCall = Expression.Call(whereMethod, collection, predicateLambda);

        var selectorLambda = Expression.Lambda(selectorBody, iterVar);
        var selectMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Select" && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 2
                && m.GetGenericArguments().Length == 2);
        var selectCall = Expression.Call(
            selectMethod.MakeGenericMethod(elementType, resultElementType),
            whereCall,
            selectorLambda);

        var toListMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "ToList" && m.IsGenericMethodDefinition);
        result = Expression.Call(
            toListMethod.MakeGenericMethod(resultElementType),
            selectCall);

        return true;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static bool IsConstant(Expression expr, object? value)
    {
        if (expr is ConstantExpression constant)
        {
            if (value is null)
                return constant.Value is null;
            return value.Equals(constant.Value);
        }

        // Check for default(T) which evaluates to 0/false/null
        if (expr is DefaultExpression && value is 0 or 0.0 or false or null)
            return true;

        return false;
    }

    private static bool IsNumericZero(Expression expr)
    {
        if (expr is ConstantExpression constant && constant.Value is not null)
        {
            return Convert.ToDouble(constant.Value) == 0.0;
        }

        return expr is DefaultExpression;
    }

    private static MethodInfo GetEnumerableMethod(string name, Type elementType, bool parameterless = false, bool hasLambda = false)
    {
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

        if (parameterless)
        {
            // Try non-generic overload first (e.g., Sum(IEnumerable<int>))
            var nonGeneric = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == name && !m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == enumerableType);
            if (nonGeneric is not null) return nonGeneric;

            // Fall back to generic overload (e.g., Count<T>(IEnumerable<T>))
            var generic = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == name && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);
            return generic.MakeGenericMethod(elementType);
        }

        if (hasLambda)
        {
            // Generic overload with predicate/selector (e.g., Count<T>(IEnumerable<T>, Func<T, bool>))
            var method = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == name && m.IsGenericMethodDefinition && m.GetParameters().Length == 2);
            return method.MakeGenericMethod(elementType);
        }

        throw new InvalidOperationException($"Could not find Enumerable.{name} for {elementType}");
    }

    private static MethodInfo GetEnumerableSumWithSelector(Type elementType, Type resultType)
    {
        // Enumerable.Sum<TSource>(IEnumerable<TSource>, Func<TSource, TResult>) — specific result type overloads
        // E.g., Sum<T>(IEnumerable<T>, Func<T, int>), Sum<T>(IEnumerable<T>, Func<T, double>), etc.
        var selectorFuncType = typeof(Func<,>).MakeGenericType(elementType, resultType);

        var methods = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "Sum" && m.GetParameters().Length == 2 && m.IsGenericMethodDefinition);

        foreach (var method in methods)
        {
            var genericMethod = method.MakeGenericMethod(elementType);
            var selectorParam = genericMethod.GetParameters()[1].ParameterType;
            if (selectorParam == selectorFuncType)
                return genericMethod;
        }

        throw new InvalidOperationException(
            $"Could not find Enumerable.Sum overload for element type '{elementType}' and result type '{resultType}'");
    }
}
