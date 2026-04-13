using System.Collections.Immutable;
using System.Text;
using ExpressiveSharp.Docs.Playground.Core.Services;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;

using RoslynCompletionList = Microsoft.CodeAnalysis.Completion.CompletionList;

namespace ExpressiveSharp.Docs.Playground.Wasm.Services;

internal static class RoslynMonacoConverters
{
    public static int ToMonacoKind(ImmutableArray<string> tags)
    {
        foreach (var tag in tags)
        {
            switch (tag)
            {
                case WellKnownTags.Class: return MonacoCompletionItemKind.Class;
                case WellKnownTags.Constant: return MonacoCompletionItemKind.Constant;
                case WellKnownTags.Delegate: return MonacoCompletionItemKind.Method;
                case WellKnownTags.Enum: return MonacoCompletionItemKind.Enum;
                case WellKnownTags.EnumMember: return MonacoCompletionItemKind.EnumMember;
                case WellKnownTags.Event: return MonacoCompletionItemKind.Event;
                case WellKnownTags.ExtensionMethod: return MonacoCompletionItemKind.Method;
                case WellKnownTags.Field: return MonacoCompletionItemKind.Field;
                case WellKnownTags.Interface: return MonacoCompletionItemKind.Interface;
                case WellKnownTags.Intrinsic: return MonacoCompletionItemKind.Keyword;
                case WellKnownTags.Keyword: return MonacoCompletionItemKind.Keyword;
                case WellKnownTags.Label: return MonacoCompletionItemKind.Text;
                case WellKnownTags.Local: return MonacoCompletionItemKind.Variable;
                case WellKnownTags.Method: return MonacoCompletionItemKind.Method;
                case WellKnownTags.Module: return MonacoCompletionItemKind.Module;
                case WellKnownTags.Namespace: return MonacoCompletionItemKind.Module;
                case WellKnownTags.Operator: return MonacoCompletionItemKind.Operator;
                case WellKnownTags.Parameter: return MonacoCompletionItemKind.Variable;
                case WellKnownTags.Property: return MonacoCompletionItemKind.Property;
                case WellKnownTags.RangeVariable: return MonacoCompletionItemKind.Variable;
                case WellKnownTags.Reference: return MonacoCompletionItemKind.Reference;
                case WellKnownTags.Snippet: return MonacoCompletionItemKind.Snippet;
                case WellKnownTags.Structure: return MonacoCompletionItemKind.Struct;
                case WellKnownTags.TypeParameter: return MonacoCompletionItemKind.TypeParameter;
            }
        }
        return MonacoCompletionItemKind.Text;
    }

    public static MonacoRange? ToMonacoRange(TextSpan span, SourceText text, SnippetWrap wrap)
    {
        var lineSpan = text.Lines.GetLinePositionSpan(span);
        if (!wrap.IsInSnippet(lineSpan.Start))
            return null;

        var startRel = wrap.ToSnippetRelative(lineSpan.Start);
        var endRel = wrap.ToSnippetRelative(lineSpan.End);
        return new MonacoRange
        {
            StartLineNumber = startRel.Line + 1,
            StartColumn = startRel.Character + 1,
            EndLineNumber = endRel.Line + 1,
            EndColumn = endRel.Character + 1,
        };
    }

    public static MonacoCompletionList ToMonacoCompletionList(
        RoslynCompletionList list,
        SourceText text,
        SnippetWrap wrap)
    {
        var suggestions = new List<MonacoCompletionItem>(list.ItemsList.Count);
        foreach (var item in list.ItemsList)
        {
            if (item.IsComplexTextEdit) continue;
            var range = ToMonacoRange(item.Span, text, wrap);
            if (range is null) continue;

            suggestions.Add(new MonacoCompletionItem
            {
                Label = item.DisplayText,
                Kind = ToMonacoKind(item.Tags),
                InsertText = item.DisplayText,
                SortText = item.SortText,
                FilterText = item.FilterText,
                Range = range,
                Detail = item.InlineDescription,
            });
        }

        return new MonacoCompletionList
        {
            Suggestions = suggestions,
            Incomplete = false,
        };
    }

    public static MonacoHover? ToMonacoHover(QuickInfoItem item, SourceText text, SnippetWrap wrap)
    {
        var range = ToMonacoRange(item.Span, text, wrap);
        if (range is null) return null;

        var sb = new StringBuilder();
        for (var s = 0; s < item.Sections.Length; s++)
        {
            if (s > 0) sb.Append("\n\n");
            foreach (var part in item.Sections[s].TaggedParts)
                sb.Append(part.Text);
        }

        return new MonacoHover
        {
            Contents = new List<MonacoMarkdownString>
            {
                new() { Value = sb.ToString(), IsTrusted = false },
            },
            Range = range,
        };
    }
}
