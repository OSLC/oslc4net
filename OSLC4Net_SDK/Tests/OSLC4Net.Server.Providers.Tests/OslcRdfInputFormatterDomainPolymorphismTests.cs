/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using CM = OSLC4Net.Domains.ChangeManagement;
using Config = OSLC4Net.Domains.ConfigurationManagement;
using TRS = OSLC4Net.Domains.TrackedResourceSet;

namespace OSLC4Net.Server.Providers.Tests;

public sealed class OslcRdfInputFormatterDomainPolymorphismTests
{
    private const string ChangeManagementDefect =
        """
        @prefix dcterms: <http://purl.org/dc/terms/> .
        @prefix oslc: <http://open-services.net/ns/core#> .
        @prefix oslc_cm: <http://open-services.net/ns/cm#> .

        <http://example.com/bugs/2314>
           a oslc_cm:Defect ;
           dcterms:identifier "00002314" ;
           oslc:shortTitle "Bug 2314" ;
           dcterms:title "Invalid installation instructions" ;
           oslc_cm:priority oslc_cm:High ;
           oslc_cm:severity <http://example.com/enums#S1> .
        """;

    private const string ConfigurationBaseline =
        """
        @prefix dcterms: <http://purl.org/dc/terms/> .
        @prefix oslc_config: <http://open-services.net/ns/config#> .

        <http://example.com/configurations/baseline-1>
           a oslc_config:Baseline ;
           dcterms:title "Release baseline" .
        """;

    private const string TrackedResourceSetChangeLog =
        """
        @prefix trs: <http://open-services.net/ns/core/trs#> .
        @prefix xsd: <http://www.w3.org/2001/XMLSchema#> .

        <http://cm1.example.com/trackedResourceSet>
          a trs:TrackedResourceSet ;
          trs:base <http://cm1.example.com/baseResources/> ;
          trs:changeLog [
            a trs:ChangeLog ;
            trs:change <urn:example:cm1.example.com:2010-10-27T17:39:33.000Z:103> ;
            trs:change <urn:example:cm1.example.com:2010-10-27T17:39:32.000Z:102> ;
            trs:change <urn:example:cm1.example.com:2010-10-27T17:39:31.000Z:101>
          ] .

        <urn:example:cm1.example.com:2010-10-27T17:39:33.000Z:103>
          a trs:Creation ;
          trs:changed <http://cm1.example.com/bugs/23> ;
          trs:order "103"^^xsd:integer .

        <urn:example:cm1.example.com:2010-10-27T17:39:32.000Z:102>
          a trs:Modification ;
          trs:changed <http://cm1.example.com/bugs/22> ;
          trs:order "102"^^xsd:integer .

        <urn:example:cm1.example.com:2010-10-27T17:39:31.000Z:101>
          a trs:Deletion ;
          trs:changed <http://cm1.example.com/bugs/21> ;
          trs:order "101"^^xsd:integer .
        """;

    [Test]
    public async Task ChangeManagementDefectResolvesAsChangeRequestSubtype()
    {
        GC.KeepAlive(typeof(CM.Defect));
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        PolymorphicDomainResult result = await PostForResult(
            client,
            "/domain-polymorphism/change-management",
            ChangeManagementDefect);

        await Assert.That(result.TypeNames).IsEquivalentTo([nameof(CM.Defect)]);
        await Assert.That(result.Abouts).IsEquivalentTo(["http://example.com/bugs/2314"]);
        await Assert.That(result.RdfTypes).Contains("http://open-services.net/ns/cm#Defect");
    }

    [Test]
    public async Task ConfigurationBaselineResolvesFromBaselineType()
    {
        GC.KeepAlive(typeof(Config.Baseline));
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        PolymorphicDomainResult result = await PostForResult(
            client,
            "/domain-polymorphism/configuration-management",
            ConfigurationBaseline);

        await Assert.That(result.TypeNames).IsEquivalentTo([nameof(Config.Baseline)]);
        await Assert.That(result.Abouts).IsEquivalentTo(["http://example.com/configurations/baseline-1"]);
        await Assert.That(result.RdfTypes).Contains("http://open-services.net/ns/config#Baseline");
    }

