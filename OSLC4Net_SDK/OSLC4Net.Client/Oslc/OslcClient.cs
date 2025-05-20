/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Core;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;
using VDS.RDF;
using System.Reflection; // For GetCustomAttribute and GetCustomAttributes
using OSLC4Net.Core.Attribute; // For OslcResourceShape
using OSLC4Net.Client.Exceptions; // For the new exception
using System.Linq; // For Enumerable methods like OfType, Any, SelectMany, Distinct

namespace OSLC4Net.Client.Oslc;

/// <summary>
/// An OSLC Client.
/// </summary>
public class OslcClient : IDisposable
{
    private readonly ILogger<OslcClient> _logger;

    // As of 2020, FF allows 20, Blink - 19, Safari - 16.
    private const int MAX_REDIRECTS = 20;

    private static readonly ILog log =
        LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected readonly ISet<MediaTypeFormatter> _formatters;
    protected readonly HttpClient _client;

    protected string AcceptHeader { get; } =
        "text/turtle;q=1.0, application/rdf+xml;q=0.9, application/n-triples;q=0.8, text/n3;q=0.7";

    /// <summary>
    /// Initialize a new OslcClient.
    /// </summary>
    public OslcClient(ILogger<OslcClient> logger) : this(false, logger)
    {
    }

    /// <summary>
    /// Initialize a new OslcClient.
    /// </summary>
    /// <param name="certCallback">optionally control SSL certificate management
    /// (null will not replace the default validation callback)</param>
    [Obsolete]
    public OslcClient(Func<HttpRequestMessage, X509Certificate2, X509Chain,
        SslPolicyErrors, bool> certCallback, ILogger<OslcClient> logger) : this(certCallback, null,
        logger)
    {
    }

    /// <summary>
    /// Initialize a new OslcClient
    /// </summary>
    /// <param name="certCallback">optionally control SSL certificate management
    /// (null will not replace the default validation callback)</param>
    /// <param name="userHttpMessageHandler">optionally use OAuth</param>
    [Obsolete]
    protected OslcClient(Func<HttpRequestMessage, X509Certificate2, X509Chain,
            SslPolicyErrors, bool> certCallback, HttpMessageHandler userHttpMessageHandler,
        ILogger<OslcClient> logger)
    {
        _logger = logger;
        this._formatters = new HashSet<MediaTypeFormatter>();

        // REVISIT: RDF/XML + Turtle support only for now (@berezovskyi 2024-10)
        _formatters.Add(new RdfXmlMediaTypeFormatter());
        _formatters.Add(new JsonMediaTypeFormatter());

        var handler = userHttpMessageHandler;
        handler ??= new HttpClientHandler { AllowAutoRedirect = false };
        if (certCallback is not null)
        {
            if (handler is HttpClientHandler httpClientHandler)
            {
                log.Warn(
                    "TLS certificate validation may be compromised! DO NOT USE IN PRODUCTION");
                httpClientHandler.ServerCertificateCustomValidationCallback = certCallback;
            }
            else
            {
                throw new ArgumentException(
                    "Must be an instance of HttpClientHandler if the certCallback is provided",
                    nameof(userHttpMessageHandler));
            }
        }

        _client = HttpClientFactory.Create(handler);
    }

    private OslcClient(HttpClientHandler customHandler, ILogger<OslcClient> logger) : this(null,
        customHandler, logger)
    {
    }

    private OslcClient(bool allowInvalidTlsCerts, ILogger<OslcClient> logger)
    {
        _logger = logger;
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };
        if (allowInvalidTlsCerts)
        {
            log.Warn(
                "TLS certificate validation is compromised! DO NOT USE IN PRODUCTION");
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        _client = new HttpClient(handler);
    }

    public static OslcClient ForBasicAuth(string username, string password,
        ILogger<OslcClient> logger,
        HttpClientHandler handler = null)
    {
        var oslcClient = new OslcClient(handler, logger);
        var client = oslcClient.GetHttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
        return oslcClient;
    }

