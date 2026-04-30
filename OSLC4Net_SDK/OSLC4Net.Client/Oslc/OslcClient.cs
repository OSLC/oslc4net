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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Core;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;
using VDS.RDF;

namespace OSLC4Net.Client.Oslc;

/// <summary>
/// An OSLC Client. This is the primary client for OSLC operations.
/// </summary>
/// <remarks>
/// OslcClient supports:
/// <list type="bullet">
///   <item>Configurable request parameters via <see cref="OslcRequestParams"/> (can be set in constructor or per-request)</item>
///   <item>Access to both unmarshalled POCOs and raw response graphs via <see cref="OslcResponse{T}"/></item>
///   <item>Accumulating responses in a single graph for discovery phases via <see cref="AccumulatingGraph"/></item>
/// </list>
/// </remarks>
public class OslcClient : IDisposable
{
    private readonly ILogger<OslcClient> _logger;

    // As of 2020, FF allows 20, Blink - 19, Safari - 16.
    private const int MAX_REDIRECTS = 20;

    protected readonly ISet<MediaTypeFormatter> _formatters;
    protected readonly HttpClient _client;

    /// <summary>
    /// Default request parameters used for all requests unless overridden.
    /// </summary>
    public OslcRequestParams DefaultRequestParams { get; }

    /// <summary>
    /// When set, all response graphs will be merged into this graph.
    /// Useful for the initial discovery phase to accumulate all service provider information.
    /// </summary>
    public Graph? AccumulatingGraph { get; private set; }

    protected string AcceptHeader { get; } =
        "text/turtle;q=1.0, application/rdf+xml;q=0.9, application/n-triples;q=0.8, text/n3;q=0.7";

    /// <summary>
    /// Initialize a new OslcClient.
    /// </summary>
    public OslcClient(ILogger<OslcClient> logger) : this(false, logger)
    {
    }

    /// <summary>
    /// Initialize a new OslcClient with custom default request parameters.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="defaultRequestParams">Default request parameters for all requests.</param>
    public OslcClient(ILogger<OslcClient> logger, OslcRequestParams defaultRequestParams)
        : this(false, logger, defaultRequestParams)
    {
    }

    /// <summary>
    /// Initialize a new OslcClient using an externally managed HttpClient (e.g. with resilience policies).
    /// </summary>
    /// <param name="client">Pre-configured HttpClient instance (lifetime managed by caller).</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="defaultRequestParams">Optional default request parameters for all requests.</param>
    public OslcClient(HttpClient client, ILogger<OslcClient> logger, OslcRequestParams? defaultRequestParams = null)
    {
        _logger = logger;
        _client = client;
        _formatters = new HashSet<MediaTypeFormatter> { new RdfXmlMediaTypeFormatter() };
        DefaultRequestParams = defaultRequestParams ?? OslcRequestParams.Default;
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

        var handler = userHttpMessageHandler;
        handler ??= new HttpClientHandler { AllowAutoRedirect = false };
        if (certCallback is not null)
        {
            // only for development
            // REVISIT: get rid of this once we confirm that this can be worked around for e.g. self-signed certs and Jazz/Polarion (@berezovskyi 2025-05)
#pragma warning disable MA0039
            if (handler is HttpClientHandler httpClientHandler)
            {
                _logger.LogWarning(
                    "TLS certificate validation may be compromised! DO NOT USE IN PRODUCTION");
                httpClientHandler.ServerCertificateCustomValidationCallback = certCallback;
            }
            else
            {
                throw new ArgumentException(
                    "Must be an instance of HttpClientHandler if the certCallback is provided",
                    nameof(userHttpMessageHandler));
            }
#pragma warning enable MA0039
        }

        _client = HttpClientFactory.Create(handler);
        DefaultRequestParams = OslcRequestParams.Default;
    }

