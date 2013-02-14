/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *  
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Michael Fiedler  - initial API and implementation
 *******************************************************************************/
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
using OSLC4Net.StockQuoteSample.Models;


namespace OSLC4Net.StockQuoteSample.Controllers
{
    public class ServiceProviderController : ApiController
    {
        public static Uri About { get; set; }
        public static Uri ServiceProviderUri { get; set; }

        private static ServiceProvider serviceProvider;
        private const string SERVICE_PROVIDER_PATH = "serviceprovider";

        public static void init(string baseUrl)
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new RdfXmlMediaTypeFormatter());

            serviceProvider = ServiceProviderFactory.CreateServiceProvider(baseUrl,
                                                                     "StockQuote Service Provider",
                                                                     "Sample OSLC Service Provider for a Stock Quote service",
                                                                     new Publisher("Codeplex OSLC4Net", "urn:codeplex:oslc4net"),
                                                                     new Type[] {typeof(StockQuoteController)});

            PrefixDefinition[] prefixDefinitions =
            {
                new PrefixDefinition(OslcConstants.DCTERMS_NAMESPACE_PREFIX,   new Uri(OslcConstants.DCTERMS_NAMESPACE)),
                new PrefixDefinition(OslcConstants.OSLC_CORE_NAMESPACE_PREFIX, new Uri(OslcConstants.OSLC_CORE_NAMESPACE)),
                new PrefixDefinition(OslcConstants.RDF_NAMESPACE_PREFIX,       new Uri(OslcConstants.RDF_NAMESPACE)),
                new PrefixDefinition(OslcConstants.RDFS_NAMESPACE_PREFIX,      new Uri(OslcConstants.RDFS_NAMESPACE)),
                new PrefixDefinition(Constants.STOCK_QUOTE_NAMESPACE_PREFIX,   new Uri(Constants.STOCK_QUOTE_NAMESPACE))
            };

            serviceProvider.SetPrefixDefinitions(prefixDefinitions);

            About = new Uri(baseUrl + "/" + Constants.PATH_STOCK_QUOTE);
            serviceProvider.SetAbout(About);

            ServiceProviderUri = new Uri(baseUrl + "/" + SERVICE_PROVIDER_PATH);

        }

        public ServiceProvider GetServiceProvider()
        {
            return ServiceProviderController.serviceProvider;
        }

    }
}
