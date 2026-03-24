using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

public class ExpressiveExpandQueryFiltersConventionPlugin : IConventionSetPlugin
{
    private readonly ExpressiveOptions _options;

    public ExpressiveExpandQueryFiltersConventionPlugin(ExpressiveOptions options)
    {
        _options = options;
    }

    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.ModelFinalizingConventions.Add(new ExpressiveExpandQueryFiltersConvention(_options));
        return conventionSet;
    }
}
