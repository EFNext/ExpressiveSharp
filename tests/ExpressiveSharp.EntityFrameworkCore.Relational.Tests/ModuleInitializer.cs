using System.Runtime.CompilerServices;

namespace ExpressiveSharp.EntityFrameworkCore.Relational.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        if (Environment.GetEnvironmentVariable("VERIFY_AUTO_APPROVE") == "true")
        {
            VerifierSettings.AutoVerify();
        }
    }
}
