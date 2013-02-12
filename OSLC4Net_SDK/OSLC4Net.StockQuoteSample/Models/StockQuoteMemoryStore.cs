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
using System.Web.Http;
using System.Net;

namespace OSLC4Net.StockQuoteSample.Models
{
    public class StockQuoteMemoryStore : IStockQuotePersistence
    {
        private List<StockQuote> stockQuotes = new List<StockQuote>();

        public StockQuoteMemoryStore()
        {
            StockQuote stockQuote = Utilities.CreateStockQuote(Exchange.NASDAQ, "AAPL");
            stockQuote.SetIdentifier(Utilities.CreateStockQuoteIdentifier(stockQuote));
            Add(stockQuote);

            stockQuote = Utilities.CreateStockQuote(Exchange.NASDAQ, "GOOG");
            stockQuote.SetIdentifier(Utilities.CreateStockQuoteIdentifier(stockQuote));
            Add(stockQuote);

            stockQuote = Utilities.CreateStockQuote(Exchange.NASDAQ, "NFLX");
            stockQuote.SetIdentifier(Utilities.CreateStockQuoteIdentifier(stockQuote));
            Add(stockQuote);

            stockQuote = Utilities.CreateStockQuote(Exchange.NYSE, "IBM");
            stockQuote.SetIdentifier(Utilities.CreateStockQuoteIdentifier(stockQuote));
            Add(stockQuote);

            stockQuote = Utilities.CreateStockQuote(Exchange.NYSE, "BWA");
            stockQuote.SetIdentifier(Utilities.CreateStockQuoteIdentifier(stockQuote));
            Add(stockQuote);
        }

        public IEnumerable<StockQuote> GetAll()
        {
            return stockQuotes;
        }

        public StockQuote Get(string tickerSymbol)
        {
            var stockQuote = stockQuotes.FirstOrDefault((sq) => sq.GetIdentifier() == tickerSymbol);
            if (stockQuote == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return stockQuote;
        }

        public StockQuote Add(StockQuote stockQuote)
        {
            if (stockQuote == null)
            {
                throw new ArgumentNullException("Null stockQuote received for Add");
            }

            stockQuotes.Add(stockQuote);
            return stockQuote;
        }

        public bool Update(StockQuote stockQuote)
        {
            if (stockQuote == null)
            {
                throw new ArgumentNullException("Null stockQuote received for Update");
            }
           
            int i = stockQuotes.FindIndex(sq => sq.GetIdentifier() == stockQuote.GetIdentifier());
            if (i == -1)
            {
                return false;
            }
            
            stockQuotes.RemoveAt(i);
            stockQuotes.Add(stockQuote);

            return true;
        }

        public void Delete(string tickerSymbol)
        {
            stockQuotes.RemoveAll(sq => sq.GetIdentifier() == tickerSymbol);
        }
    }
}