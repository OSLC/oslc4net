namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

internal static partial class ControllerLogExtensions
{
    /// <summary>Replaces all Unicode line endings with a visible marker to prevent log forging (CWE-117).</summary>
    internal static string SanitizeForLog(this string value) => value.ReplaceLineEndings("↵");

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Getting service provider catalog.")]
    public static partial void LogGetCatalog(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Putting service provider catalog.")]
    public static partial void LogPutCatalog(this ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Getting service provider with id: {Id}")]
    public static partial void LogGetProvider(this ILogger logger, string id);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Getting resource with id: {Id}")]
    public static partial void LogGetResource(this ILogger logger, string id);
}
