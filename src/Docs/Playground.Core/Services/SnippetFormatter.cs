using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExpressiveSharp.Docs.Playground.Core.Services;

public static class SnippetFormatter
{
    public static string Format(string snippet)
    {
        if (string.IsNullOrWhiteSpace(snippet)) return snippet;

        try
        {
            var expr = SyntaxFactory.ParseExpression(snippet);

            foreach (var diag in expr.GetDiagnostics())
                if (diag.Severity == DiagnosticSeverity.Error)
                    return snippet;

            var dotsToBreak = new List<SyntaxToken>();
            ExpressionSyntax cursor = expr;
            while (true)
            {
                if (cursor is InvocationExpressionSyntax invocation)
                {
                    cursor = invocation.Expression;
                    continue;
                }
                if (cursor is MemberAccessExpressionSyntax member)
                {
                    dotsToBreak.Add(member.OperatorToken);
                    cursor = member.Expression;
                    continue;
                }
                break;
            }

            if (dotsToBreak.Count < 2) return snippet;

            var newline = SyntaxFactory.EndOfLine("\n");
            var indent = SyntaxFactory.Whitespace("    ");
            var rewritten = expr.ReplaceTokens(dotsToBreak, (oldToken, _) =>
                oldToken.WithLeadingTrivia(
                    oldToken.LeadingTrivia.Add(newline).Add(indent)));

            return rewritten.ToFullString();
        }
        catch
        {
            return snippet;
        }
    }
}
