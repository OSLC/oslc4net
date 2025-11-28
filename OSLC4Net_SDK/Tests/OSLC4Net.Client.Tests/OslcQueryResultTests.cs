/*******************************************************************************
 * Copyright (c) 2024 Andrii Berezovskyi and oslc4net contributors.
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OSLC4Net.Client.Oslc.Resources;

public class OslcQueryResultTests
{
    private IHost AppHost { get; }
    private ILoggerFactory LoggerFactory { get; }

    public OslcQueryResultTests()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(
                builder =>
                {
                    builder.AddConsole();
                }).Build();
        LoggerFactory = AppHost.Services.GetRequiredService<ILoggerFactory>();
    }

    [Test]
    public async Task OslcQueryMultiResponseTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        await Assert.That(oslcQueryResult.GetMembersUrls().Length).IsEqualTo(20);
    }

    [Test, Skip("TODO: figure out if .Current is supposed to work only if the next page is present")]
    public async Task OslcQueryMultiResponsePagingTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();
        await Assert.That(oslcQueryResult.Current?.GetMembersUrls().Length).IsEqualTo(20);
    }

    [Test]
    public async Task OslcQueryMultiResponseIterTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        // oslcQueryResult.
        foreach (var resultItem in oslcQueryResult.GetMembers<ChangeRequest>())
        {
            Console.WriteLine(resultItem.GetAbout().ToString());
        }

        Console.WriteLine($"Total: {oslcQueryResult.TotalCount}");

        await Assert.That(oslcQueryResult.GetMembersUrls().Length).IsEqualTo(20);
    }

    private async Task<OslcQueryResult> GetMockOslcQueryResultMulti()
    {
        var responseText = await File.ReadAllTextAsync("data/multiResponseQuery.rdf").ConfigureAwait(false);
        var testQuery = new OslcQuery(new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            "https://nordic.clm.ibmcloud.com/ccm/oslc/contexts/_2nC4UBNvEeutmoeSPr3-Ag/workitems");
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseText)
        };
        var oslcQueryResult = new OslcQueryResult(testQuery, httpResponseMessage);
        return oslcQueryResult;
    }
}