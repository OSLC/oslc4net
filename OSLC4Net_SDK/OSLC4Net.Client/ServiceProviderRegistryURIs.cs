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
using System.Net.NetworkInformation;
using System.Text;

using OSLC4Net.Core.Model;

using log4net;

namespace OSLC4Net.Client
{
    /// <summary>
    /// This class calculates and store a ServiceProvider Registry URI.
    /// </summary>
    public static class ServiceProviderRegistryURIs
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(ServiceProviderRegistryURIs));

        private static readonly string SYSTEM_PROPERTY_NAME_REGISTRY_URI = typeof(ServiceProviderRegistryURIs).Assembly.FullName + ".registryuri";

        private static string SERVICE_PROVIDER_REGISTRY_URI;

        static ServiceProviderRegistryURIs()
        {
            string registryURI = Environment.GetEnvironmentVariable(SYSTEM_PROPERTY_NAME_REGISTRY_URI);

            string defaultBase = null;

            if (registryURI == null)
            {
                // We need at least one default URI

                string hostName = "localhost";

                try
                {
                    hostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
                }
                catch (Exception)
                {
                    // Default to localhost
                }

                defaultBase = "http://" + hostName + ":8080/";
            }

            if (registryURI != null)
            {
                SERVICE_PROVIDER_REGISTRY_URI = registryURI;
            }
            else
            {
                // In order to force Jena to show SPC first in XML, add a bogus identifier to the SPC URI.
                // This is because Jena can show an object anywhere in its graph where it is referenced.  Since the
                // SPC URI (without tailing identifier) is the same as its QueryCapability's queryBase, it can
                // be strangely rendered with the SPC nested under the queryBase.
                // This also allows us to distinguish between array and single results within the ServiceProviderCatalogResource.
                SERVICE_PROVIDER_REGISTRY_URI = defaultBase + "OSLC4JRegistry/catalog/singleton";

                LOGGER.Warn("System property '" + SYSTEM_PROPERTY_NAME_REGISTRY_URI + "' not set.  Using calculated value '" + SERVICE_PROVIDER_REGISTRY_URI + "'");
            }
           
        }

        /// <summary>
        /// Get the ServiceProviderRegistry URI
        /// </summary>
        /// <returns></returns>
        public static string getServiceProviderRegistryURI()
        {
            return SERVICE_PROVIDER_REGISTRY_URI;
        }

    }
}
