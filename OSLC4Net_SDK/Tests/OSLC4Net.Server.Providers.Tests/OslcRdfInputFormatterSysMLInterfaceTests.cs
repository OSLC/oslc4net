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
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using OSLC4Net.Domains.SysMLV2;

namespace OSLC4Net.Server.Providers.Tests;

public sealed class OslcRdfInputFormatterSysMLInterfaceTests
{
    private const string Turtle =
        """
        @prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
        @prefix sysml: <https://www.omg.org/spec/sysml/vocabulary#> .

        <http://example.org/element> rdf:type sysml:Element .
        <http://example.org/relationship> rdf:type sysml:Relationship .
        """;

    private const string MultipleTypesTurtle =
        """
        @prefix ex: <http://example.org/vocab#> .
        @prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .

        <http://example.org/derived> rdf:type ex:Base, ex:Derived .
        """;

    private const string AmbiguousTypesTurtle =
        """
        @prefix ex: <http://example.org/vocab#> .
        @prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .

        <http://example.org/ambiguous> rdf:type ex:Alpha, ex:Beta .
        """;

    [Test]
    public async Task ControllerCanAcceptSingleSysMLInterfaceResource()
    {
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/sysml-input/single",
            new StringContent(Turtle, Encoding.UTF8, OslcMediaType.TEXT_TURTLE));

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        ResourceResult result = await ReadResourceResult(response);
        await Assert.That(result.TypeName).IsEqualTo(nameof(Element));
        await Assert.That(result.IsExtendedResource).IsTrue();
        await Assert.That(result.About).IsEqualTo("http://example.org/element");
    }

    [Test]
    public async Task ControllerChoosesMostConcreteTypeWhenResourceHasMultipleRdfTypes()
    {
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/sysml-input/single",
            new StringContent(MultipleTypesTurtle, Encoding.UTF8, OslcMediaType.TEXT_TURTLE));

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        ResourceResult result = await ReadResourceResult(response);
        await Assert.That(result.TypeName).IsEqualTo(nameof(DerivedElement));
    }

    [Test]
    public async Task ControllerRejectsAmbiguousUnrelatedRdfTypes()
    {
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        var action = () => client.PostAsync(
            "/sysml-input/single",
            new StringContent(AmbiguousTypesTurtle, Encoding.UTF8, OslcMediaType.TEXT_TURTLE));

        InvalidOperationException exception = await Assert.That(action).Throws<InvalidOperationException>();
        await Assert.That(exception.Message).Contains("Multiple unrelated CLR resource types match RDF resource");
    }

    [Test]
    public async Task ControllerCanAcceptMultipleSysMLInterfaceResources()
    {
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/sysml-input/multiple",
            new StringContent(Turtle, Encoding.UTF8, OslcMediaType.TEXT_TURTLE));

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        ResourcesResult result = await ReadResourcesResult(response);
        await Assert.That(result.TypeNames).IsEquivalentTo([nameof(Element), nameof(Relationship)]);
    }

    [Test]
    public async Task ControllerCanAcceptOslcRequestForSysMLInterfaceResources()
    {
        using TestServer server = CreateServer();
        using HttpClient client = server.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/sysml-input/request",
            new StringContent(Turtle, Encoding.UTF8, OslcMediaType.TEXT_TURTLE));

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        RequestResult result = await ReadRequestResult(response);
        await Assert.That(result.TypeNames).IsEquivalentTo([nameof(Element), nameof(Relationship)]);
        await Assert.That(result.TripleCount).IsEqualTo(2);
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
                    .AddApplicationPart(typeof(SysMLInterfaceInputController).Assembly);
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

    private static async Task<ResourceResult> ReadResourceResult(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<ResourceResult>() ??
            throw new InvalidOperationException("The response did not contain a resource result.");
    }

    private static async Task<ResourcesResult> ReadResourcesResult(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<ResourcesResult>() ??
            throw new InvalidOperationException("The response did not contain a resources result.");
    }

    private static async Task<RequestResult> ReadRequestResult(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<RequestResult>() ??
            throw new InvalidOperationException("The response did not contain a request result.");
    }
}

[OslcNamespace("http://example.org/vocab#")]
[OslcResourceShape(title = "Alpha Shape", describes = ["http://example.org/vocab#Alpha"])]
public partial record AlphaElement : AbstractResourceRecord, IElement;

[OslcNamespace("http://example.org/vocab#")]
[OslcResourceShape(title = "Beta Shape", describes = ["http://example.org/vocab#Beta"])]
public partial record BetaElement : AbstractResourceRecord, IElement;

[OslcNamespace("http://example.org/vocab#")]
[OslcResourceShape(title = "Base Shape", describes = ["http://example.org/vocab#Base"])]
public partial record BaseElement : AbstractResourceRecord, IElement;

[OslcNamespace("http://example.org/vocab#")]
[OslcResourceShape(title = "Derived Shape", describes = ["http://example.org/vocab#Derived"])]
public partial record DerivedElement : BaseElement;

[ApiController]
[Route("sysml-input")]
public sealed class SysMLInterfaceInputController : ControllerBase
{
    [HttpPost("single")]
    [Consumes(OslcMediaType.TEXT_TURTLE)]
    public IActionResult Single([FromBody] IElement element)
    {
        return Ok(ResourceResult.From(element));
    }

    [HttpPost("multiple")]
    [Consumes(OslcMediaType.TEXT_TURTLE)]
    public IActionResult Multiple([FromBody] List<IElement> elements)
    {
        return Ok(new ResourcesResult(elements.Select(static element => element.GetType().Name).ToArray()));
    }

    [HttpPost("request")]
    [Consumes(OslcMediaType.TEXT_TURTLE)]
    public IActionResult Request([FromBody] OslcRequest<IElement> request)
    {
        return Ok(new RequestResult(
            request.Resources.Select(static resource => resource.GetType().Name).ToArray(),
            request.Graph.Triples.Count));
    }
}

public sealed record ResourceResult(
    string TypeName,
    string? About,
    bool IsExtendedResource)
{
    public static ResourceResult From(IElement element)
    {
        return new ResourceResult(
            element.GetType().Name,
            element.About?.AbsoluteUri,
            element is IExtendedResource);
    }
}

public sealed record ResourcesResult(string[] TypeNames);

public sealed record RequestResult(string[] TypeNames, int TripleCount);
