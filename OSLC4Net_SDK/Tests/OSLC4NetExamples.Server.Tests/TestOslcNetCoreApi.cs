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
using Xunit;

namespace OSLC4NetExamples.Server.Tests;

[Collection("AspireApp")]
public class TestOslcNetCoreApi : IDisposable
{
    private readonly RefimplAspireFixture _aspireFixture;
    private readonly HttpClient _httpClient;

    public TestOslcNetCoreApi(RefimplAspireFixture aspireFixture)
        {
        _aspireFixture = aspireFixture;

        // Create HTTP client for direct API testing
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task TestRootServicesEndpoint()
    {
        // Arrange
        var rootServicesUrl = $"{_aspireFixture.NetCoreApiBaseUri}.well-known/oslc/rootservices.xml";

        // Act
        using var response = await _httpClient.GetAsync(rootServicesUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/rdf+xml", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        // Verify it's valid XML
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(content);

        // Verify it contains expected OSLC elements
        // Note: Log the actual content to see what we're getting
        Console.WriteLine($"Actual XML content: {content}");

        // Look for the actual XML elements that should be present
        Assert.Contains("oauthRealmName", content, StringComparison.Ordinal);
        Assert.Contains("cmServiceProviders", content, StringComparison.Ordinal);
        Assert.Contains("rmServiceProviders", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TestCatalogGetEndpoint()
    {
        // Arrange
        var catalogUrl = $"{_aspireFixture.NetCoreApiBaseUri}oslc/catalog";
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));

        // Act
        using var response = await _httpClient.GetAsync(catalogUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        // Verify it's valid JSON-LD
        using var jsonDoc = JsonDocument.Parse(content);

        // Check for OSLC catalog structure
        var hasServiceProvider = content.Contains("ServiceProvider", StringComparison.Ordinal) ||
                                 content.Contains("serviceProvider", StringComparison.Ordinal);
        Assert.True(hasServiceProvider, "Response should contain ServiceProvider reference");
    }

    [Fact]
    public async Task TestCatalogPutEndpoint()
    {
        // Arrange
        var catalogUrl = $"{_aspireFixture.NetCoreApiBaseUri}oslc/catalog";

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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseContent);

        // Verify response is valid JSON-LD
        using var jsonDoc = JsonDocument.Parse(responseContent);

        // Verify the service provider was processed
        var hasServiceProvider = responseContent.Contains("ServiceProvider", StringComparison.Ordinal) ||
                                 responseContent.Contains("serviceProvider", StringComparison.Ordinal);
        Assert.True(hasServiceProvider, "Response should contain ServiceProvider reference");
    }

    [Fact]
    public async Task TestCatalogGetWithRdfXml()
    {
        // Arrange
        var catalogUrl = $"{_aspireFixture.NetCoreApiBaseUri}oslc/catalog";
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/rdf+xml"));

        // Act
        using var response = await _httpClient.GetAsync(catalogUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/rdf+xml", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        // Verify it's valid XML
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(content);

        // Check for RDF structure
        Assert.Contains("rdf:RDF", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TestCatalogGetWithTurtle()
    {
        // Arrange
        var catalogUrl = $"{_aspireFixture.NetCoreApiBaseUri}oslc/catalog";
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/turtle"));

        // Act
        using var response = await _httpClient.GetAsync(catalogUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/turtle", response.Content.Headers.ContentType?.MediaType);

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        // Basic Turtle format validation
        Assert.Contains("@prefix", content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TestCatalogContentNegotiation()
    {
        // Arrange
        var catalogUrl = $"{_aspireFixture.NetCoreApiBaseUri}oslc/catalog";

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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(mediaType, response.Content.Headers.ContentType?.MediaType);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content);
        }
    }

    [Fact]
    public async Task TestRootServicesContainsCorrectUrls()
    {
        // Arrange
        var rootServicesUrl = $"{_aspireFixture.NetCoreApiBaseUri}.well-known/oslc/rootservices.xml";

        // Act
        using var response = await _httpClient.GetAsync(rootServicesUrl);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify dynamic URL generation based on request
        Assert.Contains(_aspireFixture.NetCoreApiBaseUri.TrimEnd('/'), content, StringComparison.Ordinal);
        Assert.Contains("/services/catalog/singleton", content, StringComparison.Ordinal);
        Assert.Contains("/services/oauth/", content, StringComparison.Ordinal);

        // Verify XML structure
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(content);

        var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
        namespaceManager.AddNamespace("oslc_cm", "http://open-services.net/xmlns/cm/1.0/");
        namespaceManager.AddNamespace("oslc_rm", "http://open-services.net/xmlns/rm/1.0/");
        namespaceManager.AddNamespace("oslc_auto", "http://open-services.net/xmlns/automation/1.0/");

        var cmServiceProviders = xmlDoc.SelectSingleNode("//oslc_cm:cmServiceProviders", namespaceManager);
        var rmServiceProviders = xmlDoc.SelectSingleNode("//oslc_rm:rmServiceProviders", namespaceManager);

        Assert.NotNull(cmServiceProviders);
        Assert.NotNull(rmServiceProviders);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
