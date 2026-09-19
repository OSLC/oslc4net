/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
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
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System.Diagnostics;
using OSLC4Net.Core.Query;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseException = OSLC4Net.Core.Query.ParseException;

namespace OSLC4Net.Core.QueryTests;

public class QueryBasicTest
{
    const string PREFIXES = "qm=<http://qm.example.com/ns/>," +
                             "olsc=<http://open-services.net/ns/core#>," +
                             "xs=<http://www.w3.org/2001/XMLSchema>";

    [Test]
    [Arguments("qm=<http://qm.example.com/ns/>,olsc=<http://open-services.net/ns/core#>,xs=<http://www.w3.org/2001/XMLSchema>", true)]
    [Arguments("qm=<http://qm.example.com/ns/>,qm=<http://qm.example.com/other/>", false)]
    [Arguments("qm=<http://qm.example.com/ns/>,XXX>", false)]
    public async Task BasicPrefixesTest(string expression, bool shouldSucceed)
    {
        try
        {
            var prefixMap =
                QueryUtils.ParsePrefixes(expression);

            Debug.WriteLine(prefixMap.ToString());

            await Assert.That(shouldSucceed).IsTrue();
        }
        catch (ParseException e)
        {
            Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

            await Assert.That(shouldSucceed).IsFalse();
        }
    }

    [Test]
    [Arguments("-qm:priority", true)]
    [Arguments("+qm:priority,-oslc:name", true)]
    [Arguments("qm:tested_by{+oslc:description}", true)]
    [Arguments("?qm:blah", false)]
    public async Task BasicOrderByTest(string expression, bool shouldSucceed)
    {
        var prefixes = "qm=<http://qm.example.com/ns/>," +
                       "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        try
        {
            var orderByClause =
                QueryUtils.ParseOrderBy(expression, prefixMap);

            Debug.WriteLine(orderByClause);

            await Assert.That(shouldSucceed).IsTrue();
        }
        catch (ParseException e)
        {
            Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

            await Assert.That(shouldSucceed).IsFalse();
        }
    }

    [Test]
    [Arguments("\"foobar\"", true)]
    [Arguments("\"foobar\",\"whatsis\",\"yousa\"", true)]
    [Arguments("-oslc:priority,+dcterms:title", false)]
    [Arguments("", false)]
    public async Task BasicSearchTermsTest(string expression, bool shouldSucceed)
    {
        try
        {
            var searchTermsClause =
                QueryUtils.ParseSearchTerms(expression);

            Debug.WriteLine(searchTermsClause);

            await Assert.That(shouldSucceed).IsTrue();
        }
        catch (ParseException e)
        {
            Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

            await Assert.That(shouldSucceed).IsFalse();
        }
    }

    [Test]
    [Arguments("*{*}", true)]
    [Arguments("qm:testcase", true)]
    [Arguments("*", true)]
    [Arguments("oslc:create,qm:verified", true)]
    [Arguments("qm:state{oslc:verified_by{oslc:owner,qm:duration}}", true)]
    [Arguments("qm:submitted{*}", true)]
    [Arguments("qm:testcase,*", true)]
    [Arguments("*,qm:state{*}", true)]
    [Arguments("XXX", false)]
    public async Task BasicSelectTest(string expression, bool shouldSucceed)
    {
        var prefixes = "qm=<http://qm.example.com/ns/>," +
                       "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        try
        {
            var selectClause =
                QueryUtils.ParseSelect(expression, prefixMap);

            Debug.WriteLine(selectClause);

            await Assert.That(shouldSucceed).IsTrue();
        }
        catch (ParseException e)
        {
            Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

            await Assert.That(shouldSucceed).IsFalse();
        }
    }

    [Test]
    [Arguments("qm:testcase=<http://example.com/tests/31459>", true)]
    [Arguments("qm:duration>=10.4", true)]
    [Arguments("oslc:create!=\"Bob\" and qm:verified!=true", true)]
    [Arguments("qm:state in [\"Done\",\"Open\"]", true)]
    [Arguments("oslc:verified_by{oslc:owner=\"Steve\" and qm:duration=-47.0} and oslc:description=\"very hairy expression\"", true)]
    [Arguments("qm:submitted<\"2011-10-10T07:00:00Z\"^^xs:dateTime", true)]
    [Arguments("oslc:label>\"The End\"@en-US", true)]
    [Arguments("XXX", false)]
    public async Task BasicWhereTest(string expression, bool shouldSucceed)
    {
        var prefixes = "qm=<http://qm.example.com/ns/>," +
                       "oslc=<http://open-services.net/ns/core#>," +
                       "xs=<http://www.w3.org/2001/XMLSchema>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        try
        {
            var whereClause =
                QueryUtils.ParseWhere(expression, prefixMap);

            Debug.WriteLine(whereClause);

            await Assert.That(shouldSucceed).IsTrue();
        }
        catch (ParseException e)
        {
            Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

            await Assert.That(shouldSucceed).IsFalse();
        }
    }

