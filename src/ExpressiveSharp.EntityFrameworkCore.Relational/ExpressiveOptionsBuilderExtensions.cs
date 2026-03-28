using ExpressiveSharp.EntityFrameworkCore.Relational;

// ReSharper disable once CheckNamespace — intentionally in parent namespace for discoverability
namespace ExpressiveSharp.EntityFrameworkCore;

public static class ExpressiveOptionsBuilderExtensions
{
    /// <summary>
    /// Enables relational database extensions: SQL window functions
    /// (ROW_NUMBER, RANK, DENSE_RANK, NTILE) and indexed Select support.
    /// </summary>
    public static ExpressiveOptionsBuilder UseRelationalExtensions(this ExpressiveOptionsBuilder builder)
        => builder.AddPlugin(new RelationalExpressivePlugin());
}
