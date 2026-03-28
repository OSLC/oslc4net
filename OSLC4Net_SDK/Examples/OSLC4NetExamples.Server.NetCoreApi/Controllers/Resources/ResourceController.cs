using Microsoft.AspNetCore.Mvc;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
public abstract partial class ResourceController<T>(ILogger<ResourceController<T>> logger) : ControllerBase
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Getting resource with id: {Id}")]
    protected partial void LogGetResource(string id);

    [HttpGet]
    public abstract T GetResource(string id);
}