    [Test]
    [Arguments("*{*}", true)]
    [Arguments("qm:testcase", true)]
    [Arguments("*", true)]
    [Arguments("oslc:create,qm:verified", true)]
    [Arguments("qm:state{oslc:verified_by{oslc:owner,qm:duration}}", true)]
    [Arguments("qm:submitted{*}", true)]
    [Arguments("qm:testcase,*", true)]
    [Arguments("*,qm:state{*}", true)]
    public async Task BasicInvertTest(string expression, bool shouldSucceed)
    {
        const string prefixes = "qm=<http://qm.example.com/ns/>," +
                                "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        try
        {
            var selectClause =
                QueryUtils.ParseSelect(expression, prefixMap);

            Debug.WriteLine(selectClause);

            await Assert.That(shouldSucceed).IsTrue();

            var _ = QueryUtils.InvertSelectedProperties(selectClause);
        }
        catch (ParseException e)
        {
            Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

            await Assert.That(shouldSucceed).IsFalse();
        }
    }

    [Test]
    public async Task InvertMalformedSelectThrowsParseException()
    {
        const string expression = "*Open\",\"In Progress\",\"Done\"";
        const string prefixes = "qm=<http://qm.example.com/ns/>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);
        var selectClause = QueryUtils.ParseSelect(expression, prefixMap);

        await Assert.That(() => QueryUtils.InvertSelectedProperties(selectClause))
            .Throws<ParseException>();
    }

    [Test]
    public async Task InvertUnknownPrefixThrowsParseException()
    {
        const string expression = "unknown:property";
        const string prefixes = "qm=<http://qm.example.com/ns/>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);
        var selectClause = QueryUtils.ParseSelect(expression, prefixMap);

        await Assert.That(() => QueryUtils.InvertSelectedProperties(selectClause))
            .Throws<ParseException>();
    }

    [Test]
    public async Task InvertDuplicateSelectPropertiesDoesNotThrow()
    {
        const string expression = "qm:property,qm:property";
        const string prefixes = "qm=<http://qm.example.com/ns/>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);
        var selectClause = QueryUtils.ParseSelect(expression, prefixMap);

        var result = QueryUtils.InvertSelectedProperties(selectClause);

        await Assert.That(result).HasCount(1);
    }

    [Test]
    public async Task InvertDuplicateNestedSelectPropertiesDoesNotThrow()
    {
        const string expression =
            "qm:property{dcterms:title},qm:property{oslc:shortTitle}";
        const string prefixes = "qm=<http://qm.example.com/ns/>," +
                                "dcterms=<http://purl.org/dc/terms/>," +
                                "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);
        var selectClause = QueryUtils.ParseSelect(expression, prefixMap);

        var result = QueryUtils.InvertSelectedProperties(selectClause);

        await Assert.That(result).HasCount(1);
        var nested = (IDictionary<string, object>)result["http://qm.example.com/ns/property"];
        await Assert.That(nested.ContainsKey("http://purl.org/dc/terms/title")).IsTrue();
        await Assert.That(nested.ContainsKey("http://open-services.net/ns/core#shortTitle")).IsTrue();
    }

    [Test]
    public async Task InvertNestedWildcardWithSiblingPropertyDoesNotThrow()
    {
        const string expression = "qm:property,*{dcterms:title}";
        const string prefixes = "qm=<http://qm.example.com/ns/>," +
                                "dcterms=<http://purl.org/dc/terms/>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);
        var selectClause = QueryUtils.ParseSelect(expression, prefixMap);

        var result = QueryUtils.InvertSelectedProperties(selectClause);

        await Assert.That(result).HasCount(1);
        await Assert.That(result.ContainsKey("http://qm.example.com/ns/property")).IsTrue();
    }

    [Test]
    public async Task TestUriRef()
    {
        var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
        var where = QueryUtils.ParseWhere(
            "qm:testCase=<http://example.org/tests/24>", prefixMap);

        var children = where.Children;
        // Where clause should only have one term
        await Assert.That(children).HasSingleItem();

        var simpleTerm = children[0];
        var prop = simpleTerm.Property;
        await Assert.That(prop.ns + prop.local).IsEqualTo("http://qm.example.com/ns/testCase");
        await Assert.That(simpleTerm is ComparisonTerm).IsTrue();

        var comparison = (ComparisonTerm)simpleTerm;
        await Assert.That(comparison.Operator).IsEqualTo(Operator.EQUALS);

        var v = comparison.Operand;
        await Assert.That(v is UriRefValue).IsTrue();

        var uriRef = (UriRefValue)v;
        await Assert.That(uriRef.Value).IsEqualTo("http://example.org/tests/24");
    }
}
