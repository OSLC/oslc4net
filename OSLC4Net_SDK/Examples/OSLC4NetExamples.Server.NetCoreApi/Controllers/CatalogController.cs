using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

/// <summary>
/// In its current form, only a flat catalog is supported. The spec allows for nested catalogs.
/// </summary>
/// <param name="logger"></param>
[ApiController]
[Route("/services/catalog")]
public class CatalogController(ILogger<CatalogController> logger) : ControllerBase
{
    [HttpGet]
    public OSLC4Net.Core.Model.ServiceProviderCatalog Get()
    {
        // TODO: inject a provider catalog service
        var catalog = new OSLC4Net.Core.Model.ServiceProviderCatalog();
        var sp = new OSLC4Net.Core.Model.ServiceProvider();
        sp.SetAbout(new Uri(Request.GetEncodedUrl()));
        sp.SetDescription("test me");
        catalog.AddServiceProvider(sp);
        return catalog;
    }
}
