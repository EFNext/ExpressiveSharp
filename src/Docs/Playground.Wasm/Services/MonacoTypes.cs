namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

// DTOs for JSInterop with Monaco editor; replace BlazorMonaco's types with
// plain records that serialize cleanly to/from the Monaco JS API.

public sealed class MonacoPosition
{
    public int LineNumber { get; set; }
    public int Column { get; set; }
}

public sealed class MonacoRange
{
    public int StartLineNumber { get; set; }
    public int StartColumn { get; set; }
    public int EndLineNumber { get; set; }
    public int EndColumn { get; set; }
}

public sealed class MonacoEditorOptions
{
    public string Language { get; set; } = "plaintext";
    public string Value { get; set; } = "";
    public bool ReadOnly { get; set; }
    public int FontSize { get; set; } = 13;
    public string LineNumbers { get; set; } = "off";
    public int LineDecorationsWidth { get; set; } = 4;
    public string WordWrap { get; set; } = "off";
    public string RenderLineHighlight { get; set; } = "line";
    public MonacoScrollbarOptions? Scrollbar { get; set; }
}

public sealed class MonacoScrollbarOptions
{
    public bool? AlwaysConsumeMouseWheel { get; set; }
    public string? Vertical { get; set; }
    public string? Horizontal { get; set; }
    public int? VerticalScrollbarSize { get; set; }
    public int? HorizontalScrollbarSize { get; set; }
}

public sealed class MonacoMarkerData
{
    public int Severity { get; set; }
    public string Message { get; set; } = "";
    public int StartLineNumber { get; set; }
    public int StartColumn { get; set; }
    public int EndLineNumber { get; set; }
    public int EndColumn { get; set; }
}

public sealed class MonacoCompletionList
{
    public List<MonacoCompletionItem> Suggestions { get; set; } = new();
    public bool Incomplete { get; set; }
}

public sealed class MonacoCompletionItem
{
    public string Label { get; set; } = "";
    public int Kind { get; set; }
    public string InsertText { get; set; } = "";
    public string? SortText { get; set; }
    public string? FilterText { get; set; }
    public string? Detail { get; set; }
    public MonacoRange? Range { get; set; }
}

public sealed class MonacoHover
{
    public List<MonacoMarkdownString> Contents { get; set; } = new();
    public MonacoRange? Range { get; set; }
}

public sealed class MonacoMarkdownString
{
    public string Value { get; set; } = "";
    public bool IsTrusted { get; set; }
}

// Values match monaco.languages.CompletionItemKind.
public static class MonacoCompletionItemKind
{
    public const int Method = 0;
    public const int Function = 1;
    public const int Constructor = 2;
    public const int Field = 3;
    public const int Variable = 4;
    public const int Class = 5;
    public const int Struct = 6;
    public const int Interface = 7;
    public const int Module = 8;
    public const int Property = 9;
    public const int Event = 10;
    public const int Operator = 11;
    public const int Unit = 12;
    public const int Value = 13;
    public const int Constant = 14;
    public const int Enum = 15;
    public const int EnumMember = 16;
    public const int Keyword = 17;
    public const int Text = 18;
    public const int Color = 19;
    public const int Reference = 23;
    public const int Snippet = 27;
    public const int TypeParameter = 24;
}

// Values match monaco.MarkerSeverity.
public static class MonacoMarkerSeverity
{
    public const int Hint = 1;
    public const int Info = 2;
    public const int Warning = 4;
    public const int Error = 8;
}
