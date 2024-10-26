OSLC4Net<img src="doc/logo.svg" align="right" width="96px" height="96px">
===========================

[![CI](https://github.com/OSLC/oslc4net/workflows/CI/badge.svg)](https://github.com/OSLC/oslc4net/actions?query=workflow%3ACI)
[![NuGet Version](https://img.shields.io/nuget/v/OSLC4Net.Core)](https://www.nuget.org/packages/OSLC4Net.Core#versions-body-tab)
[![Discourse forum](https://img.shields.io/discourse/users?color=28bd84&server=https%3A%2F%2Fforum.open-services.net%2F)](https://forum.open-services.net/c/sdks/oslc4net/10)

## OSLC4Net, an OSLC SDK for dotnet

**OSLC4Net** is an SDK and sample applications that help the .NET community
adopt Open Services for Lifecycle Collaboration (OSLC,
[homepage](http://open-services.net)) and build OSLC-conformant tools.

The SDK allows developers to create OSLC servers and clients by adding OSLC
annotations to .NET objects to represent them as OSLC resources. It includes a
library based on the [dotNetRDF](https://dotnetrdf.org/) package, which assists
with representing these resources as RDF and helps parse Turle, RDF/XML, and
JSON-LD documents into OSLC .NET objects.

The [OSLC4Net.Client package](https://www.nuget.org/packages/OSLC4Net.Client/)
can be used to help create consumer REST requests. On the server side, the
project offers an RDF-specific `MediaTypeFormatter` that can help process OSLC
REST requests within an ASP.NET MVC 5 API. **Join the [discussion on the .NET
Core migration](https://github.com/OSLC/oslc4net/issues/25).**

## Getting started

If you do not have a .NET development environment, start by downloading VS Code
[C# Dev
Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit).
Make sure to install .NET 8 SDK for development. Libraries target NETStandard
2.0 and should run on .NET Framework 4.8 or .NET 6+ (recommended).

### A simple OSLC Client

Create a new console application targeting .NET 6+, add a NuGet dependency to
`OSLC4Net.Client` and add the following code:

```csharp
var oslcClient = OslcClient.ForBasicAuth(username, password);

var resourceUri =
    "https://jazz.net/sandbox01-ccm/resource/itemName/com.ibm.team.workitem.WorkItem/1300";
OslcResponse<ChangeRequest> response = await oslcClient.GetResourceAsync<ChangeRequest>(resourceUri);
if (response.Resource is not null)
{
    ChangeRequest wi1300 = response.Resource;
    logger.LogInformation($"{wi1300.GetShortTitle()} {wi1300.GetTitle()}");
}
else
{
    logger.LogError("Something went wrong: {} {}", (response.StatusCode as int?) ?? -1,
        response.ResponseMessage?.ReasonPhrase);
}

```

Replace `resourceUri` with a valid OSLC resource URI. This should give you a
valid response. See [full example
project](./OSLC4Net_SDK/Examples/Oslc4NetExamples.Client/) for more details.

> [!TIP]
>
> Use https://github.com/oslc-op/refimpl to quickly run a few conformant OSLC
> servers.

## OSLC Server support

Server parts of the SDK have not yet been migrated from .NET Framework to .NET
8+.

## More information on OSLC

*   See the [OSLC](http://open-services.net/) site for more details on OSLC
    specifications and community activities.
*   See the [Eclipse Lyo](http://eclipse.org/lyo) site for information on OSLC
    SDKs and samples for other technologies.

## OSLC4Net License

OSLC4Net is licensed under the [Eclipse Public License 1.0](LICENSE)
