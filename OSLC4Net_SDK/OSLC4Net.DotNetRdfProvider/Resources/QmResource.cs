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

namespace OSLC4Net.Core.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OSLC4Net.Core.Attribute;
    using OSLC4Net.Core.Model;

    /// <summary>
    /// http://open-services.net/bin/view/Main/QmSpecificationV2
    /// </summary>
    public abstract class QmResource : AbstractResource
    {
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java

        private DateTime? _created;
        private string _identifier;
        private Uri _instanceShape;
        private DateTime? _modified;
        private Uri _serviceProvider;
        private string _title;

        public QmResource() : base()
        {
            _rdfTypes.Add(GetRdfType());
        }

        public QmResource(Uri about) : base(about)
        {
            _rdfTypes.Add(GetRdfType());
        }

        public void AddRdfType(Uri rdfType)
        {
            _rdfTypes.Add(rdfType);
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return _created;
        }

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return _identifier;
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return _instanceShape;
        }

        [OslcDescription("Timestamp last latest resource modification.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
        [OslcReadOnly]
        [OslcTitle("Modified")]
        public DateTime? GetModified()
        {
            return _modified;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return _rdfTypes.ToArray();
        }

        [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return _serviceProvider;
        }

        [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcTitle("Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return _title;
        }

        public void SetCreated(DateTime? created)
        {
            _created = created;
        }

        public void SetIdentifier(string identifier)
        {
            _identifier = identifier;
        }

        public void SetInstanceShape(Uri instanceShape)
        {
            _instanceShape = instanceShape;
        }

        public void SetModified(DateTime? modified)
        {
            _modified = modified;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            _rdfTypes.Clear();

            if (rdfTypes != null)
            {
                _rdfTypes.AddAll(rdfTypes);
            }
        }

        public void SetServiceProvider(Uri serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        protected abstract Uri GetRdfType();
    }
}
