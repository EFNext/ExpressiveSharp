using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

public class ExpressiveExpandQueryFiltersConventionPlugin : IConventionSetPlugin
{
    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.ModelFinalizingConventions.Add(new ExpressiveExpandQueryFiltersConvention());
        return conventionSet;
    }
}