    [Obsolete("Not for public use; just provide the callback if necessary")]
    /// <summary>
    /// Create an SSL Web Request Handler
    /// </summary>
    /// <param name="certCallback">optionally control SSL certificate management
    /// (use HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    /// in .NET 5+ if really needed)</param>
    /// <returns></returns>
    public static HttpClientHandler CreateSSLHandler(Func<HttpRequestMessage, X509Certificate2,
        X509Chain, SslPolicyErrors, bool>? certCallback = null)
    {
        var handler = new HttpClientHandler();

        if (certCallback != null)
        {
            log.Warn("TLS certificate validation may be compromised! DO NOT USE IN PRODUCTION");
            handler.ServerCertificateCustomValidationCallback = certCallback;
        }

        return handler;
    }

    /// <summary>
    /// Returns the HTTP client associated with this OSLC Client
    /// </summary>
    /// <returns>the HTTP client</returns>
    public HttpClient GetHttpClient()
    {
        return _client;
    }


    public async Task<OslcResponse<T>> GetResourceAsync<T>(string resourceUri, string? mediaType)
        where T : IExtendedResource, new()
    {
        var httpResponseMessage = await GetResourceRawAsync(resourceUri, mediaType).ConfigureAwait(false);
        // REVISIT: according to the spec, non-success codes may also come with a RDF response - should, actually! (@berezovskyi 2024-10)
        // consider adding .ErrorResource to the OslcResponse
        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.Content is not null)
        {
            httpResponseMessage.Content.Headers.Add(OSLC4NetConstants.INNER_URI_HEADER, resourceUri);
            // Buffer content at the beginning
            await httpResponseMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);

            // Deserialize Initial Resources
            var dummy = new T[0];
            T[]? initialResources = await httpResponseMessage.Content.ReadAsAsync(dummy.GetType(), _formatters).ConfigureAwait(false) as T[];

            List<T> validResources = new List<T>();
            if (initialResources != null && initialResources.Any())
            {
                var shapesFromHierarchy = typeof(T).GetCustomAttributes(typeof(OslcResourceShape), true)
                                                   .OfType<OslcResourceShape>()
                                                   .ToList();

                bool typeCheckIsActive = shapesFromHierarchy.Any(s => s.describes != null && s.describes.Any());

                foreach (var resourceInstance in initialResources)
                {
                    if (resourceInstance == null) continue;

                    bool isMatch = false;
                    if (!typeCheckIsActive)
                    {
                        isMatch = true; // Type check effectively skipped
                    }
                    else
                    {
                        ICollection<Uri> actualRdfTypes = resourceInstance.GetTypes();
                        if (!actualRdfTypes.Any()) // Resource has no rdf:type, but shapes expect specific types
                        {
                            isMatch = false; 
                        }
                        else
                        {
                            // Check against all shapes in the hierarchy
                            foreach (var shape in shapesFromHierarchy)
                            {
                                if (shape.describes != null && shape.describes.Any())
                                {
                                    foreach (var expectedType in shape.describes)
                                    {
                                        if (actualRdfTypes.Any(actualType => string.Equals(actualType.ToString(), expectedType, StringComparison.OrdinalIgnoreCase)))
                                        {
                                            isMatch = true; // Match found
                                            break; 
                                        }
                                    }
                                }
                                if (isMatch) break; // Current resource instance matches this shape
                            }
                        }
                    }

                    if (isMatch)
                    {
                        validResources.Add(resourceInstance);
                    }
                    else
                    {
                        var actualTypesStr = string.Join(", ", resourceInstance.GetTypes().Select(uri => uri.ToString()));
                        _logger.LogWarning($"Resource <{resourceInstance.GetAbout()}> of type(s) [{actualTypesStr}] was discarded because it does not match the expected rdf:type(s) for type {typeof(T).FullName} based on its OslcResourceShape hierarchy.");
                    }
                }

                // After loop, check if a single fetched resource was filtered out
                if (initialResources.Length == 1 && !validResources.Any() && typeCheckIsActive)
                {
                    var resourceInstance = initialResources[0];
                    var actualRdfTypes = resourceInstance.GetTypes();
                    var actualTypesStr = actualRdfTypes.Any() ? string.Join(", ", actualRdfTypes.Select(uri => uri.ToString())) : "none";
                    
                    List<string> expectedTypesFromAllShapes = shapesFromHierarchy
                        .Where(s => s.describes != null && s.describes.Any())
                        .SelectMany(s => s.describes)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                    string expectedTypesOutput = expectedTypesFromAllShapes.Any() 
                        ? string.Join(", ", expectedTypesFromAllShapes) 
                        : "specific types (based on active shapes)";

                    _logger.LogError($"Single resource <{resourceInstance.GetAbout()}> with rdf:type(s) [{actualTypesStr}] failed OSLC type check for target type {typeof(T).FullName}. Expected one of: [{expectedTypesOutput}]");
                    throw new OslcRdfTypeMismatchException(resourceInstance.GetAbout().ToString(), expectedTypesOutput, actualTypesStr);
                }
            }
            // If initialResources is null or empty, validResources remains empty.

