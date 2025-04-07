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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

/// <summary>
///     OSLC Service Provider Catalog resource
/// </summary>
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Service Provider Catalog Resource Shape",
    describes = new[] { OslcConstants.TYPE_SERVICE_PROVIDER_CATALOG })]
public class ServiceProviderCatalog : AbstractResource
{
    private readonly SortedSet<Uri> domains = new SortedUriSet();
    private readonly SortedSet<Uri> referencedServiceProviderCatalogs = new SortedUriSet();
    private readonly IList<ServiceProvider> serviceProviders = new List<ServiceProvider>();

    private string description;
    private OAuthConfiguration oauthConfiguration;
    private Publisher publisher;
    private string title;

    public void AddDomain(Uri domain)
    {
        domains.Add(domain);
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
        serviceProviders.Add(serviceProvider);
    }

    [OslcDescription("Description of the service provider catalog")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
    [OslcReadOnly]
    [OslcTitle("Description")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetDescription()
    {
        return description;
    }

    [OslcDescription(
        "URIs of the OSLC domain specifications that may be implemented by referenced services")]
    [OslcName("domain")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "domain")]
    [OslcReadOnly]
    [OslcTitle("Domains")]
    public Uri[] GetDomains()
    {
        return domains.ToArray();
    }

    [OslcDescription(
        "Defines the three OAuth URIs required for a client to act as an OAuth consumer")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "oauthConfiguration")]
    [OslcRange(OslcConstants.TYPE_O_AUTH_CONFIGURATION)]
    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
    [OslcTitle("OAuth URIs")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" +
                    OslcConstants.PATH_OAUTH_CONFIGURATION)]
    [OslcValueType(ValueType.LocalResource)]
    public OAuthConfiguration GetOauthConfiguration()
    {
        return oauthConfiguration;
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
        return publisher;
    }

    [OslcDescription("Additional service provider catalogs")]
    [OslcName("serviceProviderCatalog")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProviderCatalog")]
    [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER_CATALOG)]
    [OslcReadOnly]
    [OslcTitle("Additional Service Provider Catalogs")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" +
                    OslcConstants.PATH_SERVICE_PROVIDER_CATALOG)]
    public Uri[] GetReferencedServiceProviderCatalogs()
    {
        return referencedServiceProviderCatalogs.ToArray();
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
        return serviceProviders.ToArray();
    }

    [OslcDescription("Title of the service provider catalog")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
    [OslcReadOnly]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle()
    {
        return title;
    }

    public void RemoveDomain(Uri domain)
    {
        domains.Remove(domain);
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
        serviceProviders.Remove(serviceProvider);
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }

    public void SetDomains(Uri[] domains)
    {
        this.domains.Clear();
        if (domains != null)
        {
            this.domains.AddAll(domains);
        }
    }

    public void SetOauthConfiguration(OAuthConfiguration oauthConfiguration)
    {
        this.oauthConfiguration = oauthConfiguration;
    }

    public void SetPublisher(Publisher publisher)
    {
        this.publisher = publisher;
    }

    public void SetReferencedServiceProviderCatalogs(Uri[] referencedServiceProviderCatalogs)
    {
        this.referencedServiceProviderCatalogs.Clear();
        if (referencedServiceProviderCatalogs != null)
        {
            this.referencedServiceProviderCatalogs.AddAll(referencedServiceProviderCatalogs);
        }
    }

    public void SetServiceProviders(ServiceProvider[] serviceProviders)
    {
        this.serviceProviders.Clear();
        if (serviceProviders != null)
        {
            this.serviceProviders.AddAll(serviceProviders);
        }
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }
}
