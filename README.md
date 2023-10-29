# OSLC4Net, an OSLC SDK for .NET

[![CI](https://github.com/OSLC/oslc4net/workflows/CI/badge.svg)](https://github.com/OSLC/oslc4net/actions?query=workflow%3ACI)
[![Discourse status](https://img.shields.io/discourse/https/meta.discourse.org/status.svg)](https://forum.open-services.net/)
[![Gitter](https://img.shields.io/gitter/room/nwjs/nw.js.svg)](https://gitter.im/OSLC/chat)

## What is OSLC4NET?

**OSLC4Net** is an SDK and sample applications that help the .NET community adopt Open Services for Lifecycle Collaboration (OSLC, [homepage](http://open-services.net)) and build OSLC-compliant tools.

The SDK allows developers to create OSLC providers or consumers by adding OSLC annotations to .NET objects to represent them as OSLC resources. It includes a library based on the [dotNetRDF](https://dotnetrdf.org/) package, which assists with representing these resources as RDF/XML and helps parse RDF/XML documents into OSLC .NET objects.

The [OSLC4Net.Client package](https://www.nuget.org/packages/OSLC4Net.Client/) can be used to help create consumer REST requests. On the server side, the project offers an RDF-specific `MediaTypeFormatter` that can help process OSLC REST requests within an ASP.NET MVC 5 API. **Join the [discussion on the .NET Core migration](https://github.com/OSLC/oslc4net/issues/25).**

## Getting started

If you do not have a .NET development environment, start by downloading VS Code [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit). Make sure to install .NET 8 SDK for development. Libraries target NETStandard 2.0 and should run on .NET Framework 4.8 or .NET 6+ (recommended).

### A simple OSLC Client

Create a new console application targeting .NET 6+, add a NuGet dependency to `OSLC4Net.Client` (make sure the *Include prerelease* is checked if you see an empty list) and add the following code:

```csharp
private const string OSLC_SERVER_URI = "https://oslc.itm.kth.se/ccm";

static void Main(string[] args)
{
    try
    {
        var helper = new JazzRootServicesHelper(OSLC_SERVER_URI, OSLCConstants.OSLC_CM_V2);
        var catUri = helper.GetCatalogUrl();
        Console.WriteLine($"The OSLC server has an OSLC Service Provider Catalog at the following URI:\n    {catUri}");
    } catch (RootServicesException e) {
        Console.WriteLine($"Failed to fetch the OSLC RootServices document from:\n    {OSLC_SERVER_URI}/rootservices");
    }
    Console.ReadLine();
}
```

This should give you a valid response.

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

## More information on OSLC

*   See the [OSLC](http://open-services.net/) site for more details on OSLC specifications and community activities.
*   See the [Eclipse Lyo](http://eclipse.org/lyo) site for information on OSLC SDKs and samples for other technologies.

## OSLC4Net License

OSLC4Net is licensed under the [Eclipse Public License 1.0](LICENSE)  
