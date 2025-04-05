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

namespace OSLC4Net.ChangeManagementTest;

[TestClass]
public class TestChangeManagementXml : TestBase
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
                MakeChangeRequestAsync(OslcMediaType.APPLICATION_XML);
                break;
        }
    }

    [TestCleanup]
    public void TestTeardown()
    {
        if ((TestContext!.TestName == "TestResourceShape") || (TestContext!.TestName == "TestDelete"))
        {
            // they remove the resource at the end
        }
        else
        {
            if (CREATED_CHANGE_REQUEST_URI is not null)
            {
                DeleteChangeRequest(OslcMediaType.APPLICATION_XML);
            }
            else
            {
                 TestContext.WriteLine("Warning: Cannot delete change request as CREATED_CHANGE_REQUEST_URI is null");
            }
        }
    }

    [TestMethod]
    public async Task TestResourceShape()
    {
        await TestResourceShapeAsync(OslcMediaType.APPLICATION_XML);
    }

    [TestMethod]
    public void TestCreate()
    {
        TestCreateAsync(OslcMediaType.APPLICATION_XML);
    }

    [TestMethod]
    public void TestRetrieve()
    {
        TestRetrieveAsync(OslcMediaType.APPLICATION_XML);
    }

    [TestMethod]
    public void TestRetrieves()
    {
        TestRetrievesAsync(OslcMediaType.APPLICATION_XML);
    }

    [TestMethod]
    public void TestCompact()
    {
        TestCompactAsync(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML,
                    OslcMediaType.APPLICATION_XML);
    }

    [TestMethod]
    public void TestUpdate()
    {
        TestUpdateAsync(OslcMediaType.APPLICATION_XML);
    }

    [TestMethod]
    public void TestDelete()
    {
        TestDeleteAsync(OslcMediaType.APPLICATION_XML);
    }
}
