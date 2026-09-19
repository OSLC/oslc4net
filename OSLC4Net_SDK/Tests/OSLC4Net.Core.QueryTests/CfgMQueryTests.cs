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

namespace OSLC4Net.Core.QueryTests;

public class CfgMQueryTests
{
    private static readonly IDictionary<string, string> Prefixes = new Dictionary<string, string>
    {
        ["dcterms"] = "http://purl.org/dc/terms/",
        ["oslc_config"] = "http://open-services.net/ns/config#",
        ["rdf"] = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
    };

    [Test]
    public async Task ParseConfigurationResourceQuery()
    {
        var where = QueryUtils.ParseWhere(
            "rdf:type=<http://open-services.net/ns/config#VersionResource> and " +
            "dcterms:title in [\"Baseline A\",\"Baseline B\",\"Active stream\"]",
            Prefixes);
        var select = QueryUtils.ParseSelect(
            "dcterms:title,oslc_config:component{dcterms:title}," +
            "oslc_config:versionResource",
            Prefixes);
        var orderBy = QueryUtils.ParseOrderBy(
            "-dcterms:title,+oslc_config:versionResource",
            Prefixes);

        await Assert.That(where.Children).HasCount(2);
        await Assert.That(((UriRefValue)((ComparisonTerm)where.Children[0]).Operand).Value)
            .IsEqualTo("http://open-services.net/ns/config#VersionResource");
        await Assert.That(((InTerm)where.Children[1]).Values).HasCount(3);

        await Assert.That(select.Children).HasCount(3);
        await Assert.That(select.Children[1] is NestedProperty).IsTrue();
        await Assert.That(((NestedProperty)select.Children[1]).Children).HasSingleItem();
        await Assert.That(orderBy.Children).HasCount(2);
        await Assert.That(((SimpleSortTerm)orderBy.Children[0]).Ascending).IsFalse();
        await Assert.That(((SimpleSortTerm)orderBy.Children[1]).Ascending).IsTrue();
    }

    [Test]
    public async Task ParseConfigurationHistorySearchTerms()
    {
        var searchTerms = QueryUtils.ParseSearchTerms(
            "\"Baseline A\",\"Baseline B\",\"Active stream\"");

        await Assert.That(searchTerms).HasCount(3);
        await Assert.That(searchTerms).Contains("Baseline A");
        await Assert.That(searchTerms).Contains("Baseline B");
        await Assert.That(searchTerms).Contains("Active stream");
    }
}
