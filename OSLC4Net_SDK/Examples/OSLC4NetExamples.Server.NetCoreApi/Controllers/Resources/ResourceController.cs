using Microsoft.AspNetCore.Mvc;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
public abstract class ResourceController<T>(ILogger<ResourceController<T>> logger) : ControllerBase
{
    protected ILogger<ResourceController<T>> Logger { get; } = logger;

    [HttpGet]
    public abstract T GetResource(string id);
}
