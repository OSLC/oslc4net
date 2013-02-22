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
        [TestMethod]
        public void BasicPrefixesTest()
        {
            Trial[] trials = {
                    new Trial("qm=<http://qm.example.com/ns>," +
                                "olsc=<http://open-services.net/ns/core#>," +
                                "xs=<http://www.w3.org/2001/XMLSchema>",
                              true),
                    new Trial("qm=<http://qm.example.com/ns>," +
                                 "XXX>",
                              false)
                };

            foreach (Trial trial in trials)
            {

                try
                {

                    IDictionary<String, String> prefixMap =
                        QueryUtils.parsePrefixes(trial.Expression);

                    Debug.WriteLine(prefixMap.ToString());

                    Assert.IsTrue(trial.ShouldSucceed);

                }
                catch (ParseException e)
                {

                    Debug.WriteLine(e.StackTrace);

                    Assert.IsFalse(trial.ShouldSucceed);
                }
            }
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
