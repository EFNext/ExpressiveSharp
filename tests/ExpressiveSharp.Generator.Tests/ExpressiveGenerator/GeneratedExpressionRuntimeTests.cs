using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class GeneratedExpressionRuntimeTests : GeneratorTestBase
{
    [TestMethod]
    public void PolymorphicDispatch_DerivedExpressiveWithoutGeneratedBody_DoesNotBreakBaseExpansion()
    {
        var baseSource = """
            namespace PolyProofBase
            {
                public class Animal
                {
                    [ExpressiveSharp.Expressive]
                    public virtual string Label => "animal";
                }
            }
            """;
        var baseCompilation = CSharpCompilation.Create(
            "RuntimeProof.PolyBase",
            new[] { CSharpSyntaxTree.ParseText(baseSource) },
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        CSharpGeneratorDriver
            .Create(new global::ExpressiveSharp.Generator.ExpressiveGenerator())
            .RunGeneratorsAndUpdateCompilation(baseCompilation, out var baseWithGenerated, out _);
        var baseBytes = EmitOrFail(baseWithGenerated);

        var derivedSource = """
            namespace PolyProofDerived
            {
                public class Dog : PolyProofBase.Animal
                {
                    [ExpressiveSharp.Expressive]
                    public override string Label => "dog";
                }
            }
            """;
        var derivedCompilation = CSharpCompilation.Create(
            "RuntimeProof.PolyDerived",
            new[] { CSharpSyntaxTree.ParseText(derivedSource) },
            GetDefaultReferences().Append(MetadataReference.CreateFromImage(baseBytes)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var derivedBytes = EmitOrFail(derivedCompilation);

        var baseAssembly = Assembly.Load(baseBytes);
        ResolveEventHandler resolveHandler = (_, args) =>
            args.Name.StartsWith("RuntimeProof.PolyBase", StringComparison.Ordinal) ? baseAssembly : null;
        AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;
        try
        {
            var derivedAssembly = Assembly.Load(derivedBytes);

            var animalType = baseAssembly.GetType("PolyProofBase.Animal");
            Assert.IsNotNull(animalType);
            var dogType = derivedAssembly.GetType("PolyProofDerived.Dog");
            Assert.IsNotNull(dogType);
            Assert.IsTrue(animalType.IsAssignableFrom(dogType),
                "Precondition: Dog must load and derive from Animal so the polymorphic scan sees it.");

            var parameter = Expression.Parameter(animalType, "a");
            var lambda = Expression.Lambda(Expression.Property(parameter, "Label"), parameter);

            var expanded = global::ExpressiveSharp.ExpressionExtensions.ExpandExpressives(lambda);
            Assert.IsNotNull(expanded);
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;
        }
    }

    [TestMethod]
    public void EightElementTupleEquality_ComparesAllElements()
    {
        var source = """
            namespace TupleProof
            {
                public static class Fx
                {
                    [ExpressiveSharp.Expressive]
                    public static bool TupleEquals8(int a, int b)
                        => (1, 2, 3, 4, 5, 6, 7, a) == (1, 2, 3, 4, 5, 6, 7, b);

                    [ExpressiveSharp.Expressive]
                    public static bool TupleEquals15(int a, int b)
                        => (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, a)
                            == (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, b);
                }
            }
            """;
        var compilation = CSharpCompilation.Create(
            "RuntimeProof.Tuple8",
            new[] { CSharpSyntaxTree.ParseText(source) },
            GetDefaultReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        CSharpGeneratorDriver
            .Create(new global::ExpressiveSharp.Generator.ExpressiveGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var withGenerated, out _);
        var assembly = Assembly.Load(EmitOrFail(withGenerated));

        var equals8 = CompileExpanded(assembly, "TupleEquals8");
        Assert.IsFalse(equals8(8, 9), "Tuples differing only in the 8th element must not compare equal.");
        Assert.IsTrue(equals8(8, 8));

        var equals15 = CompileExpanded(assembly, "TupleEquals15");
        Assert.IsFalse(equals15(15, 16), "Tuples differing only in the 15th element must not compare equal.");
        Assert.IsTrue(equals15(15, 15));
    }

    private static Func<int, int, bool> CompileExpanded(Assembly assembly, string methodName)
    {
        var method = assembly.GetType("TupleProof.Fx")!.GetMethod(methodName)!;
        var x = Expression.Parameter(typeof(int), "x");
        var y = Expression.Parameter(typeof(int), "y");
        var lambda = Expression.Lambda<Func<int, int, bool>>(Expression.Call(method, x, y), x, y);

        var expanded = (Expression<Func<int, int, bool>>)
            global::ExpressiveSharp.ExpressionExtensions.ExpandExpressives(lambda);
        return expanded.Compile();
    }

    private static byte[] EmitOrFail(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        Assert.IsTrue(emitResult.Success,
            "Fixture compilation must succeed:\n" + string.Join("\n",
                emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
        return stream.ToArray();
    }
}
