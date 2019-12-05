/*******************************************************************************
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using OSLC4Net.Core.Model;

namespace OSLC4Net.Client
{
    /// <summary>
    /// A class providing client utilities to query/get, create, update and delete OSLC resources
    /// </summary>
    public sealed class OslcRestClient
    {
        public const int DEFAULT_READ_TIMEOUT = 60000;

        private readonly ISet<MediaTypeFormatter>   formatters;
        private readonly string uri;
        private readonly HttpClient                 client;
        private readonly string mediaType;
        private readonly int                        readTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="uri"></param>
        /// <param name="mediaType"></param>
        /// <param name="readTimeout"></param>
	    public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
                              string uri,
                              string mediaType,
	                          int                       readTimeout)
	    {
	        this.formatters  = formatters;
		    this.uri         = uri;
		    this.mediaType   = mediaType;
		    this.readTimeout = readTimeout;

		    client = new HttpClient();

            client.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * readTimeout);
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="uri"></param>
        /// <param name="mediaType"></param>
	    public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
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
        public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
                              string uri,
                              int                       timeout) :
            this(formatters, uri, OslcMediaType.APPLICATION_RDF_XML, timeout)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="uri"></param>
        public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
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
        public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
                              Uri                       uri,
                              string mediaType,
                              int                       readTimeout) :
            this(formatters, uri.ToString(), mediaType, readTimeout)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="uri"></param>
        /// <param name="mediaType"></param>
	    public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
                              Uri                       uri,
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
        public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
                              Uri                       uri,
                              int                       timeout) :
            this(formatters, uri.ToString(), OslcMediaType.APPLICATION_RDF_XML, timeout)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="uri"></param>
	    public OslcRestClient(ISet<MediaTypeFormatter>  formatters,
                              Uri                       uri) :
            this(formatters, uri.ToString(), OslcMediaType.APPLICATION_RDF_XML, DEFAULT_READ_TIMEOUT)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    public ISet<MediaTypeFormatter> GetFormatters()
	    {
	        return formatters;
	    }

	    public string GetMediaType()
	    {
	        return mediaType;
	    }

	    public int GetReadTimeout()
	    {
	        return readTimeout;
	    }

        public string GetUri()
        {
            return uri;
        }

	    public HttpClient GetClient()
	    {
		    return client;
	    }

        /// <summary>
        /// Retrieve an OSLC resource of the specified type.  The type must have an associated .NET class with OSLC4Net annotations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    public T GetOslcResource<T>() where T : class
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

    	    HttpResponseMessage response = client.GetAsync(uri).Result;
            HttpStatusCode statusCode = response.StatusCode;

            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    return response.Content.ReadAsAsync<T>(formatters).Result;
                case HttpStatusCode.NoContent:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Gone:
                    return null;
                default:
                    throw new HttpRequestException(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Get an array of OSLC resources of the specified type.  The type must have an associated .NET class with OSLC4Net annotations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    public T[] GetOslcResources<T>()
	    {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

    	    HttpResponseMessage response = client.GetAsync(uri).Result;
            HttpStatusCode statusCode = response.StatusCode;

            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    T[] dummy = new T[0];
                    return (T[])response.Content.ReadAsAsync(dummy.GetType(), formatters).Result;
                case HttpStatusCode.NoContent:
                case HttpStatusCode.NotFound:
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
        public T AddOslcResource<T>(T oslcResource)
	    {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(mediaType);
            MediaTypeFormatter formatter =
                new MediaTypeFormatterCollection(formatters).FindWriter(oslcResource.GetType(), mediaTypeValue);
            ObjectContent<T> content = new ObjectContent<T>(oslcResource, formatter);

            content.Headers.ContentType = mediaTypeValue;

            return client.PostAsync(uri, content).ContinueWith(response =>
                {
                    HttpStatusCode status = response.Result.StatusCode;

                    if (status != HttpStatusCode.Created && status != HttpStatusCode.OK)
                    {
                        throw new HttpRequestException(response.Result.ReasonPhrase);
                    }

                    return response;
                }).Result.Result.Content.ReadAsAsync<T>(formatters).Result;
            }

        /// <summary>
        /// Add an OSLC resource of the specified type and return an HttpResponseMessage.
        /// The type must have an associated .NET class with OSLC4Net annotations.
        /// </summary>
        /// <param name="oslcResource"></param>
        /// <returns></returns>
	    public HttpResponseMessage AddOslcResourceReturnClientResponse(object oslcResource)
	    {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(mediaType);
            MediaTypeFormatter formatter =
                new MediaTypeFormatterCollection(formatters).FindWriter(oslcResource.GetType(), mediaTypeValue);
            ObjectContent content = new ObjectContent(oslcResource.GetType(), oslcResource, formatter);

            content.Headers.ContentType = mediaTypeValue;

            HttpResponseMessage response = client.PostAsync(uri, content).Result;
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
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            MediaTypeFormatter formatter =
                new MediaTypeFormatterCollection(formatters).FindWriter(oslcResource.GetType(), new MediaTypeHeaderValue(mediaType));
            ObjectContent content = new ObjectContent(oslcResource.GetType(), oslcResource, formatter);

            content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            HttpResponseMessage response = client.PutAsync(uri, content).Result;

            return response;
        }

        
        /// <summary>
        /// Remove an OSLC resource
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage RemoveOslcResourceReturnClientResponse()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            HttpResponseMessage response = client.DeleteAsync(uri).Result;

            return response;
        }
    }
}
