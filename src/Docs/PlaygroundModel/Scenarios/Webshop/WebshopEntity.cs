namespace ExpressiveSharp.Docs.PlaygroundModel.Webshop;

// Shared base so Cosmos can host all 4 types in a single container, accessed
// as `DbSet<WebshopEntity>.OfType<T>()`. `Id` lives here because EF Core's TPH
// discriminator model requires the key on the root type.
public abstract class WebshopEntity
{
    public int Id { get; set; }
}
