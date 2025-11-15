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
using TUnit.Assertions;
using TUnit.Core;

namespace OSLC4Net.Client.Oslc.Resources;

[TestFixture]
public class OslcQueryResultTests
{
    private IHost AppHost { get; set; }
    private ILoggerFactory LoggerFactory { get; set; }

    [BeforeEachTest]
    public void Setup()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(
                builder =>
                {
                    builder.AddTUnitLogger();
                }).Build();
        LoggerFactory = AppHost.Services.GetRequiredService<ILoggerFactory>();
    }

    [Test]
    public async Task OslcQueryMultiResponseTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        await Assert.That(oslcQueryResult.GetMembersUrls().Length).Is.EqualTo(20);
    }

    [Test(Is.Skipped = true, SkipReason = "TODO: figure out if .Current is supposed to work only if the next page is present")]
    public async Task OslcQueryMultiResponsePagingTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();
        await Assert.That(oslcQueryResult.Current?.GetMembersUrls().Length).Is.EqualTo(20);
    }

    [Test]
    public async Task OslcQueryMultiResponseIterTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        // oslcQueryResult.
        foreach (var resultItem in oslcQueryResult.GetMembers<ChangeRequest>())
        {
            TestContext.WriteLine(resultItem.GetAbout().ToString());
        }

        TestContext.WriteLine($"Total: {oslcQueryResult.TotalCount}");

        await Assert.That(oslcQueryResult.GetMembersUrls().Length).Is.EqualTo(20);
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
