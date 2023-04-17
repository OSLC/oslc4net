using System.Web;
using System.Web.Mvc;

namespace OSLC4Net.StockQuoteExp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