            // Read Full Graph for Response (from buffered content)
            // Ensure the stream is at the beginning before reading for the graph
            var contentStreamForGraph = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (contentStreamForGraph.CanSeek)
            {
                 contentStreamForGraph.Seek(0, SeekOrigin.Begin);
            }
            Graph? graph = await httpResponseMessage.Content.ReadAsAsync(typeof(Graph), _formatters).ConfigureAwait(false) as Graph;

            // Return Success with Validated Resources
            return OslcResponse<T>.WithSuccess(validResources, graph, httpResponseMessage);
        }
        else
        {
            Graph? g = null;
            Error? resource = null;
            if (httpResponseMessage.Content is not null)
            {
                try
                {
                    g = await httpResponseMessage.Content.ReadAsAsync(typeof(Graph)).ConfigureAwait(false) as Graph;
                }
                catch
                {
                }

                Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                stream.Seek(0, SeekOrigin.Begin);

                try
                {
                    resource = await httpResponseMessage.Content.ReadAsAsync<Error>(_formatters).ConfigureAwait(false);
                }
                catch
                {
                }
            }

            return OslcResponse<T>.WithError(httpResponseMessage, resource);
        }
    }

    public Task<OslcResponse<T>> GetResourceAsync<T>(string resourceUri) where T : IExtendedResource, new()
    {
        return GetResourceAsync<T>(resourceUri, null);
    }


    public Task<OslcResponse<T>> GetResourceAsync<T>(Uri typeURI) where T : IExtendedResource, new()
    {
        return GetResourceAsync<T>(typeURI.ToString(), null);
    }

    /// <summary>
    /// Consider using <see cref="GetResourceAsync{T}"/> instead.
    /// </summary>
    public async Task<HttpResponseMessage> GetResourceRawAsync(string url, string? mediaType = null)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        // TODO: use uniformly (@berezovskyi 2024-10)
        _client.DefaultRequestHeaders.Accept.ParseAdd(mediaType ?? AcceptHeader);
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        HttpResponseMessage response;
        bool redirect;
        byte redirectCount = 0;
        var requestUrl = url;
        do
        {
            response = await _client.GetAsync(requestUrl);

            if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                response.StatusCode == HttpStatusCode.Moved)
            {
                _logger.LogTrace("Encountered redirect {code}: {from} -> {to}", response.StatusCode,
                    requestUrl, response.Headers.Location.AbsoluteUri);
                requestUrl = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
                if (++redirectCount > MAX_REDIRECTS)
                {
                    // max redirects reached
                    throw new OslcCoreRequestException(-1, response, null,
                        new Error
                        {
                            Message = $"Maximum redirects reached (allowed: {MAX_REDIRECTS})."
                        });
                }
            }
            else
            {
                _logger.LogTrace("Encountered reponse {code}: {url} (for {origUrl})",
                    response.StatusCode,
                    requestUrl, url);
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    /// <summary>
    /// Delete an OSLC resource and return a HttpResponseMessage
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Obsolete]
    public HttpResponseMessage DeleteResource(string url)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        HttpResponseMessage response;
        bool redirect;
        do
        {
            response = _client.DeleteAsync(url).Result;

            if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                response.StatusCode == HttpStatusCode.Moved)
            {
                url = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
            }
            else
            {
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    /// <summary>
    /// Delete an OSLC resource and return a HttpResponseMessage
    /// </summary>
    /// <param name="url"></param>
    /// <param name="ct">Cancellation token</param>
    /// <returns></returns>
    public Task<HttpResponseMessage> DeleteResourceAsync(Uri url, CancellationToken? ct = null)
    {
        return DeleteResourceAsync(url.ToString(), ct);
    }

    /// <summary>
    /// Delete an OSLC resource and return a HttpResponseMessage
    /// </summary>
    /// <param name="url"></param>
    /// <param name="ct">Cancellation token</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> DeleteResourceAsync(string url, CancellationToken? ct = null)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        HttpResponseMessage response;
        bool redirect;
        do
        {
            if (ct is not null)
            {
                ct.Value.ThrowIfCancellationRequested();
                response = await _client.DeleteAsync(url, ct.Value).ConfigureAwait(false);
            }
            else
            {
                response = await _client.DeleteAsync(url).ConfigureAwait(false);
            }

            if (ShallFollowRedirectNonGet(response))
            {
                url = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
            }
            else
            {
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    public async Task<OslcResponse<T>> CreateResourceAsync<T>(string url, T artifact, string? mediaType = null)
        where T : IExtendedResource, new()
    {
        var response = await CreateResourceRawAsync(url, artifact, mediaType).ConfigureAwait(false);
        // a bit outside the spec, but these should be success statuses
        if (response.StatusCode == HttpStatusCode.OK
            || response.StatusCode == HttpStatusCode.Created
            || response.StatusCode == HttpStatusCode.SeeOther
            || response.StatusCode == HttpStatusCode.NoContent)
        {
            // we have two options: the Location header points to a newly created resource or the resource is returned directly
            // I think OSLC mandates Location, so let's start with that
            var createdUri = response.Headers.Location.AbsoluteUri;
            return await GetResourceAsync<T>(createdUri, mediaType).ConfigureAwait(false);
        }
        else
        {
            // TODO: try to read the error resource if any
            return OslcResponse<T>.WithError(response);
        }
    }

    /// <summary>
    /// Create (POST) an artifact to a URL - usually an OSLC Creation Factory
    /// </summary>
    /// <param name="url"></param>
    /// <param name="artifact"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> CreateResourceRawAsync(string url, IResource artifact, string? mediaType)
    {
        return await CreateResourceRawAsync(url, artifact, mediaType ?? OSLCConstants.CT_RDF, AcceptHeader)
            .ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> CreateResourceRawAsync(Uri uri, IResource artifact, string? mediaType)
    {
        return await CreateResourceRawAsync(uri.ToString(), artifact, mediaType, AcceptHeader).ConfigureAwait(false);
    }

    /// <summary>
    /// Create (POST) an artifact to a URL - usually an OSLC Creation Factory
    /// </summary>
    /// <param name="url"></param>
    /// <param name="artifact"></param>
    /// <param name="mediaType"></param>
    /// <param name="acceptType"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> CreateResourceRawAsync(string url, IResource artifact, string mediaType,
        string acceptType)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        foreach (var acceptSingle in acceptType.Split(','))
            _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(acceptSingle));
        //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

            content.Headers.ContentType = mediaTypeValue;

            response = await _client.PostAsync(url, content).ConfigureAwait(false);

            if (ShallFollowRedirectNonGet(response))
            {
                url = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
            }
            else
            {
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    public async Task<HttpResponseMessage> UpdateResourceRawAsync(Uri uri, IResource artifact,
        string mediaType = OSLCConstants.CT_RDF, string? acceptType = null)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        foreach (var acceptSingle in (acceptType ?? AcceptHeader).Split(','))
            _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(acceptSingle));
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        var url = uri.ToString();
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

            content.Headers.ContentType = mediaTypeValue;

            response = await _client.PutAsync(url, content).ConfigureAwait(false);

            if (ShallFollowRedirectNonGet(response))
            {
                url = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
            }
            else
            {
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    /// <summary>
    /// Update (PUT) an artifact to a URL - usually the URL for an existing OSLC artifact
    /// </summary>
    /// <param name="url"></param>
    /// <param name="artifact"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    [Obsolete]
    public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType)
    {
        return UpdateResource(url, artifact, mediaType, "*/*");
    }

    /// <summary>
    /// Update (PUT) an artifact to a URL - usually the URL for an existing OSLC artifact
    /// </summary>
    /// <param name="url"></param>
    /// <param name="artifact"></param>
    /// <param name="mediaType"></param>
    /// <param name="acceptType"></param>
    /// <returns></returns>
    [Obsolete]
    public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType,
        string acceptType)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

            content.Headers.ContentType = mediaTypeValue;

            response = _client.PutAsync(url, content).Result;

            if (ShallFollowRedirect(response))
            {
                url = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
            }
            else
            {
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    /// <summary>
    /// Update (PUT) an artifact to a URL - usually the URL for an existing OSLC artifact
    /// </summary>
    /// <param name="url"></param>
    /// <param name="artifact"></param>
    /// <param name="mediaType"></param>
    /// <param name="acceptType"></param>
    /// <param name="ifMatch"></param>
    /// <returns></returns>
    [Obsolete]
    public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType,
        string acceptType, string ifMatch)
    {
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        _client.DefaultRequestHeaders.Add(HttpRequestHeader.IfMatch.ToString(), ifMatch);

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

            content.Headers.ContentType = mediaTypeValue;

            response = _client.PutAsync(url, content).Result;

            if (ShallFollowRedirect(response))
            {
                url = response.Headers.Location.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
            }
            else
            {
                redirect = false;
            }
        } while (redirect);

        return response;
    }

    /// <summary>
    /// Lookup the URL of a specific OSLC Service Provider in an OSLC Catalog using the service provider's title
    /// </summary>
    /// <param name="catalogUrl"></param>
    /// <param name="serviceProviderTitle"></param>
    /// <returns></returns>
    public async Task<string> LookupServiceProviderUrl(string catalogUrl, string serviceProviderTitle)
    {
        string? retval = null;
        var response = await GetResourceAsync<ServiceProviderCatalog>(catalogUrl).ConfigureAwait(false);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new ResourceNotFoundException(catalogUrl, serviceProviderTitle);

        var catalog = response.Resources?.SingleOrDefault();

        if (catalog != null)
            foreach (var sp in catalog.GetServiceProviders())
                if (sp.GetTitle() != null &&
                    string.Compare(sp.GetTitle(), serviceProviderTitle,
                        StringComparison.OrdinalIgnoreCase) == 0)
                {
                    retval = sp.GetAbout().ToString();
                    break;
                }

        if (retval == null) throw new ResourceNotFoundException(catalogUrl, serviceProviderTitle);

        return retval;
    }

    /// <summary>
    /// Find the OSLC Query Capability URL for a given OSLC resource type.  If no resource type is given, returns
    /// the default Query Capability, if it exists.
    /// </summary>
    /// <param name="serviceProviderUrl"></param>
    /// <param name="oslcDomain"></param>
    /// <param name="oslcResourceType">the resource type of the desired query capability.   This may differ from the OSLC artifact type.</param>
    /// <returns>URL of requested Query Capability or null if not found.</returns>
    public async Task<string> LookupQueryCapabilityAsync(string serviceProviderUrl, string oslcDomain,
        string oslcResourceType)
    {
        QueryCapability? defaultQueryCapability = null;
        QueryCapability? firstQueryCapability = null;

        var response = await GetResourceAsync<ServiceProvider>(serviceProviderUrl).ConfigureAwait(false);


        if (response.StatusCode != HttpStatusCode.OK)
            throw new ResourceNotFoundException(serviceProviderUrl, "QueryCapability");

        var serviceProvider = response.Resources.SingleOrDefault();

        if (serviceProvider != null)
            foreach (var service in serviceProvider.GetServices())
            {
                var domain = service.GetDomain();
                if (domain != null && domain.ToString().Equals(oslcDomain))
                {
                    var queryCapabilities = service.GetQueryCapabilities();
                    if (queryCapabilities != null && queryCapabilities.Length > 0)
                    {
                        firstQueryCapability = queryCapabilities[0];
                        foreach (var queryCapability in service.GetQueryCapabilities())
                        {
                            foreach (var resourceType in queryCapability.GetResourceTypes())
                                //return as soon as domain + resource type are matched
                                if (resourceType.ToString() != null &&
                                    resourceType.ToString().Equals(oslcResourceType))
                                    return queryCapability.GetQueryBase().OriginalString;

                            //Check if this is the default capability
                            foreach (var usage in queryCapability.GetUsages())
                                if (usage.ToString() != null && usage.ToString()
                                        .Equals(OSLCConstants.USAGE_DEFAULT_URI))
                                    defaultQueryCapability = queryCapability;
                        }
                    }
                }
            }

        //If we reached this point, there was no resource type match
        if (defaultQueryCapability != null)
            //return default, if present
            return defaultQueryCapability.GetQueryBase().ToString();

        if (firstQueryCapability != null &&
            firstQueryCapability.GetResourceTypes().Length == 0)
            //return the first for the domain, if present
            return firstQueryCapability.GetQueryBase().ToString();

        throw new ResourceNotFoundException(serviceProviderUrl, "QueryCapability");
    }

    /// <summary>
    /// Find the OSLC Creation Factory URL for a given OSLC resource type.  If no resource type is given, returns
    /// the default Creation Factory, if it exists.
    /// </summary>
    /// <param name="serviceProviderUrl"></param>
    /// <param name="oslcDomain"></param>
    /// <param name="oslcResourceType">the resource type of the desired query capability.   This may differ from the OSLC artifact type.</param>
    /// <returns>URL of requested Creation Factory or null if not found.</returns>
    public async Task<string> LookupCreationFactoryAsync(string serviceProviderUrl, string oslcDomain,
        string oslcResourceType)
    {
        CreationFactory? defaultCreationFactory = null;
        CreationFactory? firstCreationFactory = null;

        var response = await GetResourceAsync<ServiceProvider>(serviceProviderUrl).ConfigureAwait(false);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new ResourceNotFoundException(serviceProviderUrl, "CreationFactory");

        var serviceProvider = response.Resources?.SingleOrDefault();

        if (serviceProvider != null)
            foreach (var service in serviceProvider.GetServices())
            {
                var domain = service.GetDomain();
                if (domain != null && domain.ToString().Equals(oslcDomain))
                {
                    var creationFactories = service.GetCreationFactories();
                    if (creationFactories != null && creationFactories.Length > 0)
                    {
                        firstCreationFactory = creationFactories[0];
                        foreach (var creationFactory in creationFactories)
                        {
                            foreach (var resourceType in creationFactory.GetResourceTypes())
                                //return as soon as domain + resource type are matched
                                if (resourceType.ToString() != null &&
                                    resourceType.ToString().Equals(oslcResourceType))
                                    return creationFactory.GetCreation().ToString();

                            //Check if this is the default factory
                            foreach (var usage in creationFactory.GetUsages())
                                if (usage.ToString() != null && usage.ToString()
                                        .Equals(OSLCConstants.USAGE_DEFAULT_URI))
                                    defaultCreationFactory = creationFactory;
                        }
                    }
                }
            }

        //If we reached this point, there was no resource type match
        if (defaultCreationFactory != null)
            //return default, if present
            return defaultCreationFactory.GetCreation().ToString();

        if (firstCreationFactory != null &&
            firstCreationFactory.GetResourceTypes().Length == 0)
            //return the first for the domain, if present
            return firstCreationFactory.GetCreation().ToString();

        throw new ResourceNotFoundException(serviceProviderUrl, "CreationFactory");
    }

    public ISet<MediaTypeFormatter> GetFormatters()
    {
        return _formatters;
    }

    /// <summary>
    /// Handle SSL server certificate processing, accepting all certificates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="certificate"></param>
    /// <param name="chain"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    [Obsolete]
    public static bool AcceptAllServerCertificates(object sender, X509Certificate certificate,
        X509Chain chain, SslPolicyErrors errors)
    {
        return true;
    }

    private static bool ShallFollowRedirect(HttpResponseMessage response)
    {
        return response.StatusCode == HttpStatusCode.MovedPermanently ||
               response.StatusCode == HttpStatusCode.Found
               || response.StatusCode == HttpStatusCode.RedirectKeepVerb
               || response.StatusCode == HttpStatusCode.PermanentRedirect
               || response.StatusCode == HttpStatusCode.SeeOther;
    }

    private static bool ShallFollowRedirectNonGet(HttpResponseMessage response)
    {
        return response.StatusCode == HttpStatusCode.MovedPermanently ||
               response.StatusCode == HttpStatusCode.Found
               || response.StatusCode == HttpStatusCode.RedirectKeepVerb
               || response.StatusCode == HttpStatusCode.PermanentRedirect;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}

public static class ConsumeContentExtension
{
    public static void ConsumeContent(this HttpResponseMessage response)
    {
        response.Content.Dispose();
    }
}
