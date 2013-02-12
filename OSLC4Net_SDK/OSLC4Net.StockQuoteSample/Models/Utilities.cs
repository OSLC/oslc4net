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