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
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace OSLC4Net.Client.Oslc;

/// <summary>
/// An OSLC Client.
/// </summary>
public class OslcClient
{
    private static readonly ILog log =
        LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected readonly ISet<MediaTypeFormatter> formatters;
    protected readonly HttpClient client;

    protected string AcceptHeader { get; } =
        "text/turtle;q=1.0, application/rdf+xml;q=0.9, application/n-triples;q=0.8, text/n3;q=0.7";

    /// <summary>
    /// Initialize a new OslcClient.
    /// </summary>
    public OslcClient() : this(false)
    {
    }

    /// <summary>
    /// Initialize a new OslcClient.
    /// </summary>
    /// <param name="certCallback">optionally control SSL certificate management
    /// (null will not replace the default validation callback)</param>
    [Obsolete]
    public OslcClient(Func<HttpRequestMessage, X509Certificate2, X509Chain,
        SslPolicyErrors, bool> certCallback) : this(certCallback, null)
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
        SslPolicyErrors, bool> certCallback, HttpMessageHandler userHttpMessageHandler)
    {
        this.formatters = new HashSet<MediaTypeFormatter>();

        // REVISIT: RDF/XML + Turtle support only for now (@berezovskyi 2024-10)
        formatters.Add(new RdfXmlMediaTypeFormatter());

        HttpMessageHandler handler = userHttpMessageHandler;
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
        client = HttpClientFactory.Create(handler);
    }

    OslcClient(HttpClientHandler customHandler) : this(null, customHandler)
    {
    }

    OslcClient(bool allowInvalidTlsCerts)
    {
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
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        client = new HttpClient(handler);
    }

    public static OslcClient ForBasicAuth(string username, string password,
        HttpClientHandler handler = null)
    {
        var oslcClient = new OslcClient(handler);
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
        X509Chain, SslPolicyErrors, bool> certCallback = null)
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
        return client;
    }


    public async Task<OslcResponse<T>> GetResourceAsync<T>(string resourceUri) where T: IExtendedResource, new()
    {
        var httpResponseMessage = await GetResourceAsync(resourceUri);
        // REVISIT: according to the spec, non-success codes may also come with a RDF response - should, actually! (@berezovskyi 2024-10)
        // consider adding .ErrorResource to the OslcResponse
        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.Content is not null)
        {
            var g = new Graph();
            // REVISIT: response.Content.ReadAsAsync<T>() and mediaformatter instead (@berezovskyi 2024-10)
            var parser = DetectRdfReader(httpResponseMessage);
            var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var streamReader = new StreamReader(stream);

            parser.Load(g, streamReader);

            var resource = (T)DotNetRdfHelper.FromDotNetRdfNode(g.CreateUriNode(new Uri(resourceUri)), g,
                typeof(T));
            return OslcResponse<T>.WithSuccess(resource, httpResponseMessage);
        }
        else
        {
            return OslcResponse<T>.WithError(httpResponseMessage);
        }
    }

    private static IRdfReader DetectRdfReader(HttpResponseMessage responseMessage)
    {
        // TODO: unify with response.Content.ReadAsAsync<T>()
        return responseMessage.Content?.Headers?.ContentType?.MediaType switch
        {
            "application/rdf+xml" => new RdfXmlParser(),
            "application/t-triples" => new NTriplesParser(),
            "text/n3" => new Notation3Parser(),
            "text/turtle" => new TurtleParser(),
            // TODO: IStoreReader because supports graphs, I assume (@berezovskyi 2024-10)
            // "application/ld+json" => new JsonLdParser(),
            // REVISIT: getting HTML back usually means a misconfigured server, e.g. an auth page (@berezovskyi 2024-10)
            // However, according to the spec, getting application/xhtml+xml or text/html could mean
            // RDFa output (though could also be embedded JSON-LD). For OslcClient, we shall consider
            // RDFa use to be VERY unlikely.
            "application/xhtml+xml" => new RdfAParser(),
            "text/html" => new RdfAParser(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Abstract method get an OSLC resource and return a HttpResponseMessage
    /// </summary>
    /// <param name="url"></param>
    /// <param name="mediaType"></param>
    /// <returns>the HttpResponseMessage</returns>
    [Obsolete("Prefer async")]
    public HttpResponseMessage GetResource(string url, string mediaType = null)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        // TODO: use uniformly (@berezovskyi 2024-10)
        client.DefaultRequestHeaders.Accept.ParseAdd(mediaType ?? AcceptHeader);
        client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        HttpResponseMessage response;
        bool redirect;
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
    /// Consider using <see cref="GetResourceAsync{T}"/> instead.
    /// </summary>
    public async Task<HttpResponseMessage> GetResourceAsync(string url, string mediaType = null)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        // TODO: use uniformly (@berezovskyi 2024-10)
        client.DefaultRequestHeaders.Accept.ParseAdd(mediaType ?? AcceptHeader);
        client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        HttpResponseMessage response;
        bool redirect;
        do
        {
            response = await client.GetAsync(url);

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
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        HttpResponseMessage response;
        bool redirect;
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
    public HttpResponseMessage CreateResource(string url, object artifact, string mediaType,
        string acceptType)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
        client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

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
    public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType,
        string acceptType)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
        client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

            content.Headers.ContentType = mediaTypeValue;

            // FIXME: await (@berezovskyi 2024-10)
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
    public HttpResponseMessage UpdateResource(string url, object artifact, string mediaType,
        string acceptType, string ifMatch)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptType));
        client.DefaultRequestHeaders.Remove(OSLCConstants.OSLC_CORE_VERSION);
        client.DefaultRequestHeaders.Add(OSLCConstants.OSLC_CORE_VERSION, "2.0");
        client.DefaultRequestHeaders.Add(HttpRequestHeader.IfMatch.ToString(), ifMatch);

        var mediaTypeValue = new MediaTypeHeaderValue(mediaType);
        var formatter =
            new MediaTypeFormatterCollection(formatters).FindWriter(artifact.GetType(),
                mediaTypeValue);
        HttpResponseMessage response;
        bool redirect;
        do
        {
            var content = new ObjectContent(artifact.GetType(), artifact, formatter);

            content.Headers.ContentType = mediaTypeValue;

            // FIXME: await (@berezovskyi 2024-10)
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
        var response = GetResource(catalogUrl, OSLCConstants.CT_RDF);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ResourceNotFoundException(catalogUrl, serviceProviderTitle);
        }

        var catalog = response.Content.ReadAsAsync<ServiceProviderCatalog>(formatters).Result;

        if (catalog != null)
        {
            foreach (var sp in catalog.GetServiceProviders())
            {
                if (sp.GetTitle() != null &&
                    string.Compare(sp.GetTitle(), serviceProviderTitle, true) == 0)
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
    public string LookupQueryCapability(string serviceProviderUrl, string oslcDomain,
        string oslcResourceType)
    {
        QueryCapability defaultQueryCapability = null;
        QueryCapability firstQueryCapability = null;

        var response = GetResource(serviceProviderUrl, OSLCConstants.CT_RDF);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ResourceNotFoundException(serviceProviderUrl, "QueryCapability");
        }

        var serviceProvider = response.Content.ReadAsAsync<ServiceProvider>(formatters).Result;

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
    public string LookupCreationFactory(string serviceProviderUrl, string oslcDomain,
        string oslcResourceType)
    {
        CreationFactory defaultCreationFactory = null;
        CreationFactory firstCreationFactory = null;

        var response = GetResource(serviceProviderUrl, OSLCConstants.CT_RDF);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ResourceNotFoundException(serviceProviderUrl, "CreationFactory");
        }

        var serviceProvider = response.Content.ReadAsAsync<ServiceProvider>(formatters).Result;

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
    [Obsolete]
    public static bool AcceptAllServerCertificates(object sender, X509Certificate certificate,
        X509Chain chain, SslPolicyErrors errors)
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
