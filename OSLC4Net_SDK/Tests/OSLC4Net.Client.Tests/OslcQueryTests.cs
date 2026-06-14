/*******************************************************************************
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 *******************************************************************************/

using System.Net;
using Microsoft.Extensions.Logging.Abstractions;

namespace OSLC4Net.Client.Oslc.Resources;

public class OslcQueryTests
{
    [Test]
    public async Task MemberRelationMustBeAnAbsoluteUri()
    {
        using var client = new OslcClient(
            new HttpClient(),
            NullLogger<OslcClient>.Instance);

        await Assert.That(() =>
            _ = new OslcQuery(
                client,
                "https://example.test/oslc/query",
                0,
                new OslcQueryParameters(),
                new Uri("relative", UriKind.Relative)))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SubmitPostUsesFormEncodedBodyWithoutDoubleEncoding()
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedBody = null;
        var handler = new OSLC4Net.Client.Tests.FakeHttpMessageHandler(async (request, _) =>
        {
            capturedRequest = request;
            capturedBody = await request.Content!.ReadAsStringAsync();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    <?xml version="1.0" encoding="UTF-8"?>
                    <rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" />
                    """)
            };
        });
        using var httpClient = new HttpClient(handler);
        using var client = new OslcClient(httpClient, NullLogger<OslcClient>.Instance);
        var parameters = new OslcQueryParameters();
        parameters.SetWhere("dcterms:title=\"two words\"");
        parameters.SetSelect("dcterms:title");
        var query = new OslcQuery(
            client,
            "https://example.test/oslc/query",
            25,
            parameters);

        await query.SubmitPost();

        await Assert.That(capturedRequest).IsNotNull();
        await Assert.That(capturedRequest!.Method).IsEqualTo(HttpMethod.Post);
        await Assert.That(capturedRequest.RequestUri).IsEqualTo(new Uri("https://example.test/oslc/query"));
        await Assert.That(capturedRequest.Content!.Headers.ContentType!.MediaType)
            .IsEqualTo("application/x-www-form-urlencoded");
        await Assert.That(capturedBody).Contains("oslc.where=dcterms%3Atitle%3D%22two%20words%22");
        await Assert.That(capturedBody).DoesNotContain("%2520");
        await Assert.That(capturedBody).Contains("oslc.pageSize=25");
    }
}
