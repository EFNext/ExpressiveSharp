using ExpressiveSharp;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Models;

// TPH hierarchy exercising runtime polymorphic dispatch of a virtual [Expressive] member.
// Description is overridden per concrete type; expansion must emit an `is Dog`/`is Cat`
// type-test chain that EF Core translates to a discriminator CASE.
public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    [Expressive]
    public virtual string Description => "Animal: " + Name;
}

public class Dog : Animal
{
    public string Breed { get; set; } = "";

    // Breed is a Dog-only column: expansion casts the receiver to ((Dog)a).Breed under `is Dog`.
    [Expressive]
    public override string Description => "Dog:" + Breed;
}

public class Cat : Animal
{
    public string Color { get; set; } = "";

    [Expressive]
    public override string Description => "Cat:" + Color;
}
