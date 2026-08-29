/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using ChangeRequest = OSLC4Net.Domains.ChangeManagement.ChangeRequest;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public sealed class DateTimeOffsetRoundtripTests
{
    [Test]
    public async Task GeneratedChangeRequestDateTimeOffsetPropertiesRoundtrip()
    {
        DateTimeOffset created = new(2026, 7, 9, 10, 11, 12, 345, TimeSpan.FromHours(2));
        DateTimeOffset modified = created.AddMinutes(5);
        DateTimeOffset closeDate = created.AddDays(1);
        ChangeRequest resource = new(new Uri("https://example.com/change-requests/1"))
        {
            Created = created,
            Modified = modified,
            CloseDate = closeDate,
        };

        RdfXmlMediaTypeFormatter formatter = new();
        string rdfXml = await RdfHelpers.SerializeAsync(
            formatter,
            resource,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        ChangeRequest roundTripped = await RdfHelpers.DeserializeAsync<ChangeRequest>(
            formatter,
            rdfXml,
            OslcMediaType.APPLICATION_RDF_XML_TYPE) ?? throw new InvalidOperationException();

        await Assert.That(roundTripped.Created).IsNotNull();
        await Assert.That(roundTripped.Modified).IsNotNull();
        await Assert.That(roundTripped.CloseDate).IsNotNull();
        await Assert.That(roundTripped.Created!.Value.EqualsExact(created)).IsTrue();
        await Assert.That(roundTripped.Modified!.Value.EqualsExact(modified)).IsTrue();
        await Assert.That(roundTripped.CloseDate!.Value.EqualsExact(closeDate)).IsTrue();
    }
}
