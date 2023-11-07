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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client;

/// <summary>
/// This classs provides methods to register and deregister with an OSLC ServiceProvider Registry (not yet
/// implemented in OSLC4Net - see Eclipse Lyo for a Java implementation)
///
/// It also provides methods to get a ServiceProviderCatalog and retrieve the ServiceProviders
/// </summary>

public sealed class ServiceProviderRegistryClient
{
    private readonly OslcRestClient _client;

    public OslcRestClient OslcClient => _client;

    /// <summary>
    ///
    /// </summary>
    /// <param name="uri">OSLC Service Provider Catalor URI</param>
    /// <param name="formatters"></param>
    /// <param name="mediaType"></param>
    public ServiceProviderRegistryClient(string uri,
                                        ISet<MediaTypeFormatter> formatters,
                                         string mediaType)
    {
        _client = new OslcRestClient(formatters, uri, mediaType);
    }

    public ServiceProviderRegistryClient(string uri, ISet<MediaTypeFormatter> formatters) :
        this(uri, formatters, OslcMediaType.APPLICATION_RDF_XML)
    {
    }

    public ServiceProviderRegistryClient(string uri) :
        // TODO: build an Accept string from the formatter list on the fly
        this(uri, OslcRestClient.DEFAULT_FORMATTERS, OslcMediaType.APPLICATION_RDF_XML)
    {
    }

