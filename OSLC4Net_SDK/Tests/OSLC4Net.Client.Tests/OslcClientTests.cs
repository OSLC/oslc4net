/*******************************************************************************
 * Copyright (c) 2025 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *******************************************************************************/

using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Oslc;
using TUnit.Core;

namespace OSLC4Net.Client.Tests;

public class OslcClientTests
{
    private IHost AppHost { get; }
    private ILoggerFactory LoggerFactory { get; }

    public OslcClientTests()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
            })
            .Build();
        LoggerFactory = AppHost.Services.GetRequiredService<ILoggerFactory>();
    }

    [Test]
    public async Task OslcClient_DefaultConstructor_HasDefaultRequestParams()
    {
        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());

        await Assert.That(client.DefaultRequestParams).IsNotNull();
        await Assert.That(client.DefaultRequestParams.AcceptHeader).IsEqualTo(OslcRequestParams.DefaultAcceptHeader);
        await Assert.That(client.DefaultRequestParams.ContentType).IsEqualTo(OslcRequestParams.DefaultContentType);

        client.Dispose();
    }

    [Test]
    public async Task OslcClient_WithCustomRequestParams_UsesProvidedParams()
    {
        var customParams = new OslcRequestParams
        {
            AcceptHeader = "custom/accept",
            ContentType = "custom/content",
            OslcCoreVersion = "3.0"
        };

        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>(), customParams);

        await Assert.That(client.DefaultRequestParams.AcceptHeader).IsEqualTo("custom/accept");
        await Assert.That(client.DefaultRequestParams.ContentType).IsEqualTo("custom/content");
        await Assert.That(client.DefaultRequestParams.OslcCoreVersion).IsEqualTo("3.0");

        client.Dispose();
    }

    [Test]
    public async Task OslcClient_ForBasicAuth_WithCustomRequestParams_UsesProvidedParams()
    {
        var customParams = new OslcRequestParams
        {
            AcceptHeader = "application/rdf+xml"
        };

        var client = OslcClient.ForBasicAuth(
            "testuser",
            "testpass",
            LoggerFactory.CreateLogger<OslcClient>(),
            null,
            customParams
        );

        await Assert.That(client.DefaultRequestParams.AcceptHeader).IsEqualTo("application/rdf+xml");

        client.Dispose();
    }

    [Test]
    public async Task EnableGraphAccumulation_ReturnsGraph()
    {
        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());

        var graph = client.EnableGraphAccumulation();

        await Assert.That(graph).IsNotNull();
        await Assert.That(client.AccumulatingGraph).IsNotNull();
        await Assert.That(client.AccumulatingGraph).IsEqualTo(graph);

        client.Dispose();
    }

    [Test]
    public async Task EnableGraphAccumulation_CalledTwice_ReturnsSameGraph()
    {
        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());

        var graph1 = client.EnableGraphAccumulation();
        var graph2 = client.EnableGraphAccumulation();

        await Assert.That(graph1).IsEqualTo(graph2);

        client.Dispose();
    }

    [Test]
    public async Task DisableGraphAccumulation_ReturnsAccumulatedGraph()
    {
        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());

        var enabledGraph = client.EnableGraphAccumulation();
        var disabledGraph = client.DisableGraphAccumulation();

        await Assert.That(disabledGraph).IsEqualTo(enabledGraph);
        await Assert.That(client.AccumulatingGraph).IsNull();

        client.Dispose();
    }

    [Test]
    public async Task DisableGraphAccumulation_WhenNotEnabled_ReturnsNull()
    {
        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());

        var graph = client.DisableGraphAccumulation();

        await Assert.That(graph).IsNull();

        client.Dispose();
    }

    [Test]
    public async Task ClearAccumulatingGraph_ClearsGraphContent()
    {
        var client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());

        var graph = client.EnableGraphAccumulation();
        // Add a triple to the graph
        var subj = graph.CreateUriNode(new Uri("http://example.org/subject"));
        var pred = graph.CreateUriNode(new Uri("http://example.org/predicate"));
        var obj = graph.CreateUriNode(new Uri("http://example.org/object"));
        graph.Assert(new VDS.RDF.Triple(subj, pred, obj));

        await Assert.That(graph.Triples.Count).IsGreaterThan(0);

        client.ClearAccumulatingGraph();

        await Assert.That(graph.Triples.Count).IsEqualTo(0);
        await Assert.That(client.AccumulatingGraph).IsNotNull(); // Still enabled

        client.Dispose();
    }

    [Test]
    public async Task GetResourceRawAsync_AppliesCustomHeaders()
    {
        string? capturedAcceptHeader = null;
        string? capturedOslcVersionHeader = null;
        string? capturedCustomHeader = null;

        var handler = new FakeHttpMessageHandler((req, ct) =>
        {
            // Capture headers from the request
            if (req.Headers.TryGetValues("Accept", out var acceptValues))
            {
                capturedAcceptHeader = string.Join(", ", acceptValues);
            }
            if (req.Headers.TryGetValues("OSLC-Core-Version", out var oslcValues))
            {
                capturedOslcVersionHeader = string.Join(", ", oslcValues);
            }
            if (req.Headers.TryGetValues("X-Custom-Header", out var customValues))
            {
                capturedCustomHeader = string.Join(", ", customValues);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"></rdf:RDF>",
                    Encoding.UTF8, "application/rdf+xml"),
                RequestMessage = req
            });
        });

        var httpClient = new HttpClient(handler);
        var customParams = new OslcRequestParams
        {
            AcceptHeader = "application/rdf+xml",
            OslcCoreVersion = "2.0",
            CustomHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "CustomValue" }
            }
        };

        var client = new OslcClient(httpClient, LoggerFactory.CreateLogger<OslcClient>(), customParams);

        await client.GetResourceRawAsync("http://example.com/resource");

        await Assert.That(capturedAcceptHeader).Contains("application/rdf+xml");
        await Assert.That(capturedOslcVersionHeader).IsEqualTo("2.0");
        await Assert.That(capturedCustomHeader).IsEqualTo("CustomValue");
    }

    [Test]
    public async Task GetResourceRawAsync_WithPerRequestOverride_UsesOverride()
    {
        string? capturedAcceptHeader = null;

        var handler = new FakeHttpMessageHandler((req, ct) =>
        {
            if (req.Headers.TryGetValues("Accept", out var acceptValues))
            {
                capturedAcceptHeader = string.Join(", ", acceptValues);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"></rdf:RDF>",
                    Encoding.UTF8, "application/rdf+xml"),
                RequestMessage = req
            });
        });

        var httpClient = new HttpClient(handler);
        var defaultParams = new OslcRequestParams
        {
            AcceptHeader = "application/rdf+xml"
        };

        var client = new OslcClient(httpClient, LoggerFactory.CreateLogger<OslcClient>(), defaultParams);

        var overrideParams = new OslcRequestParams
        {
            AcceptHeader = "text/turtle"
        };

        await client.GetResourceRawAsync("http://example.com/resource", null, overrideParams);

        await Assert.That(capturedAcceptHeader).Contains("text/turtle");
    }

    [Test]
    public async Task HttpClient_Constructor_AcceptsPreConfiguredClient()
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-Pre-Configured", "true");

        // When using an externally-provided HttpClient, the OslcClient does NOT own it,
        // so we must ensure the HttpClient is not disposed when the OslcClient is disposed.
        // In this case, we manage the HttpClient with using and don't dispose the OslcClient.
        var client = new OslcClient(httpClient, LoggerFactory.CreateLogger<OslcClient>());

        await Assert.That(client.GetHttpClient()).IsEqualTo(httpClient);
        await Assert.That(client.GetHttpClient().DefaultRequestHeaders.Contains("X-Pre-Configured")).IsTrue();
    }
}
