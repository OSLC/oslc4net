using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
public abstract class ResourceController<T>(ILogger<ResourceController<T>> logger) : ControllerBase
{
    [HttpGet]
    public abstract T GetResource(string id);
    // {
    //     var tInstance = ActivatorUtilities.CreateInstance<T>(HttpContext.RequestServices, id);
    //     return tInstance;
    // }
}
