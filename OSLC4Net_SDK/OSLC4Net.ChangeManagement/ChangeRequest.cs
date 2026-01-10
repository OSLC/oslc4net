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
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.ChangeManagement;

/// <summary>
/// OSLC Change Management resource
/// </summary>
[OslcNamespace(Constants.CHANGE_MANAGEMENT_NAMESPACE)]
[OslcResourceShape(title = "Change Request Resource Shape", describes = new string[] { Constants.TYPE_CHANGE_REQUEST })]
public class ChangeRequest : AbstractResource
{
    private Severity _severity = ChangeManagement.Severity.Unclassified; // TODO - Added severity for demo
    private readonly ISet<Type> _dctermsTypes = new HashSet<Type>(); // XXX - TreeSet<> in Java

    public ChangeRequest() : base()
    {
        AddType(new Uri(Constants.TYPE_CHANGE_REQUEST));
    }

    public ChangeRequest(Uri about) : base(about)
    {
        AddType(new Uri(Constants.TYPE_CHANGE_REQUEST));
    }

    public void AddAffectedByDefect(Link affectedByDefect)
    {
        AffectedByDefects.Add(affectedByDefect);
    }

    public void AddAffectsPlanItem(Link affectsPlanItem)
    {
        AffectsPlanItems.Add(affectsPlanItem);
    }

    public void AddAffectsRequirement(Link affectsRequirement)
    {
        AffectsRequirements.Add(affectsRequirement);
    }

    public void AddAffectsTestResult(Link affectsTestResult)
    {
        AffectsTestResults.Add(affectsTestResult);
    }

    public void AddBlocksTestExecutionRecord(Link blocksTestExecutionRecord)
    {
        BlocksTestExecutionRecords.Add(blocksTestExecutionRecord);
    }

    public void AddContributor(Uri contributor)
    {
        Contributors.Add(contributor);
    }

    public void AddCreator(Uri creator)
    {
        Creators.Add(creator);
    }

    public void AddDctermsType(string dctermsType)
    {
        _dctermsTypes.Add(TypeExtension.FromString(dctermsType));
    }

    public void AddImplementsRequirement(Link implementsRequirement)
    {
        ImplementsRequirements.Add(implementsRequirement);
    }

    public void AddRdfType(Uri rdfType)
    {
        AddType(rdfType);
    }

    public void AddRelatedChangeRequest(Link relatedChangeRequest)
    {
        RelatedChangeRequests.Add(relatedChangeRequest);
    }

    public void AddRelatedResource(Link relatedResource)
    {
        RelatedResources.Add(relatedResource);
    }

    public void AddRelatedTestCase(Link relatedTestCase)
    {
        RelatedTestCases.Add(relatedTestCase);
    }

    public void AddRelatedTestExecutionRecord(Link relatedTestExecutionRecord)
    {
        RelatedTestExecutionRecords.Add(relatedTestExecutionRecord);
    }

    public void AddRelatedTestPlan(Link relatedTestPlan)
    {
        RelatedTestPlans.Add(relatedTestPlan);
    }

    public void AddRelatedTestScript(Link relatedTestScript)
    {
        RelatedTestScripts.Add(relatedTestScript);
    }

    public void AddSubject(string subject)
    {
        Subjects.Add(subject);
    }

    public void AddTestedByTestCase(Link testedByTestCase)
    {
        TestedByTestCases.Add(testedByTestCase);
    }

    public void AddTracksChangeSet(Link tracksChangeSet)
    {
        TracksChangeSets.Add(tracksChangeSet);
    }

    public void AddTracksRequirement(Link tracksRequirement)
    {
        TracksRequirements.Add(tracksRequirement);
    }

