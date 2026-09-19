/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Query;
using QueryStringValue = OSLC4Net.Core.Query.StringValue;

namespace OSLC4Net.Core.QueryTests;

public class OslcQuerySpecificationTests
{
    private static readonly IDictionary<string, string> Prefixes = new Dictionary<string, string>
    {
        ["dcterms"] = "http://purl.org/dc/terms/",
        ["oslc"] = "http://open-services.net/ns/core#",
        ["rdf"] = "http://www.w3.org/1999/02/22-rdf-syntax-ns#",
        ["xs"] = "http://www.w3.org/2001/XMLSchema"
    };

    [Test]
    public async Task ParseWhereBuildsTheSpecificationValueTypes()
    {
        var where = QueryUtils.ParseWhere(
            "dcterms:identifier=<https://example.test/requirements/1> and " +
            "dcterms:title=\"Requirement\" and " +
            "oslc:status in [\"Open\",\"Done\"] and " +
            "oslc:priority>=1.50 and " +
            "oslc:approved=true and " +
            "dcterms:modified=\"2026-09-19T00:00:00Z\"^^xs:dateTime and " +
            "dcterms:title=\"Anforderung\"@de-DE",
            Prefixes);

        await Assert.That(where.Children).HasCount(7);

        var uriComparison = (ComparisonTerm)where.Children[0];
        await Assert.That(uriComparison.Operator).IsEqualTo(Operator.EQUALS);
        await Assert.That(uriComparison.Operand is UriRefValue).IsTrue();
        await Assert.That(((UriRefValue)uriComparison.Operand).Value)
            .IsEqualTo("https://example.test/requirements/1");

        var stringComparison = (ComparisonTerm)where.Children[1];
        await Assert.That(stringComparison.Operand is QueryStringValue).IsTrue();
        await Assert.That(((QueryStringValue)stringComparison.Operand).Value)
            .IsEqualTo("Requirement");

        var inTerm = (InTerm)where.Children[2];
        await Assert.That(inTerm.Values).HasCount(2);
        await Assert.That(((QueryStringValue)inTerm.Values[0]).Value).IsEqualTo("Open");
        await Assert.That(((QueryStringValue)inTerm.Values[1]).Value).IsEqualTo("Done");

        var decimalComparison = (ComparisonTerm)where.Children[3];
        await Assert.That(decimalComparison.Operator).IsEqualTo(Operator.GREATER_EQUALS);
        await Assert.That(decimalComparison.Operand is DecimalValue).IsTrue();
        await Assert.That(((DecimalValue)decimalComparison.Operand).Value).IsEqualTo("1.50");

        var booleanComparison = (ComparisonTerm)where.Children[4];
        await Assert.That(booleanComparison.Operand is BooleanValue).IsTrue();
        await Assert.That(((BooleanValue)booleanComparison.Operand).Value).IsTrue();

        var typedComparison = (ComparisonTerm)where.Children[5];
        await Assert.That(typedComparison.Operand is TypedValue).IsTrue();
        var typedValue = (TypedValue)typedComparison.Operand;
        await Assert.That(typedValue.Value).IsEqualTo("2026-09-19T00:00:00Z");
        await Assert.That(typedValue.PrefixedName.ns).IsEqualTo(Prefixes["xs"]);
        await Assert.That(typedValue.PrefixedName.local).IsEqualTo("dateTime");

        var languageComparison = (ComparisonTerm)where.Children[6];
        await Assert.That(languageComparison.Operand is LangedStringValue).IsTrue();
        var languageValue = (LangedStringValue)languageComparison.Operand;
        await Assert.That(languageValue.Value).IsEqualTo("Anforderung");
        await Assert.That(languageValue.LangTag).IsEqualTo("de-DE");
    }

