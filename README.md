# Welcome to the OSLC4Net project

[![Build status](https://ci.appveyor.com/api/projects/status/1d5u49mres43mkhw/branch/master?svg=true)](https://ci.appveyor.com/project/berezovskyi/oslc4net/branch/master)
[![](https://img.shields.io/badge/discuss%20on-zulip-1E8F54.svg)](https://oslc.zulipchat.com/#narrow/stream/114268-github/topic/oslc4net)

## What is OSLC4NET?

**OSLC4Net** is an SDK and sample applications to help the .NET community to adopt [OSLC](http://open-services.net) (Open Services for Lifecycle Collaboration) and build OSLC-compliant tools.  

The current content of the project is the source for a .NET SDK written in C#. The SDK allows developers to create OSLC providers or consumers by adding OSLC annotations to .NET objects to allow them to be represented as OSLC resources. It includes a library based on the [dotNetRDF](https://dotnetrdf.codeplex.com/) package which assists with representing these resources as RDF/XML and which helps parse RDF/XML documents into OSLC .NET objects.  

The ASP.NET MVC 4 library can be used to assist with creating consumer REST requests or to help create a provider which can process OSLC REST requests.  

The project currently contains the SDK and a sample OSLC Change Management client. A sample provider using the SDK is one of the top priorities for the project.  

## Getting started

If you do not have a .NET development environment, start by downloading [Visual Studio 2017 Community](https://www.visualstudio.com/vs/community/) and [SQL Server 2016 Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads). Make sure to install ASP.NET-related options for Visual Studio and LocalDB for SQL Server.

### Running the sample StockQuote provider

`OSLC4Net.StockQuoteSample` is a sample OSLC provider which implements one resource type, a StockQuote. This resource is not defined by an OSLC specification, it shows how OSLC4Net can be used to create an experimental OSLC provider.

1. Build the `OSLC4Net_SDK\OSLC4Net.Core.sln` solution
1. Right click the `OSLC4Net.StockQuoteSample` project and run it via _Debug->Start new instance_

You'll see a web page created - that is currently just a skeleton provided by ASP.NET. You can close it. Try performing a GET request on the follwing URLs in a tool like Poster or Advanced REST Client (make sure to set the `Accept` header to `application/rdf+xml`:

* http://localhost:7077/api/stockquote - returns all StockQuotes
* http://localhost:7077/api/stockquote?getShape=true - returns the StockQuote OSLC resource shape
* http://localhost:7077/api/stockquote/nasdaq_goog - returns an individual StockQuote

## More information on OSLC

*   See the [OSLC](http://open-services.net/) site for more details on OSLC specifications and community activities.
*   See the [Eclipse Lyo](http://eclipse.org/lyo) site for information on OSLC SDKs and samples for other technologies.

## OSLC4Net License

OSLC4Net is licensed under the [Eclipse Public License](LICENSE)  
