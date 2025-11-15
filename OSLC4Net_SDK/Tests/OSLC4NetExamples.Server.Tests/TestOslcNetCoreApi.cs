/*******************************************************************************
 * Copyright (c) 2025 OSLC4Net contributors.
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
 *     GitHub Copilot - initial API and implementation
 *******************************************************************************/

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using TUnit.Assertions;
using TUnit.Core;

namespace OSLC4NetExamples.Server.Tests;

[TestFixture]
public class TestOslcNetCoreApi
{
    private HttpClient _httpClient;

    [BeforeEachTest]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }

    [Test]
    public async Task TestRootServicesEndpoint()
    {
        // Arrange
        var rootServicesUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}.well-known/oslc/rootservices.xml";

        // Act
        using var response = await _httpClient.GetAsync(rootServicesUrl);

        // Assert
        await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);
        await Assert.That(response.Content.Headers.ContentType?.MediaType).Is.EqualTo("application/rdf+xml");

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Is.Not.Empty();

        // Verify it's valid XML
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(content);

        // Verify it contains expected OSLC elements
        // Note: Log the actual content to see what we're getting
        TestContext.WriteLine($"Actual XML content: {content}");

        // Look for the actual XML elements that should be present
        await Assert.That(content).Contains("oauthRealmName");
        await Assert.That(content).Contains("cmServiceProviders");
        await Assert.That(content).Contains("rmServiceProviders");
    }

    [Test]
    public async Task TestCatalogGetEndpoint()
    {
        // Arrange
        var catalogUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}oslc/catalog";
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));

        // Act
        using var response = await _httpClient.GetAsync(catalogUrl);

        // Assert
        await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Is.Not.Empty();

        // Verify it's valid JSON-LD
        using var jsonDoc = JsonDocument.Parse(content);

        // Check for OSLC catalog structure
        var hasServiceProvider = content.Contains("ServiceProvider", StringComparison.Ordinal) ||
                                 content.Contains("serviceProvider", StringComparison.Ordinal);
        await Assert.That(hasServiceProvider).Is.True("Response should contain ServiceProvider reference");
    }

    [Test]
    public async Task TestCatalogPutEndpoint()
    {
        // Arrange
        var catalogUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}oslc/catalog";

        var serviceProviderJson = """
        [
          {
            "@id": "https://localhost:44387/oslc/catalog",
            "@type": [
              "http://open-services.net/ns/core#ServiceProvider"
            ],
            "http://purl.org/dc/terms/description": [
              {
                "@value": "test me",
                "@type": "http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral"
              }
            ]
          }
        ]
        """;

        using var requestContent = new StringContent(serviceProviderJson, Encoding.UTF8, "application/ld+json");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));

        // Act
        using var response = await _httpClient.PutAsync(catalogUrl, requestContent);

        // Assert
        await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        await Assert.That(responseContent).Is.Not.Empty();

        // Verify response is valid JSON-LD
        using var jsonDoc = JsonDocument.Parse(responseContent);

        // Verify the service provider was processed
        var hasServiceProvider = responseContent.Contains("ServiceProvider", StringComparison.Ordinal) ||
                                 responseContent.Contains("serviceProvider", StringComparison.Ordinal);
        await Assert.That(hasServiceProvider).Is.True("Response should contain ServiceProvider reference");
    }

    [Test]
    public async Task TestCatalogGetWithRdfXml()
    {
        // Arrange
        var catalogUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}oslc/catalog";
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/rdf+xml"));

        // Act
        using var response = await _httpClient.GetAsync(catalogUrl);

        // Assert
        await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);
        await Assert.That(response.Content.Headers.ContentType?.MediaType).Is.EqualTo("application/rdf+xml");

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Is.Not.Empty();

        // Verify it's valid XML
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(content);

        // Check for RDF structure
        await Assert.That(content).Contains("rdf:RDF");
    }

    [Test]
    public async Task TestCatalogGetWithTurtle()
    {
        // Arrange
        var catalogUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}oslc/catalog";
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/turtle"));

        // Act
        using var response = await _httpClient.GetAsync(catalogUrl);

        // Assert
        await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);
        await Assert.That(response.Content.Headers.ContentType?.MediaType).Is.EqualTo("text/turtle");

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Is.Not.Empty();

        // Basic Turtle format validation
        await Assert.That(content).Contains("@prefix");
    }

    [Test]
    public async Task TestCatalogContentNegotiation()
    {
        // Arrange
        var catalogUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}oslc/catalog";

        var mediaTypes = new[]
        {
            "application/rdf+xml",
            "text/turtle",
            "application/ld+json",
            "application/n-triples"
        };

        foreach (var mediaType in mediaTypes)
        {
            // Act
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            using var response = await _httpClient.GetAsync(catalogUrl);

            // Assert
            await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);
            await Assert.That(response.Content.Headers.ContentType?.MediaType).Is.EqualTo(mediaType);

            var content = await response.Content.ReadAsStringAsync();
            await Assert.That(content).Is.Not.Empty();
        }
    }

    [Test]
    public async Task TestRootServicesContainsCorrectUrls()
    {
        // Arrange
        var rootServicesUrl = $"{AspireAppLifecycle.NetCoreApiBaseUri}.well-known/oslc/rootservices.xml";

        // Act
        using var response = await _httpClient.GetAsync(rootServicesUrl);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        await Assert.That(response.StatusCode).Is.EqualTo(HttpStatusCode.OK);

        // Verify dynamic URL generation based on request
        await Assert.That(content).Contains(AspireAppLifecycle.NetCoreApiBaseUri.TrimEnd('/'));
        await Assert.That(content).Contains("/services/catalog/singleton");
        await Assert.That(content).Contains("/services/oauth/");

        // Verify XML structure
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(content);

        var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
        namespaceManager.AddNamespace("oslc_cm", "http://open-services.net/xmlns/cm/1.0/");
        namespaceManager.AddNamespace("oslc_rm", "http://open-services.net/xmlns/rm/1.0/");
        namespaceManager.AddNamespace("oslc_auto", "http://open-services.net/xmlns/automation/1.0/");

        var cmServiceProviders = xmlDoc.SelectSingleNode("//oslc_cm:cmServiceProviders", namespaceManager);
        var rmServiceProviders = xmlDoc.SelectSingleNode("//oslc_rm:rmServiceProviders", namespaceManager);

        await Assert.That(cmServiceProviders).Is.Not.Null();
        await Assert.That(rmServiceProviders).Is.Not.Null();
    }

    [AfterEachTest]
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
