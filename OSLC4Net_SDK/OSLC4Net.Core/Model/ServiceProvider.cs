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
    /// OSLC ServiceProvider resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(
        title = "OSLC Service Provider Resource Shape",
        describes = new string[] { OslcConstants.TYPE_SERVICE_PROVIDER })]
    public class ServiceProvider : AbstractResource
    {
        private readonly SortedSet<Uri> _details = new SortedUriSet();

        private readonly IList<PrefixDefinition> _prefixDefinitions = new List<PrefixDefinition>();

        private readonly IList<Service> _services = new List<Service>();

        private DateTime? _created; // TODO - ServiceProvider.created nice to have, but not required.

        private string _description;

        private string _identifier; // TODO - ServiceProvider.identifier nice to have, but not required.

        private OAuthConfiguration _oauthConfiguration;

        private Publisher _publisher;

        private string _title;

        public ServiceProvider()
            : base()
        {
        }

        public void AddService(Service srvc)
        {
            _services.Add(srvc);
        }

        [OslcDescription("The date and time that this resource was created")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return _created;
        }

        [OslcDescription("Description of the service provider")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcReadOnly]
        [OslcTitle("Description")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return _description;
        }

        [OslcDescription(
            "URLs that may be used to retrieve web pages to determine additional details about the service provider")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "details")]
        [OslcReadOnly]
        [OslcTitle("Details")]
        public Uri[] GetDetails()
        {
            return _details.ToArray();
        }

        [OslcDescription("A unique identifier for this resource")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return _identifier;
        }

        [OslcDescription("Defines the three OAuth URIs required for a client to act as an OAuth consumer")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "oauthConfiguration")]
        [OslcRange(OslcConstants.TYPE_O_AUTH_CONFIGURATION)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("OAuth Configuration")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_OAUTH_CONFIGURATION)]
        [OslcValueType(ValueType.LocalResource)]
        public OAuthConfiguration GetOauthConfiguration()
        {
            return _oauthConfiguration;
        }

        [OslcDescription(
            "Defines namespace prefixes for use in JSON representations and in forming OSLC Query Syntax strings")]
        [OslcName("prefixDefinition")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "prefixDefinition")]
        [OslcRange(OslcConstants.TYPE_PREFIX_DEFINITION)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Prefix Definitions")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PREFIX_DEFINITION)]
        [OslcValueType(ValueType.LocalResource)]
        public PrefixDefinition[] GetPrefixDefinitions()
        {
            return _prefixDefinitions.ToArray();
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
        public Service[] GetServices()
        {
            return _services.ToArray();
        }

        [OslcDescription("Title of the service provider")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcReadOnly]
        [OslcTitle("Title")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return _title;
        }

        public void SetCreated(DateTime? created)
        {
            _created = created;
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void SetDetails(Uri[] details)
        {
            _details.Clear();
            if (details != null)
            {
                _details.AddAll(details);
            }
        }

        public void SetIdentifier(string identifier)
        {
            _identifier = identifier;
        }

        public void SetOauthConfiguration(OAuthConfiguration oauthConfiguration)
        {
            _oauthConfiguration = oauthConfiguration;
        }

        public void SetPrefixDefinitions(PrefixDefinition[] prefixDefinitions)
        {
            _prefixDefinitions.Clear();
            if (prefixDefinitions != null)
            {
                _prefixDefinitions.AddAll(prefixDefinitions);
            }
        }

        public void SetPublisher(Publisher publisher)
        {
            _publisher = publisher;
        }

        public void SetServices(Service[] services)
        {
            _services.Clear();
            if (services != null)
            {
                _services.AddAll(services);
            }
        }

        public void SetTitle(string title)
        {
            _title = title;
        }
    }
}