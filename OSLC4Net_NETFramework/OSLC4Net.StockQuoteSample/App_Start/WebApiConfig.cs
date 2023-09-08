using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.StockQuoteSample.Controllers;
using System;
using System.Web;
using System.Web.Http;

namespace OSLC4Net.StockQuoteSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Clear();
            config.Formatters.Add(new RdfXmlMediaTypeFormatter());

            HttpContext context = HttpContext.Current;
            // see https://github.com/OSLC/oslc4net/issues/12
            // string applicationBase = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/');
            string applicationBase = "http://localhost:7077";
            string baseUrl = applicationBase + "/api";
            ServiceProviderController.init(baseUrl);

        }
    }
}
