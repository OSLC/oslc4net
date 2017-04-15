# Welcome to the OSLC4Net project

[![Gitter](https://img.shields.io/gitter/room/nwjs/nw.js.svg)](https://gitter.im/OSLC)
[![StackExchange](https://img.shields.io/stackexchange/stackoverflow/t/oslc.svg)](http://stackoverflow.com/questions/tagged/oslc)
[![Twitter Follow](https://img.shields.io/twitter/follow/oslcNews.svg?style=social&label=Follow)](https://twitter.com/oslcNews)

## What is OSLC4NET?

**OSLC4Net** is an SDK and sample applications to help the .NET community to adopt [OSLC](http://open-services.net) (Open Services for Lifecycle Collaboration) and build OSLC-compliant tools.  

The current content of the project is the source for a .NET SDK written in C#. The SDK allows developers to create OSLC providers or consumers by adding OSLC annotations to .NET objects to allow them to be represented as OSLC resources. It includes a library based on the [dotNetRDF](https://dotnetrdf.codeplex.com/) package which assists with representing these resources as RDF/XML and which helps parse RDF/XML documents into OSLC .NET objects.  

The ASP.NET MVC 4 library can be used to assist with creating consumer REST requests or to help create a provider which can process OSLC REST requests.  

The project currently contains the SDK and a sample OSLC Change Management client. A sample provider using the SDK is one of the top priorities for the project.  

### More information on OSLC

*   See the [OSLC](http://open-services.net/) site for more details on OSLC specifications and community activities.
*   See the [Eclipse Lyo](http://eclipse.org/lyo) site for information on OSLC SDKs and samples for other technologies.

## Pre-requisites

**OSLC4Net** depends on the following libraries  

*   [ASP.NET MVC 4](http://www.asp.net/whitepapers/mvc4-release-notes)
*   [dotNetRDF](https://dotnetrdf.codeplex.com/)
*   [HTML Agility Pack](https://htmlagilitypack.codeplex.com/)
*   [JSON.Net](http://json.codeplex.com/)
*   [DotNetOpenAuth](http://dotnetopenauth.net/)

## OSLC4Net License

OSLC4Net is licensed under the [Eclipse Public License](LICENSE)  
