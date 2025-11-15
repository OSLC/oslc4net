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
using TUnit.Core;

namespace OSLC4Net.ChangeManagementTest;

[TestFixture]
[Category("RunningOslcServerRequired")]
public class TestChangeManagementRdfXml : TestBase
{
    private readonly RefimplAspireFixture _fixture;

    public TestChangeManagementRdfXml()
    {
        _fixture = new RefimplAspireFixture();
        ServiceProviderCatalogUri = _fixture.ServiceProviderCatalogUriCM;
    }

    [Test]
    public async Task TestRdfXml()
    {
        const string mediaType = OslcMediaType.APPLICATION_RDF_XML;

        // arrange
        await MakeChangeRequestAsync(OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(true);

        // act & assert
        await TestResourceShapeAsync(mediaType).ConfigureAwait(true);
        await TestCreateAsync(mediaType).ConfigureAwait(true);
        await Task.WhenAll(TestRetrieveAsync(mediaType), TestRetrievesAsync(mediaType),
            TestCompactAsync(
                OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML,
                mediaType)).ConfigureAwait(true);
        await TestUpdateAsync(mediaType).ConfigureAwait(true);
        await TestDeleteAsync(mediaType).ConfigureAwait(true);

        // cleanup
        // await DeleteChangeRequestAsync(OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(true);
    }
}
