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

// [TestClass]
public class TestChangeManagementRdfXml : TestBase
{
    public TestContext? TestContext { set; get; }

    [TestInitialize]
    public void TestSetup()
    {
        switch (TestContext.TestName)
        {
            case "TestResourceShape":
            case "TestCreate":
                break;
            default:
                MakeChangeRequestAsync(OslcMediaType.APPLICATION_RDF_XML);
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
                DeleteChangeRequest(OslcMediaType.APPLICATION_RDF_XML);
                break;
        }
    }

    [TestMethod]
    public async Task TestRdfXml()
    {
        const string mediaType = OslcMediaType.APPLICATION_RDF_XML;
        await TestResourceShapeAsync(mediaType);
        await TestCreateAsync(mediaType);
        await Task.WhenAll(new[] {
            TestRetrieveAsync(mediaType),
            TestRetrievesAsync(mediaType),
            TestCompactAsync(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML,
                        mediaType)
        });
        await TestUpdateAsync(mediaType);
        await TestDeleteAsync(mediaType);
    }
}
