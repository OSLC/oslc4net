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

## Root Services Discovery

OSLC discovery begins at `/.well-known/oslc/rootservices.xml`.

Steps:
- GET the document.
- Parse for domain-specific service provider links (e.g. `oslc_cm:cmServiceProviders`, `oslc_rm:rmServiceProviders`).

Pseudo-code:
```csharp
string rootServicesUri = $"{baseUrl}/.well-known/oslc/rootservices.xml";
// For root services there is no dedicated typed resource class yet.
// Using HttpClient here is acceptable; avoid it for catalog & providers.
HttpClient httpClient = new HttpClient();
string rootServicesXml = await httpClient.GetStringAsync(rootServicesUri);
// Extract the catalog URL by parsing RDF/XML for the desired property.
string catalogUrl = ExtractCatalogUrl(rootServicesXml, "http://open-services.net/xmlns/cm/1.0/", "cmServiceProviders");
```

Keep the parsing simple initially; then move to a proper RDF parser if needed.

> [!NOTE]
>
> JazzRootServicesHelper has not yet been migrated to .NET 10.

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
