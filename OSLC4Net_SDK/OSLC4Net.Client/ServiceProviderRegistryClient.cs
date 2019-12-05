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

using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client
{
    

    /// <summary>
    /// This classs provides methods to register and deregister with an OSLC ServiceProvider Registry (not yet
    /// implemented in OSLC4Net - see Eclipse Lyo for a Java implementation)
    /// 
    /// It also provides methods to get a ServiceProviderCatalog and retrieve the ServiceProviders 
    /// </summary>
  
   
    public sealed class ServiceProviderRegistryClient
    {
        private OslcRestClient client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="mediaType"></param>
        /// <param name="uri"></param>
        public ServiceProviderRegistryClient(ISet<MediaTypeFormatter>   formatters,
                                             string mediaType,
                                             string uri) 
        {
    	    client = new OslcRestClient(formatters,
    									     uri,
    									     mediaType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        /// <param name="mediaType"></param>
        public ServiceProviderRegistryClient(ISet<MediaTypeFormatter>   formatters,
                                             string mediaType) :
            this(formatters, mediaType, ServiceProviderRegistryURIs.getServiceProviderRegistryURI())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatters"></param>
        public ServiceProviderRegistryClient(ISet<MediaTypeFormatter> formatters) :
            this (formatters, OslcMediaType.APPLICATION_RDF_XML)
        {
        }

        /// <summary>
        /// Register a ServiceProvider
        /// </summary>
        /// <param name="serviceProviderToRegister"></param>
        /// <returns></returns>
        public Uri registerServiceProvider(ServiceProvider serviceProviderToRegister)
        {
            Uri typeServiceProviderURI = new Uri(OslcConstants.TYPE_SERVICE_PROVIDER);
            Uri oslcUsageDefault       = new Uri(OslcConstants.OSLC_USAGE_DEFAULT);

            ServiceProvider[] serviceProviders;

            // We have to first get the ServiceProvider for ServiceProviders and then find the CreationFactory for a ServiceProvider

            // We first try for a ServiceProviderCatalog
            ServiceProviderCatalog serviceProviderCatalog = getServiceProviderCatalog();

            if (serviceProviderCatalog != null)
            {
                serviceProviders = serviceProviderCatalog.GetServiceProviders();
            }
            else
            {
                // Secondly we try for a ServiceProvider which is acting as a ServiceProvider registry
                ServiceProvider serviceProvider = GetServiceProvider();

                if (serviceProvider != null)
                {
                    serviceProviders = new ServiceProvider[] {serviceProvider};
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
                CreationFactory firstCreationFactory        = null;
                CreationFactory firstDefaultCreationFactory = null;

                for (int serviceProviderIndex = 0;
                     ((serviceProviderIndex < serviceProviders.Length) &&
                      (firstDefaultCreationFactory == null));
                     serviceProviderIndex++)
                {
                    ServiceProvider serviceProvider = serviceProviders[serviceProviderIndex];

                    Service[] services = serviceProvider.GetServices();

                    if (services != null)
                    {
                        for (int serviceIndex = 0;
                             ((serviceIndex < services.Length) &&
                              (firstDefaultCreationFactory == null));
                             serviceIndex++)
                        {
                            Service service = services[serviceIndex];

                            CreationFactory[] creationFactories = service.GetCreationFactories();

                            if (creationFactories != null)
                            {
                                for (int creationFactoryIndex = 0;
                                     ((creationFactoryIndex < creationFactories.Length) &&
                                      (firstDefaultCreationFactory == null));
                                     creationFactoryIndex++)
                                {
                                    CreationFactory creationFactory = creationFactories[creationFactoryIndex];

                                    Uri[] resourceTypes = creationFactory.GetResourceTypes();

                                    if (resourceTypes != null)
                                    {
                                        for (int resourceTypeIndex = 0;
                                             ((resourceTypeIndex < resourceTypes.Length) &&
                                              (firstDefaultCreationFactory == null));
                                             resourceTypeIndex++)
                                        {
                                            Uri resourceType = resourceTypes[resourceTypeIndex];

                                            if (typeServiceProviderURI.Equals(resourceType))
                                            {
                                                if (firstCreationFactory == null)
                                                {
                                                    firstCreationFactory = creationFactory;
                                                }

                                                Uri[] usages = creationFactory.GetUsages();

                                                for (int usageIndex = 0;
                                                     ((usageIndex < usages.Length) &&
                                                      (firstDefaultCreationFactory == null));
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

                    OslcRestClient oslcRestClient = new OslcRestClient(client.GetFormatters(),
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
            HttpResponseMessage clientResponse = new OslcRestClient(client.GetFormatters(), serviceProviderURI).RemoveOslcResourceReturnClientResponse();

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
        public ServiceProviderCatalog getServiceProviderCatalog()
        {
            return client.GetOslcResource<ServiceProviderCatalog>();
        }



        /// <summary>
        /// If aServiceProvider is being used as a ServiceProvider registry without an owning ServiceProviderCatalog,
        /// this will return the ServiceProvider.
        /// Otherwise null will be returned.
        /// </summary>
        /// <returns></returns>
        public ServiceProvider GetServiceProvider()
        {
            return client.GetOslcResource<ServiceProvider>();
        }

        /// <summary>
        /// Return the registered ServiceProvider's.
        /// </summary>
        /// <returns></returns>
        public ServiceProvider[] GetServiceProviders()
        {
            // We first try for a ServiceProviderCatalog
            ServiceProviderCatalog serviceProviderCatalog = getServiceProviderCatalog();

            if (serviceProviderCatalog != null)
            {
                return serviceProviderCatalog.GetServiceProviders();
            }

            // Secondly we try for a ServiceProvider which is acting as a ServiceProvider registry
            ServiceProvider serviceProvider = GetServiceProvider();

            if (serviceProvider != null)
            {
                Service[] services = serviceProvider.GetServices();

                if (services != null)
                {
                    QueryCapability firstQueryCapability        = null;
                    QueryCapability firstDefaultQueryCapability = null;

                    for (int serviceIndex = 0;
                         ((serviceIndex < services.Length) &&
                          (firstDefaultQueryCapability == null));
                         serviceIndex++)
                    {
                        Service service = services[serviceIndex];

                        QueryCapability[] queryCapabilities = service.GetQueryCapabilities();

                        if (queryCapabilities != null)
                        {
                            for (int queryCapabilityIndex = 0;
                                 ((queryCapabilityIndex < queryCapabilities.Length) &&
                                  (firstDefaultQueryCapability == null));
                                 queryCapabilityIndex++)
                            {
                                QueryCapability queryCapability = queryCapabilities[queryCapabilityIndex];

                                Uri[] resourceTypes = queryCapability.GetResourceTypes();

                                if (resourceTypes != null)
                                {
                                    for (int resourceTypeIndex = 0;
                                         ((resourceTypeIndex < resourceTypes.Length) &&
                                          (firstDefaultQueryCapability == null));
                                         resourceTypeIndex++)
                                    {
                                        Uri resourceType = resourceTypes[resourceTypeIndex];

                                        if (OslcConstants.TYPE_SERVICE_PROVIDER.Equals(resourceType.ToString()))
                                        {
                                            if (firstQueryCapability == null)
                                            {
                                                firstQueryCapability = queryCapability;
                                            }

                                            Uri[] usages = queryCapability.GetUsages();

                                            for (int usageIndex = 0;
                                                 ((usageIndex < usages.Length) &&
                                                  (firstDefaultQueryCapability == null));
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
                        QueryCapability queryCapability = firstDefaultQueryCapability != null ? firstDefaultQueryCapability : firstQueryCapability;

                        Uri queryBase = queryCapability.GetQueryBase();

                        // Foundation Registry Services requires the query string of oslc.select=* in order to flesh out the ServiceProviders
                        string query = queryBase.ToString() + "?oslc.select=*";

                        OslcRestClient oslcRestClient = new OslcRestClient(client.GetFormatters(),
                                                                                 query);

                        return oslcRestClient.GetOslcResources<ServiceProvider>();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the OslcClient associated with this SerivceProviderRegistryClient
        /// </summary>
        /// <returns></returns>
	    public OslcRestClient getClient() {
		    return client;
	    }
    }
}
