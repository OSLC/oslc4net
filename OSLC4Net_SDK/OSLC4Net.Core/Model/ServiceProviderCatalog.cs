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

namespace OSLC4Net.Core.Model
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OSLC4Net.Core.Attribute;

    #endregion

    /// <summary>
    /// OSLC Service Provider Catalog resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(
        title = "OSLC Service Provider Catalog Resource Shape",
        describes = new string[] { OslcConstants.TYPE_SERVICE_PROVIDER_CATALOG })]
    public class ServiceProviderCatalog : AbstractResource
    {
        private readonly SortedSet<Uri> _domains = new SortedUriSet();

        private readonly SortedSet<Uri> _referencedServiceProviderCatalogs = new SortedUriSet();

        private readonly IList<ServiceProvider> _serviceProviders = new List<ServiceProvider>();

        private string _description;

        private OAuthConfiguration _oauthConfiguration;

        private Publisher _publisher;

        private string _title;

        public ServiceProviderCatalog()
            : base()
        {
        }

        public void AddDomain(Uri domain)
        {
            _domains.Add(domain);
        }

        public void AddDomains(ICollection<Uri> domains)
        {
            foreach (var domain in domains)
            {
                AddDomain(domain);
            }
        }

        public void AddServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProviders.Add(serviceProvider);
        }

        [OslcDescription("Description of the service provider catalog")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcReadOnly]
        [OslcTitle("Description")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return _description;
        }

        [OslcDescription("URIs of the OSLC domain specifications that may be implemented by referenced services")]
        [OslcName("domain")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "domain")]
        [OslcReadOnly]
        [OslcTitle("Domains")]
        public Uri[] GetDomains()
        {
            return _domains.ToArray();
        }

        [OslcDescription("Defines the three OAuth URIs required for a client to act as an OAuth consumer")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "oauthConfiguration")]
        [OslcRange(OslcConstants.TYPE_O_AUTH_CONFIGURATION)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("OAuth URIs")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_OAUTH_CONFIGURATION)]
        [OslcValueType(ValueType.LocalResource)]
        public OAuthConfiguration GetOauthConfiguration()
        {
            return _oauthConfiguration;
        }

        [OslcDescription("Describes the software product that provides the implementation")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "publisher")]
        [OslcRange(OslcConstants.TYPE_PUBLISHER)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Publisher")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PUBLISHER)]
        [OslcValueType(ValueType.LocalResource)]
        public Publisher GetPublisher()
        {
            return _publisher;
        }

        [OslcDescription("Additional service provider catalogs")]
        [OslcName("serviceProviderCatalog")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProviderCatalog")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER_CATALOG)]
        [OslcReadOnly]
        [OslcTitle("Additional Service Provider Catalogs")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_SERVICE_PROVIDER_CATALOG)]
        public Uri[] GetReferencedServiceProviderCatalogs()
        {
            return _referencedServiceProviderCatalogs.ToArray();
        }

        [OslcDescription("Service providers")]
        [OslcName("serviceProvider")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Service Providers")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_SERVICE_PROVIDER)]
        [OslcValueType(ValueType.LocalResource)]
        public ServiceProvider[] GetServiceProviders()
        {
            return _serviceProviders.ToArray();
        }

        [OslcDescription("Title of the service provider catalog")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcReadOnly]
        [OslcTitle("Title")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return _title;
        }

        public void RemoveDomain(Uri domain)
        {
            _domains.Remove(domain);
        }

        public void RemoveDomains(ICollection<Uri> domains)
        {
            foreach (var domain in domains)
            {
                RemoveDomain(domain);
            }
        }

        public void RemoveServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProviders.Remove(serviceProvider);
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void SetDomains(Uri[] domains)
        {
            _domains.Clear();
            if (domains != null)
            {
                _domains.AddAll(domains);
            }
        }

        public void SetOauthConfiguration(OAuthConfiguration oauthConfiguration)
        {
            _oauthConfiguration = oauthConfiguration;
        }

        public void SetPublisher(Publisher publisher)
        {
            _publisher = publisher;
        }

        public void SetReferencedServiceProviderCatalogs(Uri[] referencedServiceProviderCatalogs)
        {
            _referencedServiceProviderCatalogs.Clear();
            if (referencedServiceProviderCatalogs != null)
            {
                _referencedServiceProviderCatalogs.AddAll(referencedServiceProviderCatalogs);
            }
        }

        public void SetServiceProviders(ServiceProvider[] serviceProviders)
        {
            _serviceProviders.Clear();
            if (serviceProviders != null)
            {
                _serviceProviders.AddAll(serviceProviders);
            }
        }

        public void SetTitle(string title)
        {
            _title = title;
        }
    }
}