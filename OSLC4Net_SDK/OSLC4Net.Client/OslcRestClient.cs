﻿/*******************************************************************************
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client;

/// <summary>
/// A class providing client utilities to query/get, create, update and delete OSLC resources
/// </summary>
public sealed class OslcRestClient
{
    /// <summary>
    /// HTTP client timeout, ms
    /// </summary>
    public const int DEFAULT_READ_TIMEOUT = 60000;

    public static readonly IEnumerable<MediaTypeFormatter> DEFAULT_FORMATTERS =
        new HashSet<MediaTypeFormatter>(new[] { new RdfXmlMediaTypeFormatter() });

    private readonly IEnumerable<MediaTypeFormatter> _formatters;
    private readonly string _uri;
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
        _formatters = formatters;
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

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MediaTypeFormatter> GetFormatters()
    {
        return _formatters;
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
    public async Task<T> GetOslcResourceAsync<T>() where T : class, IResource
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType, 1.0));
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));

        HttpResponseMessage response = await _client.GetAsync(_uri);
        HttpStatusCode statusCode = response.StatusCode;

        return statusCode switch
        {
            HttpStatusCode.OK => await response.Content.ReadAsAsync<T>(_formatters),
            HttpStatusCode.NoContent or HttpStatusCode.NotFound or HttpStatusCode.Gone => null,
            _ => throw new HttpRequestException(response.ReasonPhrase),
        };
    }

    /// <summary>
    /// Get an array of OSLC resources of the specified type.  The type must have an associated .NET class with OSLC4Net annotations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<ICollection<T>> GetOslcResourcesAsync<T>()
    {
        // _client.DefaultRequestHeaders.Clear();
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType));

        HttpResponseMessage response = await _client.GetAsync(_uri);
        HttpStatusCode statusCode = response.StatusCode;

        switch (statusCode)
        {
            case HttpStatusCode.OK:
                // TODO: check if we can get rid of this Java-looking code
                var dummy = new T[0];
                return await response.Content.ReadAsAsync(dummy.GetType(), _formatters) as T[];
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
    public async Task<T> AddOslcResourceAsync<T>(T oslcResource) where T : class, IResource
    {
        // FIXME: stop clearing
        // _client.DefaultRequestHeaders.Clear();

        // FIXME: move to per-request basis
        // content.Headers doesn't allow to set the Accept header
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType, 1.0));
        // _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.9));

        MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(_mediaType);
        MediaTypeFormatter formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(oslcResource.GetType(),
                mediaTypeValue);
        ObjectContent<T> content = new ObjectContent<T>(oslcResource, formatter);

        content.Headers.ContentType = mediaTypeValue;

        var creation = await _client.PostAsync(_uri, content);
        var status = creation.StatusCode;

        if (status == HttpStatusCode.OK)
        {
            return await creation.Content.ReadAsAsync<T>(_formatters);
        }
        else if (status == HttpStatusCode.Created)
        {
            // FIXME: stop allocating a new client for every request
            var followUpClient =
                new OslcRestClient(_formatters, creation.Headers.Location, _mediaType);

            return await followUpClient.GetOslcResourceAsync<T>();
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

        MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(_mediaType);
        MediaTypeFormatter formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(oslcResource.GetType(),
                mediaTypeValue);
        ObjectContent content = new ObjectContent(oslcResource.GetType(), oslcResource, formatter);

        content.Headers.ContentType = mediaTypeValue;

        HttpResponseMessage response = _client.PostAsync(_uri, content).Result;
        HttpStatusCode statusCode = response.StatusCode;

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

        MediaTypeFormatter formatter =
            new MediaTypeFormatterCollection(_formatters).FindWriter(oslcResource.GetType(),
                new MediaTypeHeaderValue(_mediaType));
        ObjectContent content = new ObjectContent(oslcResource.GetType(), oslcResource, formatter);

        content.Headers.ContentType = new MediaTypeHeaderValue(_mediaType);

        HttpResponseMessage response = _client.PutAsync(_uri, content).Result;

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

        HttpResponseMessage response = _client.DeleteAsync(_uri).Result;

        return response;
    }
}
