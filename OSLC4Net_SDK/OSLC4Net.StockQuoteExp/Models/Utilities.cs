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

namespace OSLC4Net.StockQuoteSample.Models
{
    public class Utilities
    {
        public static StockQuote CreateStockQuote( Exchange exchange, string   symbol)
        {
            StockQuote stockQuote = new StockQuote();

            stockQuote.SetExchange(exchange.ToString());
            stockQuote.SetSymbol(symbol);

            return stockQuote;
        }

        public static string CreateStockQuoteIdentifier(StockQuote stockQuote)
        {
            return CreateStockQuoteIdentifier(stockQuote.GetExchange().ToString(),
                                              stockQuote.GetSymbol());
        }

        public  static String CreateStockQuoteIdentifier(String exchange,
                                                         String symbol)
        {
            return exchange.ToLower() + "_" + symbol.ToLower();
        }
    }
}