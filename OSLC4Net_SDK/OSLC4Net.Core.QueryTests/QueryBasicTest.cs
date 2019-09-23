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

using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.Core.Query;
using ParseException = OSLC4Net.Core.Query.ParseException;

namespace OSLC4Net.Core.QueryTests
{
    [TestClass]
    public class QueryBasicTest
    {
	    static String PREFIXES = "qm=<http://qm.example.com/ns/>," +
			    "olsc=<http://open-services.net/ns/core#>," +
			    "xs=<http://www.w3.org/2001/XMLSchema>";

        [TestMethod]
        public void BasicPrefixesTest()
        {
            Trial[] trials = {
                    new Trial("qm=<http://qm.example.com/ns/>," +
                                "olsc=<http://open-services.net/ns/core#>," +
                                "xs=<http://www.w3.org/2001/XMLSchema>",
                              true),
                    new Trial("qm=<http://qm.example.com/ns/>," +
                                 "XXX>",
                              false)
                };

            foreach (Trial trial in trials)
            {
                try
                {

                    IDictionary<String, String> prefixMap =
                        QueryUtils.ParsePrefixes(trial.Expression);

                    Debug.WriteLine(prefixMap.ToString());

                    Assert.IsTrue(trial.ShouldSucceed);

                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
        }

        [TestMethod]
        public void BasicOrderByTest()
        {
            String prefixes = "qm=<http://qm.example.com/ns/>," +
                "oslc=<http://open-services.net/ns/core#>";
            IDictionary<String, String> prefixMap = QueryUtils.ParsePrefixes(prefixes);

            Trial[] trials = {
                    new Trial("-qm:priority", true),
                    new Trial("+qm:priority,-oslc:name", true),
                    new Trial("qm:tested_by{+oslc:description}", true),
                    new Trial("?qm:blah", false)
                };

            foreach (Trial trial in trials)
            {
                try
                {
                    OrderByClause orderByClause =
                        QueryUtils.ParseOrderBy(trial.Expression, prefixMap);

                    Debug.WriteLine(orderByClause);

                    Assert.IsTrue(trial.ShouldSucceed);

                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
        }

        [TestMethod]
        public void BasicSearchTermsTest()
        {
            Trial[] trials = {
                    new Trial("\"foobar\"", true),
                    new Trial("\"foobar\",\"whatsis\",\"yousa\"", true),
                    new Trial("", false)
                };

            foreach (Trial trial in trials)
            {
                try
                {
                    SearchTermsClause searchTermsClause =
                        QueryUtils.ParseSearchTerms(trial.Expression);

                    Debug.WriteLine(searchTermsClause);

                    Assert.IsTrue(trial.ShouldSucceed);

                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
        }

        [TestMethod]
        public void BasicSelectTest()
        {
            String prefixes = "qm=<http://qm.example.com/ns/>," +
                "oslc=<http://open-services.net/ns/core#>";
            IDictionary<String, String> prefixMap = QueryUtils.ParsePrefixes(prefixes);

            Trial[] trials = {
                    new Trial("*{*}", true),
                    new Trial("qm:testcase", true),
                    new Trial("*", true),
                    new Trial("oslc:create,qm:verified", true),
                    new Trial("qm:state{oslc:verified_by{oslc:owner,qm:duration}}", true),
                    new Trial("qm:submitted{*}", true),
                    new Trial("qm:testcase,*", true),
                    new Trial("*,qm:state{*}", true),
                    new Trial("XXX", false)
                };

            foreach (Trial trial in trials)
            {
                try
                {
                    SelectClause selectClause =
                        QueryUtils.ParseSelect(trial.Expression, prefixMap);

                    Debug.WriteLine(selectClause);

                    Assert.IsTrue(trial.ShouldSucceed);

                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
        }

        [TestMethod]
        public void BasicWhereTest()
        {
            String prefixes = "qm=<http://qm.example.com/ns/>," +
                "oslc=<http://open-services.net/ns/core#>," +
                "xs=<http://www.w3.org/2001/XMLSchema>";
            IDictionary<String, String> prefixMap = QueryUtils.ParsePrefixes(prefixes);

            Trial[] trials = {
                    new Trial("qm:testcase=<http://example.com/tests/31459>", true),
                    new Trial("qm:duration>=10.4", true),
                    new Trial("oslc:create!=\"Bob\" and qm:verified!=true", true),
                    new Trial("qm:state in [\"Done\",\"Open\"]", true),
                    new Trial("oslc:verified_by{oslc:owner=\"Steve\" and qm:duration=-47.0} and oslc:description=\"very hairy expression\"", true),
                    new Trial("qm:submitted<\"2011-10-10T07:00:00Z\"^^xs:dateTime", true),
                    new Trial("oslc:label>\"The End\"@en-US", true),
                    new Trial("XXX", false)
                };

            foreach (Trial trial in trials)
            {
                try
                {
                    WhereClause whereClause =
                        QueryUtils.ParseWhere(trial.Expression, prefixMap);

                    Debug.WriteLine(whereClause);

                    Assert.IsTrue(trial.ShouldSucceed);

                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
        }

        [TestMethod]
        public void BasicInvertTest()
        {
            String prefixes = "qm=<http://qm.example.com/ns/>," +
                "oslc=<http://open-services.net/ns/core#>";
            IDictionary<String, String> prefixMap = QueryUtils.ParsePrefixes(prefixes);

            Trial[] trials = {
                    new Trial("*{*}", true),
                    new Trial("qm:testcase", true),
                    new Trial("*", true),
                    new Trial("oslc:create,qm:verified", true),
                    new Trial("qm:state{oslc:verified_by{oslc:owner,qm:duration}}", true),
                    new Trial("qm:submitted{*}", true),
                    new Trial("qm:testcase,*", true),
                    new Trial("*,qm:state{*}", true),
                };

            foreach (Trial trial in trials)
            {
                try
                {
                    SelectClause selectClause =
                        QueryUtils.ParseSelect(trial.Expression, prefixMap);

                    Debug.WriteLine(selectClause);

                    Assert.IsTrue(trial.ShouldSucceed);

                    IDictionary<String, object> invertedProperties = QueryUtils.InvertSelectedProperties(selectClause);
                }
                catch (ParseException e)
                {
                    Debug.WriteLine(e.GetType().ToString() + ": " + e.Message + ":\n" + e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
        }
	
        [TestMethod]
	    public void TestUriRef()
        {
		    IDictionary<String, String> prefixMap = QueryUtils.ParsePrefixes(PREFIXES);
		    WhereClause where = QueryUtils.ParseWhere(
				    "qm:testCase=<http://example.org/tests/24>", prefixMap);
		
		    IList<SimpleTerm> children = where.Children;
		    Assert.AreEqual(1, children.Count, "Where clause should only have one term");
		
		    SimpleTerm simpleTerm = children[0];
		    PName prop = simpleTerm.Property;
            Assert.AreEqual(prop.ns + prop.local, "http://qm.example.com/ns/testCase");
		    Assert.IsTrue(simpleTerm is ComparisonTerm);
	
		    ComparisonTerm comparison = (ComparisonTerm) simpleTerm;
            Assert.AreEqual(comparison.Operator, Operator.EQUALS);
		
		    IValue v = comparison.Operand;
		    Assert.IsTrue(v is IUriRefValue);
		
		    IUriRefValue uriRef = (IUriRefValue) v;
            Assert.AreEqual("http://example.org/tests/24", uriRef.Value);
	    }
    }

    public class Trial
    {
        public Trial(
            string expression,
            bool shouldSucceed
        )
        {
            this.expression = expression;
            this.shouldSucceed = shouldSucceed;
        }

        public string
        Expression { get { return expression; } }

        public bool
        ShouldSucceed { get { return shouldSucceed; } }

        private string expression;
        private bool shouldSucceed;
    }
}
