using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web;
using OSLC4Net.StockQuoteSample.Controllers;
using OSLC4Net.Core.DotNetRdfProvider;

namespace OSLC4Net.StockQuoteSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Custom initialization
            config.Formatters.Clear();
            config.Formatters.Insert(0, new RdfXmlMediaTypeFormatter());

            HttpContext context = HttpContext.Current;
            string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + "/api";
            ServiceProviderController.init(baseUrl);
        }
   
    }
}
