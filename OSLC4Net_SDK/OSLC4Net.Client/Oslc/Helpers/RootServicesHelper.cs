namespace OSLC4Net.Client.Oslc.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using OSLC4Net.Client.Exceptions;
using VDS.RDF;
using VDS.RDF.Parsing;

/// <summary>
/// Helper for OSLC rootservices discovery (Jazz/OSLC servers).
/// Checks both $base/rootservices and host($base)/.well-known/oslc/rootservices.xml.
/// </summary>
public class RootServicesHelper
{
    private readonly string _baseUrl;
    private readonly string _catalogNamespace;
    private readonly string _catalogProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootServicesHelper"/> class.
    /// </summary>
    /// <param name="baseUrl">The base URL of the OSLC server.</param>
    /// <param name="catalogNamespace">The namespace of the OSLC domain.</param>
    /// <param name="catalogProperty">The property name for the catalog in the rootservices doc.</param>
    public RootServicesHelper(string baseUrl, string catalogNamespace = "http://open-services.net/xmlns/oslc/", string catalogProperty = "serviceProviderCatalog")
    {
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentNullException.ThrowIfNull(catalogNamespace);
        ArgumentNullException.ThrowIfNull(catalogProperty);
        _baseUrl = baseUrl.TrimEnd('/');
        _catalogNamespace = catalogNamespace;
        _catalogProperty = catalogProperty;
    }

    /// <summary>
    /// Attempts to discover the rootservices document and parse it into a strongly-typed object.
    /// </summary>
    public async Task<RootServicesDocument> DiscoverAsync()
    {
        using var httpClient = new HttpClient();
        return await DiscoverAsync(httpClient).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to discover the rootservices document using the provided HttpClient and parse it.
    /// </summary>
    public async Task<RootServicesDocument> DiscoverAsync(HttpClient httpClient)
    {
        var candidates = new List<string>();

        // 1. If URL already ends with /rootservices or /rootservices.xml, use as-is
        if (_baseUrl.EndsWith("/rootservices", StringComparison.OrdinalIgnoreCase) ||
            _baseUrl.EndsWith("/rootservices.xml", StringComparison.OrdinalIgnoreCase))
        {
            candidates.Add(_baseUrl);
        }
        else
        {
            // 2. Try well-known location first
            candidates.Add(GetWellKnownRootServicesUrl(_baseUrl));
            // 3. Try base URL + /rootservices
            candidates.Add($"{_baseUrl}/rootservices");
        }

        Exception? lastEx = null;
        foreach (var url in candidates)
        {
            try
            {
                var doc = await TryParseRootServicesAsync(httpClient, url).ConfigureAwait(false);
                if (doc != null)
                {
                    return doc;
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
            }
        }
        throw new RootServicesException(_baseUrl, lastEx ?? new ResourceNotFoundException(_baseUrl, "rootservices"));
    }

    private static string GetWellKnownRootServicesUrl(string baseUrl)
    {
        var uri = new Uri(baseUrl);
        var host = uri.GetLeftPart(UriPartial.Authority);
        return $"{host}/.well-known/oslc/rootservices.xml";
    }

    private async Task<RootServicesDocument?> TryParseRootServicesAsync(HttpClient httpClient, string rootServicesUrl)
    {
        using var response = await httpClient.GetAsync(rootServicesUrl).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var rdfGraph = new Graph();
        var parser = new RdfXmlParser();
        using var streamReader = new StreamReader(stream);
        parser.Load(rdfGraph, streamReader);

        // Find catalog URL
        var catalogUrl = TryGetPropertyValue(rdfGraph, _catalogNamespace, _catalogProperty)
                      ?? TryGetPropertyValue(rdfGraph, "http://open-services.net/xmlns/oslc/", "serviceProviderCatalog");

        if (catalogUrl == null)
        {
            return null;
        }

        // Extract OAuth URLs (Jazz namespace)
        const string jfsNamespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/";
        var oauthRequestToken = TryGetPropertyValue(rdfGraph, jfsNamespace, "oauthRequestTokenUrl");
        var oauthAuthorization = TryGetPropertyValue(rdfGraph, jfsNamespace, "oauthUserAuthorizationUrl");
        var oauthAccessToken = TryGetPropertyValue(rdfGraph, jfsNamespace, "oauthAccessTokenUrl");
        var oauthRequestConsumerKey = TryGetPropertyValue(rdfGraph, jfsNamespace, "oauthRequestConsumerKeyUrl");
        var oauthApproval = TryGetPropertyValue(rdfGraph, jfsNamespace, "oauthApprovalModuleUrl");
        var oauthRealm = TryGetPropertyValue(rdfGraph, jfsNamespace, "oauthRealmName");

        // Only create OAuth document if any OAuth properties are present
        RootServicesOAuth10aDocument? oauthDoc = null;
        if (!string.IsNullOrEmpty(oauthRequestToken) ||
            !string.IsNullOrEmpty(oauthAuthorization) ||
            !string.IsNullOrEmpty(oauthAccessToken) ||
            !string.IsNullOrEmpty(oauthRequestConsumerKey) ||
            !string.IsNullOrEmpty(oauthApproval) ||
            !string.IsNullOrEmpty(oauthRealm))
        {
            oauthDoc = new RootServicesOAuth10aDocument(
                oauthRequestToken,
                oauthAuthorization,
                oauthAccessToken,
                oauthRequestConsumerKey,
                oauthApproval,
                oauthRealm
            );
        }

        // Extract title
        var title = TryGetPropertyValue(rdfGraph, "http://purl.org/dc/terms/", "title");

        return new RootServicesDocument(
            catalogUrl,
            oauthDoc,
            title
        );
    }

    private static string? TryGetPropertyValue(Graph rdfGraph, string ns, string property)
    {
        // Try with and without '#' separator
        var predicates = new List<Uri> { new Uri(ns + property) };
        if (!ns.EndsWith("#", StringComparison.Ordinal) && !ns.EndsWith("/", StringComparison.Ordinal))
        {
            predicates.Add(new Uri(ns + "#" + property));
        }
        else if (ns.EndsWith("/", StringComparison.Ordinal))
        {
            predicates.Add(new Uri(ns.TrimEnd('/') + "#" + property));
        }

        foreach (var predicateUri in predicates)
        {
            var propNode = rdfGraph.CreateUriNode(predicateUri);
            var triples = rdfGraph.GetTriplesWithPredicate(propNode);
            var triple = triples.FirstOrDefault();
            if (triple?.Object is IUriNode uriNode)
            {
                return uriNode.Uri.ToString();
            }
            if (triple?.Object is ILiteralNode litNode)
            {
                return litNode.Value;
            }
        }
        return null;
    }
}
