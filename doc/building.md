## Building the SDK from source

To look at the samples or build **OSLC4Net** from source, follow these steps:  

*   install [ASP.NET MVC 4](http://www.asp.net/mvc/mvc4/)
*   git clone https://git01.codeplex.com/oslc4net master
*   open the solution OSLC4Net_SDK\OSLC4Net.Core.slnx
*   build the solution

## Running the StockQuote provider

OSLC4Net.StockQuoteSample is a sample OSLC provider which implements one resource type, a StockQuote. This resource is not defined by an OSLC specification, it shows how OSLC4Net can be used to create an experimental OSLC provider.  

### Project structure and prereqs

This project was originally created using the VS 2010 new project wizard to create an ASP.NET MVC 4 project. If you get the project from Git, it will be missing an important directory created by the wizard. Until we find the best way to package the files in this directory, you can do the following:  

1.  In Solution Explorer, right click the solution -> Add Project and then select ASP.NET MVC 4 Web Application. Give it any name you would like and complete the wizard.
2.  Go to Windows Explorer and go to the root directory of the new project. There will be a directory called "packages". Copy the directory and paste it into the OSLC4Net_SDK directory

The OSLC4Net.StockQuoteSamples has 3 interesting folders:  

1.  App_Start - See **WebApiConfig.cs** for added customization to register our RDF/XML formatter and to get the application context
2.  Models - This is where the resources are defined, especially **StockQuote.cs**
3.  Controllers - This is where the REST services are defined. **ServiceProviderController** sets up the Service Provider and serves it. **StockQuoteController.cs** contains the REST services for the main OSLC provider

### Running the sample

*   Right click the OSLC4Net.StockQuoteSample project->Debug->Start new instance
*   You'll see a web page created - that is currently just a skeleton provided by ASP.NET. You can close it.
*   Try the following URLs in a tool like Poster or Advanced REST Client
    *   Set the Accept header to application/rdf+xml
    *   GET http://localhost:7077/api/stockquote - returns all StockQuotes
    *   GET http://localhost:7077/api/stockquote?getShape=true - returns the StockQuote OSLC resource shape
    *   GET http://localhost:7077/api/stockquote/nasdaq_goog - returns an individual StockQuote

## Running the DotNetRdfProvider tests

The tests for the RDF/XML provider in the DotNetRdfProviderTests project require no configuration and can be run as-is.  

## Running the ChangeManagement tests

**OSLC4Net** does not currently have an OSLC provider implementation. That is high on the priority list of contributions to the project. The SDK itself does support being used in provider implementations.  

The ChangeManagementTests can be run against the **Eclipse Lyo** OSLC4J ChangeManagement provider and serve as an example of a ChangeManagement consumer. They create, retrieve, update and delete OSLC ChangeManagement resources.  

The tests assume the OSLC4J ChangeManagement sample is running on port 8080 on the same system where VisualStudio and the OSLC4Net.ChangeManagementTest project are located.  

See the [instructions](http://wiki.eclipse.org/Lyo/BuildingOSLC4J) on the **Eclipse Lyo** site for information on building and running the ChangeManagement sample provider.  

Once we have a sample provider with tests in OSLC4Net, this won't be necessary.  

## Binary zip

A zip of the **OSLC4Net** library DLLs and their dependencies can be found here: http://oslc4net.codeplex.com/releases/view/100344</div>
