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
using OSLC4Net.Domains.RequirementsManagement;
using Xunit;
using Constants = OSLC4Net.ChangeManagement.Constants;

namespace OSLC4Net.ChangeManagementTest;

using DC = OslcConstants.Domains.DCElements.Q;
using PROV = OslcConstants.Domains.PROV.Q;
using DCTerms = OslcConstants.Domains.DCTerms.Q;

[Trait("TestCategory", "RunningOslcServerRequired")]
public class TestRequirementsManagementTurtle : TestBase
{
    private readonly RefimplAspireFixture _fixture;
    private readonly string MediaType = OslcMediaType.TEXT_TURTLE;

    public TestRequirementsManagementTurtle(RefimplAspireFixture fixture, ITestOutputHelper output)
        :
        base(output)
    {
        _fixture = fixture;
        ServiceProviderCatalogUri = _fixture.ServiceProviderCatalogUriRM;
    }

    [Fact]
    public async Task TestCreateRequirement()
    {
        Requirement resource = new()
        {
            Identifier = "REQ-001",
            Title = "Test requirement",
            Constrains =
                [new Uri("http://example.com/REQ-002"), new Uri("http://example.com/REQ-003")],
            Creator = [new Uri("https://github.com/berezovskyi")],
            ExtendedProperties =
            {
                [DCTerms.Description] = "A sample description",
                [PROV.AtLocation] =
                    new Uri("http://dbpedia.org/resource/Stockholm"),
                [DC.Language] = "en-GB"
            }
        };

        var creation = await GetCreationAsync(MediaType, Constants.TYPE_REQUIREMENT)
            .ConfigureAwait(true);

        var newRequirement = await TestClient
            .CreateResourceAsync(creation, resource, MediaType)
            .ConfigureAwait(true);
        var createdResource = newRequirement.Resources?.SingleOrDefault();

        Assert.NotNull(createdResource);
        Assert.Equal(resource.Title, createdResource?.Title);
        Assert.Equal(resource.Identifier, createdResource?.Identifier);
        Assert.Equal(resource.ExtendedProperties[DCTerms.Description],
            createdResource?.Description);
        Assert.Equal(resource.ExtendedProperties[PROV.AtLocation],
            createdResource?.ExtendedProperties[PROV.AtLocation]);
        Assert.Equal(resource.ExtendedProperties[DC.Language],
            createdResource?.ExtendedProperties[DC.Language]);
        Assert.Equal(2, createdResource?.Constrains.Count);
    }
}
