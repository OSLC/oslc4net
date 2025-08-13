using System.Text;
using Microsoft.AspNetCore.Mvc;
using OSLC4NetExamples.Server.NetCoreApi.Models;

namespace OSLC4NetExamples.Server.NetCoreApi.Controllers;

[ApiController]
[Route("/.well-known/oslc/rootservices.xml")]
public class RootServicesController(ILogger<RootServicesController> logger) : ControllerBase
{
    [HttpGet]
    [Produces("application/rdf+xml")]
    public IActionResult GetRootServices()
    {
        try
        {
            // Get the base URL from the current request
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var rootServicesUrl = $"{baseUrl}/.well-known/oslc/rootservices.xml";
            var catalogUrl = $"{baseUrl}/oslc/catalog";

            // Create the DTO with dynamic URLs based on the request
            var rootServices = new RootServicesDto
            {
                About = rootServicesUrl,
                AmServiceProviders = new ResourceReference { Resource = catalogUrl },
                RmServiceProviders = new ResourceReference { Resource = catalogUrl },
                CmServiceProviders = new ResourceReference { Resource = catalogUrl },
                OAuthRealmName = "OSLC",
                OAuthDomain = $"{baseUrl}/",
                OAuthRequestConsumerKeyUrl = new ResourceReference { Resource = $"{baseUrl}/services/oauth/requestKey" },
                OAuthApprovalModuleUrl = new ResourceReference { Resource = $"{baseUrl}/services/oauth/approveKey" },
                OAuthRequestTokenUrl = new ResourceReference { Resource = $"{baseUrl}/services/oauth/requestToken" },
                OAuthUserAuthorizationUrl = new ResourceReference { Resource = $"{baseUrl}/services/oauth/authorize" },
                OAuthAccessTokenUrl = new ResourceReference { Resource = $"{baseUrl}/services/oauth/accessToken" }
            };            // Serialize to XML with proper formatting
            var xmlContent = rootServices.ToXml();

            logger.LogDebug("Generated root services XML for base URL: {BaseUrl}", baseUrl.Replace("\n", "\\n", StringComparison.Ordinal));

            return Content(xmlContent, "application/rdf+xml", Encoding.UTF8);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating root services XML");
            return StatusCode(500, "Internal server error while generating root services");
        }
    }
}
