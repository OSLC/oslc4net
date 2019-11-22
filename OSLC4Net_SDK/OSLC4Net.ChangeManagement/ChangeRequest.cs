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
using System.Text;
using System.Reflection;

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagement
{
    /// <summary>
    /// OSLC Change Management resource
    /// </summary>
    [OslcNamespace(Constants.CHANGE_MANAGEMENT_NAMESPACE)]
    [OslcResourceShape(title = "Change Request Resource Shape", describes = new string[] { Constants.TYPE_CHANGE_REQUEST })]
    public class ChangeRequest : AbstractResource
    {
        private readonly ISet<Link>     affectedByDefects           = new HashSet<Link>();
        private readonly ISet<Link>     affectsPlanItems            = new HashSet<Link>();
        private readonly ISet<Link>     affectsRequirements         = new HashSet<Link>();
        private readonly ISet<Link>     affectsTestResults          = new HashSet<Link>();
        private readonly ISet<Link>     blocksTestExecutionRecords  = new HashSet<Link>();
        private readonly ISet<Uri>      contributors                = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      creators                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Type>     dctermsTypes                = new HashSet<Type>(); // XXX - TreeSet<> in Java
        private readonly ISet<Link>     implementsRequirements      = new HashSet<Link>();
        private readonly ISet<Link>     relatedChangeRequests       = new HashSet<Link>();
        private readonly ISet<Link>     relatedResources            = new HashSet<Link>(); // TODO - Extension to point to any other OSLC resource(s).
        private readonly ISet<Link>     relatedTestCases            = new HashSet<Link>();
        private readonly ISet<Link>     relatedTestExecutionRecords = new HashSet<Link>();
        private readonly ISet<Link>     relatedTestPlans            = new HashSet<Link>();
        private readonly ISet<Link>     relatedTestScripts          = new HashSet<Link>();
        private readonly ISet<string>   subjects                    = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Link>     testedByTestCases           = new HashSet<Link>();
        private readonly ISet<Link>     tracksChangeSets            = new HashSet<Link>();
        private readonly ISet<Link>     tracksRequirements          = new HashSet<Link>();
        private readonly ISet<Uri>      rdfTypes                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java

        private bool?       approved;
        private bool?       closed;
        private DateTime?   closeDate;
        private DateTime?   created;
        private string      description;
        private Uri         discussedBy;
        private bool?       isFixed;
        private string      identifier;
        private bool?       inProgress;
        private Uri         instanceShape;
        private DateTime?   modified;
        private bool?       reviewed;
        private Uri         serviceProvider;
        private Severity    severity = Severity.Unclassified; // TODO - Added severity for demo
        private string      shortTitle;
        private string      status;
        private string      title;
        private bool?       verified;

        public ChangeRequest() : base()
        {
            rdfTypes.Add(new Uri(Constants.TYPE_CHANGE_REQUEST));
        }

        public ChangeRequest(Uri about) : base(about)
        {
            rdfTypes.Add(new Uri(Constants.TYPE_CHANGE_REQUEST));
        }

        public void AddAffectedByDefect(Link affectedByDefect)
        {
            affectedByDefects.Add(affectedByDefect);
        }

        public void AddAffectsPlanItem(Link affectsPlanItem)
        {
            affectsPlanItems.Add(affectsPlanItem);
        }

        public void AddAffectsRequirement(Link affectsRequirement)
        {
            affectsRequirements.Add(affectsRequirement);
        }

        public void AddAffectsTestResult(Link affectsTestResult)
        {
            affectsTestResults.Add(affectsTestResult);
        }

        public void AddBlocksTestExecutionRecord(Link blocksTestExecutionRecord)
        {
            blocksTestExecutionRecords.Add(blocksTestExecutionRecord);
        }

        public void AddContributor(Uri contributor)
        {
            contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            creators.Add(creator);
        }

        public void AddDctermsType(string dctermsType)
        {
            dctermsTypes.Add(TypeExtension.FromString(dctermsType));
        }

        public void AddImplementsRequirement(Link implementsRequirement)
        {
            implementsRequirements.Add(implementsRequirement);
        }

        public void AddRdfType(Uri rdfType)
        {
            rdfTypes.Add(rdfType);
        }

        public void AddRelatedChangeRequest(Link relatedChangeRequest)
        {
            relatedChangeRequests.Add(relatedChangeRequest);
        }

        public void AddRelatedResource(Link relatedResource)
        {
            relatedResources.Add(relatedResource);
        }

        public void AddRelatedTestCase(Link relatedTestCase)
        {
            relatedTestCases.Add(relatedTestCase);
        }

        public void AddRelatedTestExecutionRecord(Link relatedTestExecutionRecord)
        {
            relatedTestExecutionRecords.Add(relatedTestExecutionRecord);
        }

        public void AddRelatedTestPlan(Link relatedTestPlan)
        {
            relatedTestPlans.Add(relatedTestPlan);
        }

        public void addRelatedTestScript(Link relatedTestScript)
        {
            relatedTestScripts.Add(relatedTestScript);
        }

        public void AddSubject(string subject)
        {
            subjects.Add(subject);
        }

        public void AddTestedByTestCase(Link testedByTestCase)
        {
            testedByTestCases.Add(testedByTestCase);
        }

        public void AddTracksChangeSet(Link tracksChangeSet)
        {
            tracksChangeSets.Add(tracksChangeSet);
        }

        public void AddTracksRequirement(Link tracksRequirement)
        {
            tracksRequirements.Add(tracksRequirement);
        }

        [OslcDescription("Change request is affected by a reported defect.")]
        [OslcName("affectedByDefect")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectedByDefect")]
        [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Affected By Defects")]
        public Link[] GetAffectedByDefects()
        {
            return affectedByDefects.ToArray();
        }

        [OslcDescription("Change request affects a plan item. ")]
        [OslcName("affectsPlanItem")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsPlanItem")]
        [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Affects Plan Items")]
        public Link[] GetAffectsPlanItems()
        {
            return affectsPlanItems.ToArray();
        }

        [OslcDescription("Change request affecting a Requirement.")]
        [OslcName("affectsRequirement")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsRequirement")]
        [OslcRange(Constants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Affects Requirements")]
        public Link[] GetAffectsRequirements()
        {
            return affectsRequirements.ToArray();
        }

        [OslcDescription("Associated QM resource that is affected by this Change Request.")]
        [OslcName("affectsTestResult")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsTestResult")]
        [OslcRange(Constants.TYPE_TEST_RESULT)]
        [OslcReadOnly(false)]
        [OslcTitle("Affects Test Results")]
        public Link[] GetAffectsTestResults()
        {
            return affectsTestResults.ToArray();
        }

        [OslcDescription("Associated QM resource that is blocked by this Change Request.")]
        [OslcName("blocksTestExecutionRecord")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "blocksTestExecutionRecord")]
        [OslcRange(Constants.TYPE_TEST_EXECUTION_RECORD)]
        [OslcReadOnly(false)]
        [OslcTitle("Blocks Test Execution Records")]
        public Link[] GetBlocksTestExecutionRecords()
        {
            return blocksTestExecutionRecords.ToArray();
        }

        [OslcDescription("The date at which no further activity or work is intended to be conducted. ")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "closeDate")]
        [OslcReadOnly]
        [OslcTitle("Close DateTime?")]
        public DateTime? GetCloseDate()
        {
            return closeDate;
        }

        [OslcDescription("The person(s) who are responsible for the work needed to complete the change request.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(Constants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return contributors.ToArray();
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return created;
        }

        [OslcDescription("Creator or creators of resource.")]
        [OslcName("creator")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
        [OslcRange(Constants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return creators.ToArray();
        }

        [OslcAllowedValue(new string[] {"Defect", "Task", "Story", "Bug Report", "Feature Request"})]
        [OslcDescription("A short string representation for the type, example 'Defect'.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public string[] GetDctermsTypes()
        {
            string[] result = new string[dctermsTypes.Count];

            int index = 0;

            foreach (Type type in dctermsTypes)
            {
                result[index++] = TypeExtension.ToString(type);
            }

            return result;
        }

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return description;
        }

        [OslcDescription("A series of notes and comments about this change request.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "discussedBy")]
        [OslcRange(Constants.TYPE_DISCUSSION)]
        [OslcTitle("Discussed By")]
        public Uri GetDiscussedBy()
        {
            return discussedBy;
        }

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return identifier;
        }

        [OslcDescription("Implements associated Requirement.")]
        [OslcName("implementsRequirement")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "implementsRequirement")]
        [OslcRange(Constants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Implements Requirements")]
        public Link[] GetImplementsRequirements()
        {
            return implementsRequirements.ToArray();
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return instanceShape;
        }

        [OslcDescription("Timestamp last latest resource modification.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
        [OslcReadOnly]
        [OslcTitle("Modified")]
        public DateTime? GetModified()
        {
            return modified;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return rdfTypes.ToArray();
        }

        [OslcDescription("This relationship is loosely coupled and has no specific meaning.")]
        [OslcName("relatedChangeRequest")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedChangeRequest")]
        [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Change Requests")]
        public Link[] GetRelatedChangeRequests()
        {
            return relatedChangeRequests.ToArray();
        }

        [OslcDescription("Related OSLC resources of any type.")]
        [OslcName("relatedResource")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedResource")]
        [OslcTitle("Related Resources")]
        public Link[] GetRelatedResources()
        {
            return relatedResources.ToArray();
        }

        [OslcDescription("Related QM test case resource.")]
        [OslcName("relatedTestCase")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestCase")]
        [OslcRange(Constants.TYPE_TEST_CASE)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Test Cases")]
        public Link[] GetRelatedTestCases()
        {
            return relatedTestCases.ToArray();
        }

        [OslcDescription("Related to a QM test execution resource.")]
        [OslcName("relatedTestExecutionRecord")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestExecutionRecord")]
        [OslcRange(Constants.TYPE_TEST_EXECUTION_RECORD)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Test Execution Records")]
        public Link[] GetRelatedTestExecutionRecords()
        {
            return relatedTestExecutionRecords.ToArray();
        }

        [OslcDescription("Related QM test plan resource.")]
        [OslcName("relatedTestPlan")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestPlan")]
        [OslcRange(Constants.TYPE_TEST_PLAN)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Test Plans")]
        public Link[] GetRelatedTestPlans()
        {
            return relatedTestPlans.ToArray();
        }

        [OslcDescription("Related QM test script resource.")]
        [OslcName("relatedTestScript")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestScript")]
        [OslcRange(Constants.TYPE_TEST_SCRIPT)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Test Scripts")]
        public Link[] GetRelatedTestScripts()
        {
            return relatedTestScripts.ToArray();
        }

        [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return serviceProvider;
        }

        [OslcAllowedValue(new string[] {"Unclassified", "Minor", "Normal", "Major", "Critical", "Blocker"})]
        [OslcDescription("Severity of change request.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "severity")]
        [OslcTitle("Severity")]
        public string GetSeverity()
        {
            return severity.ToString();
        }

        [OslcDescription("Short name identifying a resource, often used as an abbreviated identifier for presentation to end-users.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "shortTitle")]
        [OslcTitle("Short Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetShortTitle()
        {
            return shortTitle;
        }

        [OslcDescription("Used to indicate the status of the change request based on values defined by the service provider. Most often a read-only property. Some possible values may include: 'Submitted', 'Done', 'InProgress', etc.")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "status")]
        [OslcTitle("Status")]
        public string GetStatus()
        {
            return status;
        }

        [OslcDescription("Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")]
        [OslcName("subject")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "subject")]
        [OslcReadOnly(false)]
        [OslcTitle("Subjects")]
        public string[] GetSubjects()
        {
            return subjects.ToArray();
        }

        [OslcDescription("Test case by which this change request is tested.")]
        [OslcName("testedByTestCase")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "testedByTestCase")]
        [OslcRange(Constants.TYPE_TEST_CASE)]
        [OslcReadOnly(false)]
        [OslcTitle("Tested by Test Cases")]
        public Link[] GetTestedByTestCases()
        {
            return testedByTestCases.ToArray();
        }

        [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcTitle("Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return title;
        }

        [OslcDescription("Tracks SCM change set resource.")]
        [OslcName("tracksChangeSet")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "tracksChangeSet")]
        [OslcRange(Constants.TYPE_CHANGE_SET)]
        [OslcReadOnly(false)]
        [OslcTitle("Tracks Change Sets")]
        public Link[] GetTracksChangeSets()
        {
            return tracksChangeSets.ToArray();
        }

        [OslcDescription("Tracks the associated Requirement or Requirement ChangeSet resources.")]
        [OslcName("tracksRequirement")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "tracksRequirement")]
        [OslcRange(Constants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Tracks Requirements")]
        public Link[] GetTracksRequirements()
        {
            return tracksRequirements.ToArray();
        }

        [OslcDescription("Whether or not the Change Request has been approved.")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "approved")]
        [OslcReadOnly]
        [OslcTitle("Approved")]
        public bool? IsApproved()
        {
            return approved;
        }

        [OslcDescription("Whether or not the Change Request is completely done, no further fixes or fix verification is needed.")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "closed")]
        [OslcReadOnly]
        [OslcTitle("Closed")]
        public bool? IsClosed()
        {
            return closed;
        }

        [OslcDescription("Whether or not the Change Request has been fixed.")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "fixed")]
        [OslcReadOnly]
        [OslcTitle("Fixed")]
        public bool? IsFixed()
        {
            return isFixed;
        }

        [OslcDescription("Whether or not the Change Request in a state indicating that active work is occurring. If oslc_cm:inprogress is true, then oslc_cm:fixed and oslc_cm:closed must also be false.")]
        [OslcName("inprogress")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "inprogress")]
        [OslcReadOnly]
        [OslcTitle("In] Progress")]
        public bool? IsInProgress()
        {
            return inProgress;
        }

        [OslcDescription("Whether or not the Change Request has been reviewed.")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "reviewed")]
        [OslcReadOnly]
        [OslcTitle("Reviewed")]
        public bool? IsReviewed()
        {
            return reviewed;
        }

        [OslcDescription("Whether or not the resolution or fix of the Change Request has been verified.")]
        [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "verified")]
        [OslcReadOnly]
        [OslcTitle("Verified")]
        public bool? IsVerified()
        {
            return verified;
        }

        public void SetAffectedByDefects(Link[] affectedByDefects)
        {
            this.affectedByDefects.Clear();

            if (affectedByDefects != null)
            {
                this.affectedByDefects.AddAll(affectedByDefects);
            }
        }

        public void SetAffectsPlanItems(Link[] affectsPlanItems)
        {
            this.affectsPlanItems.Clear();

            if (affectsPlanItems != null)
            {
                this.affectsPlanItems.AddAll(affectsPlanItems);
            }
        }

        public void SetAffectsRequirements(Link[] affectsRequirements)
        {
            this.affectsRequirements.Clear();

            if (affectsRequirements != null)
            {
                this.affectsRequirements.AddAll(affectsRequirements);
            }
        }

        public void SetAffectsTestResults(Link[] affectsTestResults)
        {
            this.affectsTestResults.Clear();

            if (affectsTestResults != null)
            {
                this.affectsTestResults.AddAll(affectsTestResults);
            }
        }

        public void SetApproved(bool? approved)
        {
            this.approved = approved;
        }

        public void SetBlocksTestExecutionRecords(Link[] blocksTestExecutionRecords)
        {
            this.blocksTestExecutionRecords.Clear();

            if (blocksTestExecutionRecords != null)
            {
                this.blocksTestExecutionRecords.AddAll(blocksTestExecutionRecords);
            }
        }

        public void SetClosed(bool? closed)
        {
            this.closed = closed;
        }

        public void SetCloseDate(DateTime? closeDate)
        {
            this.closeDate = closeDate;
        }

        public void SetContributors(Uri[] contributors)
        {
            this.contributors.Clear();

            if (contributors != null)
            {
                this.contributors.AddAll(contributors);
            }
        }

        public void SetCreated(DateTime? created)
        {
            this.created = created;
        }

        public void SetCreators(Uri[] creators)
        {
            this.creators.Clear();

            if (creators != null)
            {
                this.creators.AddAll(creators);
            }
        }

        public void SetDctermsTypes(string[] dctermsTypes)
        {
            this.dctermsTypes.Clear();

            if (dctermsTypes != null)
            {
                foreach (string type in dctermsTypes)
                {
                    this.dctermsTypes.Add(TypeExtension.FromString(type));
                }
            }
        }

        public void SetDescription(string description)
        {
            this.description = description;
        }

        public void SetDiscussedBy(Uri discussedBy)
        {
            this.discussedBy = discussedBy;
        }

        public void SetFixed(bool? isFixed)
        {
            this.isFixed = isFixed;
        }

        public void SetIdentifier(string identifier)
        {
            this.identifier = identifier;
        }

        public void SetImplementsRequirements(Link[] implementsRequirements)
        {
            this.implementsRequirements.Clear();

            if (implementsRequirements != null)
            {
                this.implementsRequirements.AddAll(implementsRequirements);
            }
        }

        public void SetInProgress(bool? inProgress)
        {
            this.inProgress = inProgress;
        }

        public void SetInstanceShape(Uri instanceShape)
        {
            this.instanceShape = instanceShape;
        }

        public void SetModified(DateTime? modified)
        {
            this.modified = modified;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            this.rdfTypes.Clear();

            if (rdfTypes != null)
            {
                this.rdfTypes.AddAll(rdfTypes);
            }
        }

        public void SetRelatedChangeRequests(Link[] relatedChangeRequests)
        {
            this.relatedChangeRequests.Clear();

            if (relatedChangeRequests != null)
            {
                this.relatedChangeRequests.AddAll(relatedChangeRequests);
            }
        }

        public void SetRelatedResources(Link[] relatedResources)
        {
            this.relatedResources.Clear();

            if (relatedResources != null)
            {
                this.relatedResources.AddAll(relatedResources);
            }
        }

        public void SetRelatedTestCases(Link[] relatedTestCases)
        {
            this.relatedTestCases.Clear();

            if (relatedTestCases != null)
            {
                this.relatedTestCases.AddAll(relatedTestCases);
            }
        }

        public void SetRelatedTestExecutionRecords(Link[] relatedTestExecutionRecords)
        {
            this.relatedTestExecutionRecords.Clear();

            if (relatedTestExecutionRecords != null)
            {
                this.relatedTestExecutionRecords.AddAll(relatedTestExecutionRecords);
            }
        }

        public void SetRelatedTestPlans(Link[] relatedTestPlans)
        {
            this.relatedTestPlans.Clear();

            if (relatedTestPlans != null)
            {
                this.relatedTestPlans.AddAll(relatedTestPlans);
            }
        }

        public void SetRelatedTestScripts(Link[] relatedTestScripts)
        {
            this.relatedTestScripts.Clear();

            if (relatedTestScripts != null)
            {
                this.relatedTestScripts.AddAll(relatedTestScripts);
            }
        }

        public void SetReviewed(bool? reviewed)
        {
            this.reviewed = reviewed;
        }

        public void SetServiceProvider(Uri serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void SetSeverity(string severity)
        {
            this.severity = SeverityExtension.FromString(severity);
        }

        public void SetShortTitle(string shortTitle)
        {
            this.shortTitle = shortTitle;
        }

        public void SetStatus(string status)
        {
            this.status = status;
        }

        public void SetSubjects(string[] subjects)
        {
            this.subjects.Clear();

            if (subjects != null)
            {
                this.subjects.AddAll(subjects);
            }
        }

        public void SetTestedByTestCases(Link[] testedByTestCases)
        {
            this.testedByTestCases.Clear();

            if (testedByTestCases != null)
            {
                this.testedByTestCases.AddAll(testedByTestCases);
            }
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void SetTracksChangeSets(Link[] tracksChangeSets)
        {
            this.tracksChangeSets.Clear();

            if (tracksChangeSets != null)
            {
                this.tracksChangeSets.AddAll(tracksChangeSets);
            }
        }

        public void SetTracksRequirements(Link[] tracksRequirements)
        {
            this.tracksRequirements.Clear();

            if (tracksRequirements != null)
            {
                this.tracksRequirements.AddAll(tracksRequirements);
            }
        }

        public void SetVerified(bool? verified)
        {
            this.verified = verified;
        }
    }
}
