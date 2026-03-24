using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using ExpressiveSharp.Generator.Tests.Infrastructure;

namespace ExpressiveSharp.Generator.Tests.ExpressiveGenerator;

[TestClass]
public class ConstructorTests : GeneratorTestBase
{
    [TestMethod]
    public Task ProjectableConstructor_BodyAssignments()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PointDto {
                    public int X { get; set; }
                    public int Y { get; set; }

                    public PointDto() { }

                    [Expressive]
                    public PointDto(int x, int y) {
                        X = x;
                        Y = y;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithBaseInitializer()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Base {
                    public int Id { get; set; }
                    public Base(int id) { Id = id; }

                    protected Base() { }
                }

                class Child : Base {
                    public string Name { get; set; }

                    public Child() { }

                    [Expressive]
                    public Child(int id, string name) : base(id) {
                        Name = name;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_Overloads()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(string firstName, string lastName) {
                        FirstName = firstName;
                        LastName = lastName;
                    }

                    [Expressive]
                    public PersonDto(string fullName) {
                        FirstName = fullName;
                        LastName = string.Empty;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(2, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees.OrderBy(t => t.FilePath).Select(t => t.ToString()));
    }

    [TestMethod]
    public Task ProjectableConstructor_WithClassArgument()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class SourceEntity {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }

                class PersonDto {
                    public int Id { get; set; }
                    public string Name { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(SourceEntity source) {
                        Id = source.Id;
                        Name = source.Name;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithMultipleClassArguments()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class NamePart {
                    public string Value { get; set; }
                }

                class PersonDto {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(NamePart first, NamePart last) {
                        FirstName = first.Value;
                        LastName = last.Value;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithIfElseLogic()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string Label { get; set; }
                    public int Score { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(int score) {
                        Score = score;
                        if (score >= 90) {
                            Label = "A";
                        } else {
                            Label = "B";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithLocalVariable()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string FullName { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(string first, string last) {
                        var full = first + " " + last;
                        FullName = full;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithBaseInitializerExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Base {
                    public string Code { get; set; }
                    public Base(string code) { Code = code; }

                    protected Base() { }
                }

                class Child : Base {
                    public string Name { get; set; }

                    public Child() { }

                    [Expressive]
                    public Child(string name, string rawCode) : base(rawCode.ToUpper()) {
                        Name = name;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithBaseInitializerAndIfElse()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Base {
                    public int Id { get; set; }
                    public Base(int id) {
                        if (id < 0) {
                            Id = 0;
                        } else {
                            Id = id;
                        }
                    }

                    protected Base() { }
                }

                class Child : Base {
                    public string Name { get; set; }

                    public Child() { }

                    [Expressive]
                    public Child(int id, string name) : base(id) {
                        Name = name;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithIfNoElse()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string Label { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(int score) {
                        Label = "none";
                        if (score >= 90) {
                            Label = "A";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ReferencingPreviouslyAssignedProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(string firstName, string lastName) {
                        FirstName = firstName;
                        LastName = lastName;
                        FullName = FirstName + " " + LastName;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ReferencingBasePropertyInDerivedBody()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Base {
                    public string Code { get; set; }
                    public Base(string code) { Code = code; }

                    protected Base() { }
                }

                class Child : Base {
                    public string Label { get; set; }

                    public Child() { }

                    [Expressive]
                    public Child(string code) : base(code) {
                        Label = "[" + Code + "]";
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ReferencingStaticConstMember()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    internal const string Separator = " - ";
                    public string FullName { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(string first, string last) {
                        FullName = first + Separator + last;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ReferencingPreviouslyAssignedInBaseCtor()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Base {
                    public int X { get; set; }
                    public int Y { get; set; }
                    public Base(int x, int y) {
                        X = x;
                        Y = x + y;
                    }

                    protected Base() { }
                }

                class Child : Base {
                    public int Sum { get; set; }

                    public Child() { }

                    [Expressive]
                    public Child(int a, int b) : base(a, b) {
                        Sum = X + Y;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ThisInitializer_SimpleOverload()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }

                    public PersonDto() { }

                    public PersonDto(string firstName, string lastName) {
                        FirstName = firstName;
                        LastName = lastName;
                    }

                    [Expressive]
                    public PersonDto(string fullName) : this(fullName.ToUpper(), fullName.ToLower()) {
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ThisInitializer_WithBodyAfter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName { get; set; }

                    public PersonDto() { }

                    public PersonDto(string firstName, string lastName) {
                        FirstName = firstName;
                        LastName = lastName;
                    }

                    [Expressive]
                    public PersonDto(string fn, string ln, bool upper) : this(fn, ln) {
                        FullName = upper ? (FirstName + " " + LastName).ToUpper() : FirstName + " " + LastName;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ThisInitializer_WithIfElseInDelegated()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string Label { get; set; }
                    public int Score { get; set; }

                    public PersonDto() { }

                    public PersonDto(int score) {
                        Score = score;
                        if (score >= 90) {
                            Label = "A";
                        } else {
                            Label = "B";
                        }
                    }

                    [Expressive]
                    public PersonDto(int score, string prefix) : this(score) {
                        Label = prefix + Label;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ThisInitializer_ChainedThisAndBase()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Base {
                    public int Id { get; set; }
                    public Base(int id) { Id = id; }
                }

                class Child : Base {
                    public string Name { get; set; }

                    public Child() : base(0) { }

                    public Child(int id, string name) : base(id) {
                        Name = name;
                    }

                    [Expressive]
                    public Child(int id, string name, string suffix) : this(id, name) {
                        Name = Name + suffix;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_ThisInitializer_RefPreviouslyAssignedProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public string FullName { get; set; }

                    public PersonDto() { }

                    public PersonDto(string firstName, string lastName) {
                        FirstName = firstName;
                        LastName = lastName;
                        FullName = FirstName + " " + LastName;
                    }

                    [Expressive]
                    public PersonDto(string firstName) : this(firstName, "Doe") {
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public void ProjectableConstructor_WithoutParameterlessConstructor_EmitsDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string Name { get; set; }

                    [Expressive]
                    public PersonDto(string name) {
                        Name = name;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(1, result.Diagnostics.Length);
        Assert.AreEqual("EXP0002", result.Diagnostics[0].Id);
        Assert.AreEqual(DiagnosticSeverity.Error, result.Diagnostics[0].Severity);
        Assert.AreEqual(0, result.GeneratedTrees.Length);
    }

    [TestMethod]
    public Task ProjectableConstructor_WithExplicitParameterlessConstructor_Succeeds()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PersonDto {
                    public string Name { get; set; }

                    public PersonDto() { }

                    [Expressive]
                    public PersonDto(string name) {
                        Name = name;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithElseIfChain()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class GradeDto {
                    public string Grade { get; set; }

                    public GradeDto() { }

                    [Expressive]
                    public GradeDto(int score) {
                        if (score >= 90) {
                            Grade = "A";
                        } else if (score >= 75) {
                            Grade = "B";
                        } else if (score >= 60) {
                            Grade = "C";
                        } else {
                            Grade = "F";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithNestedIfElse()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class StatusDto {
                    public string Status { get; set; }

                    public StatusDto() { }

                    [Expressive]
                    public StatusDto(bool isActive, bool isPremium) {
                        if (isActive) {
                            if (isPremium) {
                                Status = "Active Premium";
                            } else {
                                Status = "Active Free";
                            }
                        } else {
                            Status = "Inactive";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithEarlyReturn_GuardClause()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class ItemDto {
                    public string Name { get; set; }
                    public string Category { get; set; }

                    public ItemDto() { }

                    [Expressive]
                    public ItemDto(string name, string category) {
                        Name = name;
                        if (string.IsNullOrEmpty(category)) {
                            Category = "Unknown";
                            return;
                        }
                        Category = category;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithMultipleEarlyReturns()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class PriorityDto {
                    public string Level { get; set; }

                    public PriorityDto() { }

                    [Expressive]
                    public PriorityDto(int value) {
                        if (value < 0) {
                            Level = "Invalid";
                            return;
                        }
                        if (value == 0) {
                            Level = "None";
                            return;
                        }
                        if (value <= 5) {
                            Level = "Low";
                            return;
                        }
                        Level = "High";
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithSequentialIfs()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class FlagDto {
                    public string Tag { get; set; }
                    public bool IsVerified { get; set; }
                    public bool IsAdmin { get; set; }

                    public FlagDto() { }

                    [Expressive]
                    public FlagDto(string role, bool verified) {
                        Tag = role;
                        if (verified) {
                            IsVerified = true;
                        }
                        if (role == "admin") {
                            IsAdmin = true;
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithTernaryAssignment()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class LabelDto {
                    public string Label { get; set; }
                    public string Display { get; set; }

                    public LabelDto() { }

                    [Expressive]
                    public LabelDto(string name, bool uppercase) {
                        Label = name;
                        Display = uppercase ? name.ToUpper() : name.ToLower();
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithNullCoalescing()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class ProductDto {
                    public string Name { get; set; }
                    public string Description { get; set; }

                    public ProductDto() { }

                    [Expressive]
                    public ProductDto(string name, string description) {
                        Name = name ?? "Unnamed";
                        Description = description ?? string.Empty;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithSwitchExpression()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class SeasonDto {
                    public string Name { get; set; }
                    public string Description { get; set; }

                    public SeasonDto() { }

                    [Expressive]
                    public SeasonDto(int month) {
                        Name = month switch {
                            12 or 1 or 2 => "Winter",
                            3 or 4 or 5  => "Spring",
                            6 or 7 or 8  => "Summer",
                            _            => "Autumn"
                        };
                        Description = "Month: " + month;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithMultipleLocalVariables()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class AddressDto {
                    public string Street { get; set; }
                    public string City { get; set; }
                    public string Full { get; set; }

                    public AddressDto() { }

                    [Expressive]
                    public AddressDto(string street, string city, string country) {
                        var trimmedStreet = street.Trim();
                        var trimmedCity   = city.Trim();
                        Street = trimmedStreet;
                        City   = trimmedCity;
                        Full   = trimmedStreet + ", " + trimmedCity + ", " + country;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithNullableParameter()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class MeasurementDto {
                    public double Value { get; set; }
                    public string Unit { get; set; }

                    public MeasurementDto() { }

                    [Expressive]
                    public MeasurementDto(double? value, string unit) {
                        Value = value ?? 0.0;
                        Unit  = unit ?? "m";
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithLocalVariableUsedInCondition()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class RangeDto {
                    public int Min { get; set; }
                    public int Max { get; set; }
                    public bool IsValid { get; set; }

                    public RangeDto() { }

                    [Expressive]
                    public RangeDto(int a, int b) {
                        var lo = a < b ? a : b;
                        var hi = a < b ? b : a;
                        Min = lo;
                        Max = hi;
                        if (hi - lo > 0) {
                            IsValid = true;
                        } else {
                            IsValid = false;
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithBaseInitializer_AndIfElse_InDerivedBody()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Animal {
                    public string Species { get; set; }
                    public Animal(string species) { Species = species; }
                    protected Animal() { }
                }

                class Pet : Animal {
                    public string Name { get; set; }
                    public string Nickname { get; set; }

                    public Pet() { }

                    [Expressive]
                    public Pet(string species, string name, bool useShortName) : base(species) {
                        Name = name;
                        if (useShortName) {
                            Nickname = name.Length > 3 ? name.Substring(0, 3) : name;
                        } else {
                            Nickname = name;
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithBaseInitializer_AndEarlyReturn()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class Vehicle {
                    public string Type { get; set; }
                    public Vehicle(string type) { Type = type; }
                    protected Vehicle() { }
                }

                class Car : Vehicle {
                    public string Model { get; set; }
                    public int Year { get; set; }

                    public Car() { }

                    [Expressive]
                    public Car(string model, int year) : base("Car") {
                        Model = model;
                        if (year <= 0) {
                            Year = 2000;
                            return;
                        }
                        Year = year;
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithDeepNestedIf()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class AccessDto {
                    public string Access { get; set; }

                    public AccessDto() { }

                    [Expressive]
                    public AccessDto(bool isLoggedIn, bool isVerified, bool isAdmin) {
                        if (isLoggedIn) {
                            if (isVerified) {
                                if (isAdmin) {
                                    Access = "Full";
                                } else {
                                    Access = "Verified";
                                }
                            } else {
                                Access = "Unverified";
                            }
                        } else {
                            Access = "Guest";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithIfInsideLocalScope_AndElse()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class OrderDto {
                    public string Status { get; set; }
                    public string Note { get; set; }
                    public bool NeedsReview { get; set; }

                    public OrderDto() { }

                    [Expressive]
                    public OrderDto(int amount, bool flagged) {
                        if (flagged) {
                            Status = "Flagged";
                            Note = "Requires manual review";
                            NeedsReview = true;
                        } else {
                            Status = amount > 1000 ? "Large" : "Normal";
                            Note = string.Empty;
                            NeedsReview = false;
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithThisInitializer_AndElseIfInBody()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class EventDto {
                    public string Title { get; set; }
                    public string Tag { get; set; }
                    public string Priority { get; set; }

                    public EventDto() { }

                    public EventDto(string title, string tag) {
                        Title = title;
                        Tag   = tag;
                    }

                    [Expressive]
                    public EventDto(string title, string tag, int urgency) : this(title, tag) {
                        if (urgency >= 10) {
                            Priority = "Critical";
                        } else if (urgency >= 5) {
                            Priority = "High";
                        } else {
                            Priority = "Normal";
                        }
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithSwitchExpression_AndExtraProperty()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                class ShapeDto {
                    public string ShapeType { get; set; }
                    public int Sides { get; set; }
                    public string Description { get; set; }

                    public ShapeDto() { }

                    [Expressive]
                    public ShapeDto(int sides) {
                        Sides = sides;
                        ShapeType = sides switch {
                            3 => "Triangle",
                            4 => "Rectangle",
                            5 => "Pentagon",
                            _ => "Polygon"
                        };
                        Description = ShapeType + " with " + sides + " sides";
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }

    [TestMethod]
    public Task ProjectableConstructor_WithFullObject()
    {
        var compilation = CreateCompilation(
            """
            namespace Foo {
                public class Customer {
                    public int Id { get; set; }
                    public string FirstName { get; set; }
                    public string LastName { get; set; }
                    public bool IsActive { get; set; }
                    public ICollection<Order> Orders { get; set; }
                }

                public class Order {
                    public int Id { get; set; }
                    public decimal Amount { get; set; }
                }

                public class CustomerDto {
                    public int Id { get; set; }
                    public string FullName { get; set; }
                    public bool IsActive { get; set; }
                    public int OrderCount { get; set; }

                    public CustomerDto() { }

                    [Expressive]
                    public CustomerDto(Customer customer)
                    {
                        Id = customer.Id;
                        FullName = customer.FirstName + " " + customer.LastName;
                        IsActive = customer.IsActive;
                        OrderCount = customer.Orders.Count();
                    }
                }
            }
            """);
        var result = RunExpressiveGenerator(compilation);

        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);

        return Verifier.Verify(result.GeneratedTrees[0].ToString());
    }
}
