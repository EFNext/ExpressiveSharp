namespace ExpressiveSharp.Generator.Emitter;

/// <summary>
/// The result of translating a C# expression body into imperative
/// <c>System.Linq.Expressions.Expression.*</c> factory calls.
/// </summary>
internal sealed class EmitResult
{
    /// <summary>
    /// C# statements that build the expression tree (the method body).
    /// The last statement is a <c>return Expression.Lambda&lt;…&gt;(…);</c>.
    /// </summary>
    public string Body { get; }

    public EmitResult(string body)
    {
        Body = body;
    }
}
