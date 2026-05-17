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

using TUnit.Core;

namespace OSLC4Net.Client.Tests;

public class OslcRequestParamsTests
{
    [Test]
    public async Task Default_HasExpectedValues()
    {
        var defaultParams = OslcRequestParams.Default;

        await Assert.That(defaultParams.AcceptHeader).IsEqualTo(OslcRequestParams.DefaultAcceptHeader);
        await Assert.That(defaultParams.ContentType).IsEqualTo(OslcRequestParams.DefaultContentType);
        await Assert.That(defaultParams.OslcCoreVersion).IsEqualTo("2.0");
        await Assert.That(defaultParams.CustomHeaders).IsNull();
    }

    [Test]
    public async Task RdfXmlOnly_HasRdfXmlValues()
    {
        var rdfXmlParams = OslcRequestParams.RdfXmlOnly;

        await Assert.That(rdfXmlParams.AcceptHeader).IsEqualTo("application/rdf+xml");
        await Assert.That(rdfXmlParams.ContentType).IsEqualTo("application/rdf+xml");
        await Assert.That(rdfXmlParams.OslcCoreVersion).IsEqualTo("2.0");
    }

    [Test]
    public async Task TurtleOnly_HasTurtleValues()
    {
        var turtleParams = OslcRequestParams.TurtleOnly;

        await Assert.That(turtleParams.AcceptHeader).IsEqualTo("text/turtle");
        await Assert.That(turtleParams.ContentType).IsEqualTo("text/turtle");
        await Assert.That(turtleParams.OslcCoreVersion).IsEqualTo("2.0");
    }

    [Test]
    public async Task Merge_WithNullOverride_ReturnsSelf()
    {
        var original = OslcRequestParams.Default;

        var result = original.Merge(null);

        await Assert.That(result).IsEqualTo(original);
    }

    [Test]
    public async Task Merge_WithOverride_UsesOverrideValues()
    {
        var original = OslcRequestParams.Default;
        var overrides = new OslcRequestParams
        {
            AcceptHeader = "custom/accept",
            ContentType = null // Should keep original
        };

        var result = original.Merge(overrides);

        await Assert.That(result.AcceptHeader).IsEqualTo("custom/accept");
        await Assert.That(result.ContentType).IsEqualTo(original.ContentType);
        await Assert.That(result.OslcCoreVersion).IsEqualTo(original.OslcCoreVersion);
    }

    [Test]
    public async Task Merge_CombinesCustomHeaders()
    {
        var original = new OslcRequestParams
        {
            AcceptHeader = "original/accept",
            CustomHeaders = new Dictionary<string, string>
            {
                { "Header1", "Value1" },
                { "Header2", "Value2" }
            }
        };
        var overrides = new OslcRequestParams
        {
            CustomHeaders = new Dictionary<string, string>
            {
                { "Header2", "OverriddenValue2" },
                { "Header3", "Value3" }
            }
        };

        var result = original.Merge(overrides);

        await Assert.That(result.CustomHeaders).IsNotNull();
        await Assert.That(result.CustomHeaders!.Count).IsEqualTo(3);
        await Assert.That(result.CustomHeaders["Header1"]).IsEqualTo("Value1");
        await Assert.That(result.CustomHeaders["Header2"]).IsEqualTo("OverriddenValue2");
        await Assert.That(result.CustomHeaders["Header3"]).IsEqualTo("Value3");
    }

    [Test]
    public async Task Builder_CreatesParamsCorrectly()
    {
        var params1 = OslcRequestParams.Builder()
            .WithAccept("custom/accept")
            .WithContentType("custom/content")
            .WithOslcCoreVersion("3.0")
            .WithHeader("X-Custom", "CustomValue")
            .Build();

        await Assert.That(params1.AcceptHeader).IsEqualTo("custom/accept");
        await Assert.That(params1.ContentType).IsEqualTo("custom/content");
        await Assert.That(params1.OslcCoreVersion).IsEqualTo("3.0");
        await Assert.That(params1.CustomHeaders).IsNotNull();
        await Assert.That(params1.CustomHeaders!["X-Custom"]).IsEqualTo("CustomValue");
    }

    [Test]
    public async Task Builder_WithMultipleHeaders_AddsAll()
    {
        var params1 = OslcRequestParams.Builder()
            .WithHeader("Header1", "Value1")
            .WithHeader("Header2", "Value2")
            .Build();

        await Assert.That(params1.CustomHeaders).IsNotNull();
        await Assert.That(params1.CustomHeaders!.Count).IsEqualTo(2);
        await Assert.That(params1.CustomHeaders["Header1"]).IsEqualTo("Value1");
        await Assert.That(params1.CustomHeaders["Header2"]).IsEqualTo("Value2");
    }

    [Test]
    public async Task DefaultContentType_IsRdfXml()
    {
        await Assert.That(OslcRequestParams.DefaultContentType).IsEqualTo("application/rdf+xml");
    }

    [Test]
    public async Task DefaultAcceptHeader_IncludesMultipleFormats()
    {
        var acceptHeader = OslcRequestParams.DefaultAcceptHeader;

        await Assert.That(acceptHeader).Contains("text/turtle");
        await Assert.That(acceptHeader).Contains("application/rdf+xml");
        await Assert.That(acceptHeader).Contains("application/n-triples");
        await Assert.That(acceptHeader).Contains("text/n3");
    }
}
