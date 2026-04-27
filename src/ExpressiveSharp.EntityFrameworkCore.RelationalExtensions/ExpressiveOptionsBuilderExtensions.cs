using ExpressiveSharp.EntityFrameworkCore.RelationalExtensions;

// ReSharper disable once CheckNamespace — intentionally in parent namespace for discoverability
namespace ExpressiveSharp.EntityFrameworkCore;

public static class ExpressiveOptionsBuilderExtensions
{
    /// <summary>
    /// Enables relational extensions: SQL window functions and indexed Select support.
    /// </summary>
    public static ExpressiveOptionsBuilder UseRelationalExtensions(this ExpressiveOptionsBuilder builder)
        => builder.AddPlugin(new RelationalExpressivePlugin());
}
