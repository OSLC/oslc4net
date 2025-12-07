using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class ChangeRequestTests
{
    [Test]
    public async Task VerifyChangeRequest_WithGettersSetters()
    {
        var changeRequest = new ChangeRequest(new Uri("http://example.com/changeRequests/1"));

        changeRequest.SetTitle("Sample Change Request");
        changeRequest.SetDescription("This is a sample change request.");
        changeRequest.SetIdentifier("101");
        changeRequest.SetStatus("Submitted");
        changeRequest.SetSeverity("Normal");
        changeRequest.SetCreated(new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Utc));
        changeRequest.SetFixed(false);
        changeRequest.SetApproved(true);

        changeRequest.AddSubject("Subject 1");
        changeRequest.AddSubject("Subject 2");

        changeRequest.AddAffectedByDefect(new Link(new Uri("http://example.com/defects/1"), "Defect 1"));

        await Verify(changeRequest);
    }

    [Test]
    public async Task TestLegacySettersRoundTrip()
    {
        // 1. Instantiate and populate using legacy setters
        var changeRequest = new ChangeRequest(new Uri("http://example.com/changeRequests/roundtrip"));
        var now = new DateTime(2023, 11, 15, 12, 0, 0, DateTimeKind.Utc);

        changeRequest.SetTitle("Round Trip Test");
        changeRequest.SetDescription("Testing legacy setters round trip.");
        changeRequest.SetIdentifier("RT-001");
        changeRequest.SetStatus("InProgress");
        changeRequest.SetSeverity("Critical");
        changeRequest.SetCreated(now);
        changeRequest.SetModified(now.AddHours(1));
        changeRequest.SetApproved(true);
        changeRequest.SetClosed(false);
        changeRequest.SetFixed(false);
        changeRequest.SetInProgress(true);
        changeRequest.SetReviewed(true);
        changeRequest.SetVerified(false);
        changeRequest.SetShortTitle("RT Test");

        changeRequest.SetSubjects(new[] { "Tag1", "Tag2" });
        changeRequest.SetDctermsTypes(new[] { "Defect" });

        var contributor = new Uri("http://example.com/users/contributor");
        changeRequest.SetContributors(new[] { contributor });

        var creator = new Uri("http://example.com/users/creator");
        changeRequest.SetCreators(new[] { creator });

        var serviceProvider = new Uri("http://example.com/serviceProvider");
        changeRequest.SetServiceProvider(serviceProvider);

        var instanceShape = new Uri("http://example.com/shapes/changeRequest");
        changeRequest.SetInstanceShape(instanceShape);

        var discussedBy = new Uri("http://example.com/discussion");
        changeRequest.SetDiscussedBy(discussedBy);

        // Links
        var defectLink = new Link(new Uri("http://example.com/defects/1"), "Defect 1");
        changeRequest.SetAffectedByDefects(new[] { defectLink });

        var planItemLink = new Link(new Uri("http://example.com/plans/1"), "Plan Item 1");
        changeRequest.SetAffectsPlanItems(new[] { planItemLink });

        var requirementLink = new Link(new Uri("http://example.com/reqs/1"), "Req 1");
        changeRequest.SetAffectsRequirements(new[] { requirementLink });

        var testResultLink = new Link(new Uri("http://example.com/results/1"), "Result 1");
        changeRequest.SetAffectsTestResults(new[] { testResultLink });

        var execRecordLink = new Link(new Uri("http://example.com/execs/1"), "Exec 1");
        changeRequest.SetBlocksTestExecutionRecords(new[] { execRecordLink });

        var implementsReqLink = new Link(new Uri("http://example.com/reqs/2"), "Req 2");
        changeRequest.SetImplementsRequirements(new[] { implementsReqLink });

        var relatedCRLink = new Link(new Uri("http://example.com/crs/2"), "CR 2");
        changeRequest.SetRelatedChangeRequests(new[] { relatedCRLink });

        var relatedResLink = new Link(new Uri("http://example.com/res/1"), "Res 1");
        changeRequest.SetRelatedResources(new[] { relatedResLink });

        var testCaseLink = new Link(new Uri("http://example.com/cases/1"), "Case 1");
        changeRequest.SetRelatedTestCases(new[] { testCaseLink });

        var relatedExecLink = new Link(new Uri("http://example.com/execs/2"), "Exec 2");
        changeRequest.SetRelatedTestExecutionRecords(new[] { relatedExecLink });

        var testPlanLink = new Link(new Uri("http://example.com/plans/2"), "Plan 2");
        changeRequest.SetRelatedTestPlans(new[] { testPlanLink });

        var testScriptLink = new Link(new Uri("http://example.com/scripts/1"), "Script 1");
        changeRequest.SetRelatedTestScripts(new[] { testScriptLink });

        var testedByLink = new Link(new Uri("http://example.com/cases/2"), "Case 2");
        changeRequest.SetTestedByTestCases(new[] { testedByLink });

        var changeSetLink = new Link(new Uri("http://example.com/changesets/1"), "ChangeSet 1");
        changeRequest.SetTracksChangeSets(new[] { changeSetLink });

        var tracksReqLink = new Link(new Uri("http://example.com/reqs/3"), "Req 3");
        changeRequest.SetTracksRequirements(new[] { tracksReqLink });

        // 2. Serialize
        var formatter = new RdfXmlMediaTypeFormatter();
        var rdfXml = await RdfHelpers.SerializeAsync(formatter, changeRequest, OslcMediaType.APPLICATION_RDF_XML_TYPE);

        // 3. Deserialize
        var deserialized = await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, rdfXml, OslcMediaType.APPLICATION_RDF_XML_TYPE);

        // 4. Assert properties on deserialized object
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized.Title).IsEqualTo("Round Trip Test");
        await Assert.That(deserialized.Description).IsEqualTo("Testing legacy setters round trip.");
        await Assert.That(deserialized.Identifier).IsEqualTo("RT-001");
        await Assert.That(deserialized.Status).IsEqualTo("InProgress");
        await Assert.That(deserialized.Severity).IsEqualTo("Critical");
        await Assert.That(deserialized.Created).IsEqualTo(now);
        await Assert.That(deserialized.Modified).IsEqualTo(now.AddHours(1));
        await Assert.That(deserialized.Approved).IsTrue();
        await Assert.That(deserialized.Closed).IsFalse();
        await Assert.That(deserialized.Fixed).IsFalse();
        await Assert.That(deserialized.InProgress).IsTrue();
        await Assert.That(deserialized.Reviewed).IsTrue();
        await Assert.That(deserialized.Verified).IsFalse();
        await Assert.That(deserialized.ShortTitle).IsEqualTo("RT Test");

        await Assert.That(deserialized.Subjects).Contains("Tag1");
        await Assert.That(deserialized.Subjects).Contains("Tag2");

        await Assert.That(deserialized.DctermsTypes).Contains("Defect");

        await Assert.That(deserialized.Contributors).Contains(contributor);
        await Assert.That(deserialized.Creators).Contains(creator);
        await Assert.That(deserialized.ServiceProvider).IsEqualTo(serviceProvider);
        await Assert.That(deserialized.InstanceShape).IsEqualTo(instanceShape);
        await Assert.That(deserialized.DiscussedBy).IsEqualTo(discussedBy);

        // Check links (checking count and existence of specific URIs)
        await Assert.That(deserialized.AffectedByDefects.Any(l => l.GetValue().Equals(defectLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.AffectsPlanItems.Any(l => l.GetValue().Equals(planItemLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.AffectsRequirements.Any(l => l.GetValue().Equals(requirementLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.AffectsTestResults.Any(l => l.GetValue().Equals(testResultLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.BlocksTestExecutionRecords.Any(l => l.GetValue().Equals(execRecordLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.ImplementsRequirements.Any(l => l.GetValue().Equals(implementsReqLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.RelatedChangeRequests.Any(l => l.GetValue().Equals(relatedCRLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.RelatedResources.Any(l => l.GetValue().Equals(relatedResLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.RelatedTestCases.Any(l => l.GetValue().Equals(testCaseLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.RelatedTestExecutionRecords.Any(l => l.GetValue().Equals(relatedExecLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.RelatedTestPlans.Any(l => l.GetValue().Equals(testPlanLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.RelatedTestScripts.Any(l => l.GetValue().Equals(testScriptLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.TestedByTestCases.Any(l => l.GetValue().Equals(testedByLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.TracksChangeSets.Any(l => l.GetValue().Equals(changeSetLink.GetValue()))).IsTrue();
        await Assert.That(deserialized.TracksRequirements.Any(l => l.GetValue().Equals(tracksReqLink.GetValue()))).IsTrue();
    }
}
