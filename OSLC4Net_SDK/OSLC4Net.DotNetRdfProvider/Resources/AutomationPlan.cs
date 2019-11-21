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

    [OslcResourceShape(title = "Automation Plan Resource Shape", describes = new string[] { AutomationConstants.TYPE_AUTOMATION_PLAN })]
    [OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_AutomationPlan
    /// </summary>
    public class AutomationPlan : AbstractResource
    {
        private readonly ISet<Uri> _contributors = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _creators = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<string> _subjects = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Property> _parameterDefinitions = new HashSet<Property>(); // XXX - TreeSet<> in Java

        private DateTime? _created;
        private string _description;
        private string _identifier;
        private Uri _instanceShape;
        private DateTime? _modified;
        private Uri _serviceProvider;
        private string _title;

        public AutomationPlan() : base()
        {
            this._rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_PLAN));
        }

        public AutomationPlan(Uri about) : base(about)
        {
            this._rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_PLAN));
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

        public void AddSubject(string subject)
        {
            this._subjects.Add(subject);
        }

        public void AddParameterDefinition(Property parameter)
        {
            this._parameterDefinitions.Add(parameter);
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
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return this._creators.ToArray();
        }

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return this._description;
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
        [OslcTitle("Mo]dified")]
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

        [OslcDescription("Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")]
        [OslcName("subject")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "subject")]
        [OslcReadOnly(false)]
        [OslcTitle("Subjects")]
        public string[] GetSubjects()
        {
            return this._subjects.ToArray();
        }

        [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcTitle("Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return this._title;
        }

        [OslcDescription("The parameter definitions for the automation plan.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("parameterDefinition")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "parameterDefinition")]
        [OslcTitle("Parameter Definitions")]
        public Property[] GetParameterDefinitions()
        {
            return this._parameterDefinitions.ToArray();
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

        public void SetDescription(string description)
        {
            this._description = description;
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

        public void SetSubjects(string[] subjects)
        {
            this._subjects.Clear();

            if (subjects != null)
            {
                this._subjects.AddAll(subjects);
            }
        }

        public void SetTitle(string title)
        {
            this._title = title;
        }

        public void SetParameterDefinitions(Property[] parameterDefinitions)
        {
            this._parameterDefinitions.Clear();

            if (parameterDefinitions != null)
            {
                this._parameterDefinitions.AddAll(parameterDefinitions);
            }
        }

        protected System.Uri GetRdfType()
        {
            return new Uri(AutomationConstants.TYPE_AUTOMATION_PLAN);
        }
    }
}
