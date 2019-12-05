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

using System;
using System.Collections.Generic;
using System.Linq;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Oslc.Resources
{
    [OslcNamespace(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE)]
    [OslcResourceShape(title = "Requirement Resource Shape", describes = new string[] { RmConstants.TYPE_REQUIREMENT })]
    public class Requirement : AbstractResource
    {
        private string _title;
        private string _description;
        private string _identifier;
        private string _shortTitle;
        private readonly ISet<string> _subjects = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _creators = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _contributors = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private DateTime? _created;
        private DateTime? _modified;
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private Uri _serviceProvider;
        private Uri _instanceShape;

        // OSLC Links
        private readonly ISet<Link> elaboratedBy = new HashSet<Link>();
        private readonly ISet<Link> elaborates = new HashSet<Link>();

        private readonly ISet<Link> specifiedBy = new HashSet<Link>();
        private readonly ISet<Link> specifies = new HashSet<Link>();

        private readonly ISet<Link> affectedBy = new HashSet<Link>();

        private readonly ISet<Link> trackedBy = new HashSet<Link>();

        private readonly ISet<Link> implementedBy = new HashSet<Link>();

        private readonly ISet<Link> validatedBy = new HashSet<Link>();

        private readonly ISet<Link> satisfiedBy = new HashSet<Link>();
        private readonly ISet<Link> satisfies = new HashSet<Link>();

        private readonly ISet<Link> decomposedBy = new HashSet<Link>();
        private readonly ISet<Link> decomposes = new HashSet<Link>();

        private readonly ISet<Link> constrainedBy = new HashSet<Link>();
        private readonly ISet<Link> constrains = new HashSet<Link>();

        public Requirement() : base()
        {
            // Only Add the type if Requirement is the created object
            if (!(this is RequirementCollection))
            {
                _rdfTypes.Add(new Uri(RmConstants.TYPE_REQUIREMENT));
            }
        }

        public Requirement(Uri about) : base(about)
        {
            // Only Add the type if Requirement is the created object
            if (!(this is RequirementCollection))
            {
                _rdfTypes.Add(new Uri(RmConstants.TYPE_REQUIREMENT));
            }
        }

        public void AddSubject(string subject)
        {
            _subjects.Add(subject);
        }

        public void AddConstrains(Link constrains)
        {
            this.constrains.Add(constrains);
        }

        public void AddConstrainedBy(Link constrainedBy)
        {
            this.constrainedBy.Add(constrainedBy);
        }

        public void AddDecomposes(Link decomposes)
        {
            this.decomposes.Add(decomposes);
        }

        public void AddDecomposedBy(Link decomposedBy)
        {
            this.decomposedBy.Add(decomposedBy);
        }

        public void AddSatisfies(Link satisfies)
        {
            this.satisfies.Add(satisfies);
        }

        public void AddSatisfiedBy(Link satisfiedBy)
        {
            this.satisfiedBy.Add(satisfiedBy);
        }

        public void AddValidatedBy(Link validatedBy)
        {
            this.validatedBy.Add(validatedBy);
        }

        public void AddTrackedBy(Link trackedBy)
        {
            this.trackedBy.Add(trackedBy);
        }

        public void AddImplementedBy(Link implementedBy)
        {
            this.implementedBy.Add(implementedBy);
        }

        public void AddAffectedBy(Link affectedBy)
        {
            this.affectedBy.Add(affectedBy);
        }

        public void AddElaboratedBy(Link elaboratedBy)
        {
            this.elaboratedBy.Add(elaboratedBy);
        }

        public void AddElaborates(Link elaborates)
        {
            this.elaborates.Add(elaborates);
        }

        public void AddSpecifiedBy(Link specifiedBy)
        {
            this.specifiedBy.Add(specifiedBy);
        }

        public void AddSpecifies(Link specifies)
        {
            this.specifies.Add(specifies);
        }

        public void AddContributor(Uri contributor)
        {
            _contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            _creators.Add(creator);
        }

        public void AddRdfType(Uri rdfType)
        {
            _rdfTypes.Add(rdfType);
        }

        [OslcDescription("Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")]
        [OslcName("subject")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "subject")]
        [OslcReadOnly(false)]
        [OslcTitle("Subjects")]
        public string[] GetSubjects()
        {
            return _subjects.ToArray();
        }

        [OslcDescription("The subject is elaborated by the object.")]
        [OslcName("elaboratedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "elaboratedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Elaborated By")]
        public Link[] GetElaboratedBy()
        {
            return elaboratedBy.ToArray();
        }

        [OslcDescription("The object is elaborated by the subject.")]
        [OslcName("elaborates")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "elaborates")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Elaborates")]
        public Link[] GetElaborates()
        {
            return elaborates.ToArray();
        }

        [OslcDescription("The subject is specified by the object.")]
        [OslcName("specifiedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "specifiedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Specified By")]
        public Link[] GetSpecifiedBy()
        {
            return specifiedBy.ToArray();
        }

        [OslcDescription("The object is specified by the subject.")]
        [OslcName("specifies")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "specifies")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Specifies")]
        public Link[] GetSpecifies()
        {
            return specifies.ToArray();
        }

        [OslcDescription("Resource, such as a change request, which implements this requirement.")]
        [OslcName("implementedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "implementedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Implemented By")]
        public Link[] GetImplementedBy()
        {
            return implementedBy.ToArray();
        }

        [OslcDescription("Requirement is affected by a resource, such as a defect or issue.")]
        [OslcName("affectedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "affectedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Affected By")]
        public Link[] GetAffectedBy()
        {
            return affectedBy.ToArray();
        }

        [OslcDescription("Resource, such as a change request, which tracks this requirement.")]
        [OslcName("trackedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "trackedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("tracked By")]
        public Link[] GetTrackedBy()
        {
            return trackedBy.ToArray();
        }

        [OslcDescription("Resource, such as a test case, which validates this requirement.")]
        [OslcName("validatedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "validatedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Validated By")]
        public Link[] GetValidatedBy()
        {
            return validatedBy.ToArray();
        }

        [OslcDescription("The subject is satisfied by the object.")]
        [OslcName("satisfiedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "satisfiedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Satisfied By")]
        public Link[] GetSatisfiedBy()
        {
            return satisfiedBy.ToArray();
        }

        [OslcDescription("The object is satisfied by the subject.")]
        [OslcName("satisfies")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "satisfies")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Satisfies")]
        public Link[] GetSatisfies()
        {
            return satisfies.ToArray();
        }

        [OslcDescription("The subject is decomposed by the object.")]
        [OslcName("decomposedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "decomposedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("DecomposedBy")]
        public Link[] GetDecomposedBy()
        {
            return decomposedBy.ToArray();
        }

        [OslcDescription("The object is decomposed by the subject.")]
        [OslcName("decomposes")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "decomposes")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Decomposes")]
        public Link[] GetDecomposes()
        {
            return decomposes.ToArray();
        }

        [OslcDescription("The subject is constrained by the object.")]
        [OslcName("constrainedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "constrainedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("ConstrainedBy")]
        public Link[] GetConstrainedBy()
        {
            return constrainedBy.ToArray();
        }

        [OslcDescription("The object is constrained by the subject.")]
        [OslcName("constrains")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "constrains")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Constrains")]
        public Link[] GetConstrains()
        {
            return constrains.ToArray();
        }

        [OslcDescription("The person(s) who are responsible for the work needed to complete the change request.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(RmConstants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return _contributors.ToArray();
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return _created;
        }

        [OslcDescription("Creator or creators of resource.")]
        [OslcName("creator")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
        [OslcRange(RmConstants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return _creators.ToArray();
        }

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return _description;
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

        [OslcDescription("Short name identifying a resource, often used as an abbreviated identifier for presentation to end-users.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "shortTitle")]
        [OslcTitle("Short Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetShortTitle()
        {
            return _shortTitle;
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

        public void SetConstrains(Link[] constrains)
        {
            this.constrains.Clear();

            if (constrains != null)
            {
                this.constrains.AddAll(constrains);
            }
        }

        public void SetConstrainedBy(Link[] constrainedBy)
        {
            this.constrainedBy.Clear();

            if (constrainedBy != null)
            {
                this.constrainedBy.AddAll(constrainedBy);
            }
        }

        public void SetDecomposes(Link[] decomposes)
        {
            affectedBy.Clear();

            if (decomposes != null)
            {
                this.decomposes.AddAll(decomposes);
            }
        }

        public void SetDecomposedBy(Link[] decomposedBy)
        {
            this.decomposedBy.Clear();

            if (decomposedBy != null)
            {
                this.decomposedBy.AddAll(decomposedBy);
            }
        }

        public void SetSatisfies(Link[] satisfies)
        {
            this.satisfies.Clear();

            if (satisfies != null)
            {
                this.satisfies.AddAll(satisfies);
            }
        }

        public void SetSatisfiedBy(Link[] satisfiedBy)
        {
            this.satisfiedBy.Clear();

            if (satisfiedBy != null)
            {
                this.satisfiedBy.AddAll(satisfiedBy);
            }
        }

        public void SetValidatedBy(Link[] validatedBy)
        {
            this.validatedBy.Clear();

            if (validatedBy != null)
            {
                this.validatedBy.AddAll(validatedBy);
            }
        }

        public void SetTrackedBy(Link[] trackedBy)
        {
            this.trackedBy.Clear();

            if (trackedBy != null)
            {
                this.trackedBy.AddAll(trackedBy);
            }
        }

        public void SetAffectedBy(Link[] affectedBy)
        {
            this.affectedBy.Clear();

            if (affectedBy != null)
            {
                this.affectedBy.AddAll(affectedBy);
            }
        }

        public void SetImplementedBy(Link[] implementedBy)
        {
            this.implementedBy.Clear();

            if (implementedBy != null)
            {
                this.implementedBy.AddAll(implementedBy);
            }
        }

        public void SetElaboratedBy(Link[] elaboratedBy)
        {
            this.elaboratedBy.Clear();

            if (elaboratedBy != null)
            {
                this.elaboratedBy.AddAll(elaboratedBy);
            }
        }

        public void SetElaborates(Link[] elaborates)
        {
            this.elaborates.Clear();

            if (elaborates != null)
            {
                this.elaborates.AddAll(elaborates);
            }
        }

        public void SetSpecifiedBy(Link[] specifiedBy)
        {
            this.specifiedBy.Clear();

            if (specifiedBy != null)
            {
                this.specifiedBy.AddAll(specifiedBy);
            }
        }

        public void SetSpecifies(Link[] specifies)
        {
            this.specifies.Clear();

            if (specifies != null)
            {
                this.specifies.AddAll(specifies);
            }
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

        public void SetShortTitle(string shortTitle)
        {
            this._shortTitle = shortTitle;
        }

        public void SetTitle(string title)
        {
            this._title = title;
        }

        public void SetSubjects(string[] subjects)
        {
            this._subjects.Clear();

            if (subjects != null)
            {
                this._subjects.AddAll(subjects);
            }
        }
    }
}