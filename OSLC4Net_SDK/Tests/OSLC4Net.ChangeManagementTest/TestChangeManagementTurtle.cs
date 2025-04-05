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

using Aspire.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagementTest;

[TestClass]
public class TestChangeManagementTurtle : TestBase
{
    // protected new readonly ISet<MediaTypeFormatter> FORMATTERS = OslcRestClient.DEFAULT_FORMATTERS;
    public TestContext? TestContext { set; get; }

    static DistributedApplication? _distributedApplication;

    [ClassInitialize]
    public static async Task ClassSetupAsync(TestContext ctx)
    {
        _distributedApplication = await SetupAspireAsync().ConfigureAwait(false);

        Thread.Sleep(30000); // Wait for the server to start
    }

    [ClassCleanup]
    public static async Task ClassCleanupAsync()
    {
        if (_distributedApplication is not null)
        {
            await _distributedApplication.DisposeAsync().ConfigureAwait(false);
        }
    }

    [TestInitialize]
    public async Task TestSetup()
    {
        switch (TestContext!.TestName)
        {
            case "TestResourceShape":
            case "TestCreate":
                break;
            default:
                await MakeChangeRequestAsync(OslcMediaType.TEXT_TURTLE).ConfigureAwait(false);
                break;
        }
    }

    [TestCleanup]
    public async Task TestTeardown()
    {
        if ((TestContext!.TestName == "TestResourceShape") || (TestContext!.TestName == "TestDelete"))
        {
            // they remove the resource at the end
        }
        else
        {
            if (CREATED_CHANGE_REQUEST_URI is not null)
            {
                DeleteChangeRequest(OslcMediaType.TEXT_TURTLE);
            }
            else
            {
                // TODO: log warning
            }
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
