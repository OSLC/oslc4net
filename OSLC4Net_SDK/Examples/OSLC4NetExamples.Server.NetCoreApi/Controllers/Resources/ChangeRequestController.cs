using Microsoft.AspNetCore.Mvc;
using OSLC4Net.ChangeManagement;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
[Route("resources/[controller]")]
public abstract class ChangeRequestController : ResourceController<ChangeRequest>
{
    [HttpGet]
    [Route("{id}")]
    public override ChangeRequest GetResource(string id)
    {
        var tInstance = ActivatorUtilities.CreateInstance<ChangeRequest>(HttpContext.RequestServices, id);
        return tInstance;
    }
}
