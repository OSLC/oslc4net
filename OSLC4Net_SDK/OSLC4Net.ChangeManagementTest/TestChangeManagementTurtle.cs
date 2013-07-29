/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
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


using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagementTest
{
    [TestClass]
    public class TestChangeManagementTurtle : TestBase
    {
        public TestContext TestContext { set; get; }

        [TestInitialize]
        public void TestSetup()
        {
            switch (TestContext.TestName)
            {
                case "TestResourceShape":
                case "TestCreate":
                    break;
                default:
                    MakeChangeRequest(OslcMediaType.TEXT_TURTLE);
                    break;
            }
        }

        [TestCleanup]
        public void TestTeardown()
        {
            switch (TestContext.TestName)
            {
                case "TestResourceShape":
                case "TestDelete":
                    break;
                default:
                    DeleteChangeRequest(OslcMediaType.TEXT_TURTLE);
                    break;
            }
        }

        [TestMethod]
        public void TestResourceShape()
        {
            TestResourceShape(OslcMediaType.TEXT_TURTLE);
        }

        [TestMethod]
        public void TestCreate()
        {
            TestCreate(OslcMediaType.TEXT_TURTLE);
        }

        [TestMethod]
        public void TestRetrieve()
        {
            TestRetrieve(OslcMediaType.TEXT_TURTLE);
        }

        [TestMethod]
        public void TestRetrieves()
        {
            TestRetrieves(OslcMediaType.TEXT_TURTLE);
        }

        [TestMethod]
        public void TestCompact()
        {
            TestCompact(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML,
                        OslcMediaType.TEXT_TURTLE);
        }

        [TestMethod]
        public void TestUpdate()
        {
            TestUpdate(OslcMediaType.TEXT_TURTLE);
        }

        [TestMethod]
        public void TestDelete()
        {
            TestDelete(OslcMediaType.TEXT_TURTLE);
        }
    }
}
