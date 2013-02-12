using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.StockQuoteSample.Controllers
{
    public class ServiceProviderController : ApiController
    {
        static ServiceProviderController()
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new RdfXmlMediaTypeFormatter());
        }
    }
}