    [Test]
    public async Task TrackedResourceSetChangeLogResolvesTrackedSetAndChangeEvents()
    {
        GC.KeepAlive(typeof(TRS.TrackedResourceSetResource));
        GC.KeepAlive(typeof(TRS.CreationEvent));
        GC.KeepAlive(typeof(TRS.ModificationEvent));
        GC.KeepAlive(typeof(TRS.DeletionEvent));
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        PolymorphicDomainResult result = await PostForResult(
            client,
            "/domain-polymorphism/trs",
            TrackedResourceSetChangeLog);

        await Assert.That(result.TypeNames).IsEquivalentTo([
            nameof(TRS.CreationEvent),
            nameof(TRS.DeletionEvent),
            nameof(TRS.ModificationEvent),
            nameof(TRS.TrackedResourceSetResource),
        ]);
        await Assert.That(result.RdfTypes).Contains("http://open-services.net/ns/core/trs#TrackedResourceSet");
        await Assert.That(result.RdfTypes).Contains("http://open-services.net/ns/core/trs#Creation");
        await Assert.That(result.RdfTypes).Contains("http://open-services.net/ns/core/trs#Modification");
        await Assert.That(result.RdfTypes).Contains("http://open-services.net/ns/core/trs#Deletion");
    }

    private static TestServer CreateServer()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(static services =>
            {
                services.AddControllers(options =>
                    {
                        options.InputFormatters.Insert(0, new OslcRdfInputFormatter());
                    })
                    .AddApplicationPart(typeof(DomainPolymorphismController).Assembly);
            })
            .Configure(static app =>
            {
                app.UseRouting();
                app.UseEndpoints(static endpoints => endpoints.MapControllers());
            });

        return new TestServer(builder)
        {
            AllowSynchronousIO = true,
        };
    }

    private static async Task<PolymorphicDomainResult> PostForResult(
        HttpClient client,
        string requestUri,
        string turtle)
    {
        using HttpResponseMessage response = await client.PostAsync(
            requestUri,
            new StringContent(turtle, Encoding.UTF8, OslcMediaType.TEXT_TURTLE));

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        return await response.Content.ReadFromJsonAsync<PolymorphicDomainResult>() ??
            throw new InvalidOperationException("The response did not contain a polymorphic domain result.");
    }
}

[ApiController]
[Route("domain-polymorphism")]
public sealed class DomainPolymorphismController : ControllerBase
{
    [HttpPost("change-management")]
    [Consumes(OslcMediaType.TEXT_TURTLE)]
    public IActionResult ChangeManagement([FromBody] OslcRequest<CM.IChangeRequest> request)
    {
        return Ok(PolymorphicDomainResult.From(request.Resources, request.Graph));
    }

    [HttpPost("configuration-management")]
    [Consumes(OslcMediaType.TEXT_TURTLE)]
    public IActionResult ConfigurationManagement([FromBody] OslcRequest<IExtendedResource> request)
    {
        return Ok(PolymorphicDomainResult.From(request.Resources, request.Graph));
    }

    [HttpPost("trs")]
    [Consumes(OslcMediaType.TEXT_TURTLE)]
    public IActionResult TrackedResourceSet([FromBody] OslcRequest<IExtendedResource> request)
    {
        return Ok(PolymorphicDomainResult.From(request.Resources, request.Graph));
    }
}

public sealed record PolymorphicDomainResult(
    string[] TypeNames,
    string[] Abouts,
    string[] RdfTypes,
    int TripleCount)
{
    public static PolymorphicDomainResult From(IEnumerable<IResource> resources, IGraph graph)
    {
        return new PolymorphicDomainResult(
            resources.Select(static resource => resource.GetType().Name).Order(StringComparer.Ordinal).ToArray(),
            resources.Select(static resource => resource.About?.AbsoluteUri ?? string.Empty).Order(StringComparer.Ordinal).ToArray(),
            ExtractRdfTypes(graph).Order(StringComparer.Ordinal).ToArray(),
            graph.Triples.Count);
    }

    private static IEnumerable<string> ExtractRdfTypes(IGraph graph)
    {
        IUriNode rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        return graph.GetTriplesWithPredicate(rdfType)
            .Select(static triple => triple.Object)
            .OfType<IUriNode>()
            .Select(static node => node.Uri.AbsoluteUri)
            .Distinct(StringComparer.Ordinal);
    }
}
