using System.Globalization;
using System.Runtime.CompilerServices;

namespace ExpressiveSharp.Generator.Tests;

public static class VerifyInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        if (Environment.GetEnvironmentVariable("VERIFY_AUTO_APPROVE") == "true")
        {
            VerifierSettings.AutoVerify();
        }

        // Scrub InterceptsLocation attribute usages — the encoded data varies by file path.
        VerifierSettings.ScrubLinesWithReplace(line =>
        {
            if (!line.TrimStart().StartsWith("[") || !line.Contains("InterceptsLocationAttribute("))
                return line;
            return line[..line.IndexOf("InterceptsLocationAttribute(")] + "InterceptsLocationAttribute(/* scrubbed */)]";
        });

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
    }
}
