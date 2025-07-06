using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OSLC4Net.Core.Model;
using ServiceProvider = OSLC4Net.Core.Model.ServiceProvider;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
[Route("/oslc/service_provider")]
[Produces(OslcMediaType.APPLICATION_RDF_XML, OslcMediaType.TEXT_TURTLE, OslcMediaType.APPLICATION_JSON_LD, OslcMediaType.APPLICATION_NTRIPLES)]
public class ServiceProviderController(ILogger<ServiceProviderController> logger) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public ServiceProvider GetProvider(string id)
    {
        var sp = new ServiceProvider();
        sp.SetAbout(new Uri(Request.GetEncodedUrl()));
        sp.SetDescription($"Service Provider for {id}");
        return sp;
    }
}
