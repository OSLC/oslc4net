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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

/// <summary>
/// OSLC ServiceProvider resource
/// </summary>
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Service Provider Resource Shape", describes = new string[] { OslcConstants.TYPE_SERVICE_PROVIDER })]
public class ServiceProvider : AbstractResource
{
    private readonly SortedSet<Uri> details = new SortedUriSet();
    private readonly IList<PrefixDefinition> prefixDefinitions = new List<PrefixDefinition>();
    private readonly IList<Service> services = new List<Service>();

    private DateTime?   created; // TODO - ServiceProvider.created nice to have, but not required.
    private string description;
    private string identifier; // TODO - ServiceProvider.identifier nice to have, but not required.
	    private OAuthConfiguration oauthConfiguration;
	    private Publisher publisher;
	    private string title;

	    public ServiceProvider() : base()
    {
	    }

	    public void AddService(Service srvc)
    {
		    this.services.Add(srvc);
	    }

	    [OslcDescription("The date and time that this resource was created")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
    [OslcReadOnly]
    [OslcTitle("Created")]
    public DateTime? GetCreated() {
        return created;
    }

	    [OslcDescription("Description of the service provider")]
	    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
	    [OslcReadOnly]
    [OslcTitle("Description")]
	    [OslcValueType(ValueType.XMLLiteral)]
    public string GetDescription() {
		    return description;
	    }

	    [OslcDescription("URLs that may be used to retrieve web pages to determine additional details about the service provider")]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "details")]
	    [OslcReadOnly]
    [OslcTitle("Details")]
    public Uri[] GetDetails() {
	        return details.ToArray();
	    }

	    [OslcDescription("A unique identifier for this resource")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
    [OslcReadOnly]
    [OslcTitle("Identifier")]
    public string GetIdentifier() {
        return identifier;
    }

	    [OslcDescription("Defines the three OAuth URIs required for a client to act as an OAuth consumer")]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "oauthConfiguration")]
	    [OslcRange(OslcConstants.TYPE_O_AUTH_CONFIGURATION)]
	    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
	    [OslcTitle("OAuth Configuration")]
	    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_OAUTH_CONFIGURATION)]
    [OslcValueType(ValueType.LocalResource)]
    public OAuthConfiguration GetOauthConfiguration() {
		    return oauthConfiguration;
	    }

	    [OslcDescription("Defines namespace prefixes for use in JSON representations and in forming OSLC Query Syntax strings")]
	    [OslcName("prefixDefinition")]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "prefixDefinition")]
	    [OslcRange(OslcConstants.TYPE_PREFIX_DEFINITION)]
	    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
	    [OslcTitle("Prefix Definitions")]
	    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PREFIX_DEFINITION)]
    [OslcValueType(ValueType.LocalResource)]
    public PrefixDefinition[] GetPrefixDefinitions() {
		    return prefixDefinitions.ToArray();
	    }

	    [OslcDescription("Describes the software product that provides the implementation")]
	    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "publisher")]
	    [OslcRange(OslcConstants.TYPE_PUBLISHER)]
	    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
	    [OslcTitle("Publisher")]
	    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PUBLISHER)]
    [OslcValueType(ValueType.LocalResource)]
    public Publisher GetPublisher() {
		    return publisher;
	    }

	    [OslcDescription("Describes services offered by the service provider")]
	    [OslcName("service")]
	    [OslcOccurs(Occurs.OneOrMany)]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "service")]
	    [OslcRange(OslcConstants.TYPE_SERVICE)]
	    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
	    [OslcTitle("Services")]
	    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_SERVICE)]
    [OslcValueType(ValueType.LocalResource)]
	    public Service[] GetServices() {
		    return services.ToArray();
	    }

	    [OslcDescription("Title of the service provider")]
	    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
	    [OslcReadOnly]
    [OslcTitle("Title")]
	    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle() {
		    return title;
	    }

	    public void SetCreated(DateTime? created) {
        this.created = created;
    }

    public void SetDescription(string description) {
		    this.description = description;
	    }

	    public void SetDetails(Uri[] details) {
	        this.details.Clear();
	        if (details != null) {
            this.details.AddAll(details);
        }
	    }

	    public void SetIdentifier(string identifier) {
        this.identifier = identifier;
    }

	    public void SetOauthConfiguration(OAuthConfiguration oauthConfiguration) {
		    this.oauthConfiguration = oauthConfiguration;
	    }

	    public void SetPrefixDefinitions(PrefixDefinition[] prefixDefinitions) {
		    this.prefixDefinitions.Clear();
		    if (prefixDefinitions != null) {
            this.prefixDefinitions.AddAll(prefixDefinitions);
        }
	    }

	    public void SetPublisher(Publisher publisher) {
		    this.publisher = publisher;
	    }

	    public void SetServices(Service[] services) {
		    this.services.Clear();
		    if (services != null) {
            this.services.AddAll(services);
        }
	    }

	    public void SetTitle(string title) {
		    this.title = title;
	    }
}
