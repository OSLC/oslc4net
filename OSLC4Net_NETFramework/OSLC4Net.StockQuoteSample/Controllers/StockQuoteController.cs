﻿/*******************************************************************************
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using OSLC4Net.StockQuoteSample.Models;
using OSLC4Net.Core.Model;
using OSLC4Net.Core.Attribute;

namespace OSLC4Net.StockQuoteSample.Controllers
{
    /// <summary>
    /// ASP.NET Controller for the StockQuote resource.  Registers the OSLC4Net RDF/XML MediaFormatter
    /// and the methods implementing the REST services.
    /// 
    /// There is no real persistence for the StockQuotes - a memory store is used.
    /// 
    /// See http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    /// for information on how routing words in ASP.NET MVC 4
    /// </summary>
    [OslcService(Constants.STOCK_QUOTE_DOMAIN)] 
    public class StockQuoteController : ApiController
    {
        static readonly IStockQuotePersistence stockQuoteStore = new StockQuoteMemoryStore();

        static StockQuoteController()
        {
        }

        /// <summary>
        /// Retrieve all StockQuotes and add them to a ResponseInfo object.
        /// 
        /// The OslcDialog and OslcQueryCapability attributes provide the information
        /// needed by the ServiceProvider for this OSLC provider.
        /// </summary>
        /// <returns></returns>
        [OslcDialog(    
            title = "Stock Quote Selection Dialog",
            label = "Stock Quote Selection Dialog",
            uri = "selection",
            hintWidth = "1000px",
            hintHeight = "600px",
            resourceTypes = new string [] {Constants.TYPE_STOCK_QUOTE},
            usages = new string [] {OslcConstants.OSLC_USAGE_DEFAULT}
        )]

        [OslcQueryCapability
        (
            title = "Stock Quote Query Capability",
            label = "Stock Quote Catalog Query",
            resourceShape = Constants.PATH_STOCK_QUOTE + "?" + Constants.PATH_STOCK_QUOTE_SHAPE,
            resourceTypes = new string [] {Constants.TYPE_STOCK_QUOTE},
            usages = new string [] {OslcConstants.OSLC_USAGE_DEFAULT}
        )]
        public ResponseInfoCollection<StockQuote> GetStockQuotes()
        {
            List<StockQuote> stockQuoteCollection = stockQuoteStore.GetAll().ToList<StockQuote>();

            //Get realtime stock quote
            retrieveStockQuoteInfoFake(stockQuoteCollection.ToArray<StockQuote>());

            //Update the resource with runtime subject and ServiceProvider URIs
            foreach (StockQuote stockQuote in stockQuoteCollection)
            {
                stockQuote.SetAbout(new Uri(ServiceProviderController.About.ToString() + "/"+ stockQuote.GetIdentifier()));
                stockQuote.SetServiceProvider(ServiceProviderController.ServiceProviderUri);
            }

            ResponseInfoCollection<StockQuote> responseInfo = 
                new ResponseInfoCollection<StockQuote>(stockQuoteCollection,
                                                       null,
                                                       stockQuoteCollection.Count,
                                                       (string)null);
 
            return responseInfo;
        }

        /// <summary>
        /// Retrieve and reaturn a single StockQuote resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StockQuote GetStockQuote(string id)
        {
            //following will throw an exception if id is bad
            StockQuote requestedStockQuote = stockQuoteStore.Get(id);

            //Get realtime stock quote
            retrieveStockQuoteInfoFake(requestedStockQuote);

            //Update the resource with runtime subject and ServiceProvider URIs
            requestedStockQuote.SetAbout(new Uri(ServiceProviderController.About.ToString() + "/" + requestedStockQuote.GetIdentifier()));
            requestedStockQuote.SetServiceProvider(ServiceProviderController.ServiceProviderUri);
            return requestedStockQuote; 
        }

        /// <summary>
        /// Create a new StockQuote and return the new resource to the caller along with 
        /// a Location header with a URI to the new resource.
        /// 
        /// The OslcCreationFactory attribute provides information needed by the 
        /// service provider.
        /// </summary>
        /// <param name="stockQuote"></param>
        /// <returns></returns>
        [OslcCreationFactory
        (
             title = "Stock Quote Creation Factory",
             label = "Stock Quote Creation",
             resourceShapes = new string[] {Constants.PATH_STOCK_QUOTE + "?" + Constants.PATH_STOCK_QUOTE_SHAPE},
             resourceTypes = new string[] {Constants.TYPE_STOCK_QUOTE},
             usages = new string[] {OslcConstants.OSLC_USAGE_DEFAULT}
        )]
        public HttpResponseMessage PostStockQuote(StockQuote stockQuote)
        {
            stockQuote.SetIdentifier(Utilities.CreateStockQuoteIdentifier(stockQuote));
            StockQuote newStockQuote = stockQuoteStore.Add(stockQuote);

            //Get realtime stock quote
            retrieveStockQuoteInfoFake(newStockQuote);

            //Update the resource with runtime subject and ServiceProvider URIs
            newStockQuote.SetAbout(new Uri(ServiceProviderController.About.ToString() + "/" + stockQuote.GetIdentifier()));
            newStockQuote.SetServiceProvider(ServiceProviderController.ServiceProviderUri);

            //Create a response containing the new resource + a Location header
            var response = Request.CreateResponse<StockQuote>(HttpStatusCode.Created, newStockQuote);
            string uri = Url.Link("DefaultApi", new { id = stockQuote.GetIdentifier() });
            response.Headers.Location = new Uri(uri);

            return response;
        }

        /// <summary>
        /// Update a single StockQuote
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stockQuote"></param>
        public void PutStockQuote(string id, StockQuote stockQuote)
        {
            stockQuote.SetIdentifier(id);
            if (!stockQuoteStore.Update(stockQuote))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }


        /// <summary>
        /// Delete a single StockQuote
        /// </summary>
        /// <param name="id"></param>
        public void DeleteStockQuote(string id)
        {
            StockQuote toDelete = stockQuoteStore.Get(id);
            if (toDelete == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            stockQuoteStore.Delete(id);
        }


        public ResourceShape GetResourceShape(bool getShape)
        {
            ServiceProvider serviceProvider = ServiceProviderController.serviceProvider;
            ResourceShape shape = 
                ResourceShapeFactory.CreateResourceShape(ServiceProviderController.BaseUri, 
                                                         Constants.PATH_STOCK_QUOTE,
                                                         Constants.PATH_STOCK_QUOTE_SHAPE, 
                                                         typeof(StockQuote));
            return shape;
        }


        private static void retrieveStockQuoteInfoFake(params StockQuote[] stockQuotes)
        {
            var random = new Random();
            Dictionary<string, StockQuote> map = new Dictionary<string, StockQuote>();

            foreach (StockQuote stockQuote in stockQuotes)
            {
                map.Add(stockQuote.GetIdentifier(), stockQuote);

                stockQuote.SetTitle(mapSymbol(stockQuote));
                stockQuote.SetChangePrice(Math.Round((decimal)((random.NextDouble() - 0.4) * 100), 2, MidpointRounding.ToEven));
                stockQuote.SetChangePricePercentage(Math.Round((decimal)((random.NextDouble()) * 150) * Math.Sign(stockQuote.GetChangePrice()), 2, MidpointRounding.ToEven));
                stockQuote.SetLastTradedPrice(Math.Round((decimal)(random.NextDouble() * 500), 2, MidpointRounding.ToEven));
                stockQuote.SetOpenPrice(Math.Round((decimal)((random.NextDouble() + 0.01) * 1000), 2, MidpointRounding.ToEven));
                stockQuote.SetLowPrice(Math.Round((decimal)((random.NextDouble() + 0.01) * 100), 2, MidpointRounding.ToEven));
                stockQuote.SetLow52WeekPrice(Math.Round(stockQuote.GetLowPrice() + (decimal)((random.NextDouble() + 0.01) * 50), 2, MidpointRounding.ToEven));
                stockQuote.SetHigh52WeekPrice(Math.Round(Math.Max(stockQuote.GetLastTradedPrice(), stockQuote.GetLow52WeekPrice()) + (decimal)(random.NextDouble() * 500), 2, MidpointRounding.ToEven));
                stockQuote.SetHighPrice(Math.Round(stockQuote.GetHigh52WeekPrice() + (decimal)(random.NextDouble() * 200), 2, MidpointRounding.ToEven));
                stockQuote.SetLastTradedDate(DateTime.Today.ToString("yyyy-MM-dd"));
            }
        }

        private static string mapSymbol(StockQuote stockQuote)
        {
            return stockQuote.GetSymbol() switch
            {
                "GOOG" => "Google",
                "AAPL" => "Apple",
                "NFLX" => "Netflix",
                "IBM" => "IBM",
                "BWA" => "BorgWarner Inc.",
                _ => stockQuote.GetSymbol()
            };
        }
    }
}


