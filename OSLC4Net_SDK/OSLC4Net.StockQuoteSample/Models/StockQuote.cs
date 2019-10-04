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
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.StockQuoteSample.Models
{
    [OslcNamespace(Constants.STOCK_QUOTE_NAMESPACE)]
    [OslcName(Constants.STOCK_QUOTE)]
    [OslcResourceShape(title = "Stock Quote Resource Shape", describes = new string[] {Constants.TYPE_STOCK_QUOTE})]
    public class StockQuote : AbstractResource
    {
        private decimal    changePrice;
        private decimal    changePricePercentage;
        private Exchange exchange;
        private decimal    high52WeekPrice;
        private decimal    highPrice;
        private string   identifier;
        private string lastTradedDate;
        private decimal    lastTradedPrice;
        private decimal    low52WeekPrice;
        private decimal    lowPrice;
        private decimal    openPrice;
        private Uri      serviceProvider;
        private string   symbol;
        private string   title;
        private readonly ISet<Uri> rdfTypes = new HashSet<Uri>();

        public StockQuote() : base()
        {
            rdfTypes.Add(new Uri(Constants.TYPE_STOCK_QUOTE));
        }

        public StockQuote(Uri about) : base(about)
        {
            rdfTypes.Add(new Uri(Constants.TYPE_STOCK_QUOTE));
        }

        public void AddRdfType(Uri rdfType)
        {
            this.rdfTypes.Add(rdfType);
        }

        [OslcDescription("Change in traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "changePrice")]
        [OslcReadOnly]
        [OslcTitle("Change in Price")]
        public decimal GetChangePrice()
        {
            return changePrice;
        }

        [OslcDescription("Percentage change in traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "changePricePercentage")]
        [OslcReadOnly]
        [OslcTitle("Change in Price Percentage")]
        public decimal GetChangePricePercentage()
        {
            return changePricePercentage;
        }

        [OslcAllowedValue(new string []{"NYSE", "NASDAQ"})]
        [OslcDescription("The stock exchange.  Possible values are NYSE and NASDAQ.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "exchange")]
        [OslcTitle("Exchange")]
        public string GetExchange()
        {
            return exchange.ToString();
        }

        [OslcDescription("High 52 week traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "high52WeekPrice")]
        [OslcReadOnly]
        [OslcTitle("High 52 Week Price")]
        public decimal GetHigh52WeekPrice()
        {
            return high52WeekPrice;
        }

        [OslcDescription("High traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "highPrice")]
        [OslcReadOnly]
        [OslcTitle("High Price")]
        public decimal GetHighPrice()
        {
            return highPrice;
        }

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return identifier;
        }

        [OslcDescription("Last traded date for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "lastTradedDate")]
        [OslcReadOnly]
        [OslcTitle("Last Traded Date")]
        public string GetLastTradedDate()
        {
            return lastTradedDate;
        }

        [OslcDescription("Last traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "lastTradedPrice")]
        [OslcReadOnly]
        [OslcTitle("Last Traded Price")]
        public decimal GetLastTradedPrice()
        {
            return lastTradedPrice;
        }

        [OslcDescription("Low 52 week traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "low52WeekPrice")]
        [OslcReadOnly]
        [OslcTitle("Low 52 Week Price")]
        public decimal GetLow52WeekPrice()
        {
            return low52WeekPrice;
        }

        [OslcDescription("Low traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "lowPrice")]
        [OslcReadOnly]
        [OslcTitle("Low Price")]
        public decimal GetLowPrice()
        {
            return lowPrice;
        }

        [OslcDescription("Open traded price for the stock.")]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "openPrice")]
        [OslcReadOnly]
        [OslcTitle("Open Price")]
        public decimal GetOpenPrice()
        {
            return openPrice;
        }

        [OslcDescription("The scope of a resource is a URI for the resource's OSLC Service Provider.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcReadOnly]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return serviceProvider;
        }

        [OslcDescription("The stock symbol.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(Constants.STOCK_QUOTE_NAMESPACE + "symbol")]
        [OslcTitle("Symbol")]
        public string GetSymbol()
        {
            return symbol;
        }

        [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcTitle("Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return title;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return rdfTypes.ToArray();
        }

        public void SetChangePrice(decimal changePrice)
        {
            this.changePrice = changePrice;
        }

        public void SetChangePricePercentage(decimal changePricePercentage)
        {
            this.changePricePercentage = changePricePercentage;
        }

        public void SetExchange(string exchange)
        {
            this.exchange = ExchangeExtension.FromString(exchange);
        }

        public void SetHigh52WeekPrice(decimal high52WeekPrice)
        {
            this.high52WeekPrice = high52WeekPrice;
        }

        public void SetHighPrice( decimal highPrice)
        {
            this.highPrice = highPrice;
        }

        public void SetIdentifier( string identifier)
        {
            this.identifier = identifier;
        }

        public void SetLastTradedDate( string lastTradedDate)
        {
            this.lastTradedDate = lastTradedDate;
        }

        public void SetLastTradedPrice( decimal lastTradedPrice)
        {
            this.lastTradedPrice = lastTradedPrice;
        }

        public void SetLow52WeekPrice( decimal low52WeekPrice)
        {
            this.low52WeekPrice = low52WeekPrice;
        }

        public void SetLowPrice( decimal lowPrice)
        {
            this.lowPrice = lowPrice;
        }

        public void SetOpenPrice( decimal openPrice)
        {
            this.openPrice = openPrice;
        }

        public void SetServiceProvider( Uri serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void SetSymbol( string symbol)
        {
            this.symbol = symbol;
        }

        public void SetTitle( string title)
        {
            this.title = title;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            this.rdfTypes.Clear();

            if (rdfTypes != null)
            {
                this.rdfTypes.AddAll(rdfTypes);
            }
        }
    }
}