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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core
{
    /// <summary>
    /// An OSLC Client.
    /// </summary>
    public class OslcClient
    {
        protected readonly ISet<MediaTypeFormatter> formatters;
        protected readonly HttpClient client;

        /// <summary>
        /// Initialize a new OslcClient, accepting all SSL certificates.
        /// </summary>
        public OslcClient() : this(null)
        {
        }

        /// <summary>
        /// Initialize a new OslcClient.
        /// </summary>
        /// <param name="certCallback">optionally control SSL certificate management</param>
        public OslcClient(RemoteCertificateValidationCallback certCallback) : this(certCallback, null)
        {
        }

        /// <summary>
        /// Initialize a new OslcClient
        /// </summary>
        /// <param name="certCallback">optionally control SSL certificate management</param>
        /// <param name="oauthHandler">optionally use OAuth</param>
        protected OslcClient(RemoteCertificateValidationCallback certCallback,
                             HttpMessageHandler oauthHandler)
        {
            formatters = new HashSet<MediaTypeFormatter>();

            formatters.Add(new RdfXmlMediaTypeFormatter());

            client = oauthHandler == null ?
                HttpClientFactory.Create(CreateSSLHandler(certCallback)) :
                HttpClientFactory.Create(oauthHandler);
        }

        /// <summary>
        /// Create an SSL Web Request Handler
        /// </summary>
        /// <param name="certCallback">optionally control SSL certificate management</param>
        /// <returns></returns>
        public static WebRequestHandler CreateSSLHandler(RemoteCertificateValidationCallback certCallback = null)
        {
            WebRequestHandler webRequestHandler = new WebRequestHandler();

            webRequestHandler.AllowAutoRedirect = false;
            webRequestHandler.ServerCertificateValidationCallback = certCallback != null ?
                certCallback :
                new RemoteCertificateValidationCallback(AcceptAllServerCertificates);

            return webRequestHandler;
        }

        /// <summary>
        /// Returns the HTTP client associated with this OSLC Client
        /// </summary>
        /// <returns>the HTTP client</returns>
        public HttpClient GetHttpClient()
        {
            return client;
        }

        /// <summary>
        /// Abstract method get an OSLC resource and return a HttpResponseMessage
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mediaType"></param>
        /// <returns>the HttpResponseMessage</returns>
	    public HttpResponseMessage GetResource(string url, string mediaType)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

            HttpResponseMessage response = null;
            bool redirect = false;

            do
            {
                response = client.GetAsync(url).Result;

                if ((response.StatusCode == HttpStatusCode.MovedPermanently) ||
                    (response.StatusCode == HttpStatusCode.Moved))
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
        /// <returns></returns>
	    public HttpResponseMessage DeleteResource(string url)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            HttpResponseMessage response = null;
            bool redirect = false;

            do
            {
                response = client.DeleteAsync(url).Result;

                if ((response.StatusCode == HttpStatusCode.MovedPermanently) ||
                    (response.StatusCode == HttpStatusCode.Moved))
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
        /// Create (POST) an artifact to a URL - usually an OSLC Creation Factory
        /// </summary>
        /// <param name="url"></param>
        /// <param name="artifact"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public HttpResponseMessage CreateResource(string url, object artifact, string mediaType)
        {
            return CreateResource(url, artifact, mediaType, "*/*");
        }

        /// <summary>
        /// Create (POST) an artifact to a URL - usually an OSLC Creation Factory
        /// </summary>
        /// <param name="url"></param>
        /// <param name="artifact"></param>
        /// <param name="mediaType"></param>
        /// <param name="acceptType"></param>
        /// <returns></returns>
        public HttpResponseMessage CreateResource(string url, object artifact, string mediaType, string acceptType)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
            client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

            MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(mediaType);
            MediaTypeFormatter formatter =
                new MediaTypeFormatterCollection(formatters).FindWriter(artifact.GetType(), mediaTypeValue);

            HttpResponseMessage response = null;
            bool redirect = false;

            do
            {
                ObjectContent content = new ObjectContent(artifact.GetType(), artifact, formatter);

                content.Headers.ContentType = mediaTypeValue;

                response = client.PostAsync(url, content).Result;

                if ((response.StatusCode == HttpStatusCode.MovedPermanently) ||
                    (response.StatusCode == HttpStatusCode.Moved))
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
        public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType, string acceptType)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
            client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

            MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(mediaType);
            MediaTypeFormatter formatter =
                new MediaTypeFormatterCollection(formatters).FindWriter(artifact.GetType(), mediaTypeValue);

            HttpResponseMessage response = null;
            bool redirect = false;

            do
            {
                ObjectContent content = new ObjectContent(artifact.GetType(), artifact, formatter);

                content.Headers.ContentType = mediaTypeValue;

                response = client.PutAsync(url, content).Result;

                if ((response.StatusCode == HttpStatusCode.MovedPermanently) ||
                    (response.StatusCode == HttpStatusCode.Moved))
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
        public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType, string acceptType, string ifMatch)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
            client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
            client.DefaultRequestHeaders.Add(HttpRequestHeader.IfMatch.ToString(), ifMatch);

            MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue(mediaType);
            MediaTypeFormatter formatter =
                new MediaTypeFormatterCollection(formatters).FindWriter(artifact.GetType(), mediaTypeValue);

            HttpResponseMessage response = null;
            bool redirect = false;

            do
            {
                ObjectContent content = new ObjectContent(artifact.GetType(), artifact, formatter);

                content.Headers.ContentType = mediaTypeValue;

                response = client.PutAsync(url, content).Result;

                if ((response.StatusCode == HttpStatusCode.MovedPermanently) ||
                    (response.StatusCode == HttpStatusCode.Moved))
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
	    public string LookupServiceProviderUrl(string catalogUrl, string serviceProviderTitle)
        {
            string retval = null;
            HttpResponseMessage response = GetResource(catalogUrl, OSLCConstants.CT_RDF);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ResourceNotFoundException(catalogUrl, serviceProviderTitle);
            }

            ServiceProviderCatalog catalog = response.Content.ReadAsAsync<ServiceProviderCatalog>(formatters).Result;

            if (catalog != null)
            {
                foreach (ServiceProvider sp in catalog.GetServiceProviders())
                {
                    if (sp.GetTitle() != null && string.Compare(sp.GetTitle(), serviceProviderTitle, true) == 0)
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
        /// <returns>URL of requested Query Capablility or null if not found.</returns>
        public string LookupQueryCapability(string serviceProviderUrl, string oslcDomain, string oslcResourceType)
        {
            QueryCapability defaultQueryCapability = null;
            QueryCapability firstQueryCapability = null;

            HttpResponseMessage response = GetResource(serviceProviderUrl, OSLCConstants.CT_RDF);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ResourceNotFoundException(serviceProviderUrl, "QueryCapability");
            }

            ServiceProvider serviceProvider = response.Content.ReadAsAsync<ServiceProvider>(formatters).Result;

            if (serviceProvider != null)
            {
                foreach (Service service in serviceProvider.GetServices())
                {
                    Uri domain = service.GetDomain();
                    if (domain != null && domain.ToString().Equals(oslcDomain))
                    {
                        QueryCapability[] queryCapabilities = service.GetQueryCapabilities();
                        if (queryCapabilities != null && queryCapabilities.Length > 0)
                        {
                            firstQueryCapability = queryCapabilities[0];
                            foreach (QueryCapability queryCapability in service.GetQueryCapabilities())
                            {
                                foreach (Uri resourceType in queryCapability.GetResourceTypes())
                                {
                                    //return as soon as domain + resource type are matched
                                    if (resourceType.ToString() != null && resourceType.ToString().Equals(oslcResourceType))
                                    {
                                        return queryCapability.GetQueryBase().OriginalString;
                                    }
                                }
                                //Check if this is the default capability
                                foreach (Uri usage in queryCapability.GetUsages())
                                {
                                    if (usage.ToString() != null && usage.ToString().Equals(OSLCConstants.USAGE_DEFAULT_URI))
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
            else if (firstQueryCapability != null && firstQueryCapability.GetResourceTypes().Length == 0)
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
        public string LookupCreationFactory(string serviceProviderUrl, string oslcDomain, string oslcResourceType)
        {
            CreationFactory defaultCreationFactory = null;
            CreationFactory firstCreationFactory = null;

            HttpResponseMessage response = GetResource(serviceProviderUrl, OSLCConstants.CT_RDF);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ResourceNotFoundException(serviceProviderUrl, "CreationFactory");
            }

            ServiceProvider serviceProvider = response.Content.ReadAsAsync<ServiceProvider>(formatters).Result;

            if (serviceProvider != null)
            {
                foreach (Service service in serviceProvider.GetServices())
                {
                    Uri domain = service.GetDomain();
                    if (domain != null && domain.ToString().Equals(oslcDomain))
                    {
                        CreationFactory[] creationFactories = service.GetCreationFactories();
                        if (creationFactories != null && creationFactories.Length > 0)
                        {
                            firstCreationFactory = creationFactories[0];
                            foreach (CreationFactory creationFactory in creationFactories)
                            {
                                foreach (Uri resourceType in creationFactory.GetResourceTypes())
                                {
                                    //return as soon as domain + resource type are matched
                                    if (resourceType.ToString() != null && resourceType.ToString().Equals(oslcResourceType))
                                    {
                                        return creationFactory.GetCreation().ToString();
                                    }
                                }
                                //Check if this is the default factory
                                foreach (Uri usage in creationFactory.GetUsages())
                                {
                                    if (usage.ToString() != null && usage.ToString().Equals(OSLCConstants.USAGE_DEFAULT_URI))
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
            else if (firstCreationFactory != null && firstCreationFactory.GetResourceTypes().Length == 0)
            {
                //return the first for the domain, if present
                return firstCreationFactory.GetCreation().ToString();
            }

            throw new ResourceNotFoundException(serviceProviderUrl, "CreationFactory");
        }

        public ISet<MediaTypeFormatter> GetFormatters()
        {
            return formatters;
        }

        /// <summary>
        /// Handle SSL server certificate processing, accepting all certificates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static bool AcceptAllServerCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }

    public static class ConsumeContentExtension
    {
        public static void ConsumeContent(this HttpResponseMessage response)
        {
            response.Content.Dispose();
        }
    }
}