namespace ExpressiveSharp.Tests.TestFixtures;

// For TypeExtensions tests — polymorphism resolution
public class Animal
{
    public virtual string Sound() => "...";
    public virtual string Name { get; } = "";
    public static void StaticMethod() { }
    public void NonVirtualMethod() { }
}

public class Dog : Animal
{
    public override string Sound() => "Woof";
    public override string Name => "Dog";
}

public interface IIdentifiable
{
    int Id { get; }
    string Describe();
}

public class Entity : IIdentifiable
{
    public int Id { get; set; }
    public string Describe() => $"Entity {Id}";
}

public class Outer
{
    public class Middle
    {
        public class Inner { }
    }
}

// For Resolver/Replacer tests — generator produces real expression registries
public class Product
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }

    [Expressive]
    public double Total => Price * Quantity;

    [Expressive]
    public string Label() => $"Product {Id}";
}
