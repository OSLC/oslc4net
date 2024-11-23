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
public class TestChangeManagementTurtle : TestBase
{
    // protected new readonly ISet<MediaTypeFormatter> FORMATTERS = OslcRestClient.DEFAULT_FORMATTERS;
    public TestContext? TestContext { set; get; }

    [TestInitialize]
    public async Task TestSetup()
    {
        switch (TestContext!.TestName)
        {
            case "TestResourceShape":
            case "TestCreate":
                break;
            default:
                await MakeChangeRequestAsync(OslcMediaType.TEXT_TURTLE);
                break;
        }
    }

    [TestCleanup]
    public void TestTeardown()
    {
        switch (TestContext!.TestName)
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
    public async Task TestResourceShape()
    {
        await TestResourceShapeAsync(OslcMediaType.TEXT_TURTLE);
    }

    /// <summary>
    /// Ordering of test methods shall not be relied upon for execution order
    /// </summary>
    [TestMethod]
    public async Task TestAcceptance()
    {
        const string mediaType = OslcMediaType.TEXT_TURTLE;
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
