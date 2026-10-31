/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Model;
using VDS.RDF;

namespace OSLC4Net.Server.Providers.Tests;

public sealed class OslcRequestTests
{
    private static IGraph CreateEmptyGraph() => new Graph();

    [Test]
    public async Task ConfigurationContext_ValidAbsoluteUri_ReturnsUri()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["Configuration-Context"] = ["https://example.com/configs/1"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.ConfigurationContext).IsNotNull();
        await Assert.That(request.ConfigurationContext!.AbsoluteUri).IsEqualTo("https://example.com/configs/1");
    }

    [Test]
    public async Task ConfigurationContext_RelativeUri_ReturnsNull()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["Configuration-Context"] = ["relative/path"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.ConfigurationContext).IsNull();
    }

    [Test]
    public async Task ConfigurationContext_InvalidUriString_ReturnsNull()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["Configuration-Context"] = ["not a valid uri"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.ConfigurationContext).IsNull();
    }

    [Test]
    public async Task ConfigurationContext_MissingHeader_ReturnsNull()
    {
        var request = new OslcRequest(CreateEmptyGraph());

        await Assert.That(request.ConfigurationContext).IsNull();
    }

    [Test]
    public async Task ConfigurationContext_CaseInsensitiveHeaderName_ReturnsUri()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["configuration-context"] = ["https://example.com/configs/2"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.ConfigurationContext).IsNotNull();
        await Assert.That(request.ConfigurationContext!.AbsoluteUri).IsEqualTo("https://example.com/configs/2");
    }

    [Test]
    public async Task Prefer_PresentHeader_ReturnsHeaderValues()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["Prefer"] = ["return=representation; include=\"http://www.w3.org/ns/ldp#PreferMinimalContainer\"", "respond-async"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.Prefer).Count().IsEqualTo(2);
        await Assert.That(request.Prefer[0]).IsEqualTo("return=representation; include=\"http://www.w3.org/ns/ldp#PreferMinimalContainer\"");
        await Assert.That(request.Prefer[1]).IsEqualTo("respond-async");
    }

    [Test]
    public async Task Prefer_MissingHeader_ReturnsEmptyList()
    {
        var request = new OslcRequest(CreateEmptyGraph());

        await Assert.That(request.Prefer).IsEmpty();
    }

    [Test]
    public async Task Prefer_CaseInsensitiveHeaderName_ReturnsValues()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["prefer"] = ["return=minimal"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.Prefer).Count().IsEqualTo(1);
        await Assert.That(request.Prefer[0]).IsEqualTo("return=minimal");
    }

    [Test]
    public async Task Slug_PresentHeader_ReturnsSlug()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["Slug"] = ["my-new-resource"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.Slug).IsEqualTo("my-new-resource");
    }

    [Test]
    public async Task Slug_MissingHeader_ReturnsNull()
    {
        var request = new OslcRequest(CreateEmptyGraph());

        await Assert.That(request.Slug).IsNull();
    }

    [Test]
    public async Task Slug_CaseInsensitiveHeaderName_ReturnsSlug()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["slug"] = ["lower-case-slug"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.Slug).IsEqualTo("lower-case-slug");
    }

    [Test]
    public async Task OslcCoreVersion_PresentHeader_ReturnsVersion()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["OSLC-Core-Version"] = ["3.0"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.OslcCoreVersion).IsEqualTo("3.0");
    }

    [Test]
    public async Task OslcCoreVersion_MissingHeader_ReturnsNull()
    {
        var request = new OslcRequest(CreateEmptyGraph());

        await Assert.That(request.OslcCoreVersion).IsNull();
    }

    [Test]
    public async Task OslcCoreVersion_CaseInsensitiveHeaderName_ReturnsVersion()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["oslc-core-version"] = ["2.0"]
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.OslcCoreVersion).IsEqualTo("2.0");
    }

    [Test]
    public async Task HeaderValueAndValues_EmptyValuesArray_ReturnsNullAndEmpty()
    {
        var headers = new Dictionary<string, string[]>
        {
            ["X-Empty"] = []
        };
        var request = new OslcRequest(CreateEmptyGraph(), headers);

        await Assert.That(request.HeaderValue("X-Empty")).IsNull();
        await Assert.That(request.HeaderValues("X-Empty")).IsEmpty();
        await Assert.That(request.HeaderValue("Non-Existent")).IsNull();
        await Assert.That(request.HeaderValues("Non-Existent")).IsEmpty();
    }

    [Test]
    public async Task GraphAndHeaders_ConstructorArguments_AreExposed()
    {
        IGraph graph = CreateEmptyGraph();
        var headers = new Dictionary<string, string[]>
        {
            ["Custom"] = ["Value"]
        };
        var request = new OslcRequest(graph, headers);

        await Assert.That(request.Graph).IsSameReferenceAs(graph);
        await Assert.That(request.Headers.ContainsKey("custom")).IsTrue();
        await Assert.That(request.Headers["Custom"][0]).IsEqualTo("Value");
    }

    [Test]
    public async Task GenericOslcRequest_ExposesTypedResourcesAndSingleResource()
    {
        IGraph graph = CreateEmptyGraph();
        var headers = new Dictionary<string, string[]>
        {
            ["Slug"] = ["typed-slug"]
        };
        var request = new OslcRequest<IResource>(graph, headers);

        await Assert.That(request.Slug).IsEqualTo("typed-slug");
        await Assert.That(request.Resources).IsEmpty();
        await Assert.That(request.Resource).IsNull();
    }
}