    /// <summary>
    /// Register a ServiceProvider
    /// </summary>
    /// <param name="serviceProviderToRegister"></param>
    /// <returns></returns>
    public async Task<Uri> RegisterServiceProviderAsync(ServiceProvider serviceProviderToRegister)
    {
        var typeServiceProviderURI = new Uri(OslcConstants.TYPE_SERVICE_PROVIDER);
        var oslcUsageDefault = new Uri(OslcConstants.OSLC_USAGE_DEFAULT);

        ServiceProvider[] serviceProviders;

        // We have to first get the ServiceProvider for ServiceProviders and then find the CreationFactory for a ServiceProvider

        // We first try for a ServiceProviderCatalog
        var serviceProviderCatalog = await FetchServiceProviderCatalogAsync();

        if (serviceProviderCatalog != null)
        {
            serviceProviders = serviceProviderCatalog.GetServiceProviders();
        }
        else
        {
            // Secondly we try for a ServiceProvider which is acting as a ServiceProvider registry
            ServiceProvider serviceProvider = await GetServiceProviderAsync();

            if (serviceProvider != null)
            {
                serviceProviders = new ServiceProvider[] { serviceProvider };
            }
            else
            {
                throw new OslcCoreRegistrationException(serviceProviderToRegister,
                                                        (int)HttpStatusCode.NotFound,
                                                        "ServiceProviderCatalog");
            }
        }

        if (serviceProviders != null)
        {
            CreationFactory firstCreationFactory = null;
            CreationFactory firstDefaultCreationFactory = null;

            for (int serviceProviderIndex = 0;
                 (serviceProviderIndex < serviceProviders.Length) &&
                  (firstDefaultCreationFactory == null);
                 serviceProviderIndex++)
            {
                ServiceProvider serviceProvider = serviceProviders[serviceProviderIndex];

                Service[] services = serviceProvider.GetServices();

                if (services != null)
                {
                    for (int serviceIndex = 0;
                         (serviceIndex < services.Length) &&
                          (firstDefaultCreationFactory == null);
                         serviceIndex++)
                    {
                        Service service = services[serviceIndex];

                        CreationFactory[] creationFactories = service.GetCreationFactories();

                        if (creationFactories != null)
                        {
                            for (int creationFactoryIndex = 0;
                                 (creationFactoryIndex < creationFactories.Length) &&
                                  (firstDefaultCreationFactory == null);
                                 creationFactoryIndex++)
                            {
                                CreationFactory creationFactory = creationFactories[creationFactoryIndex];

                                Uri[] resourceTypes = creationFactory.GetResourceTypes();

                                if (resourceTypes != null)
                                {
                                    for (int resourceTypeIndex = 0;
                                         (resourceTypeIndex < resourceTypes.Length) &&
                                          (firstDefaultCreationFactory == null);
                                         resourceTypeIndex++)
                                    {
                                        Uri resourceType = resourceTypes[resourceTypeIndex];

                                        if (typeServiceProviderURI.Equals(resourceType))
                                        {
                                            firstCreationFactory ??= creationFactory;

                                            Uri[] usages = creationFactory.GetUsages();

                                            for (int usageIndex = 0;
                                                 (usageIndex < usages.Length) &&
                                                  (firstDefaultCreationFactory == null);
                                                 usageIndex++)
                                            {
                                                Uri usage = usages[usageIndex];

                                                if (oslcUsageDefault.Equals(usage))
                                                {
                                                    firstDefaultCreationFactory = creationFactory;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (firstCreationFactory != null)
            {
                CreationFactory creationFactory = firstDefaultCreationFactory != null ? firstDefaultCreationFactory : firstCreationFactory;

                Uri creation = creationFactory.GetCreation();

                OslcRestClient oslcRestClient = new OslcRestClient(_client.GetFormatters(),
                                                                         creation);

                HttpResponseMessage clientResponse = oslcRestClient.AddOslcResourceReturnClientResponse(serviceProviderToRegister);

                HttpStatusCode statusCode = clientResponse.StatusCode;

                if (statusCode != HttpStatusCode.Created)
                {
                    throw new OslcCoreRegistrationException(serviceProviderToRegister,
                                                            (int)statusCode,
                                                            clientResponse.ReasonPhrase);
                }

                return clientResponse.Headers.Location;
            }
        }

        throw new OslcCoreRegistrationException(serviceProviderToRegister,
                                                (int)HttpStatusCode.NotFound,
                                                "CreationFactory");
    }

    /// <summary>
    /// Remove registration for a ServiceProvider.
    /// </summary>
    /// <param name="serviceProviderURI"></param>
    public void DeregisterServiceProvider(Uri serviceProviderURI)
    {
        HttpResponseMessage clientResponse = new OslcRestClient(_client.GetFormatters(), serviceProviderURI).RemoveOslcResourceReturnClientResponse();

        HttpStatusCode statusCode = clientResponse.StatusCode;
        if (statusCode != HttpStatusCode.OK)
        {
            throw new OslcCoreDeregistrationException(serviceProviderURI,
                                                      (int)statusCode,
                                                      clientResponse.ReasonPhrase);
        }
    }

    /// <summary>
    /// If a {@link ServiceProviderCatalog} is being used, this will return that object.
    /// Otherwise null will be returned.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceProviderCatalog> FetchServiceProviderCatalogAsync()
    {
        return await _client.GetOslcResourceAsync<ServiceProviderCatalog>();
    }

    /// <summary>
    /// If aServiceProvider is being used as a ServiceProvider registry without an owning ServiceProviderCatalog,
    /// this will return the ServiceProvider.
    /// Otherwise null will be returned.
    /// </summary>
    /// <returns></returns>
    public async Task<ServiceProvider> GetServiceProviderAsync()
    {
        return await _client.GetOslcResourceAsync<ServiceProvider>();
    }

    /// <summary>
    /// Return the registered ServiceProvider's.
    /// </summary>
    /// <returns></returns>
    public async Task<ICollection<ServiceProvider>> GetServiceProvidersAsync()
    {
        // We first try for a ServiceProviderCatalog
        var serviceProviderCatalog = await FetchServiceProviderCatalogAsync();

        if (serviceProviderCatalog != null)
        {
            return serviceProviderCatalog.GetServiceProviders();
        }

        // Secondly we try for a ServiceProvider which is acting as a ServiceProvider registry
        ServiceProvider serviceProvider = await GetServiceProviderAsync();

        if (serviceProvider != null)
        {
            Service[] services = serviceProvider.GetServices();

            if (services != null)
            {
                QueryCapability firstQueryCapability = null;
                QueryCapability firstDefaultQueryCapability = null;

                for (int serviceIndex = 0;
                     (serviceIndex < services.Length) &&
                      (firstDefaultQueryCapability == null);
                     serviceIndex++)
                {
                    Service service = services[serviceIndex];

                    QueryCapability[] queryCapabilities = service.GetQueryCapabilities();

                    if (queryCapabilities != null)
                    {
                        for (int queryCapabilityIndex = 0;
                             (queryCapabilityIndex < queryCapabilities.Length) &&
                              (firstDefaultQueryCapability == null);
                             queryCapabilityIndex++)
                        {
                            QueryCapability queryCapability = queryCapabilities[queryCapabilityIndex];

                            Uri[] resourceTypes = queryCapability.GetResourceTypes();

                            if (resourceTypes != null)
                            {
                                for (int resourceTypeIndex = 0;
                                     (resourceTypeIndex < resourceTypes.Length) &&
                                      (firstDefaultQueryCapability == null);
                                     resourceTypeIndex++)
                                {
                                    Uri resourceType = resourceTypes[resourceTypeIndex];

                                    if (OslcConstants.TYPE_SERVICE_PROVIDER.Equals(resourceType.ToString()))
                                    {
                                        firstQueryCapability ??= queryCapability;

                                        Uri[] usages = queryCapability.GetUsages();

                                        for (int usageIndex = 0;
                                             (usageIndex < usages.Length) &&
                                              (firstDefaultQueryCapability == null);
                                             usageIndex++)
                                        {
                                            Uri usage = usages[usageIndex];

                                            if (OslcConstants.OSLC_USAGE_DEFAULT.Equals(usage.ToString()))
                                            {
                                                firstDefaultQueryCapability = queryCapability;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (firstQueryCapability != null)
                {
                    // respect the OslcConstants.OSLC_USAGE_DEFAULT hint if possible
                    var queryCapability = firstDefaultQueryCapability ?? firstQueryCapability;

                    Uri queryBase = queryCapability.GetQueryBase();

                    // Foundation Registry Services requires the query string of oslc.select=* in order to flesh out the ServiceProviders
                    var query = queryBase.ToString() + "?oslc.select=*";

                    var oslcRestClient = new OslcRestClient(_client.GetFormatters(), query);

                    return await oslcRestClient.GetOslcResourcesAsync<ServiceProvider>();
                }
            }
        }

        return null;
    }

}
