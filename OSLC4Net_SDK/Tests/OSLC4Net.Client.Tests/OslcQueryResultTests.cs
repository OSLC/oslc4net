/*******************************************************************************
 * Copyright (c) 2024 Andrii Berezovskyi and oslc4net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 *******************************************************************************/

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Oslc.Resources;

public class OslcQueryResultTests
{
    private IHost AppHost { get; }
    private ILoggerFactory LoggerFactory { get; }

    public OslcQueryResultTests()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(
                builder =>
                {
                    builder.AddConsole();
                }).Build();
        LoggerFactory = AppHost.Services.GetRequiredService<ILoggerFactory>();
    }

    [Test]
    public async Task OslcQueryMultiResponseTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        await Assert.That(oslcQueryResult.GetMembersUrls().Length).IsEqualTo(20);
    }

    [Test, Skip("TODO: figure out if .Current is supposed to work only if the next page is present")]
    public async Task OslcQueryMultiResponsePagingTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();
        await Assert.That(oslcQueryResult.Current?.GetMembersUrls().Length).IsEqualTo(20);
    }

    [Test]
    public async Task OslcQueryMultiResponseIterTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        // oslcQueryResult.
        foreach (var resultItem in oslcQueryResult.GetMembers<ChangeRequest>())
        {
            Console.WriteLine(resultItem.GetAbout().ToString());
        }

        Console.WriteLine($"Total: {oslcQueryResult.TotalCount}");

        await Assert.That(oslcQueryResult.GetMembersUrls().Length).IsEqualTo(20);
    }

    [Test]
    public async Task ExplicitMembershipPredicateIsRecognizedWithoutGuessing()
    {
        const string capabilityUrl = "https://example.test/qm/testcases";
        const string memberUrl = "https://example.test/qm/testcases/1";
        var responseText = """
            <?xml version="1.0" encoding="UTF-8"?>
            <rdf:RDF
                xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
                xmlns:dcterms="http://purl.org/dc/terms/"
                xmlns:oslc="http://open-services.net/ns/core#"
                xmlns:foaf="http://xmlns.com/foaf/0.1/"
                xmlns:oslc_qm="http://open-services.net/ns/qm#">
              <rdf:Description rdf:about="https://example.test/qm/testcases?oslc.where=true">
                <rdf:type rdf:resource="http://open-services.net/ns/core#ResponseInfo" />
                <oslc:totalCount>1</oslc:totalCount>
              </rdf:Description>
              <rdf:Description rdf:about="https://example.test/qm/testcases">
                <rdf:type rdf:resource="http://open-services.net/ns/qm#TestCaseQuery" />
                <foaf:maker rdf:resource="https://example.test/users/not-a-member" />
                <oslc_qm:testCase rdf:resource="https://example.test/qm/testcases/1" />
              </rdf:Description>
              <rdf:Description rdf:about="https://example.test/qm/testcases/1">
                <rdf:type rdf:resource="http://open-services.net/ns/qm#TestCase" />
                <dcterms:title>Flight test</dcterms:title>
              </rdf:Description>
            </rdf:RDF>
            """;
        var testQuery = new OslcQuery(
            new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            capabilityUrl,
            0,
            new OslcQueryParameters(),
            new Uri(OSLCConstants.QM_TEST_CASE));
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var result = new OslcQueryResult(testQuery, response);

        await Assert.That(result.GetMembersUrls()).IsEquivalentTo([memberUrl]);
        var member = result.GetMembers<AnyResource>().Single();
        await Assert.That(member.GetExtendedProperties().Keys.Any(key =>
            key.NamespaceUri == "http://purl.org/dc/terms/" && key.LocalPart == "title")).IsTrue();
    }

    [Test]
    public async Task LdpHasMemberRelationAndMembershipResourceAreRecognized()
    {
        const string capabilityUrl = "https://example.test/qm/testcases";
        const string memberUrl = "https://example.test/qm/testcases/1";
        var responseText = """
            <?xml version="1.0" encoding="UTF-8"?>
            <rdf:RDF
                xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
                xmlns:ldp="http://www.w3.org/ns/ldp#"
                xmlns:oslc_qm="http://open-services.net/ns/qm#">
              <rdf:Description rdf:about="https://example.test/qm/testcases">
                <rdf:type rdf:resource="http://www.w3.org/ns/ldp#DirectContainer" />
                <ldp:membershipResource rdf:resource="https://example.test/qm/testcase-members" />
                <ldp:hasMemberRelation rdf:resource="http://open-services.net/ns/qm#testCase" />
                <ldp:contains rdf:resource="https://example.test/qm/not-a-query-result" />
              </rdf:Description>
              <rdf:Description rdf:about="https://example.test/qm/testcase-members">
                <oslc_qm:testCase rdf:resource="https://example.test/qm/testcases/1" />
              </rdf:Description>
            </rdf:RDF>
            """;
        var testQuery = new OslcQuery(
            new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            capabilityUrl,
            0,
            new OslcQueryParameters(),
            new Uri("http://xmlns.com/foaf/0.1/maker"));
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var result = new OslcQueryResult(testQuery, response);

        await Assert.That(result.GetMembersUrls()).IsEquivalentTo([memberUrl]);
    }

    [Test]
    public async Task LdpContainsIsRecognizedAsAStandardMembershipPredicate()
    {
        const string capabilityUrl = "https://example.test/qm/testcases";
        const string memberUrl = "https://example.test/qm/testcases/1";
        var responseText = """
            <?xml version="1.0" encoding="UTF-8"?>
            <rdf:RDF
                xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
                xmlns:ldp="http://www.w3.org/ns/ldp#">
              <rdf:Description rdf:about="https://example.test/qm/testcases">
                <rdf:type rdf:resource="http://www.w3.org/ns/ldp#BasicContainer" />
                <ldp:contains rdf:resource="https://example.test/qm/testcases/1" />
              </rdf:Description>
            </rdf:RDF>
            """;
        var testQuery = new OslcQuery(
            new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            capabilityUrl);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var result = new OslcQueryResult(testQuery, response);

        await Assert.That(result.GetMembersUrls()).IsEquivalentTo([memberUrl]);
    }

    [Test]
    public async Task ResponseInfoReturnsMembersAndLiteralTotalCount()
    {
        const string capabilityUrl =
            "https://example.test/oslc/workitems/query/release";
        const string memberUrl =
            "https://example.test/oslc/workitems/REL-1";
        var responseText = """
            <?xml version="1.0" encoding="UTF-8"?>
            <rdf:RDF
                xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
                xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#"
                xmlns:oslc="http://open-services.net/ns/core#"
                xmlns:xsd="http://www.w3.org/2001/XMLSchema#">
              <oslc:ResponseInfo rdf:about="https://example.test/oslc/workitems/query/release">
                <rdfs:member rdf:resource="https://example.test/oslc/workitems/REL-1" />
                <oslc:totalCount rdf:datatype="http://www.w3.org/2001/XMLSchema#int">1</oslc:totalCount>
              </oslc:ResponseInfo>
            </rdf:RDF>
            """;
        var testQuery = new OslcQuery(
            new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            capabilityUrl);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var result = new OslcQueryResult(testQuery, response);

        await Assert.That(result.GetMembersUrls()).IsEquivalentTo([memberUrl]);
        await Assert.That(result.TotalCount).IsEqualTo(1);
    }

    [Test]
    public async Task UndeclaredDomainPredicateIsNotGuessed()
    {
        const string capabilityUrl = "https://example.test/qm/testcases";
        var responseText = """
            <?xml version="1.0" encoding="UTF-8"?>
            <rdf:RDF
                xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
                xmlns:oslc_qm="http://open-services.net/ns/qm#">
              <rdf:Description rdf:about="https://example.test/qm/testcases">
                <rdf:type rdf:resource="http://open-services.net/ns/qm#TestCaseQuery" />
                <oslc_qm:testCase rdf:resource="https://example.test/qm/testcases/1" />
              </rdf:Description>
            </rdf:RDF>
            """;
        var testQuery = new OslcQuery(
            new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            capabilityUrl);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var result = new OslcQueryResult(testQuery, response);

        await Assert.That(result.GetMembersUrls()).IsEmpty();
    }

    private async Task<OslcQueryResult> GetMockOslcQueryResultMulti()
    {
        var responseText = await File.ReadAllTextAsync("data/multiResponseQuery.rdf").ConfigureAwait(false);
        var testQuery = new OslcQuery(new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            "https://nordic.clm.ibmcloud.com/ccm/oslc/contexts/_2nC4UBNvEeutmoeSPr3-Ag/workitems");
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var oslcQueryResult = new OslcQueryResult(testQuery, httpResponseMessage);
        return oslcQueryResult;
    }
}
