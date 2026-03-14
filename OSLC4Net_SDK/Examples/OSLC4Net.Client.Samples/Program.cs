/*******************************************************************************
 * Sample Launcher - allows running any of the OSLC samples
 * Usage: dotnet run [ewm|erm|etm] -- --url ... --user ... --password ... --project ...
 *******************************************************************************/

using System.Globalization;
using System.Reflection;

namespace OSLC4Net.Client.Samples;

sealed class Program
{
    static async Task Main(string[] args)
    {
        // If first arg is a sample selector, use it; otherwise default to ETM
        string sampleName = "ETMSample";
        string[] sampleArgs = args;

        if (args.Length > 0 && !args[0].StartsWith("--", StringComparison.Ordinal))
        {
            sampleName = args[0].ToLower(CultureInfo.InvariantCulture) switch
            {
                "ewm" => "EWMSample",
                "erm" => "ERMSample",
                "etm" => "ETMSample",
                _ => "ETMSample"
            };
            // Remove the sample name from args
            sampleArgs = new string[args.Length - 1];
            Array.Copy(args, 1, sampleArgs, 0, args.Length - 1);
        }

        // Find and invoke the appropriate sample's Main method
        var sampleType = Type.GetType($"OSLC4Net.Client.Samples.{sampleName}");
        if (sampleType == null)
        {
            Console.Error.WriteLine($"Sample '{sampleName}' not found");
            Console.Error.WriteLine("Usage: dotnet run [ewm|erm|etm] -- --url ... --user ... --password ... --project ...");
            Environment.Exit(1);
        }

        var mainMethod = sampleType.GetMethod("Run",
            BindingFlags.Static | BindingFlags.Public,
            null, new[] { typeof(string[]) }, null);

        if (mainMethod == null)
        {
            Console.Error.WriteLine($"Run method not found in {sampleName}");
            Environment.Exit(1);
        }

        try
        {
            var result = mainMethod.Invoke(null, new object[] { sampleArgs });
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error running {sampleName}: {ex.InnerException ?? ex}");
            Environment.Exit(1);
        }
    }
}

