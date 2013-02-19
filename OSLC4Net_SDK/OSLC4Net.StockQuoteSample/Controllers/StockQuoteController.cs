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
            retrieveStockQuoteInfo(stockQuoteCollection.ToArray<StockQuote>());

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
            retrieveStockQuoteInfo(requestedStockQuote);

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
            retrieveStockQuoteInfo(newStockQuote);

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


        /// <summary>
        /// Call Google's stock quote service and retrieve the data from the JSON response.
        /// Populate the fields of the requested StockQuote with the retrieved data
        /// </summary>
        /// <param name="stockQuotes"></param>
        private static void retrieveStockQuoteInfo(params StockQuote[] stockQuotes)
        {
            string uri = "http://www.google.com/finance/info?infotype=infoquoteall&q=";
            
            Dictionary <string,StockQuote> map = new Dictionary<string,StockQuote>();

            bool first = true;
            foreach(StockQuote stockQuote in stockQuotes)
            {
                
                if (first)
                    first = false;
                else
                    uri +=",";

                uri += stockQuote.GetExchange() + ":" + stockQuote.GetSymbol();
                map.Add(stockQuote.GetIdentifier(),stockQuote);
            }

            WebRequest request = WebRequest.Create(uri);
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(request.GetResponse().GetResponseStream());
            }
            catch (System.Net.WebException e)
            {
                throw new System.Net.WebException("Error accessing uri: " + uri,
                                                   e.Status);
                                                   
            }
            string response = reader.ReadToEnd();

            int indexOf = response.IndexOf('[');
            if (indexOf < 0) indexOf = 0;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string,string>> stockList = 
                serializer.Deserialize<List<Dictionary<string,string>>>(response.Substring(indexOf));

            foreach (Dictionary<string,string> stockEntry in stockList)
            {
                string exchange = stockEntry["e"];
                string ticker = stockEntry["t"];

                string stockId = Utilities.CreateStockQuoteIdentifier(exchange, ticker);
                StockQuote stockQuote = map[stockId];
                if (stockQuote == null)
                {
                    throw new InvalidDataException("Could not find StockQuote with id: " + stockId);
                }

                stockQuote.SetChangePrice(decimal.Parse(stockEntry["c"]));
                stockQuote.SetChangePricePercentage(decimal.Parse(stockEntry["cp"]));
                stockQuote.SetHighPrice(decimal.Parse(stockEntry["hi"]));
                stockQuote.SetHigh52WeekPrice(decimal.Parse(stockEntry["hi52"]));
                stockQuote.SetLastTradedPrice(decimal.Parse(stockEntry["l"]));
                stockQuote.SetLastTradedDate(stockEntry["lt"]);
                stockQuote.SetLowPrice(decimal.Parse(stockEntry["lo"]));
                stockQuote.SetLow52WeekPrice(decimal.Parse(stockEntry["lo52"]));
                stockQuote.SetOpenPrice(decimal.Parse(stockEntry["op"]));
                stockQuote.SetTitle(stockEntry["name"]);

            }
                
        }
    }
}


