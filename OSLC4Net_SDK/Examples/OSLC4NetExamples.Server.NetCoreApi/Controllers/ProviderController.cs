using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
[Route("/services/provider")]
public class ProviderController(ILogger<ProviderController> logger) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public OSLC4Net.Core.Model.ServiceProvider GetProvider(string id)
    {
        var sp = new OSLC4Net.Core.Model.ServiceProvider();
        sp.SetAbout(new Uri(Request.GetEncodedUrl()));
        sp.SetDescription("test me");
        return sp;
    }
}