    [Test]
    public async Task ParseWhereSupportsAllComparisonOperatorsAndEscapedStrings()
    {
        var where = QueryUtils.ParseWhere(
            "oslc:a=\"one\" and oslc:b!=\"two\" and oslc:c<\"three\" and " +
            "oslc:d>\"four\" and oslc:e<=\"five\" and oslc:f>=\"six\" and " +
            "dcterms:title=\"A \\\"quoted\\\" title\"",
            Prefixes);

        await Assert.That(where.Children).HasCount(7);
        await Assert.That(((ComparisonTerm)where.Children[0]).Operator)
            .IsEqualTo(Operator.EQUALS);
        await Assert.That(((ComparisonTerm)where.Children[1]).Operator)
            .IsEqualTo(Operator.NOT_EQUALS);
        await Assert.That(((ComparisonTerm)where.Children[2]).Operator)
            .IsEqualTo(Operator.LESS_THAN);
        await Assert.That(((ComparisonTerm)where.Children[3]).Operator)
            .IsEqualTo(Operator.GREATER_THAN);
        await Assert.That(((ComparisonTerm)where.Children[4]).Operator)
            .IsEqualTo(Operator.LESS_EQUALS);
        await Assert.That(((ComparisonTerm)where.Children[5]).Operator)
            .IsEqualTo(Operator.GREATER_EQUALS);
        await Assert.That(((ComparisonTerm)where.Children[6]).Operand is QueryStringValue).IsTrue();
    }

    [Test]
    public async Task ParseSelectBuildsNestedPropertiesAndWildcards()
    {
        var select = QueryUtils.ParseSelect(
            "dcterms:title,oslc:shortTitle,oslc:relatedArtifact{" +
            "dcterms:title,oslc:identifier},*",
            Prefixes);

        await Assert.That(select.Children).HasCount(4);
        await Assert.That(select.Children[0].Identifier.local).IsEqualTo("title");
        await Assert.That(select.Children[1].Identifier.local).IsEqualTo("shortTitle");
        await Assert.That(select.Children[2] is NestedProperty).IsTrue();

        var nested = (NestedProperty)select.Children[2];
        await Assert.That(nested.Identifier.local).IsEqualTo("relatedArtifact");
        await Assert.That(nested.Children).HasCount(2);
        await Assert.That(select.Children[3].IsWildcard).IsTrue();

        var inverted = QueryUtils.InvertSelectedProperties(
            QueryUtils.ParseSelect("dcterms:title,oslc:shortTitle", Prefixes));
        await Assert.That(inverted.ContainsKey("http://purl.org/dc/terms/title")).IsTrue();
        await Assert.That(inverted.ContainsKey("http://open-services.net/ns/core#shortTitle")).IsTrue();
    }

    [Test]
    public async Task ParseOrderByBuildsDirectionsAndScopedTerms()
    {
        var orderBy = QueryUtils.ParseOrderBy(
            "-dcterms:title,+oslc:identifier,oslc:relatedArtifact{-dcterms:title}",
            Prefixes);

        await Assert.That(orderBy.Children).HasCount(3);

        var descending = (SimpleSortTerm)orderBy.Children[0];
        await Assert.That(descending.Ascending).IsFalse();
        await Assert.That(descending.Identifier.local).IsEqualTo("title");

        var ascending = (SimpleSortTerm)orderBy.Children[1];
        await Assert.That(ascending.Ascending).IsTrue();
        await Assert.That(ascending.Identifier.local).IsEqualTo("identifier");

        await Assert.That(orderBy.Children[2] is ScopedSortTerm).IsTrue();
        var scoped = (ScopedSortTerm)orderBy.Children[2];
        await Assert.That(scoped.SortTerms.Children).HasSingleItem();
        await Assert.That(((SimpleSortTerm)scoped.SortTerms.Children[0]).Ascending).IsFalse();
    }

    [Test]
    public async Task ParseSearchTermsPreservesTermsContainingSpacesAndEscapes()
    {
        var searchTerms = QueryUtils.ParseSearchTerms(
            "\"Baseline A\",\"Active stream\",\"A \\\"quoted\\\" value\"");

        await Assert.That(searchTerms).HasCount(3);
        await Assert.That(searchTerms[0]).IsEqualTo("Baseline A");
        await Assert.That(searchTerms[1]).IsEqualTo("Active stream");
        await Assert.That(searchTerms[2]).IsEqualTo("A \\\"quoted\\\" value");
    }
}
