### Running the sample StockQuote provider

> [!NOTE]
> Sample StockQuote provider has not yet been migrated to .NET 6+.

> [!WARNING]
> Sample StockQuote provider is a toy implementation and does not correspond to any [standards-track OSLC specifications](https://open-services.net/specifications/).

`OSLC4Net.StockQuoteSample` is a sample OSLC provider which implements one resource type, a StockQuote. This resource is not defined by an OSLC specification, it shows how OSLC4Net can be used to create an experimental OSLC provider.

1. Build the `OSLC4Net_SDK\OSLC4Net.Core.sln` solution
1. Right click the `OSLC4Net.StockQuoteSample` project and run it via _Debug->Start new instance_

You'll see a web page created - that is currently just a skeleton provided by ASP.NET. Try performing a GET request using [Postman](https://www.postman.com/) (make sure to set the `Accept` header to `application/rdf+xml`:

* http://localhost:7077/api/stockquote - returns all StockQuotes
* http://localhost:7077/api/stockquote?getShape=true - returns the StockQuote OSLC resource shape
* http://localhost:7077/api/stockquote/nasdaq_goog - returns an individual StockQuote

A request to the first URL using Fiddler should have the following response:

![](https://raw.githubusercontent.com/OSLC/oslc4net/master/doc/stockquote.png)
