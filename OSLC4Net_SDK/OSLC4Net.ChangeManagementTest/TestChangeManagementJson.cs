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

using System.Net.Http.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSLC4Net.Core.JsonProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagementTest;

// FIXME: re-enable
// [TestClass]
public class TestChangeManagementJson : TestBase
{
    protected override IEnumerable<MediaTypeFormatter> Formatters { get; } =
        new HashSet<MediaTypeFormatter>() { new OslcJsonMediaTypeFormatter() };

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
                await MakeChangeRequestAsync(OslcMediaType.APPLICATION_JSON);
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
                DeleteChangeRequest(OslcMediaType.APPLICATION_JSON);
                break;
        }
    }

    [TestMethod]
    public async Task TestResourceShape()
    {
        await TestResourceShapeAsync(OslcMediaType.APPLICATION_JSON);
    }

    [TestMethod]
    public async Task TestCreate()
    {
        await TestCreateAsync(OslcMediaType.APPLICATION_JSON);
    }

    [TestMethod]
    public async Task TestRetrieve()
    {
        await TestRetrieveAsync(OslcMediaType.APPLICATION_JSON);
    }

    [TestMethod]
    public async Task TestRetrieves()
    {
        await TestRetrievesAsync(OslcMediaType.APPLICATION_JSON);
    }

    [TestMethod]
    public async Task TestCompact()
    {
        await TestCompactAsync(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML,
                    OslcMediaType.APPLICATION_JSON);
    }

    [TestMethod]
    public async Task TestUpdate()
    {
        await TestUpdateAsync(OslcMediaType.APPLICATION_JSON);
    }

    [TestMethod]
    public async Task TestDelete()
    {
        await TestDeleteAsync(OslcMediaType.APPLICATION_JSON);
    }
}
