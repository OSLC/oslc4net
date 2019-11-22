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

    [OslcNamespace(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE)]
    [OslcResourceShape(title = "Requirement Resource Shape", describes = new string[] { RmConstants.TYPE_REQUIREMENT })]
    public class Requirement : AbstractResource
    {
        private readonly ISet<string> _subjects = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _creators = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _contributors = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java

        // OSLC Links
        private readonly ISet<Link> _elaboratedBy = new HashSet<Link>();

        private readonly ISet<Link> _elaborates = new HashSet<Link>();

        private readonly ISet<Link> _specifiedBy = new HashSet<Link>();
        private readonly ISet<Link> _specifies = new HashSet<Link>();

        private readonly ISet<Link> _affectedBy = new HashSet<Link>();

        private readonly ISet<Link> _trackedBy = new HashSet<Link>();

        private readonly ISet<Link> _implementedBy = new HashSet<Link>();

        private readonly ISet<Link> _validatedBy = new HashSet<Link>();

        private readonly ISet<Link> _satisfiedBy = new HashSet<Link>();
        private readonly ISet<Link> _satisfies = new HashSet<Link>();

        private readonly ISet<Link> _decomposedBy = new HashSet<Link>();
        private readonly ISet<Link> _decomposes = new HashSet<Link>();

        private readonly ISet<Link> _constrainedBy = new HashSet<Link>();
        private readonly ISet<Link> _constrains = new HashSet<Link>();

        private string _title;
        private string _description;
        private string _identifier;
        private string _shortTitle;

        private DateTime? _created;
        private DateTime? _modified;
        private Uri _serviceProvider;
        private Uri _instanceShape;

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
            _constrains.Add(constrains);
        }

        public void AddConstrainedBy(Link constrainedBy)
        {
            _constrainedBy.Add(constrainedBy);
        }

        public void AddDecomposes(Link decomposes)
        {
            _decomposes.Add(decomposes);
        }

        public void AddDecomposedBy(Link decomposedBy)
        {
            _decomposedBy.Add(decomposedBy);
        }

        public void AddSatisfies(Link satisfies)
        {
            _satisfies.Add(satisfies);
        }

        public void AddSatisfiedBy(Link satisfiedBy)
        {
            _satisfiedBy.Add(satisfiedBy);
        }

        public void AddValidatedBy(Link validatedBy)
        {
            _validatedBy.Add(validatedBy);
        }

        public void AddTrackedBy(Link trackedBy)
        {
            _trackedBy.Add(trackedBy);
        }

        public void AddImplementedBy(Link implementedBy)
        {
            _implementedBy.Add(implementedBy);
        }

        public void AddAffectedBy(Link affectedBy)
        {
            _affectedBy.Add(affectedBy);
        }

        public void AddElaboratedBy(Link elaboratedBy)
        {
            _elaboratedBy.Add(elaboratedBy);
        }

        public void AddElaborates(Link elaborates)
        {
            _elaborates.Add(elaborates);
        }

        public void AddSpecifiedBy(Link specifiedBy)
        {
            _specifiedBy.Add(specifiedBy);
        }

        public void AddSpecifies(Link specifies)
        {
            _specifies.Add(specifies);
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
            return _elaboratedBy.ToArray();
        }

        [OslcDescription("The object is elaborated by the subject.")]
        [OslcName("elaborates")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "elaborates")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Elaborates")]
        public Link[] GetElaborates()
        {
            return _elaborates.ToArray();
        }

        [OslcDescription("The subject is specified by the object.")]
        [OslcName("specifiedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "specifiedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Specified By")]
        public Link[] GetSpecifiedBy()
        {
            return _specifiedBy.ToArray();
        }

        [OslcDescription("The object is specified by the subject.")]
        [OslcName("specifies")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "specifies")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Specifies")]
        public Link[] GetSpecifies()
        {
            return _specifies.ToArray();
        }

        [OslcDescription("Resource, such as a change request, which implements this requirement.")]
        [OslcName("implementedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "implementedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Implemented By")]
        public Link[] GetImplementedBy()
        {
            return _implementedBy.ToArray();
        }

        [OslcDescription("Requirement is affected by a resource, such as a defect or issue.")]
        [OslcName("affectedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "affectedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Affected By")]
        public Link[] GetAffectedBy()
        {
            return _affectedBy.ToArray();
        }

        [OslcDescription("Resource, such as a change request, which tracks this requirement.")]
        [OslcName("trackedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "trackedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("tracked By")]
        public Link[] GetTrackedBy()
        {
            return _trackedBy.ToArray();
        }

        [OslcDescription("Resource, such as a test case, which validates this requirement.")]
        [OslcName("validatedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "validatedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Validated By")]
        public Link[] GetValidatedBy()
        {
            return _validatedBy.ToArray();
        }

        [OslcDescription("The subject is satisfied by the object.")]
        [OslcName("satisfiedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "satisfiedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Satisfied By")]
        public Link[] GetSatisfiedBy()
        {
            return _satisfiedBy.ToArray();
        }

        [OslcDescription("The object is satisfied by the subject.")]
        [OslcName("satisfies")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "satisfies")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Satisfies")]
        public Link[] GetSatisfies()
        {
            return _satisfies.ToArray();
        }

        [OslcDescription("The subject is decomposed by the object.")]
        [OslcName("decomposedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "decomposedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("DecomposedBy")]
        public Link[] GetDecomposedBy()
        {
            return _decomposedBy.ToArray();
        }

        [OslcDescription("The object is decomposed by the subject.")]
        [OslcName("decomposes")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "decomposes")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Decomposes")]
        public Link[] GetDecomposes()
        {
            return _decomposes.ToArray();
        }

        [OslcDescription("The subject is constrained by the object.")]
        [OslcName("constrainedBy")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "constrainedBy")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("ConstrainedBy")]
        public Link[] GetConstrainedBy()
        {
            return _constrainedBy.ToArray();
        }

        [OslcDescription("The object is constrained by the subject.")]
        [OslcName("constrains")]
        [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "constrains")]
        [OslcRange(RmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Constrains")]
        public Link[] GetConstrains()
        {
            return _constrains.ToArray();
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
            _constrains.Clear();

            if (constrains != null)
            {
                _constrains.AddAll(constrains);
            }
        }

        public void SetConstrainedBy(Link[] constrainedBy)
        {
            _constrainedBy.Clear();

            if (constrainedBy != null)
            {
                _constrainedBy.AddAll(constrainedBy);
            }
        }

        public void SetDecomposes(Link[] decomposes)
        {
            _affectedBy.Clear();

            if (decomposes != null)
            {
                _decomposes.AddAll(decomposes);
            }
        }

        public void SetDecomposedBy(Link[] decomposedBy)
        {
            _decomposedBy.Clear();

            if (decomposedBy != null)
            {
                _decomposedBy.AddAll(decomposedBy);
            }
        }

        public void SetSatisfies(Link[] satisfies)
        {
            _satisfies.Clear();

            if (satisfies != null)
            {
                _satisfies.AddAll(satisfies);
            }
        }

        public void SetSatisfiedBy(Link[] satisfiedBy)
        {
            _satisfiedBy.Clear();

            if (satisfiedBy != null)
            {
                _satisfiedBy.AddAll(satisfiedBy);
            }
        }

        public void SetValidatedBy(Link[] validatedBy)
        {
            _validatedBy.Clear();

            if (validatedBy != null)
            {
                _validatedBy.AddAll(validatedBy);
            }
        }

        public void SetTrackedBy(Link[] trackedBy)
        {
            _trackedBy.Clear();

            if (trackedBy != null)
            {
                _trackedBy.AddAll(trackedBy);
            }
        }

        public void SetAffectedBy(Link[] affectedBy)
        {
            _affectedBy.Clear();

            if (affectedBy != null)
            {
                _affectedBy.AddAll(affectedBy);
            }
        }

        public void SetImplementedBy(Link[] implementedBy)
        {
            _implementedBy.Clear();

            if (implementedBy != null)
            {
                _implementedBy.AddAll(implementedBy);
            }
        }

        public void SetElaboratedBy(Link[] elaboratedBy)
        {
            _elaboratedBy.Clear();

            if (elaboratedBy != null)
            {
                _elaboratedBy.AddAll(elaboratedBy);
            }
        }

        public void SetElaborates(Link[] elaborates)
        {
            _elaborates.Clear();

            if (elaborates != null)
            {
                _elaborates.AddAll(elaborates);
            }
        }

        public void SetSpecifiedBy(Link[] specifiedBy)
        {
            _specifiedBy.Clear();

            if (specifiedBy != null)
            {
                _specifiedBy.AddAll(specifiedBy);
            }
        }

        public void SetSpecifies(Link[] specifies)
        {
            _specifies.Clear();

            if (specifies != null)
            {
                _specifies.AddAll(specifies);
            }
        }

        public void SetContributors(Uri[] contributors)
        {
            _contributors.Clear();

            if (contributors != null)
            {
                _contributors.AddAll(contributors);
            }
        }

        public void SetCreated(DateTime? created)
        {
            _created = created;
        }

        public void SetCreators(Uri[] creators)
        {
            _creators.Clear();

            if (creators != null)
            {
                _creators.AddAll(creators);
            }
        }

        public void SetDescription(string description)
        {
            _description = description;
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

        public void SetShortTitle(string shortTitle)
        {
            _shortTitle = shortTitle;
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public void SetSubjects(string[] subjects)
        {
            _subjects.Clear();

            if (subjects != null)
            {
                _subjects.AddAll(subjects);
            }
        }
    }
}
