using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OSLC4Net.Core.Model;
using ServiceProvider = OSLC4Net.Core.Model.ServiceProvider;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

/// <summary>
/// In its current form, only a flat catalog is supported. The spec allows for nested catalogs.
/// </summary>
/// <param name="logger"></param>
[ApiController]
[Route("/oslc/catalog")]
[Produces(OslcMediaType.APPLICATION_RDF_XML, OslcMediaType.TEXT_TURTLE, OslcMediaType.APPLICATION_JSON_LD, OslcMediaType.APPLICATION_NTRIPLES)]
[Consumes(OslcMediaType.APPLICATION_RDF_XML, OslcMediaType.TEXT_TURTLE, OslcMediaType.APPLICATION_JSON_LD, OslcMediaType.APPLICATION_NTRIPLES)]
public class CatalogController(ILogger<CatalogController> logger) : ControllerBase
{
    [HttpGet]
    public ServiceProviderCatalog Get()
    {
        var catalog = new ServiceProviderCatalog();
        var sp = new ServiceProvider();
        sp.SetAbout(new Uri(Request.GetEncodedUrl()));
        sp.SetDescription("Dummy Service Provider");
        catalog.AddServiceProvider(sp);
        return catalog;
    }

    [HttpPut]
    public ServiceProviderCatalog Put(ServiceProvider sp)
    {
        var catalog = new ServiceProviderCatalog();
        catalog.AddServiceProvider(sp);
        return catalog;
    }
}
