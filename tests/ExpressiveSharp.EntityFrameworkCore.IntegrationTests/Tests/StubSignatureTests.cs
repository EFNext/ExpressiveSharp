using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressiveSharp.EntityFrameworkCore.IntegrationTests.Tests;

[TestClass]
public class StubSignatureTests
{
    [TestMethod]
    public void Stubs_TEntityClassConstraint_MatchesEfCoreTargets()
    {
        var efNamesWithClassConstraint = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.IsGenericMethodDefinition)
            .ToLookup(m => m.Name, HasClassConstraint);

        var overConstrained = typeof(ExpressiveQueryableEfCoreExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.IsGenericMethodDefinition && efNamesWithClassConstraint.Contains(m.Name))
            .Where(m => HasClassConstraint(m) && !efNamesWithClassConstraint[m.Name].Any(c => c))
            .Select(m => m.ToString())
            .ToList();

        Assert.AreEqual(0, overConstrained.Count);
    }

    private static bool HasClassConstraint(MethodInfo method) =>
        method.GetGenericArguments()[0].GenericParameterAttributes
            .HasFlag(GenericParameterAttributes.ReferenceTypeConstraint);
}
