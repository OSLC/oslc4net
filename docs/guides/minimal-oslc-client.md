---
title: Writing a Minimal OSLC Client
description: Discover root services, locate a Service Provider, and fetch OSLC domain resources using OSLC4Net.
---

# Writing a Minimal OSLC Client (Discovery + Fetch)

This guide shows the essential steps to use OSLC4Net as a client: discover endpoints, locate a Service Provider, and fetch a domain resource.

## Authentication Strategy

Start with the simplest viable auth for the target server.

- Basic authentication – use `OslcClient.ForBasicAuth`.
- OAuth / delegated flows – rely on standard .NET mechanisms.

```csharp
OslcClient oslcClient = OslcClient.ForBasicAuth(username, password, logger);
```

### Root Services Discovery

OSLC servers may expose the Root Services document in two locations:

- Well-known path: `{host}/.well-known/oslc/rootservices.xml` (OSLC standard)
- Application path: `{baseUrl}/rootservices` (e.g., `https://server:9443/ccm/rootservices` or `https://server:9443/services/rootservices`)

The client helper tries both paths in order, preferring the well-known location when available and gracefully falling back to appending `/rootservices` to your base URL. If your base URL already ends with `/rootservices` or `/rootservices.xml`, it uses that directly. Provide your base application URL (for example, `https://server:9443/ccm` or `https://server:9443/services`) and the OSLC catalog property you want to extract (typically `rmServiceProviders`, `cmServiceProviders`, or the generic `serviceProviderCatalog`).

Example:

```csharp
var helper = new RootServicesHelper(
    baseUrl: "https://server:9443/ccm",
    catalogNamespace: "http://open-services.net/xmlns/cm/1.0/",
    catalogProperty: "cmServiceProviders");

using var http = new HttpClient();
var rootServices = await helper.DiscoverAsync(http);

// Access the service provider catalog
Console.WriteLine($"Catalog: {rootServices.ServiceProviderCatalog}");

// Access OAuth configuration if present
if (rootServices.OAuth != null)
{
    Console.WriteLine($"OAuth Request Token: {rootServices.OAuth.RequestTokenUrl}");
    Console.WriteLine($"OAuth Authorization: {rootServices.OAuth.AuthorizationUrl}");
    Console.WriteLine($"OAuth Access Token: {rootServices.OAuth.AccessTokenUrl}");
}
```

This resolves to a `RootServicesDocument` containing the service provider catalog URL and optional OAuth configuration. The helper also automatically falls back to the generic `oslc:serviceProviderCatalog` property if the domain-specific property is not found.

## Service Provider Catalog

the `OslcClient` provides lookup helpers that fetch and scan the catalog in one step:

```csharp
// Given a catalog URL and known Service Provider title
string serviceProviderTitle = "my-org/my-repo"; // or a StrictDoc document title
string serviceProviderUrl = await oslcClient.LookupServiceProviderUrl(catalogUrl, serviceProviderTitle)
    ;
// serviceProviderUrl is the About URI of the matching ServiceProvider
```

You can use the `ServiceProviderRegistryClient` when you need richer filtering (identifier match, partial search, multiple selections):

```csharp
ServiceProviderRegistryClient registryClient = new ServiceProviderRegistryClient(catalogUrl, oslcClient, loggerFactory);

ICollection<ServiceProvider> serviceProviders = await registryClient.GetServiceProvidersAsync();
ServiceProvider? githubRepoProvider = serviceProviders
    .FirstOrDefault(sp => sp.GetTitle()?.Contains("my-org/my-repo", StringComparison.OrdinalIgnoreCase) == true);
ServiceProvider? strictDocProvider = serviceProviders
    .FirstOrDefault(sp => sp.GetIdentifier() == "DOC-12345");

if (githubRepoProvider == null && strictDocProvider == null)
{
    logger.LogWarning("No matching ServiceProvider found.");
}
```

## Service Provider & Query Capability

OSLC Client provides lookups for common resources. Query capability:

```csharp
// Domain URIs follow OSLC specs; resourceType is the specific OSLC resource type URI
string oslcDomain = "http://open-services.net/ns/cm#"; // or rm# for requirements
string oslcResourceType = "http://open-services.net/ns/cm#ChangeRequest";
string queryCapabilityUrl = await oslcClient
    .LookupQueryCapabilityAsync(serviceProviderUri, oslcDomain, oslcResourceType)
    ;
// Append oslc.where / oslc.select parameters as needed to queryCapabilityUrl
```

Creation capability:

```csharp
string creationFactoryUrl = await oslcClient
    .LookupCreationFactoryAsync(serviceProviderUri, oslcDomain, oslcResourceType)
    ;
// POST a new ChangeRequest or Requirement using oslcClient.CreateResourceAsync<T>()
```

For a general case, you can traverse the service provider document manually:

```csharp
OslcResponse<ServiceProvider> serviceProviderResponse = await oslcClient
    .GetResourceAsync<ServiceProvider>(serviceProviderUri);
ServiceProvider? serviceProvider = serviceProviderResponse.Resources?.Single();
Uri? queryBase = serviceProvider?
    .GetServices()?.FirstOrDefault()?
    .GetQueryCapabilities()?.FirstOrDefault()?
    .GetQueryBase();
```

## Fetch a Domain Resource Directly

If you already know a resource URI (e.g. from an external system pattern or a prior query):
```csharp
OslcResponse<ChangeRequest> changeRequestResponse = await oslcClient
    .GetResourceAsync<ChangeRequest>(changeRequestUri);
ChangeRequest? changeRequest = changeRequestResponse.Resources?.Single();
if (changeRequest != null)
{
    logger.LogInformation("{id} {title}", changeRequest.GetIdentifier(), changeRequest.GetTitle());
}
else
{
    logger.LogError("ChangeRequest fetch failed: {status} {reason}",
        (int?)changeRequestResponse.StatusCode ?? -1,
        changeRequestResponse.ResponseMessage?.ReasonPhrase);
}
```

## Error Handling

Check `StatusCode` and `ReasonPhrase` in `OslcResponse<T>`. Retain raw responses for diagnostics only when needed.
