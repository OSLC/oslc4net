using Microsoft.AspNetCore.Mvc;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
public abstract class ResourceController<T> : ControllerBase
{
    [HttpGet]
    public abstract T GetResource(string id);
    // {
    //     var tInstance = ActivatorUtilities.CreateInstance<T>(HttpContext.RequestServices, id);
    //     return tInstance;
    // }
}
