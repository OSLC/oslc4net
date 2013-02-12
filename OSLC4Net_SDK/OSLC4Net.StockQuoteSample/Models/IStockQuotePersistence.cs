using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSLC4Net.StockQuoteSample.Models
{
    interface IStockQuotePersistence
    {
        IEnumerable<StockQuote> GetAll();
        StockQuote Get(string tickerSymbol);
        StockQuote Add(StockQuote stockQuote);
        bool Update(StockQuote stockQuote);
        void Delete(string tickerSymbol);

    }
}
