using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

public class ExpressivePropertiesNotMappedConventionPlugin : IConventionSetPlugin
{
    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.EntityTypeAddedConventions.Add(new ExpressivePropertiesNotMappedConvention());
        return conventionSet;
    }
}
