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

using OSLC4Net.Core.Model;
using Xunit;

namespace OSLC4Net.ChangeManagementTest;

[Trait("TestCategory", "RunningOslcServerRequired")]
public class TestChangeManagementTurtle : TestBase
{
    private readonly RefimplAspireFixture _fixture;

    public TestChangeManagementTurtle(RefimplAspireFixture fixture, ITestOutputHelper output) :
        base(output)
    {
        _fixture = fixture;
        ServiceProviderCatalogUri = _fixture.ServiceProviderCatalogURI;
    }


    /// <summary>
    ///     Ordering of test methods shall not be relied upon for execution order
    /// </summary>
    [Fact]
    public async Task TestAcceptance()
    {
        const string mediaType = OslcMediaType.TEXT_TURTLE;
        await TestResourceShapeAsync(mediaType);
        await TestCreateAsync(mediaType);
        await Task.WhenAll(TestRetrieveAsync(mediaType), TestRetrievesAsync(mediaType),
            TestCompactAsync(
                OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML,
                mediaType));
        await TestUpdateAsync(mediaType);
        await TestDeleteAsync(mediaType);
    }
}
