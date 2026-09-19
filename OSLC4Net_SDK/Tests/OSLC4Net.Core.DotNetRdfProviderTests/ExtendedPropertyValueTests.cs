/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class ExtendedPropertyValueTests
{
    [Test]
    public async Task HandleExtendedPropertyValue_LiteralNodes_DeserializesCorrectly()
    {
        var helper = new DotNetRdfHelper();
        IGraph graph = new Graph();

        var subjectNode = graph.CreateUriNode(new Uri("http://example.com/cr/1"));
        var predicateBool = graph.CreateUriNode(new Uri("http://example.com/ns#extBool"));
        var predicateInt = graph.CreateUriNode(new Uri("http://example.com/ns#extInt"));
        var predicateDouble = graph.CreateUriNode(new Uri("http://example.com/ns#extDouble"));
        var predicateString = graph.CreateUriNode(new Uri("http://example.com/ns#extString"));
        var predicateType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var typeNode = graph.CreateUriNode(new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest"));

        graph.Assert(new Triple(subjectNode, predicateType, typeNode));
        graph.Assert(new Triple(subjectNode, predicateBool, true.ToLiteral(graph)));
        graph.Assert(new Triple(subjectNode, predicateInt, 42.ToLiteral(graph)));
        graph.Assert(new Triple(subjectNode, predicateDouble, 3.14.ToLiteral(graph)));
        graph.Assert(new Triple(subjectNode, predicateString, "hello".ToLiteral(graph)));

        var result = (ChangeRequest)helper.FromDotNetRdfNode(subjectNode, graph, typeof(ChangeRequest));

        await Assert.That(result).IsNotNull();
        var extProps = result.GetExtendedProperties();

        var boolKey = new QName("http://example.com/ns#", "extBool");
        var intKey = new QName("http://example.com/ns#", "extInt");
        var doubleKey = new QName("http://example.com/ns#", "extDouble");
        var stringKey = new QName("http://example.com/ns#", "extString");

        await Assert.That(extProps[boolKey]).IsEqualTo(true);
        await Assert.That(extProps[intKey]).IsEqualTo(42L);
        await Assert.That(extProps[doubleKey]).IsEqualTo(3.14d);
        await Assert.That(extProps[stringKey]).IsEqualTo("hello");
    }

    [Test]
    public async Task HandleExtendedPropertyValue_UnsignedLong_DeserializesValuesAboveLongMaxValue()
    {
        var helper = new DotNetRdfHelper();
        IGraph graph = new Graph();

        var subjectNode = graph.CreateUriNode(new Uri("http://example.com/cr/unsigned-long"));
        var predicateType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var typeNode = graph.CreateUriNode(new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest"));
        var predicateUnsignedLong = graph.CreateUriNode(new Uri("http://example.com/ns#extUnsignedLong"));
        var unsignedLongNode = graph.CreateLiteralNode(
            "18446744073709551615",
            new Uri("http://www.w3.org/2001/XMLSchema#unsignedLong"));

        graph.Assert(new Triple(subjectNode, predicateType, typeNode));
        graph.Assert(new Triple(subjectNode, predicateUnsignedLong, unsignedLongNode));

        var result = (ChangeRequest)helper.FromDotNetRdfNode(subjectNode, graph, typeof(ChangeRequest));

        await Assert.That(result).IsNotNull();
        var extProps = result.GetExtendedProperties();
        var key = new QName("http://example.com/ns#", "extUnsignedLong");

        await Assert.That(extProps[key]).IsEqualTo(ulong.MaxValue);
    }

    [Test]
    public async Task HandleExtendedPropertyValue_ResourceReference_DeserializesUri()
    {
        var helper = new DotNetRdfHelper();
        IGraph graph = new Graph();

        var subjectNode = graph.CreateUriNode(new Uri("http://example.com/cr/2"));
        var predicateType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var typeNode = graph.CreateUriNode(new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest"));

        var refPredicate = graph.CreateUriNode(new Uri("http://example.com/ns#extRef"));
        var targetUri = new Uri("http://example.com/otherResource");
        var targetNode = graph.CreateUriNode(targetUri);

        graph.Assert(new Triple(subjectNode, predicateType, typeNode));
        graph.Assert(new Triple(subjectNode, refPredicate, targetNode));

        var result = (ChangeRequest)helper.FromDotNetRdfNode(subjectNode, graph, typeof(ChangeRequest));

        await Assert.That(result).IsNotNull();
        var extProps = result.GetExtendedProperties();

        var refKey = new QName("http://example.com/ns#", "extRef");
        await Assert.That(extProps[refKey]).IsEqualTo(targetUri);
    }

    [Test]
    public async Task HandleExtendedPropertyValue_InlineBlankNode_DeserializesAnyResource()
    {
        var helper = new DotNetRdfHelper();
        IGraph graph = new Graph();

        var subjectNode = graph.CreateUriNode(new Uri("http://example.com/cr/3"));
        var predicateType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var typeNode = graph.CreateUriNode(new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest"));

        var inlinePredicate = graph.CreateUriNode(new Uri("http://example.com/ns#extInline"));
        var blankNode = graph.CreateBlankNode();
        var innerPredicate = graph.CreateUriNode(new Uri("http://example.com/ns#innerProp"));

        graph.Assert(new Triple(subjectNode, predicateType, typeNode));
        graph.Assert(new Triple(subjectNode, inlinePredicate, blankNode));
        graph.Assert(new Triple(blankNode, innerPredicate, "innerValue".ToLiteral(graph)));

        var result = (ChangeRequest)helper.FromDotNetRdfNode(subjectNode, graph, typeof(ChangeRequest));

        await Assert.That(result).IsNotNull();
        var extProps = result.GetExtendedProperties();

        var inlineKey = new QName("http://example.com/ns#", "extInline");
        await Assert.That(extProps[inlineKey]).IsTypeOf<AnyResource>();

        var anyResource = (AnyResource)extProps[inlineKey];
        var innerKey = new QName("http://example.com/ns#", "innerProp");
        await Assert.That(anyResource.GetExtendedProperties()[innerKey]).IsEqualTo("innerValue");
    }

    [Test]
    public async Task HandleExtendedPropertyValue_InlineUriNode_DeserializesAnyResourceAndReusesVisited()
    {
        var helper = new DotNetRdfHelper();
        IGraph graph = new Graph();

        var subjectNode = graph.CreateUriNode(new Uri("http://example.com/cr/4"));
        var predicateType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var typeNode = graph.CreateUriNode(new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest"));

        var inlinePredicate1 = graph.CreateUriNode(new Uri("http://example.com/ns#extInline1"));
        var inlinePredicate2 = graph.CreateUriNode(new Uri("http://example.com/ns#extInline2"));
        var nestedUriNode = graph.CreateUriNode(new Uri("http://example.com/nested/1"));
        var innerPredicate = graph.CreateUriNode(new Uri("http://example.com/ns#innerProp"));

        graph.Assert(new Triple(subjectNode, predicateType, typeNode));
        graph.Assert(new Triple(subjectNode, inlinePredicate1, nestedUriNode));
        graph.Assert(new Triple(subjectNode, inlinePredicate2, nestedUriNode));
        graph.Assert(new Triple(nestedUriNode, innerPredicate, "nestedValue".ToLiteral(graph)));

        var result = (ChangeRequest)helper.FromDotNetRdfNode(subjectNode, graph, typeof(ChangeRequest));

        await Assert.That(result).IsNotNull();
        var extProps = result.GetExtendedProperties();

        var key1 = new QName("http://example.com/ns#", "extInline1");
        var key2 = new QName("http://example.com/ns#", "extInline2");

        await Assert.That(extProps[key1]).IsTypeOf<AnyResource>();
        await Assert.That(extProps[key2]).IsTypeOf<AnyResource>();

        // Second lookup should reuse the exact same visited AnyResource instance
        await Assert.That(extProps[key1]).IsSameReferenceAs(extProps[key2]);
    }
}
