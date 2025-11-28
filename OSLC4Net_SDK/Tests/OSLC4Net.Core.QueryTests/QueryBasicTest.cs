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
using Xunit;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParseException = OSLC4Net.Core.Query.ParseException;
using Antlr.Runtime.Tree; // Added for new tests
using System.Linq; // Added for new tests
using System; // Required for Uri in new tests, might have been in original ifdefs

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace OSLC4Net.Core.QueryTests
{
    public class QueryBasicTest
    {
        const string PREFIXES = "qm=<http://qm.example.com/ns/>," +
                                 "olsc=<http://open-services.net/ns/core#>," +
                                 "xs=<http://www.w3.org/2001/XMLSchema>";

        [Fact]
        public void BasicPrefixesTest()
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

                    Assert.True(trial.ShouldSucceed);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.False(trial.ShouldSucceed);
                }
            }
        }

        [Fact]
        public void BasicOrderByTest()
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

                    Assert.True(trial.ShouldSucceed);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.False(trial.ShouldSucceed);
                }
            }
        }

        [Fact]
        public void BasicSearchTermsTest()
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

                    Assert.True(trial.ShouldSucceed);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.False(trial.ShouldSucceed);
                }
            }
        }

        [Fact]
        public void BasicSelectTest()
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

                    Assert.True(trial.ShouldSucceed);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.False(trial.ShouldSucceed);
                }
            }
        }

        [Fact]
        public void BasicWhereTest()
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

                    Assert.True(trial.ShouldSucceed);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.False(trial.ShouldSucceed);
                }
            }
        }

        [Fact]
        public void BasicInvertTest()
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

                    Assert.True(trial.ShouldSucceed);

                    var _ = QueryUtils.InvertSelectedProperties(selectClause);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.False(trial.ShouldSucceed);
                }
            }
        }

        [Fact]
        public void TestUriRef()
        {
            var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
            var where = QueryUtils.ParseWhere(
                "qm:testCase=<http://example.org/tests/24>", prefixMap);

            var children = where.Children;
            // Where clause should only have one term
            Assert.Single(children);

            var simpleTerm = children[0];
            var prop = simpleTerm.Property;
            Assert.Equal("http://qm.example.com/ns/testCase", prop.ns + prop.local);
            Assert.True(simpleTerm is ComparisonTerm);

            var comparison = (ComparisonTerm)simpleTerm;
            Assert.Equal(Operator.EQUALS, comparison.Operator);

            var v = comparison.Operand;
            Assert.True(v is UriRefValue);

            var uriRef = (UriRefValue)v;
            Assert.Equal("http://example.org/tests/24", uriRef.Value);
        }

        // Tests for enhancements from kuribara-hideaki/oslc4net fork

        // Related to fork commit db49995, efcacd7
        [Fact]
        public void TestInvalidWhereClauseSetsErrorProperty()
        {
            var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
            // Intentionally invalid syntax (e.g., missing operand)
            string invalidWhereExpression = "qm:testcase=";

            var whereClause = QueryUtils.ParseWhere(invalidWhereExpression, prefixMap);

            // Assuming IBaseClause is implemented by WhereClauseImpl and has IsError property
            // This will likely fail until the actual code from the fork is integrated.
            Assert.True((whereClause as dynamic)?.IsError, "WhereClause should have IsError set to true for invalid expression.");
            // Optionally, check ErrorReason if it's standardized
            // Assert.NotEmpty((whereClause as dynamic)?.ErrorReason);
        }

        // Related to fork commit db49995, efcacd7
        [Fact]
        public void TestInvalidOrderByClauseSetsErrorProperty()
        {
            var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
            // Intentionally invalid syntax (e.g., unknown prefix, invalid character)
            string invalidOrderByExpression = "unknown:property+";

            var orderByClause = QueryUtils.ParseOrderBy(invalidOrderByExpression, prefixMap);

            // Assuming IBaseClause is implemented by OrderByClause/SortTermsImpl
            // This will likely fail until the actual code from the fork is integrated.
            Assert.True((orderByClause as dynamic)?.IsError, "OrderByClause should have IsError set to true for invalid expression.");
            // Assert.NotEmpty((orderByClause as dynamic)?.ErrorReason);
        }

        // Related to fork commit 127e068
        [Fact]
        public void TestWhereInClauseWithSyntaxErrorSetsErrorProperty()
        {
            var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
            // Syntax error within the 'in' list (e.g., unclosed string)
            // Corrected the intentionally broken string for C# syntax, error is for the parser
            string whereExpression = "qm:state in [\"Done\", \"Open\"";

            var whereClause = QueryUtils.ParseWhere(whereExpression, prefixMap);

            // This will likely fail until the actual code from the fork is integrated.
            Assert.True((whereClause as dynamic)?.IsError, "WhereClause should have IsError set for syntax error in 'in' list.");
        }

        // Related to fork commit 31d2858
        [Fact]
        public void TestWhereClauseWithAsteriskOperand()
        {
            var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
            string whereExpression = "dcterms:title = \"*\"";

            var whereClause = QueryUtils.ParseWhere(whereExpression, prefixMap);

            Assert.False((whereClause as dynamic)?.IsError ?? true, "WhereClause should not have IsError for valid asterisk operand."); // Default to true if IsError is not found

            var children = whereClause.Children;
            Assert.Single(children);
            var simpleTerm = children[0];
            Assert.True(simpleTerm is ComparisonTerm, "Term should be a ComparisonTerm.");

            var comparison = (ComparisonTerm)simpleTerm;
            Assert.Equal(Operator.EQUALS, comparison.Operator);

            var operand = comparison.Operand;
            Assert.True(operand is StringValue, "Operand should be StringValue.");
            Assert.Equal("*", ((StringValue)operand).Value);
        }

        // Related to fork commit 834cddd
        [Fact]
        public void TestOrderByReturnsITreeChildren()
        {
            var prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
            string orderByExpression = "dcterms:title";

            var orderByClause = QueryUtils.ParseOrderBy(orderByExpression, prefixMap);

            Assert.False((orderByClause as dynamic)?.IsError ?? true, "OrderByClause should not have IsError for valid expression."); // Default to true if IsError is not found

            // Test the signature of Children property
            // This will fail until the SortTerms interface and Impl are updated from the fork.
            var children = ((SortTerms)orderByClause).Children;
            Assert.NotNull(children);
            // Could add: if (children.Count > 0) Assert.True(children[0] is Antlr.Runtime.Tree.ITree);
        }
    } // Closing brace for QueryBasicTest class

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
} // Closing brace for namespace
