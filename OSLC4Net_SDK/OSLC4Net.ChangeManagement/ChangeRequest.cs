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
    private readonly HashSet<Link> _affectedByDefects = new();
    private readonly HashSet<Link> _affectsPlanItems = new();
    private readonly HashSet<Link> _affectsRequirements = new();
    private readonly HashSet<Link> _affectsTestResults = new();
    private readonly HashSet<Link> _blocksTestExecutionRecords = new();
    private readonly HashSet<Uri> _contributors = new(); // XXX - TreeSet<> in Java
    private readonly HashSet<Uri> _creators = new(); // XXX - TreeSet<> in Java
    private readonly HashSet<Type> _dctermsTypes = new(); // XXX - TreeSet<> in Java
    private readonly HashSet<Link> _implementsRequirements = new();
    private readonly HashSet<Link> _relatedChangeRequests = new();

    private readonly HashSet<Link>
        _relatedResources = new(); // TODO - Extension to point to any other OSLC resource(s).

    private readonly HashSet<Link> _relatedTestCases = new();
    private readonly HashSet<Link> _relatedTestExecutionRecords = new();
    private readonly HashSet<Link> _relatedTestPlans = new();
    private readonly HashSet<Link> _relatedTestScripts = new();
    private readonly HashSet<string> _subjects = new(); // XXX - TreeSet<> in Java
    private readonly HashSet<Link> _testedByTestCases = new();
    private readonly HashSet<Link> _tracksChangeSets = new();
    private readonly HashSet<Link> _tracksRequirements = new();

    private bool? _approved;
    private bool? _closed;
    private DateTime? _closeDate;
    private DateTime? _created;
    private string _description;
    private Uri _discussedBy;
    private bool? _isFixed;
    private string _identifier;
    private bool? _inProgress;
    private Uri _instanceShape;
    private DateTime? _modified;
    private bool? _reviewed;
    private Uri _serviceProvider;
    private Severity _severity = Severity.Unclassified; // TODO - Added severity for demo
    private string _shortTitle;
    private string _status;
    private string _title;
    private bool? _verified;

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
        _affectedByDefects.Add(affectedByDefect);
    }

    public void AddAffectsPlanItem(Link affectsPlanItem)
    {
        _affectsPlanItems.Add(affectsPlanItem);
    }

    public void AddAffectsRequirement(Link affectsRequirement)
    {
        _affectsRequirements.Add(affectsRequirement);
    }

    public void AddAffectsTestResult(Link affectsTestResult)
    {
        _affectsTestResults.Add(affectsTestResult);
    }

    public void AddBlocksTestExecutionRecord(Link blocksTestExecutionRecord)
    {
        _blocksTestExecutionRecords.Add(blocksTestExecutionRecord);
    }

    public void AddContributor(Uri contributor)
    {
        _contributors.Add(contributor);
    }

    public void AddCreator(Uri creator)
    {
        _creators.Add(creator);
    }

    public void AddDctermsType(string dctermsType)
    {
        _dctermsTypes.Add(TypeExtension.FromString(dctermsType));
    }

    public void AddImplementsRequirement(Link implementsRequirement)
    {
        _implementsRequirements.Add(implementsRequirement);
    }

    public void AddRdfType(Uri rdfType)
    {
        AddType(rdfType);
    }

    public void AddRelatedChangeRequest(Link relatedChangeRequest)
    {
        _relatedChangeRequests.Add(relatedChangeRequest);
    }

    public void AddRelatedResource(Link relatedResource)
    {
        _relatedResources.Add(relatedResource);
    }

    public void AddRelatedTestCase(Link relatedTestCase)
    {
        _relatedTestCases.Add(relatedTestCase);
    }

    public void AddRelatedTestExecutionRecord(Link relatedTestExecutionRecord)
    {
        _relatedTestExecutionRecords.Add(relatedTestExecutionRecord);
    }

    public void AddRelatedTestPlan(Link relatedTestPlan)
    {
        _relatedTestPlans.Add(relatedTestPlan);
    }

    public void AddRelatedTestScript(Link relatedTestScript)
    {
        _relatedTestScripts.Add(relatedTestScript);
    }

    public void AddSubject(string subject)
    {
        _subjects.Add(subject);
    }

    public void AddTestedByTestCase(Link testedByTestCase)
    {
        _testedByTestCases.Add(testedByTestCase);
    }

    public void AddTracksChangeSet(Link tracksChangeSet)
    {
        _tracksChangeSets.Add(tracksChangeSet);
    }

    public void AddTracksRequirement(Link tracksRequirement)
    {
        _tracksRequirements.Add(tracksRequirement);
    }

    [OslcDescription("Change request is affected by a reported defect.")]
    [OslcName("affectedByDefect")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectedByDefect")]
    [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
    [OslcReadOnly(false)]
    [OslcTitle("Affected By Defects")]
    public Link[] GetAffectedByDefects()
    {
        return _affectedByDefects.ToArray();
    }

    [OslcDescription("Change request affects a plan item. ")]
    [OslcName("affectsPlanItem")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsPlanItem")]
    [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
    [OslcReadOnly(false)]
    [OslcTitle("Affects Plan Items")]
    public Link[] GetAffectsPlanItems()
    {
        return _affectsPlanItems.ToArray();
    }

    [OslcDescription("Change request affecting a Requirement.")]
    [OslcName("affectsRequirement")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsRequirement")]
    [OslcRange(Constants.TYPE_REQUIREMENT)]
    [OslcReadOnly(false)]
    [OslcTitle("Affects Requirements")]
    public Link[] GetAffectsRequirements()
    {
        return _affectsRequirements.ToArray();
    }

    [OslcDescription("Associated QM resource that is affected by this Change Request.")]
    [OslcName("affectsTestResult")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "affectsTestResult")]
    [OslcRange(Constants.TYPE_TEST_RESULT)]
    [OslcReadOnly(false)]
    [OslcTitle("Affects Test Results")]
    public Link[] GetAffectsTestResults()
    {
        return _affectsTestResults.ToArray();
    }

    [OslcDescription("Associated QM resource that is blocked by this Change Request.")]
    [OslcName("blocksTestExecutionRecord")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "blocksTestExecutionRecord")]
    [OslcRange(Constants.TYPE_TEST_EXECUTION_RECORD)]
    [OslcReadOnly(false)]
    [OslcTitle("Blocks Test Execution Records")]
    public Link[] GetBlocksTestExecutionRecords()
    {
        return _blocksTestExecutionRecords.ToArray();
    }

    [OslcDescription("The date at which no further activity or work is intended to be conducted. ")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "closeDate")]
    [OslcReadOnly]
    [OslcTitle("Close DateTime?")]
    public DateTime? GetCloseDate()
    {
        return _closeDate;
    }

    [OslcDescription("The person(s) who are responsible for the work needed to complete the change request.")]
    [OslcName("contributor")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
    [OslcRange(Constants.TYPE_PERSON)]
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
    [OslcRange(Constants.TYPE_PERSON)]
    [OslcTitle("Creators")]
    public Uri[] GetCreators()
    {
        return _creators.ToArray();
    }

    [OslcAllowedValue(new string[] { "Defect", "Task", "Story", "Bug Report", "Feature Request" })]
    [OslcDescription("A short string representation for the type, example 'Defect'.")]
    [OslcName("type")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "type")]
    [OslcTitle("Types")]
    public string[] GetDctermsTypes()
    {
        var result = new string[_dctermsTypes.Count];

        var index = 0;

        foreach (var type in _dctermsTypes)
        {
            result[index++] = TypeExtension.ToString(type);
        }

        return result;
    }

    [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
    [OslcTitle("Description")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetDescription()
    {
        return _description;
    }

    [OslcDescription("A series of notes and comments about this change request.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "discussedBy")]
    [OslcRange(Constants.TYPE_DISCUSSION)]
    [OslcTitle("Discussed By")]
    public Uri GetDiscussedBy()
    {
        return _discussedBy;
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

    [OslcDescription("Implements associated Requirement.")]
    [OslcName("implementsRequirement")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "implementsRequirement")]
    [OslcRange(Constants.TYPE_REQUIREMENT)]
    [OslcReadOnly(false)]
    [OslcTitle("Implements Requirements")]
    public Link[] GetImplementsRequirements()
    {
        return _implementsRequirements.ToArray();
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

    // [OslcDescription("The resource type URIs.")]
    // [OslcName("type")]
    // [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
    // [OslcTitle("Types")]
    [Obsolete]
    public Uri[] GetRdfTypes()
    {
        return GetTypes().ToArray();
    }

    [OslcDescription("This relationship is loosely coupled and has no specific meaning.")]
    [OslcName("relatedChangeRequest")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedChangeRequest")]
    [OslcRange(Constants.TYPE_CHANGE_REQUEST)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Change Requests")]
    public Link[] GetRelatedChangeRequests()
    {
        return _relatedChangeRequests.ToArray();
    }

    [OslcDescription("Related OSLC resources of any type.")]
    [OslcName("relatedResource")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedResource")]
    [OslcTitle("Related Resources")]
    public Link[] GetRelatedResources()
    {
        return _relatedResources.ToArray();
    }

    [OslcDescription("Related QM test case resource.")]
    [OslcName("relatedTestCase")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestCase")]
    [OslcRange(Constants.TYPE_TEST_CASE)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Cases")]
    public Link[] GetRelatedTestCases()
    {
        return _relatedTestCases.ToArray();
    }

    [OslcDescription("Related to a QM test execution resource.")]
    [OslcName("relatedTestExecutionRecord")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestExecutionRecord")]
    [OslcRange(Constants.TYPE_TEST_EXECUTION_RECORD)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Execution Records")]
    public Link[] GetRelatedTestExecutionRecords()
    {
        return _relatedTestExecutionRecords.ToArray();
    }

    [OslcDescription("Related QM test plan resource.")]
    [OslcName("relatedTestPlan")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestPlan")]
    [OslcRange(Constants.TYPE_TEST_PLAN)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Plans")]
    public Link[] GetRelatedTestPlans()
    {
        return _relatedTestPlans.ToArray();
    }

    [OslcDescription("Related QM test script resource.")]
    [OslcName("relatedTestScript")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "relatedTestScript")]
    [OslcRange(Constants.TYPE_TEST_SCRIPT)]
    [OslcReadOnly(false)]
    [OslcTitle("Related Test Scripts")]
    public Link[] GetRelatedTestScripts()
    {
        return _relatedTestScripts.ToArray();
    }

    [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
    [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
    [OslcTitle("Service Provider")]
    public Uri GetServiceProvider()
    {
        return _serviceProvider;
    }

    [OslcAllowedValue(new string[] { "Unclassified", "Minor", "Normal", "Major", "Critical", "Blocker" })]
    [OslcDescription("Severity of change request.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "severity")]
    [OslcTitle("Severity")]
    public string GetSeverity()
    {
        return _severity.ToString();
    }

    [OslcDescription("Short name identifying a resource, often used as an abbreviated identifier for presentation to end-users.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "shortTitle")]
    [OslcTitle("Short Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetShortTitle()
    {
        return _shortTitle;
    }

    [OslcDescription("Used to indicate the status of the change request based on values defined by the service provider. Most often a read-only property. Some possible values may include: 'Submitted', 'Done', 'InProgress', etc.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "status")]
    [OslcTitle("Status")]
    public string GetStatus()
    {
        return _status;
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

    [OslcDescription("Test case by which this change request is tested.")]
    [OslcName("testedByTestCase")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "testedByTestCase")]
    [OslcRange(Constants.TYPE_TEST_CASE)]
    [OslcReadOnly(false)]
    [OslcTitle("Tested by Test Cases")]
    public Link[] GetTestedByTestCases()
    {
        return _testedByTestCases.ToArray();
    }

    [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle()
    {
        return _title;
    }

    [OslcDescription("Tracks SCM change set resource.")]
    [OslcName("tracksChangeSet")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "tracksChangeSet")]
    [OslcRange(Constants.TYPE_CHANGE_SET)]
    [OslcReadOnly(false)]
    [OslcTitle("Tracks Change Sets")]
    public Link[] GetTracksChangeSets()
    {
        return _tracksChangeSets.ToArray();
    }

    [OslcDescription("Tracks the associated Requirement or Requirement ChangeSet resources.")]
    [OslcName("tracksRequirement")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "tracksRequirement")]
    [OslcRange(Constants.TYPE_REQUIREMENT)]
    [OslcReadOnly(false)]
    [OslcTitle("Tracks Requirements")]
    public Link[] GetTracksRequirements()
    {
        return _tracksRequirements.ToArray();
    }

    [OslcDescription("Whether or not the Change Request has been approved.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "approved")]
    [OslcReadOnly]
    [OslcTitle("Approved")]
    public bool? IsApproved()
    {
        return _approved;
    }

    [OslcDescription("Whether or not the Change Request is completely done, no further fixes or fix verification is needed.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "closed")]
    [OslcReadOnly]
    [OslcTitle("Closed")]
    public bool? IsClosed()
    {
        return _closed;
    }

    [OslcDescription("Whether or not the Change Request has been fixed.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "fixed")]
    [OslcReadOnly]
    [OslcTitle("Fixed")]
    public bool? IsFixed()
    {
        return _isFixed;
    }

    [OslcDescription("Whether or not the Change Request in a state indicating that active work is occurring. If oslc_cm:inprogress is true, then oslc_cm:fixed and oslc_cm:closed must also be false.")]
    [OslcName("inprogress")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "inprogress")]
    [OslcReadOnly]
    [OslcTitle("In] Progress")]
    public bool? IsInProgress()
    {
        return _inProgress;
    }

    [OslcDescription("Whether or not the Change Request has been reviewed.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "reviewed")]
    [OslcReadOnly]
    [OslcTitle("Reviewed")]
    public bool? IsReviewed()
    {
        return _reviewed;
    }

    [OslcDescription("Whether or not the resolution or fix of the Change Request has been verified.")]
    [OslcPropertyDefinition(Constants.CHANGE_MANAGEMENT_NAMESPACE + "verified")]
    [OslcReadOnly]
    [OslcTitle("Verified")]
    public bool? IsVerified()
    {
        return _verified;
    }

    public void SetAffectedByDefects(Link[] affectedByDefects)
    {
        _affectedByDefects.Clear();

        if (affectedByDefects != null)
        {
            _affectedByDefects.AddAll(affectedByDefects);
        }
    }

    public void SetAffectsPlanItems(Link[] affectsPlanItems)
    {
        _affectsPlanItems.Clear();

        if (affectsPlanItems != null)
        {
            _affectsPlanItems.AddAll(affectsPlanItems);
        }
    }

    public void SetAffectsRequirements(Link[] affectsRequirements)
    {
        _affectsRequirements.Clear();

        if (affectsRequirements != null)
        {
            _affectsRequirements.AddAll(affectsRequirements);
        }
    }

    public void SetAffectsTestResults(Link[] affectsTestResults)
    {
        _affectsTestResults.Clear();

        if (affectsTestResults != null)
        {
            _affectsTestResults.AddAll(affectsTestResults);
        }
    }

    public void SetApproved(bool? approved)
    {
        _approved = approved;
    }

    public void SetBlocksTestExecutionRecords(Link[] blocksTestExecutionRecords)
    {
        _blocksTestExecutionRecords.Clear();

        if (blocksTestExecutionRecords != null)
        {
            _blocksTestExecutionRecords.AddAll(blocksTestExecutionRecords);
        }
    }

    public void SetClosed(bool? closed)
    {
        _closed = closed;
    }

    public void SetCloseDate(DateTime? closeDate)
    {
        _closeDate = closeDate;
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

    public void SetDctermsTypes(string[] dctermsTypes)
    {
        _dctermsTypes.Clear();

        if (dctermsTypes != null)
        {
            foreach (var type in dctermsTypes)
            {
                _dctermsTypes.Add(TypeExtension.FromString(type));
            }
        }
    }

    public void SetDescription(string description)
    {
        _description = description;
    }

    public void SetDiscussedBy(Uri discussedBy)
    {
        _discussedBy = discussedBy;
    }

    public void SetFixed(bool? isFixed)
    {
        _isFixed = isFixed;
    }

    public void SetIdentifier(string identifier)
    {
        _identifier = identifier;
    }

    public void SetImplementsRequirements(Link[] implementsRequirements)
    {
        _implementsRequirements.Clear();

        if (implementsRequirements != null)
        {
            _implementsRequirements.AddAll(implementsRequirements);
        }
    }

    public void SetInProgress(bool? inProgress)
    {
        _inProgress = inProgress;
    }

    public void SetInstanceShape(Uri instanceShape)
    {
        _instanceShape = instanceShape;
    }

    public void SetModified(DateTime? modified)
    {
        _modified = modified;
    }

    [Obsolete]
    public void SetRdfTypes(Uri[] rdfTypes)
    {
        SetTypes(rdfTypes);
    }

    public void SetRelatedChangeRequests(Link[] relatedChangeRequests)
    {
        _relatedChangeRequests.Clear();

        if (relatedChangeRequests != null)
        {
            _relatedChangeRequests.AddAll(relatedChangeRequests);
        }
    }

    public void SetRelatedResources(Link[] relatedResources)
    {
        _relatedResources.Clear();

        if (relatedResources != null)
        {
            _relatedResources.AddAll(relatedResources);
        }
    }

    public void SetRelatedTestCases(Link[] relatedTestCases)
    {
        _relatedTestCases.Clear();

        if (relatedTestCases != null)
        {
            _relatedTestCases.AddAll(relatedTestCases);
        }
    }

    public void SetRelatedTestExecutionRecords(Link[] relatedTestExecutionRecords)
    {
        _relatedTestExecutionRecords.Clear();

        if (relatedTestExecutionRecords != null)
        {
            _relatedTestExecutionRecords.AddAll(relatedTestExecutionRecords);
        }
    }

    public void SetRelatedTestPlans(Link[] relatedTestPlans)
    {
        _relatedTestPlans.Clear();

        if (relatedTestPlans != null)
        {
            _relatedTestPlans.AddAll(relatedTestPlans);
        }
    }

    public void SetRelatedTestScripts(Link[] relatedTestScripts)
    {
        _relatedTestScripts.Clear();

        if (relatedTestScripts != null)
        {
            _relatedTestScripts.AddAll(relatedTestScripts);
        }
    }

    public void SetReviewed(bool? reviewed)
    {
        _reviewed = reviewed;
    }

    public void SetServiceProvider(Uri serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetSeverity(string severity)
    {
        _severity = SeverityExtension.FromString(severity);
    }

    public void SetShortTitle(string shortTitle)
    {
        _shortTitle = shortTitle;
    }

    public void SetStatus(string status)
    {
        _status = status;
    }

    public void SetSubjects(string[] subjects)
    {
        _subjects.Clear();

        if (subjects != null)
        {
            _subjects.AddAll(subjects);
        }
    }

    public void SetTestedByTestCases(Link[] testedByTestCases)
    {
        _testedByTestCases.Clear();

        if (testedByTestCases != null)
        {
            _testedByTestCases.AddAll(testedByTestCases);
        }
    }

    public void SetTitle(string title)
    {
        _title = title;
    }

    public void SetTracksChangeSets(Link[] tracksChangeSets)
    {
        _tracksChangeSets.Clear();

        if (tracksChangeSets != null)
        {
            _tracksChangeSets.AddAll(tracksChangeSets);
        }
    }

    public void SetTracksRequirements(Link[] tracksRequirements)
    {
        _tracksRequirements.Clear();

        if (tracksRequirements != null)
        {
            _tracksRequirements.AddAll(tracksRequirements);
        }
    }

    public void SetVerified(bool? verified)
    {
        _verified = verified;
    }
}
