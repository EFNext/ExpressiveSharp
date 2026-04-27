namespace ExpressiveSharp.Generator.Emitter;

internal sealed class EmitResult
{
    // C# statements that build the expression tree (the method body); ends with a return.
    public string Body { get; }

    public EmitResult(string body)
    {
        Body = body;
    }
}
