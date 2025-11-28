/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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
    public async Task BasicPrefixesTest()
    {
        Trial[] trials =
        {
            new("qm=<http://qm.example.com/ns/>," +
                "olsc=<http://open-services.net/ns/core#>," +
                "xs=<http://www.w3.org/2001/XMLSchema>",
                true),
            new("qm=<http://qm.example.com/ns/>," +
                "XXX>",
                false)
        };

        foreach (var trial in trials)
        {
            try
            {
                var prefixMap =
                    QueryUtils.ParsePrefixes(trial.Expression);

                Debug.WriteLine(prefixMap.ToString());

                await Assert.That(trial.ShouldSucceed).IsTrue();
            }
            catch (ParseException e)
            {
                Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                await Assert.That(trial.ShouldSucceed).IsFalse();
            }
        }
    }

    [Test]
    public async Task BasicOrderByTest()
    {
        var prefixes = "qm=<http://qm.example.com/ns/>," +
                       "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        Trial[] trials =
        {
            new(expression: "-qm:priority", shouldSucceed: true), new("+qm:priority,-oslc:name", true),
            new("qm:tested_by{+oslc:description}", true), new("?qm:blah", false)
        };

        foreach (var trial in trials)
        {
            try
            {
                var orderByClause =
                    QueryUtils.ParseOrderBy(trial.Expression, prefixMap);

                Debug.WriteLine(orderByClause);

                await Assert.That(trial.ShouldSucceed).IsTrue();
            }
            catch (ParseException e)
            {
                Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                await Assert.That(trial.ShouldSucceed).IsFalse();
            }
        }
    }

    [Test]
    public async Task BasicSearchTermsTest()
    {
        Trial[] trials =
        {
            new("\"foobar\"", true), new("\"foobar\",\"whatsis\",\"yousa\"", true),
            new("", false)
        };

        foreach (var trial in trials)
        {
            try
            {
                var searchTermsClause =
                    QueryUtils.ParseSearchTerms(trial.Expression);

                Debug.WriteLine(searchTermsClause);

                await Assert.That(trial.ShouldSucceed).IsTrue();
            }
            catch (ParseException e)
            {
                Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                await Assert.That(trial.ShouldSucceed).IsFalse();
            }
        }
    }

    [Test]
    public async Task BasicSelectTest()
    {
        var prefixes = "qm=<http://qm.example.com/ns/>," +
                       "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        Trial[] trials =
        {
            new("*{*}", true), new("qm:testcase", true), new("*", true),
            new("oslc:create,qm:verified", true),
            new("qm:state{oslc:verified_by{oslc:owner,qm:duration}}", true),
            new("qm:submitted{*}", true), new("qm:testcase,*", true),
            new("*,qm:state{*}", true), new("XXX", false)
        };

        foreach (var trial in trials)
        {
            try
            {
                var selectClause =
                    QueryUtils.ParseSelect(trial.Expression, prefixMap);

                Debug.WriteLine(selectClause);

                await Assert.That(trial.ShouldSucceed).IsTrue();
            }
            catch (ParseException e)
            {
                Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                await Assert.That(trial.ShouldSucceed).IsFalse();
            }
        }
    }

    [Test]
    public async Task BasicWhereTest()
    {
        var prefixes = "qm=<http://qm.example.com/ns/>," +
                       "oslc=<http://open-services.net/ns/core#>," +
                       "xs=<http://www.w3.org/2001/XMLSchema>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        Trial[] trials =
        {
            new("qm:testcase=<http://example.com/tests/31459>", true),
            new("qm:duration>=10.4", true),
            new("oslc:create!=\"Bob\" and qm:verified!=true", true),
            new("qm:state in [\"Done\",\"Open\"]", true),
            new(
                "oslc:verified_by{oslc:owner=\"Steve\" and qm:duration=-47.0} and oslc:description=\"very hairy expression\"",
                true),
            new("qm:submitted<\"2011-10-10T07:00:00Z\"^^xs:dateTime", true),
            new("oslc:label>\"The End\"@en-US", true), new("XXX", false)
        };

        foreach (var trial in trials)
        {
            try
            {
                var whereClause =
                    QueryUtils.ParseWhere(trial.Expression, prefixMap);

                Debug.WriteLine(whereClause);

                await Assert.That(trial.ShouldSucceed).IsTrue();
            }
            catch (ParseException e)
            {
                Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                await Assert.That(trial.ShouldSucceed).IsFalse();
            }
        }
    }

    [Test]
    public async Task BasicInvertTest()
    {
        const string prefixes = "qm=<http://qm.example.com/ns/>," +
                                "oslc=<http://open-services.net/ns/core#>";
        var prefixMap = QueryUtils.ParsePrefixes(prefixes);

        Trial[] trials =
        {
            new(expression: "*{*}", shouldSucceed: true), new("qm:testcase", true), new("*", true),
            new("oslc:create,qm:verified", true),
            new("qm:state{oslc:verified_by{oslc:owner,qm:duration}}", true),
            new("qm:submitted{*}", true), new("qm:testcase,*", true),
            new("*,qm:state{*}", true),
        };

        foreach (var trial in trials)
        {
            try
            {
                var selectClause =
                    QueryUtils.ParseSelect(trial.Expression, prefixMap);

                Debug.WriteLine(selectClause);

                await Assert.That(trial.ShouldSucceed).IsTrue();

                var _ = QueryUtils.InvertSelectedProperties(selectClause);
            }
            catch (ParseException e)
            {
                Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                await Assert.That(trial.ShouldSucceed).IsFalse();
            }
        }
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

public class Trial
{
    public Trial(
        string expression,
        bool shouldSucceed
    )
    {
        Expression = expression;
        ShouldSucceed = shouldSucceed;
    }

    public string
        Expression
    { get; }

    public bool
        ShouldSucceed
    { get; }
}