    [OslcDescription("Change request is affected by a reported defect.")]
    [OslcName("affectedByDefect")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectedByDefect")]
    [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
    [OslcReadOnly(false)]
    [OslcTitle("Affected By Defects")]
    public ISet<Link> AffectedByDefects
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Change request affects a plan item. ")]
    [OslcName("affectsPlanItem")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsPlanItem")]
    [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
    [OslcReadOnly(false)]
    [OslcTitle("Affects Plan Items")]
    public ISet<Link> AffectsPlanItems
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Change request affecting a Requirement.")]
    [OslcName("affectsRequirement")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsRequirement")]
    [OslcRange(Constants.TYPE_REQUIREMENT)]
    [OslcReadOnly(false)]
    [OslcTitle("Affects Requirements")]
    public ISet<Link> AffectsRequirements
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Associated QM resource that is affected by this Change Request.")]
    [OslcName("affectsTestResult")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsTestResult")]
    [OslcRange(Constants.TYPE_TEST_RESULT)]
    [OslcReadOnly(false)]
    [OslcTitle("Affects Test Results")]
    public ISet<Link> AffectsTestResults
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Associated QM resource that is blocked by this Change Request.")]
    [OslcName("blocksTestExecutionRecord")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "blocksTestExecutionRecord")]
    [OslcRange(Constants.TYPE_TEST_EXECUTION_RECORD)]
    [OslcReadOnly(false)]
    [OslcTitle("Blocks Test Execution Records")]
    public ISet<Link> BlocksTestExecutionRecords
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("The date at which no further activity or work is intended to be conducted. ")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "closeDate")]
    [OslcReadOnly]
    [OslcTitle("Close DateTime?")]
    public DateTime? CloseDate { get; set; }

    [OslcDescription("The person(s) who are responsible for the work needed to complete the change request.")]
    [OslcName("contributor")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
    [OslcRange(Constants.TYPE_PERSON)]
    [OslcTitle("Contributors")]
    public ISet<Uri> Contributors
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Uri>();

    [OslcDescription("Timestamp of resource creation.")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
    [OslcReadOnly]
    [OslcTitle("Created")]
    public DateTime? Created { get; set; }

    [OslcDescription("Creator or creators of resource.")]
    [OslcName("creator")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
    [OslcRange(Constants.TYPE_PERSON)]
    [OslcTitle("Creators")]
    public ISet<Uri> Creators
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Uri>();

    [OslcAllowedValue(new string[] { "Defect", "Task", "Story", "Bug Report", "Feature Request" })]
    [OslcDescription("A short string representation for the type, example 'Defect'.")]
    [OslcName("type")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "type")]
    [OslcTitle("Types")]
    public string[] DctermsTypes
    {
        get
        {
            var result = new string[_dctermsTypes.Count];
            var index = 0;
            foreach (var type in _dctermsTypes)
            {
                result[index++] = TypeExtension.ToString(type);
            }
            return result;
        }
        set
        {
            _dctermsTypes.Clear();
            if (value != null)
            {
                foreach (var type in value)
                {
                    _dctermsTypes.Add(TypeExtension.FromString(type));
                }
            }
        }
    }

    [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
    [OslcTitle("Description")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string Description { get; set; }

    [OslcDescription("A series of notes and comments about this change request.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "discussedBy")]
    [OslcRange(Constants.TYPE_DISCUSSION)]
    [OslcTitle("Discussed By")]
    public Uri DiscussedBy { get; set; }

    [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
    [OslcReadOnly]
    [OslcTitle("Identifier")]
    public string Identifier { get; set; }

    [OslcDescription("Implements associated Requirement.")]
    [OslcName("implementsRequirement")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "implementsRequirement")]
    [OslcRange(Constants.TYPE_REQUIREMENT)]
    [OslcReadOnly(false)]
    [OslcTitle("Implements Requirements")]
    public ISet<Link> ImplementsRequirements
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
    [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
    [OslcTitle("Instance Shape")]
    public Uri InstanceShape { get; set; }

    [OslcDescription("Timestamp last latest resource modification.")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
    [OslcReadOnly]
    [OslcTitle("Modified")]
    public DateTime? Modified { get; set; }

    [OslcDescription("This relationship is loosely coupled and has no specific meaning.")]
    [OslcName("relatedChangeRequest")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedChangeRequest")]
    [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Change Requests")]
    public ISet<Link> RelatedChangeRequests
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Related OSLC resources of any type.")]
    [OslcName("relatedResource")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedResource")]
    [OslcTitle("Related Resources")]
    public ISet<Link> RelatedResources
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>(); // TODO - Extension to point to any other OSLC resource(s).

    [OslcDescription("Related QM test case resource.")]
    [OslcName("relatedTestCase")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestCase")]
    [OslcRange(Constants.TYPE_TEST_CASE)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Cases")]
    public ISet<Link> RelatedTestCases
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Related to a QM test execution resource.")]
    [OslcName("relatedTestExecutionRecord")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestExecutionRecord")]
    [OslcRange(Constants.TYPE_TEST_EXECUTION_RECORD)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Execution Records")]
    public ISet<Link> RelatedTestExecutionRecords
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Related QM test plan resource.")]
    [OslcName("relatedTestPlan")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestPlan")]
    [OslcRange(Constants.TYPE_TEST_PLAN)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Plans")]
    public ISet<Link> RelatedTestPlans
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Related QM test script resource.")]
    [OslcName("relatedTestScript")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestScript")]
    [OslcRange(Constants.TYPE_TEST_SCRIPT)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Scripts")]
    public ISet<Link> RelatedTestScripts
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
    [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
    [OslcTitle("Service Provider")]
    public Uri ServiceProvider { get; set; }

    [OslcAllowedValue(new string[] { "Unclassified", "Minor", "Normal", "Major", "Critical", "Blocker" })]
    [OslcDescription("Severity of change request.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "severity")]
    [OslcTitle("Severity")]
    public string Severity
    {
        get => _severity.ToString();
        set => _severity = SeverityExtension.FromString(value);
    }

    [OslcDescription("Short name identifying a resource, often used as an abbreviated identifier for presentation to end-users.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "shortTitle")]
    [OslcTitle("Short Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string ShortTitle { get; set; }

    [OslcDescription("Used to indicate the status of the change request based on values defined by the service provider. Most often a read-only property. Some possible values may include: 'Submitted', 'Done', 'InProgress', etc.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "status")]
    [OslcTitle("Status")]
    public string Status { get; set; }

    [OslcDescription("Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")]
    [OslcName("subject")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "subject")]
    [OslcReadOnly(false)]
    [OslcTitle("Subjects")]
    public ISet<string> Subjects
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<string>(StringComparer.Ordinal); // XXX - TreeSet<> in Java

    [OslcDescription("Test case by which this change request is tested.")]
    [OslcName("testedByTestCase")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "testedByTestCase")]
    [OslcRange(Constants.TYPE_TEST_CASE)]
    [OslcReadOnly(false)]
    [OslcTitle("Tested by Test Cases")]
    public ISet<Link> TestedByTestCases
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string Title { get; set; }

    [OslcDescription("Tracks SCM change set resource.")]
    [OslcName("tracksChangeSet")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "tracksChangeSet")]
    [OslcRange(Constants.TYPE_CHANGE_SET)]
    [OslcReadOnly(false)]
    [OslcTitle("Tracks Change Sets")]
    public ISet<Link> TracksChangeSets
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Tracks the associated Requirement or Requirement ChangeSet resources.")]
    [OslcName("tracksRequirement")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "tracksRequirement")]
    [OslcRange(Constants.TYPE_REQUIREMENT)]
    [OslcReadOnly(false)]
    [OslcTitle("Tracks Requirements")]
    public ISet<Link> TracksRequirements
    {
        get;
        set
        {
            if (ReferenceEquals(field, value)) return;
            field.Clear();
            if (value != null)
            {
                field.AddAll(value);
            }
        }
    } = new HashSet<Link>();

    [OslcDescription("Whether or not the Change Request has been approved.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "approved")]
    [OslcReadOnly]
    [OslcTitle("Approved")]
    public bool? Approved { get; set; }

    [OslcDescription("Whether or not the Change Request is completely done, no further fixes or fix verification is needed.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "closed")]
    [OslcReadOnly]
    [OslcTitle("Closed")]
    public bool? Closed { get; set; }

    [OslcDescription("Whether or not the Change Request has been fixed.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "fixed")]
    [OslcReadOnly]
    [OslcTitle("Fixed")]
    public bool? Fixed { get; set; }

    [OslcDescription("Whether or not the Change Request in a state indicating that active work is occurring. If oslc_cm:inprogress is true, then oslc_cm:fixed and oslc_cm:closed must also be false.")]
    [OslcName("inprogress")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "inprogress")]
    [OslcReadOnly]
    [OslcTitle("In] Progress")]
    public bool? InProgress { get; set; }

    [OslcDescription("Whether or not the Change Request has been reviewed.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "reviewed")]
    [OslcReadOnly]
    [OslcTitle("Reviewed")]
    public bool? Reviewed { get; set; }

    [OslcDescription("Whether or not the resolution or fix of the Change Request has been verified.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "verified")]
    [OslcReadOnly]
    [OslcTitle("Verified")]
    public bool? Verified { get; set; }

    [Obsolete("Use AffectedByDefects property instead")]
    public Link[] GetAffectedByDefects()
    {
        return AffectedByDefects.ToArray();
    }

    [Obsolete("Use AffectsPlanItems property instead")]
    public Link[] GetAffectsPlanItems()
    {
        return AffectsPlanItems.ToArray();
    }

    [Obsolete("Use AffectsRequirements property instead")]
    public Link[] GetAffectsRequirements()
    {
        return AffectsRequirements.ToArray();
    }

    [Obsolete("Use AffectsTestResults property instead")]
    public Link[] GetAffectsTestResults()
    {
        return AffectsTestResults.ToArray();
    }

    [Obsolete("Use BlocksTestExecutionRecords property instead")]
    public Link[] GetBlocksTestExecutionRecords()
    {
        return BlocksTestExecutionRecords.ToArray();
    }

    [Obsolete("Use CloseDate property instead")]
    public DateTime? GetCloseDate()
    {
        return CloseDate;
    }

    [Obsolete("Use Contributors property instead")]
    public Uri[] GetContributors()
    {
        return Contributors.ToArray();
    }

    [Obsolete("Use Created property instead")]
    public DateTime? GetCreated()
    {
        return Created;
    }

    [Obsolete("Use Creators property instead")]
    public Uri[] GetCreators()
    {
        return Creators.ToArray();
    }

    [Obsolete("Use DctermsTypes property instead")]
    public string[] GetDctermsTypes()
    {
        return DctermsTypes;
    }

    [Obsolete("Use Description property instead")]
    public string GetDescription()
    {
        return Description;
    }

    [Obsolete("Use DiscussedBy property instead")]
    public Uri GetDiscussedBy()
    {
        return DiscussedBy;
    }

    [Obsolete("Use Identifier property instead")]
    public string GetIdentifier()
    {
        return Identifier;
    }

    [Obsolete("Use ImplementsRequirements property instead")]
    public Link[] GetImplementsRequirements()
    {
        return ImplementsRequirements.ToArray();
    }

    [Obsolete("Use InstanceShape property instead")]
    public Uri GetInstanceShape()
    {
        return InstanceShape;
    }

    [Obsolete("Use Modified property instead")]
    public DateTime? GetModified()
    {
        return Modified;
    }

    // [OslcDescription("The resource type URIs.")]
    // [OslcName("type")]
    // [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
    // [OslcTitle("Types")]
    [Obsolete]
    public Uri[] GetRdfTypes()
    {
        return GetTypes().ToArray();
    }

    [Obsolete("Use RelatedChangeRequests property instead")]
    public Link[] GetRelatedChangeRequests()
    {
        return RelatedChangeRequests.ToArray();
    }

    [Obsolete("Use RelatedResources property instead")]
    public Link[] GetRelatedResources()
    {
        return RelatedResources.ToArray();
    }

    [Obsolete("Use RelatedTestCases property instead")]
    public Link[] GetRelatedTestCases()
    {
        return RelatedTestCases.ToArray();
    }

    [Obsolete("Use RelatedTestExecutionRecords property instead")]
    public Link[] GetRelatedTestExecutionRecords()
    {
        return RelatedTestExecutionRecords.ToArray();
    }

    [Obsolete("Use RelatedTestPlans property instead")]
    public Link[] GetRelatedTestPlans()
    {
        return RelatedTestPlans.ToArray();
    }

    [Obsolete("Use RelatedTestScripts property instead")]
    public Link[] GetRelatedTestScripts()
    {
        return RelatedTestScripts.ToArray();
    }

    [Obsolete("Use ServiceProvider property instead")]
    public Uri GetServiceProvider()
    {
        return ServiceProvider;
    }

    [Obsolete("Use Severity property instead")]
    public string GetSeverity()
    {
        return Severity;
    }

    [Obsolete("Use ShortTitle property instead")]
    public string GetShortTitle()
    {
        return ShortTitle;
    }

    [Obsolete("Use Status property instead")]
    public string GetStatus()
    {
        return Status;
    }

    [Obsolete("Use Subjects property instead")]
    public string[] GetSubjects()
    {
        return Subjects.ToArray();
    }

    [Obsolete("Use TestedByTestCases property instead")]
    public Link[] GetTestedByTestCases()
    {
        return TestedByTestCases.ToArray();
    }

    [Obsolete("Use Title property instead")]
    public string GetTitle()
    {
        return Title;
    }

    [Obsolete("Use TracksChangeSets property instead")]
    public Link[] GetTracksChangeSets()
    {
        return TracksChangeSets.ToArray();
    }

    [Obsolete("Use TracksRequirements property instead")]
    public Link[] GetTracksRequirements()
    {
        return TracksRequirements.ToArray();
    }

    [Obsolete("Use Approved property instead")]
    public bool? IsApproved()
    {
        return Approved;
    }

    [Obsolete("Use Closed property instead")]
    public bool? IsClosed()
    {
        return Closed;
    }

    [Obsolete("Use Fixed property instead")]
    public bool? IsFixed()
    {
        return Fixed;
    }

    [Obsolete("Use InProgress property instead")]
    public bool? IsInProgress()
    {
        return InProgress;
    }

    [Obsolete("Use Reviewed property instead")]
    public bool? IsReviewed()
    {
        return Reviewed;
    }

    [Obsolete("Use Verified property instead")]
    public bool? IsVerified()
    {
        return Verified;
    }

    [Obsolete("Use AffectedByDefects property instead")]
    public void SetAffectedByDefects(Link[] affectedByDefects)
    {
        AffectedByDefects.Clear();

        if (affectedByDefects != null)
        {
            AffectedByDefects.AddAll(affectedByDefects);
        }
    }

    [Obsolete("Use AffectsPlanItems property instead")]
    public void SetAffectsPlanItems(Link[] affectsPlanItems)
    {
        AffectsPlanItems.Clear();

        if (affectsPlanItems != null)
        {
            AffectsPlanItems.AddAll(affectsPlanItems);
        }
    }

    [Obsolete("Use AffectsRequirements property instead")]
    public void SetAffectsRequirements(Link[] affectsRequirements)
    {
        AffectsRequirements.Clear();

        if (affectsRequirements != null)
        {
            AffectsRequirements.AddAll(affectsRequirements);
        }
    }

    [Obsolete("Use AffectsTestResults property instead")]
    public void SetAffectsTestResults(Link[] affectsTestResults)
    {
        AffectsTestResults.Clear();

        if (affectsTestResults != null)
        {
            AffectsTestResults.AddAll(affectsTestResults);
        }
    }

    [Obsolete("Use Approved property instead")]
    public void SetApproved(bool? approved)
    {
        Approved = approved;
    }

    [Obsolete("Use BlocksTestExecutionRecords property instead")]
    public void SetBlocksTestExecutionRecords(Link[] blocksTestExecutionRecords)
    {
        BlocksTestExecutionRecords.Clear();

        if (blocksTestExecutionRecords != null)
        {
            BlocksTestExecutionRecords.AddAll(blocksTestExecutionRecords);
        }
    }

    [Obsolete("Use Closed property instead")]
    public void SetClosed(bool? closed)
    {
        Closed = closed;
    }

    [Obsolete("Use CloseDate property instead")]
    public void SetCloseDate(DateTime? closeDate)
    {
        CloseDate = closeDate;
    }

    [Obsolete("Use Contributors property instead")]
    public void SetContributors(Uri[] contributors)
    {
        Contributors.Clear();

        if (contributors != null)
        {
            Contributors.AddAll(contributors);
        }
    }

    [Obsolete("Use Created property instead")]
    public void SetCreated(DateTime? created)
    {
        Created = created;
    }

    [Obsolete("Use Creators property instead")]
    public void SetCreators(Uri[] creators)
    {
        Creators.Clear();

        if (creators != null)
        {
            Creators.AddAll(creators);
        }
    }

    [Obsolete("Use DctermsTypes property instead")]
    public void SetDctermsTypes(string[] dctermsTypes)
    {
        DctermsTypes = dctermsTypes;
    }

    [Obsolete("Use Description property instead")]
    public void SetDescription(string description)
    {
        Description = description;
    }

    [Obsolete("Use DiscussedBy property instead")]
    public void SetDiscussedBy(Uri discussedBy)
    {
        DiscussedBy = discussedBy;
    }

    [Obsolete("Use Fixed property instead")]
    public void SetFixed(bool? isFixed)
    {
        Fixed = isFixed;
    }

    [Obsolete("Use Identifier property instead")]
    public void SetIdentifier(string identifier)
    {
        Identifier = identifier;
    }

    [Obsolete("Use ImplementsRequirements property instead")]
    public void SetImplementsRequirements(Link[] implementsRequirements)
    {
        ImplementsRequirements.Clear();

        if (implementsRequirements != null)
        {
            ImplementsRequirements.AddAll(implementsRequirements);
        }
    }

    [Obsolete("Use InProgress property instead")]
    public void SetInProgress(bool? inProgress)
    {
        InProgress = inProgress;
    }

    [Obsolete("Use InstanceShape property instead")]
    public void SetInstanceShape(Uri instanceShape)
    {
        InstanceShape = instanceShape;
    }

    [Obsolete("Use Modified property instead")]
    public void SetModified(DateTime? modified)
    {
        Modified = modified;
    }

    [Obsolete]
    public void SetRdfTypes(Uri[] rdfTypes)
    {
        SetTypes(rdfTypes);
    }

    [Obsolete("Use RelatedChangeRequests property instead")]
    public void SetRelatedChangeRequests(Link[] relatedChangeRequests)
    {
        RelatedChangeRequests.Clear();

        if (relatedChangeRequests != null)
        {
            RelatedChangeRequests.AddAll(relatedChangeRequests);
        }
    }

    [Obsolete("Use RelatedResources property instead")]
    public void SetRelatedResources(Link[] relatedResources)
    {
        RelatedResources.Clear();

        if (relatedResources != null)
        {
            RelatedResources.AddAll(relatedResources);
        }
    }

    [Obsolete("Use RelatedTestCases property instead")]
    public void SetRelatedTestCases(Link[] relatedTestCases)
    {
        RelatedTestCases.Clear();

        if (relatedTestCases != null)
        {
            RelatedTestCases.AddAll(relatedTestCases);
        }
    }

    [Obsolete("Use RelatedTestExecutionRecords property instead")]
    public void SetRelatedTestExecutionRecords(Link[] relatedTestExecutionRecords)
    {
        RelatedTestExecutionRecords.Clear();

        if (relatedTestExecutionRecords != null)
        {
            RelatedTestExecutionRecords.AddAll(relatedTestExecutionRecords);
        }
    }

    [Obsolete("Use RelatedTestPlans property instead")]
    public void SetRelatedTestPlans(Link[] relatedTestPlans)
    {
        RelatedTestPlans.Clear();

        if (relatedTestPlans != null)
        {
            RelatedTestPlans.AddAll(relatedTestPlans);
        }
    }

    [Obsolete("Use RelatedTestScripts property instead")]
    public void SetRelatedTestScripts(Link[] relatedTestScripts)
    {
        RelatedTestScripts.Clear();

        if (relatedTestScripts != null)
        {
            RelatedTestScripts.AddAll(relatedTestScripts);
        }
    }

    [Obsolete("Use Reviewed property instead")]
    public void SetReviewed(bool? reviewed)
    {
        Reviewed = reviewed;
    }

    [Obsolete("Use ServiceProvider property instead")]
    public void SetServiceProvider(Uri serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    [Obsolete("Use Severity property instead")]
    public void SetSeverity(string severity)
    {
        Severity = severity;
    }

    [Obsolete("Use ShortTitle property instead")]
    public void SetShortTitle(string shortTitle)
    {
        ShortTitle = shortTitle;
    }

    [Obsolete("Use Status property instead")]
    public void SetStatus(string status)
    {
        Status = status;
    }

    [Obsolete("Use Subjects property instead")]
    public void SetSubjects(string[] subjects)
    {
        Subjects.Clear();

        if (subjects != null)
        {
            Subjects.AddAll(subjects);
        }
    }

    [Obsolete("Use TestedByTestCases property instead")]
    public void SetTestedByTestCases(Link[] testedByTestCases)
    {
        TestedByTestCases.Clear();

        if (testedByTestCases != null)
        {
            TestedByTestCases.AddAll(testedByTestCases);
        }
    }

    [Obsolete("Use Title property instead")]
    public void SetTitle(string title)
    {
        Title = title;
    }

    [Obsolete("Use TracksChangeSets property instead")]
    public void SetTracksChangeSets(Link[] tracksChangeSets)
    {
        TracksChangeSets.Clear();

        if (tracksChangeSets != null)
        {
            TracksChangeSets.AddAll(tracksChangeSets);
        }
    }

    [Obsolete("Use TracksRequirements property instead")]
    public void SetTracksRequirements(Link[] tracksRequirements)
    {
        TracksRequirements.Clear();

        if (tracksRequirements != null)
        {
            TracksRequirements.AddAll(tracksRequirements);
        }
    }

    [Obsolete("Use Verified property instead")]
    public void SetVerified(bool? verified)
    {
        Verified = verified;
    }
}