    private OslcClient(HttpClientHandler? customHandler, ILogger<OslcClient> logger,
        OslcRequestParams? defaultRequestParams = null) : this(null,
        customHandler, logger)
    {
        DefaultRequestParams = defaultRequestParams ?? OslcRequestParams.Default;
    }

    private OslcClient(bool allowInvalidTlsCerts, ILogger<OslcClient> logger)
        : this(allowInvalidTlsCerts, logger, null)
    {
    }

    private OslcClient(bool allowInvalidTlsCerts, ILogger<OslcClient> logger,
        OslcRequestParams? defaultRequestParams)
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
            _logger.LogWarning(
                "TLS certificate validation is compromised! DO NOT USE IN PRODUCTION");
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        _formatters = new HashSet<MediaTypeFormatter>();
        _formatters.Add(new RdfXmlMediaTypeFormatter());

        _client = new HttpClient(handler);
        DefaultRequestParams = defaultRequestParams ?? OslcRequestParams.Default;
    }


    public static OslcClient ForBasicAuth(string username, string password,
        ILogger<OslcClient> logger,
        HttpClientHandler? handler = null,
        OslcRequestParams? defaultRequestParams = null)
    {
        var oslcClient = new OslcClient(handler, logger, defaultRequestParams);
        var client = oslcClient.GetHttpClient();
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
        return oslcClient;
    }

    /// <summary>
    /// Create an OslcClient for Basic Auth using a pre-configured HttpClient (e.g. with resilience policies).
    /// </summary>
    public static OslcClient ForBasicAuth(HttpClient httpClient, string username, string password,
        ILogger<OslcClient> logger, OslcRequestParams? defaultRequestParams = null)
    {
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        return new OslcClient(httpClient, logger, defaultRequestParams);
    }

    /// <summary>
    /// Returns the HTTP client associated with this OSLC Client
    /// </summary>
    /// <returns>the HTTP client</returns>
    public HttpClient GetHttpClient()
    {
        return _client;
    }

    /// <summary>
    /// Enables graph accumulation mode. All response graphs will be merged into a single graph.
    /// Useful for the initial discovery phase to accumulate all service provider information.
    /// </summary>
    /// <returns>The accumulating graph that will contain all merged response data.</returns>
    public Graph EnableGraphAccumulation()
    {
        AccumulatingGraph ??= new Graph();
        return AccumulatingGraph;
    }

    /// <summary>
    /// Disables graph accumulation mode and optionally returns the accumulated graph.
    /// </summary>
    /// <returns>The accumulated graph, or null if accumulation was not enabled.</returns>
    public Graph? DisableGraphAccumulation()
    {
        var result = AccumulatingGraph;
        AccumulatingGraph = null;
        return result;
    }

    /// <summary>
    /// Clears the accumulating graph without disabling accumulation mode.
    /// </summary>
    public void ClearAccumulatingGraph()
    {
        AccumulatingGraph?.Clear();
    }


    public async Task<OslcResponse<T>> GetResourceAsync<T>(string resourceUri, string? mediaType)
        where T : IExtendedResource, new()
    {
        return await GetResourceAsync<T>(resourceUri, mediaType, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Get an OSLC resource with request parameter overrides.
    /// </summary>
    /// <typeparam name="T">The type of resource to retrieve.</typeparam>
    /// <param name="resourceUri">The URI of the resource.</param>
    /// <param name="mediaType">The media type to accept (deprecated, use requestParams instead).</param>
    /// <param name="requestParams">Request parameters to override defaults.</param>
    /// <returns>An OslcResponse containing the resource(s) and graph.</returns>
    public async Task<OslcResponse<T>> GetResourceAsync<T>(string resourceUri, string? mediaType,
        OslcRequestParams? requestParams)
        where T : IExtendedResource, new()
    {
        var effectiveParams = DefaultRequestParams.Merge(requestParams);
        var acceptHeader = mediaType ?? effectiveParams.AcceptHeader ?? AcceptHeader;

        var httpResponseMessage = await GetResourceRawAsync(resourceUri, acceptHeader, effectiveParams)
            .ConfigureAwait(false);
        // REVISIT: according to the spec, non-success codes may also come with a RDF response - should, actually! (@berezovskyi 2024-10)
        // consider adding .ErrorResource to the OslcResponse
        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.Content is not null)
        {
            httpResponseMessage.Content.Headers.Add(OSLC4NetConstants.INNER_URI_HEADER, resourceUri);
            await httpResponseMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);

            var dummy = Array.Empty<T>();
            var resources = await httpResponseMessage.Content
                .ReadAsAsync(dummy.GetType(), _formatters).ConfigureAwait(false) as T[];

            var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (contentStream.CanSeek)
            {
                contentStream.Seek(0, SeekOrigin.Begin);
            }

            var graph = await httpResponseMessage.Content.ReadAsAsync(typeof(Graph), _formatters)
                .ConfigureAwait(false) as Graph;

            // Merge into accumulating graph if enabled
            if (AccumulatingGraph is not null && graph is not null)
            {
                AccumulatingGraph.Merge(graph);
            }

            return OslcResponse<T>.WithSuccess(resources?.ToList(), graph, httpResponseMessage);
        }
        else
        {
            Error? resource = null;
            if (httpResponseMessage.Content is not null)
            {
                try
                {
                    var g = await httpResponseMessage.Content.ReadAsAsync(typeof(Graph)).ConfigureAwait(false) as Graph;
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
        return GetResourceAsync<T>(resourceUri, null, null);
    }


    public Task<OslcResponse<T>> GetResourceAsync<T>(Uri typeURI) where T : IExtendedResource, new()
    {
        return GetResourceAsync<T>(typeURI.ToString(), null, null);
    }

    /// <summary>
    /// Get an OSLC resource with request parameter overrides.
    /// </summary>
    /// <typeparam name="T">The type of resource to retrieve.</typeparam>
    /// <param name="typeURI">The URI of the resource.</param>
    /// <param name="requestParams">Request parameters to override defaults.</param>
    /// <returns>An OslcResponse containing the resource(s) and graph.</returns>
    public Task<OslcResponse<T>> GetResourceAsync<T>(Uri typeURI, OslcRequestParams? requestParams)
        where T : IExtendedResource, new()
    {
        return GetResourceAsync<T>(typeURI.ToString(), null, requestParams);
    }

    /// <summary>
    /// Consider using <see cref="GetResourceAsync{T}"/> instead.
    /// </summary>
    public async Task<HttpResponseMessage> GetResourceRawAsync(string url, string? mediaType = null)
    {
        return await GetResourceRawAsync(url, mediaType, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Consider using <see cref="GetResourceAsync{T}"/> instead.
    /// </summary>
    /// <param name="url">The URL to fetch.</param>
    /// <param name="mediaType">The Accept media type.</param>
    /// <param name="requestParams">Optional request parameters to override defaults.</param>
    public async Task<HttpResponseMessage> GetResourceRawAsync(string url, string? mediaType,
        OslcRequestParams? requestParams)
    {
        var effectiveParams = DefaultRequestParams.Merge(requestParams);
        var accept = mediaType ?? effectiveParams.AcceptHeader ?? AcceptHeader;
        var oslcVersion = effectiveParams.OslcCoreVersion ?? OslcRequestParams.DefaultOslcCoreVersion;

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.ParseAdd(accept);
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, oslcVersion);

        // Apply custom headers from request parameters
        if (effectiveParams.CustomHeaders is not null)
        {
            foreach (var header in effectiveParams.CustomHeaders)
            {
                _client.DefaultRequestHeaders.Remove(header.Key);
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        HttpResponseMessage response;
        bool redirect;
        byte redirectCount = 0;
        var requestUrl = url;
        do
        {
            response = await _client.GetAsync(requestUrl).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                response.StatusCode == HttpStatusCode.Moved)
            {
                _logger.LogTrace("Encountered redirect {code}: {from} -> {to}", response.StatusCode,
                    requestUrl, response.Headers.Location?.AbsoluteUri);
                requestUrl = response.Headers.Location?.AbsoluteUri;
                response.ConsumeContent();
                redirect = true;
                if (++redirectCount > MAX_REDIRECTS)
                {
                    // max redirects reached
                    throw new OslcCoreRequestException(HttpStatusCode.LoopDetected,
                        response.ReasonPhrase, null,
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
                var locationUrl = response.Headers.Location?.AbsoluteUri;
                if (locationUrl is null)
                {
                    break;
                }

                url = locationUrl;
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
                var locationUrl = response.Headers.Location?.AbsoluteUri;
                if (locationUrl is null)
                {
                    break;
                }

                url = locationUrl;
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
        return await CreateResourceAsync(url, artifact, mediaType, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Create an OSLC resource with request parameter overrides.
    /// </summary>
    /// <typeparam name="T">The type of resource to create.</typeparam>
    /// <param name="url">The creation factory URL.</param>
    /// <param name="artifact">The resource to create.</param>
    /// <param name="mediaType">The Content-Type media type (deprecated, use requestParams instead).</param>
    /// <param name="requestParams">Request parameters to override defaults.</param>
    /// <returns>An OslcResponse containing the created resource.</returns>
    public async Task<OslcResponse<T>> CreateResourceAsync<T>(string url, T artifact, string? mediaType,
        OslcRequestParams? requestParams)
        where T : IExtendedResource, new()
    {
        var effectiveParams = DefaultRequestParams.Merge(requestParams);
        var contentType = mediaType ?? effectiveParams.ContentType ?? OSLCConstants.CT_RDF;
        var acceptType = effectiveParams.AcceptHeader ?? AcceptHeader;

        var response = await CreateResourceRawAsync(url, artifact, contentType, acceptType, effectiveParams)
            .ConfigureAwait(false);
        // a bit outside the spec, but these should be success statuses
        if (response.StatusCode == HttpStatusCode.OK
            || response.StatusCode == HttpStatusCode.Created
            || response.StatusCode == HttpStatusCode.SeeOther
            || response.StatusCode == HttpStatusCode.NoContent)
        {
            // we have two options: the Location header points to a newly created resource or the resource is returned directly
            // I think OSLC mandates Location, so let's start with that
            var createdUri = response.Headers.Location?.AbsoluteUri;
            return await GetResourceAsync<T>(createdUri, acceptType, requestParams).ConfigureAwait(false);
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
        return await CreateResourceRawAsync(url, artifact, mediaType, acceptType, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Create (POST) an artifact to a URL - usually an OSLC Creation Factory
    /// </summary>
    /// <param name="url">The creation factory URL.</param>
    /// <param name="artifact">The resource to create.</param>
    /// <param name="mediaType">The Content-Type.</param>
    /// <param name="acceptType">The Accept header.</param>
    /// <param name="requestParams">Optional request parameters to apply custom headers.</param>
    /// <returns>The HTTP response.</returns>
    public async Task<HttpResponseMessage> CreateResourceRawAsync(string url, IResource artifact, string mediaType,
        string acceptType, OslcRequestParams? requestParams)
    {
        var effectiveParams = DefaultRequestParams.Merge(requestParams);
        var oslcVersion = effectiveParams.OslcCoreVersion ?? OslcRequestParams.DefaultOslcCoreVersion;

        _client.DefaultRequestHeaders.Accept.Clear();
        foreach (var acceptSingle in acceptType.Split(','))
        {
            _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(acceptSingle));
        }
        _client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        _client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, oslcVersion);

        // Apply custom headers from request parameters
        if (effectiveParams.CustomHeaders is not null)
        {
            foreach (var header in effectiveParams.CustomHeaders)
            {
                _client.DefaultRequestHeaders.Remove(header.Key);
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

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
                var locationUrl = response.Headers.Location?.AbsoluteUri;
                if (locationUrl is null)
                {
                    break;
                }

                url = locationUrl;
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
        {
            _client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(acceptSingle));
        }

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
                var locationUrl = response.Headers.Location?.AbsoluteUri;
                if (locationUrl is null)
                {
                    break;
                }

                url = locationUrl;
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
                var locationUrl = response.Headers.Location?.AbsoluteUri;
                if (locationUrl is null)
                {
                    break;
                }

                url = locationUrl;
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
                var locationUrl = response.Headers.Location?.AbsoluteUri;
                if (locationUrl is null)
                {
                    break;
                }

                url = locationUrl;
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
        {
            throw new ResourceNotFoundException(catalogUrl, serviceProviderTitle);
        }

        var catalog = response.Resources?.SingleOrDefault();

        if (catalog != null)
        {
            foreach (var sp in catalog.GetServiceProviders())
            {
                if (sp.GetTitle() != null &&
                    string.Compare(sp.GetTitle(), serviceProviderTitle,
                        StringComparison.OrdinalIgnoreCase) == 0)
                {
                    retval = sp.GetAbout().ToString();
                    break;
                }
            }
        }

        if (retval == null)
        {
            throw new ResourceNotFoundException(catalogUrl, serviceProviderTitle);
        }

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
        {
            throw new ResourceNotFoundException(serviceProviderUrl, "QueryCapability");
        }

        var serviceProvider = response.Resources.SingleOrDefault();

        if (serviceProvider != null)
        {
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
                            {
                                //return as soon as domain + resource type are matched
                                if (resourceType.ToString() != null &&
                                    resourceType.ToString().Equals(oslcResourceType))
                                {
                                    return queryCapability.GetQueryBase().OriginalString;
                                }
                            }

                            //Check if this is the default capability
                            foreach (var usage in queryCapability.GetUsages())
                            {
                                if (usage.ToString() != null && usage.ToString()
                                        .Equals(OSLCConstants.USAGE_DEFAULT_URI))
                                {
                                    defaultQueryCapability = queryCapability;
                                }
                            }
                        }
                    }
                }
            }
        }

        //If we reached this point, there was no resource type match
        if (defaultQueryCapability != null)
        {
            //return default, if present
            return defaultQueryCapability.GetQueryBase().ToString();
        }

        if (firstQueryCapability != null &&
            firstQueryCapability.GetResourceTypes().Length == 0)
        {
            //return the first for the domain, if present
            return firstQueryCapability.GetQueryBase().ToString();
        }

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
        {
            throw new ResourceNotFoundException(serviceProviderUrl, "CreationFactory");
        }

        var serviceProvider = response.Resources?.SingleOrDefault();

        if (serviceProvider != null)
        {
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
                            {
                                //return as soon as domain + resource type are matched
                                if (resourceType.ToString() != null &&
                                    resourceType.ToString().Equals(oslcResourceType))
                                {
                                    return creationFactory.GetCreation().ToString();
                                }
                            }

                            //Check if this is the default factory
                            foreach (var usage in creationFactory.GetUsages())
                            {
                                if (usage.ToString() != null && usage.ToString()
                                        .Equals(OSLCConstants.USAGE_DEFAULT_URI))
                                {
                                    defaultCreationFactory = creationFactory;
                                }
                            }
                        }
                    }
                }
            }
        }

        //If we reached this point, there was no resource type match
        if (defaultCreationFactory != null)
        {
            //return default, if present
            return defaultCreationFactory.GetCreation().ToString();
        }

        if (firstCreationFactory != null &&
            firstCreationFactory.GetResourceTypes().Length == 0)
        {
            //return the first for the domain, if present
            return firstCreationFactory.GetCreation().ToString();
        }

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

