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
    /// <summary>
    /// ASP.NET Controller for the ServiceProvider resource.  Initializes a ServiceProvider for
    /// StockQuote resources and returns a ServiceProvider resource when requested.
    /// 
    /// There is no real persistence for the StockQuotes - a memory store is used.
    /// 
    /// See http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    /// for information on how routing words in ASP.NET MVC 4
    /// </summary>

    public class ServiceProviderController : ApiController
    {
        public static string BaseUri { get; set; }            //URI (as string) of the webapps root context
        public static Uri About { get; set; }                //URI for the StockQuote service
        public static Uri ServiceProviderUri { get; set; }   //URI for the ServiceProvider service

        public static ServiceProvider serviceProvider;      
        private const string SERVICE_PROVIDER_PATH = "serviceprovider";

        public static void init(string baseUri)
        {
            BaseUri = baseUri;

            serviceProvider = ServiceProviderFactory.CreateServiceProvider(BaseUri,
                                                                     "StockQuote Service Provider",
                                                                     "Sample OSLC Service Provider for a Stock Quote service",
                                                                     new Publisher("Codeplex OSLC4Net", "urn:codeplex:oslc4net"),
                                                                     new Type[] {typeof(StockQuoteController)});

            //Register prefix definitions this service will use
            PrefixDefinition[] prefixDefinitions =
            {
                new PrefixDefinition(OslcConstants.DCTERMS_NAMESPACE_PREFIX,   new Uri(OslcConstants.DCTERMS_NAMESPACE)),
                new PrefixDefinition(OslcConstants.OSLC_CORE_NAMESPACE_PREFIX, new Uri(OslcConstants.OSLC_CORE_NAMESPACE)),
                new PrefixDefinition(OslcConstants.RDF_NAMESPACE_PREFIX,       new Uri(OslcConstants.RDF_NAMESPACE)),
                new PrefixDefinition(OslcConstants.RDFS_NAMESPACE_PREFIX,      new Uri(OslcConstants.RDFS_NAMESPACE)),
                new PrefixDefinition(Constants.STOCK_QUOTE_NAMESPACE_PREFIX,   new Uri(Constants.STOCK_QUOTE_NAMESPACE))
            };

            serviceProvider.SetPrefixDefinitions(prefixDefinitions);

            About = new Uri(BaseUri + "/" + Constants.PATH_STOCK_QUOTE);
            serviceProvider.SetAbout(About);

            ServiceProviderUri = new Uri(BaseUri + "/" + SERVICE_PROVIDER_PATH);

        }

        public ServiceProvider GetServiceProvider()
        {
            return serviceProvider;
        }

    }
}
