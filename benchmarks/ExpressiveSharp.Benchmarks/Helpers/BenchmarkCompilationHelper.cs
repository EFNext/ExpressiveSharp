using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ExpressiveSharp.Benchmarks.Helpers;

public static class BenchmarkCompilationHelper
{
    public static Compilation CreateCompilation(IReadOnlyList<string> expressiveSources)
    {
        var noiseSources = BuildNoiseSources(expressiveSources.Count);
        var allSources = expressiveSources.Concat(noiseSources);

        var references = Basic.Reference.Assemblies.Net100.References.All.ToList();
        references.Add(MetadataReference.CreateFromFile(typeof(ExpressiveAttribute).Assembly.Location));

        return CSharpCompilation.Create(
            "GeneratorBenchmarkInput",
            allSources.Select((s, idx) => CSharpSyntaxTree.ParseText(s, path: $"File{idx}.cs")),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    /// <summary>
    /// Splits <paramref name="expressiveCount"/> members across multiple synthetic source
    /// files so that each file holds between 1 and 9 members (one chunk per file).
    /// Nine is the natural cycle length (one per emitter path).
    /// </summary>
    public static IReadOnlyList<string> BuildExpressiveSources(int expressiveCount)
    {
        const int membersPerFile = 9;
        var sources = new List<string>();

        var enumEmitted = false;
        var fileIndex = 0;
        for (var start = 0; start < expressiveCount; start += membersPerFile)
        {
            var count = Math.Min(membersPerFile, expressiveCount - start);
            sources.Add(BuildOrderClassSource(fileIndex, start, count, emitEnum: !enumEmitted));
            enumEmitted = true;
            fileIndex++;
        }

        if (!enumEmitted)
        {
            sources.Add(
                "using System;\n" +
                "using ExpressiveSharp;\n" +
                "namespace GeneratorBenchmarkInput;\n" +
                "public enum OrderStatus { Pending, Active, Completed, Cancelled }\n");
        }

        var ctorCount = Math.Max(1, expressiveCount / 9);
        for (var j = 0; j < ctorCount; j++)
        {
            sources.Add(BuildDtoClassSource(j));
        }

        return sources;
    }

    public static IReadOnlyList<string> BuildNoiseSources(int count)
        => Enumerable.Range(0, Math.Max(1, count))
            .Select(BuildNoiseClassSource)
            .ToList();

    // Nine member kinds — one per emitter path in the generator:
    //  0  simple string-concat property
    //  1  boolean null-check property
    //  2  single-param decimal method
    //  3  multi-param string method
    //  4  null-conditional property
    //  5  switch-expression method
    //  6  is-pattern property
    //  7  block-bodied if/else chain
    //  8  block-bodied switch with local var
    private static string BuildOrderClassSource(int fileIndex, int startIndex, int count, bool emitEnum)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using ExpressiveSharp;");
        sb.AppendLine();
        sb.AppendLine("namespace GeneratorBenchmarkInput;");
        sb.AppendLine();

        if (emitEnum)
        {
            sb.AppendLine("public enum OrderStatus { Pending, Active, Completed, Cancelled }");
            sb.AppendLine();
        }

        sb.AppendLine($"public class Order{fileIndex}");
        sb.AppendLine("{");
        sb.AppendLine("    public string FirstName { get; set; } = string.Empty;");
        sb.AppendLine("    public string LastName { get; set; } = string.Empty;");
        sb.AppendLine("    public string? Email { get; set; }");
        sb.AppendLine("    public decimal Amount { get; set; }");
        sb.AppendLine("    public decimal TaxRate { get; set; }");
        sb.AppendLine("    public DateTime? DeletedAt { get; set; }");
        sb.AppendLine("    public bool IsEnabled { get; set; }");
        sb.AppendLine("    public int Priority { get; set; }");
        sb.AppendLine("    public OrderStatus Status { get; set; }");
        sb.AppendLine();

        for (var i = startIndex; i < startIndex + count; i++)
        {
            switch (i % 9)
            {
                case 0:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public string FullName{i} => $\"{{FirstName}} {{LastName}}\";");
                    break;
                case 1:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public bool IsActive{i} => DeletedAt == null && IsEnabled;");
                    break;
                case 2:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public decimal TotalWithTax{i}(decimal taxRate) => Amount * (1 + taxRate);");
                    break;
                case 3:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public string FormatSummary{i}(string prefix, int count) => $\"{{prefix}}: {{FirstName}} x{{count}}\";");
                    break;
                case 4:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public int? EmailLength{i} => Email?.Length;");
                    break;
                case 5:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public string GetGrade{i}(int score) => score switch {{ >= 90 => \"A\", >= 70 => \"B\", _ => \"C\" }};");
                    break;
                case 6:
                    sb.AppendLine("    [Expressive]");
                    sb.AppendLine($"    public bool HasEmail{i} => Email is not null;");
                    break;
                case 7:
                    sb.AppendLine("    [Expressive(AllowBlockBody = true)]");
                    sb.AppendLine($"    public string GetStatusLabel{i}()");
                    sb.AppendLine("    {");
                    sb.AppendLine("        if (DeletedAt != null)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            return \"Deleted\";");
                    sb.AppendLine("        }");
                    sb.AppendLine("        if (IsEnabled)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            return $\"Active: {FirstName}\";");
                    sb.AppendLine("        }");
                    sb.AppendLine("        return \"Inactive\";");
                    sb.AppendLine("    }");
                    break;
                case 8:
                    sb.AppendLine("    [Expressive(AllowBlockBody = true)]");
                    sb.AppendLine($"    public string GetPriorityName{i}()");
                    sb.AppendLine("    {");
                    sb.AppendLine("        var p = Priority;");
                    sb.AppendLine("        switch (p)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            case 1: return \"Low\";");
                    sb.AppendLine("            case 2: return \"Medium\";");
                    sb.AppendLine("            case 3: return \"High\";");
                    sb.AppendLine("            default: return \"Unknown\";");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    break;
            }

            sb.AppendLine();
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string BuildDtoClassSource(int j)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using ExpressiveSharp;");
        sb.AppendLine();
        sb.AppendLine("namespace GeneratorBenchmarkInput;");
        sb.AppendLine();
        sb.AppendLine($"public class OrderSummaryDto{j}");
        sb.AppendLine("{");
        sb.AppendLine("    public string FullName { get; set; } = string.Empty;");
        sb.AppendLine("    public decimal Total { get; set; }");
        sb.AppendLine("    public bool IsActive { get; set; }");
        sb.AppendLine();
        sb.AppendLine($"    public OrderSummaryDto{j}() {{ }}");
        sb.AppendLine();
        sb.AppendLine("    [Expressive]");
        sb.AppendLine($"    public OrderSummaryDto{j}(string firstName, string lastName, decimal amount, decimal taxRate, bool isActive)");
        sb.AppendLine("    {");
        sb.AppendLine("        FullName = $\"{firstName} {lastName}\";");
        sb.AppendLine("        Total = amount * (1 + taxRate);");
        sb.AppendLine("        IsActive = isActive && amount > 0;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();

        return sb.ToString();
    }

    private static string BuildNoiseClassSource(int j)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("namespace GeneratorBenchmarkInput;");
        sb.AppendLine();
        sb.AppendLine($"public class NoiseEntity{j}");
        sb.AppendLine("{");
        sb.AppendLine("    public int Id { get; set; }");
        sb.AppendLine("    public string Name { get; set; } = string.Empty;");
        sb.AppendLine("    public DateTime CreatedAt { get; set; }");
        sb.AppendLine("    public bool IsEnabled { get; set; }");
        sb.AppendLine("    public decimal Amount { get; set; }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
