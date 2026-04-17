using ExpressiveSharp.Services;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace ExpressiveSharp.EntityFrameworkCore.Infrastructure.Internal;

public class ExpressivePropertiesNotMappedConventionPlugin : IConventionSetPlugin
{
    private readonly IExpressiveResolver _resolver;

    public ExpressivePropertiesNotMappedConventionPlugin(IExpressiveResolver resolver)
    {
        _resolver = resolver;
    }

    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.EntityTypeAddedConventions.Add(new ExpressivePropertiesNotMappedConvention(_resolver));
        return conventionSet;
    }
}
