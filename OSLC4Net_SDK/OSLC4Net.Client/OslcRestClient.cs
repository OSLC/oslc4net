/*******************************************************************************
 * Copyright (c) 2023 Andrii Berezovskyi and OSLC4Net contributors.
 * Copyright (c) 2012 IBM Corporation.
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
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client;

/// <summary>
/// A class providing client utilities to query/get, create, update and delete OSLC resources
/// </summary>
[Obsolete("Use OslcClient instead.")]
public sealed class OslcRestClient
{
    /// <summary>
    /// HTTP client timeout, ms
    /// </summary>
    public const int DEFAULT_READ_TIMEOUT = 60000;

    public IReadOnlyCollection<MediaTypeFormatter> Formatters { get; }

    private readonly string _uri;
    private readonly DotNetRdfHelper _rdfHelper;
    private readonly HttpClient _client;
    private readonly string _mediaType;
    private readonly int _readTimeout;

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    /// <param name="mediaType"></param>
    /// <param name="readTimeout">in milliseconds</param>
    public OslcRestClient(
        IEnumerable<MediaTypeFormatter> formatters,
        string uri,
        string mediaType,
        int readTimeout)
    {
        Formatters = formatters.ToList();
        _uri = uri;
        _mediaType = mediaType;
        _readTimeout = readTimeout;

        _client = new HttpClient
        {
            Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * readTimeout)
        };
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(_mediaType, 1.0));
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));
    }

    public OslcRestClient(
        string uri,
        DotNetRdfHelper rdfHelper,
        string? mediaType = null,
        int? readTimeout = null)
    {
        _uri = uri;
        _rdfHelper = rdfHelper;
        _mediaType = mediaType;
        _readTimeout = readTimeout ?? DEFAULT_READ_TIMEOUT;
        Formatters =
            new HashSet<MediaTypeFormatter>(new[] { new RdfXmlMediaTypeFormatter(_rdfHelper) })
            ;

        _client = new HttpClient
        {
            Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * _readTimeout)
        };
        _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(_mediaType, 1.0));
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    /// <param name="mediaType"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        string uri,
        string mediaType) :
        this(formatters, uri, mediaType, DEFAULT_READ_TIMEOUT)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    /// <param name="timeout"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        string uri,
        int timeout) :
        this(formatters, uri, OslcMediaType.APPLICATION_RDF_XML, timeout)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        string uri) :
        this(formatters, uri, OslcMediaType.APPLICATION_RDF_XML, DEFAULT_READ_TIMEOUT)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    /// <param name="mediaType"></param>
    /// <param name="readTimeout"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        Uri uri,
        string mediaType,
        int readTimeout) :
        this(formatters, uri.ToString(), mediaType, readTimeout)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    /// <param name="mediaType"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        Uri uri,
        string mediaType) :
        this(formatters, uri.ToString(), mediaType, DEFAULT_READ_TIMEOUT)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    /// <param name="timeout"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        Uri uri,
        int timeout) :
        this(formatters, uri.ToString(), OslcMediaType.APPLICATION_RDF_XML, timeout)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="formatters"></param>
    /// <param name="uri"></param>
    public OslcRestClient(IEnumerable<MediaTypeFormatter> formatters,
        Uri uri) :
        this(formatters, uri.ToString(), OslcMediaType.APPLICATION_RDF_XML, DEFAULT_READ_TIMEOUT)
    {
    }

    public string GetMediaType()
    {
        return _mediaType;
    }

    public int GetReadTimeout()
    {
        return _readTimeout;
    }

    public string GetUri()
    {
        return _uri;
    }

    public HttpClient GetClient()
    {
        return _client;
    }

    /// <summary>
    /// Retrieve an OSLC resource of the specified type.  The type must have an associated .NET class with OSLC4Net annotations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T?> GetOslcResourceAsync<T>() where T : class, IResource
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType, 1.0));
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));

        var response = await _client.GetAsync(_uri).ConfigureAwait(false);
        var statusCode = response.StatusCode;

        return statusCode switch
        {
            HttpStatusCode.OK => await response.Content.ReadAsAsync<T>(Formatters)
                .ConfigureAwait(false),
            HttpStatusCode.NoContent or HttpStatusCode.NotFound or HttpStatusCode.Gone => null,
            _ => throw new HttpRequestException(response.ReasonPhrase)
        };
    }

    /// <summary>
    /// Get an array of OSLC resources of the specified type.  The type must have an associated .NET class with OSLC4Net annotations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<ICollection<T>?> GetOslcResourcesAsync<T>()
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType));

        var response = await _client.GetAsync(_uri).ConfigureAwait(false);
        var statusCode = response.StatusCode;

        switch (statusCode)
        {
            case HttpStatusCode.OK:
                // TODO: check if we can get rid of this Java-looking code
                var dummy = Array.Empty<T>();
                return await response.Content.ReadAsAsync(dummy.GetType(), Formatters)
                    .ConfigureAwait(false) as T[];
            case HttpStatusCode.NoContent:
            // case HttpStatusCode.NotFound:
            case HttpStatusCode.Gone:
                return null;
            default:
                throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    /// <summary>
    /// Create an OSLC resource of the specified type.  The type must have an associated .NET class with OSLC4Net annotations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="oslcResource"></param>
    /// <returns></returns>
    public async Task<T?> AddOslcResourceAsync<T>(T oslcResource) where T : class, IResource
    {
        // FIXME: stop clearing
        // _client.DefaultRequestHeaders.Clear();

        // FIXME: move to per-request basis
        // content.Headers doesn't allow to set the Accept header
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType, 1.0));
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));

        var mediaTypeValue = new MediaTypeHeaderValue(_mediaType);
        var formatter =
            new MediaTypeFormatterCollection(Formatters).FindWriter(oslcResource.GetType(),
                mediaTypeValue);
        var content = new ObjectContent<T>(oslcResource, formatter);

        content.Headers.ContentType = mediaTypeValue;

        var creation = await _client.PostAsync(_uri, content).ConfigureAwait(false);
        var status = creation.StatusCode;

        if (status == HttpStatusCode.OK)
        {
            return await creation.Content.ReadAsAsync<T>(Formatters).ConfigureAwait(false);
        }
        else if (status == HttpStatusCode.Created)
        {
            // FIXME: stop allocating a new client for every request
            var followUpClient =
                new OslcRestClient(Formatters, creation.Headers.Location, _mediaType);

            return await followUpClient.GetOslcResourceAsync<T>().ConfigureAwait(false);
        }

        throw new HttpRequestException(creation.ReasonPhrase);
    }

    /// <summary>
    /// Add an OSLC resource of the specified type and return an HttpResponseMessage.
    /// The type must have an associated .NET class with OSLC4Net annotations.
    /// </summary>
    /// <param name="oslcResource"></param>
    /// <returns></returns>
    public HttpResponseMessage AddOslcResourceReturnClientResponse(object oslcResource)
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var mediaTypeValue = new MediaTypeHeaderValue(_mediaType);
        var formatter =
            new MediaTypeFormatterCollection(Formatters).FindWriter(oslcResource.GetType(),
                mediaTypeValue);
        var content = new ObjectContent(oslcResource.GetType(), oslcResource, formatter);

        content.Headers.ContentType = mediaTypeValue;

        var response = _client.PostAsync(_uri, content).Result;
        var statusCode = response.StatusCode;

        switch (statusCode)
        {
            case HttpStatusCode.OK:
            case HttpStatusCode.Created:
                return response;
            default:
                throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    /// <summary>
    /// Update an OSLC resource of the specified type and return an HttpResponseMessage.
    /// The type must have an associated .NET class with OSLC4Net annotations.
    /// </summary>
    /// <param name="oslcResource"></param>
    /// <returns></returns>
    public HttpResponseMessage UpdateOslcResourceReturnClientResponse(object oslcResource)
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var formatter =
            new MediaTypeFormatterCollection(Formatters).FindWriter(oslcResource.GetType(),
                new MediaTypeHeaderValue(_mediaType));
        var content = new ObjectContent(oslcResource.GetType(), oslcResource, formatter);

        content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType);

        var response = _client.PutAsync(_uri, content).Result;

        return response;
    }

    /// <summary>
    /// Remove an OSLC resource
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage RemoveOslcResourceReturnClientResponse()
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var response = _client.DeleteAsync(_uri).Result;

        return response;
    }
}
