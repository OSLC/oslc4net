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
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace OSLC4Net.Client.Oslc.Resources;

public class OslcQueryResultTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private IHost AppHost { get; }
    private ILoggerFactory LoggerFactory { get; }

    public OslcQueryResultTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(
                builder =>
                {
                    builder.Services.AddSingleton<ILoggerProvider>(
                        new XUnitLoggerProvider(testOutputHelper, false));
                }).Build();
        LoggerFactory = AppHost.Services.GetRequiredService<ILoggerFactory>();
    }

    [Fact]
    public async Task OslcQueryMultiResponseTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        Assert.Equal(20, oslcQueryResult.GetMembersUrls().Length);
    }

    [Fact(Skip =
        "TODO: figure out if .Current is supposed to work only if the next page is present")]
    public async Task OslcQueryMultiResponsePagingTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();
        Assert.Equal(20, oslcQueryResult.Current?.GetMembersUrls().Length);
    }


    [Fact]
    public async Task OslcQueryMultiResponseIterTest()
    {
        var oslcQueryResult = await GetMockOslcQueryResultMulti();

        // oslcQueryResult.
        foreach (var resultItem in oslcQueryResult.GetMembers<ChangeRequest>())
        {
            _testOutputHelper.WriteLine(resultItem.GetAbout().ToString());
        }

        _testOutputHelper.WriteLine($"Total: {oslcQueryResult.TotalCount}");

        Assert.Equal(20, oslcQueryResult.GetMembersUrls().Length);
    }

    private async Task<OslcQueryResult> GetMockOslcQueryResultMulti()
    {
        var responseText = await File.ReadAllTextAsync("data/multiResponseQuery.rdf");
        var testQuery = new OslcQuery(new OslcClient(LoggerFactory.CreateLogger<OslcClient>()),
            "https://nordic.clm.ibmcloud.com/ccm/oslc/contexts/_2nC4UBNvEeutmoeSPr3-Ag/workitems");
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK, Content = new StringContent(responseText)
        };
        var oslcQueryResult = new OslcQueryResult(testQuery, httpResponseMessage);
        return oslcQueryResult;
    }
}
