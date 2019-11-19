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

    [OslcResourceShape(title = "Architecture Management LinkType Resource Shape", describes = new string[] { ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE })]
    [OslcNamespace(ArchitectureConstants.ARCHITECTURE_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/architecture-management/OSLC-Architecture-Management-Specification-Version-2.0/
    /// </summary>
    public class ArchitectureLinkType : AbstractResource
    {
        private readonly ISet<Uri> _contributors = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _creators = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java

        private DateTime? _created;
        private string _comment;
        private string _label;
        private string _identifier;
        private Uri _instanceShape;
        private DateTime? _modified;
        private Uri _serviceProvider;

        public ArchitectureLinkType() : base()
        {
            this._rdfTypes.Add(new Uri(ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE));
        }

        public ArchitectureLinkType(Uri about) : base(about)
        {
            this._rdfTypes.Add(new Uri(ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE));
        }

        public void AddContributor(Uri contributor)
        {
            this._contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            this._creators.Add(creator);
        }

        public void AddRdfType(Uri rdfType)
        {
            this._rdfTypes.Add(rdfType);
        }

        [OslcDescription("The person(s) who are responsible for the work needed to complete the automation plan.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return this._contributors.ToArray();
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return this._created;
        }

        [OslcDescription("Creator or creators of resource.")]
        [OslcName("creator")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
        [OslcRange(ArchitectureConstants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return this._creators.ToArray();
        }

        [OslcDescription("The human readable name for this link type.")]
        [OslcPropertyDefinition(OslcConstants.RDFS_NAMESPACE + "label")]
        [OslcTitle("Label")]
        [OslcOccurs(Occurs.ExactlyOne)]
        public string GetLabel()
        {
            return this._label;
        }

        [OslcDescription("Descriptive text about link type. ")]
        [OslcPropertyDefinition(OslcConstants.RDFS_NAMESPACE + "comment")]
        [OslcTitle("Comment")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        public string GetComment()
        {
            return this._comment;
        }

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return this._identifier;
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return this._instanceShape;
        }

        [OslcDescription("Timestamp last latest resource modification.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
        [OslcReadOnly]
        [OslcTitle("Modified")]
        public DateTime? GetModified()
        {
            return this._modified;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return this._rdfTypes.ToArray();
        }

        [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return this._serviceProvider;
        }

        public void SetContributors(Uri[] contributors)
        {
            this._contributors.Clear();

            if (contributors != null)
            {
                this._contributors.AddAll(contributors);
            }
        }

        public void SetCreated(DateTime? created)
        {
            this._created = created;
        }

        public void SetCreators(Uri[] creators)
        {
            this._creators.Clear();

            if (creators != null)
            {
                this._creators.AddAll(creators);
            }
        }

        public void SetLabel(string label)
        {
            this._label = label;
        }

        public void SetComment(string comment)
        {
            this._comment = comment;
        }

        public void SetIdentifier(string identifier)
        {
            this._identifier = identifier;
        }

        public void SetInstanceShape(Uri instanceShape)
        {
            this._instanceShape = instanceShape;
        }

        public void SetModified(DateTime? modified)
        {
            this._modified = modified;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            this._rdfTypes.Clear();

            if (rdfTypes != null)
            {
                this._rdfTypes.AddAll(rdfTypes);
            }
        }

        public void SetServiceProvider(Uri serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        protected Uri GetRdfType()
        {
            return new Uri(ArchitectureConstants.TYPE_ARCHITECTURE_LINK_TYPE);
        }
    }
}
