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

using System.CommandLine;
using System.Net;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Helpers;
using OSLC4Net.Client.Oslc.Jazz;
using OSLC4Net.Client.Oslc.Resources;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Samples;

/// <summary>
/// Samples of logging in to Enterprise Test Management (ETM) and running OSLC operations
/// 
/// - run an OLSC TestResult query and retrieve OSLC TestResults and de-serialize them as .NET objects
/// - retrieve an OSLC TestResult and print it as XML
/// - create a new TestCase
/// - update an existing TestCase
/// </summary>
sealed class ETMSample : SampleBase<TestResult>
{
    private readonly ILoggerFactory _loggerFactory;

    public ETMSample(ILoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<ETMSample>())
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Entry point for ETM Sample
    /// </summary>
    /// <param name="args"></param>
    public static async Task Run(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var urlOption = new System.CommandLine.Option<string>(name: "--url") { Arity = ArgumentArity.ExactlyOne };
        var userOption = new System.CommandLine.Option<string>(name: "--user") { Arity = ArgumentArity.ExactlyOne };
        var passwordOption = new System.CommandLine.Option<string>(name: "--password") { Arity = ArgumentArity.ExactlyOne };
        var projectOption = new System.CommandLine.Option<string>(name: "--project") { Arity = ArgumentArity.ExactlyOne };

        var rootCommand = new System.CommandLine.RootCommand("ETM Sample");
        rootCommand.Add(urlOption);
        rootCommand.Add(userOption);
        rootCommand.Add(passwordOption);
        rootCommand.Add(projectOption);

        var parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            foreach (var error in parseResult.Errors)
            {
                Console.Error.WriteLine(error.Message);
            }

            return;
        }

        var url = parseResult.GetValue(urlOption)!;
        var user = parseResult.GetValue(userOption)!;
        var password = parseResult.GetValue(passwordOption)!;
        var project = parseResult.GetValue(projectOption)!;

        var sample = new ETMSample(loggerFactory);
        await sample.RunAsync(url, user, password, project).ConfigureAwait(false);
    }

    protected override void PrintResourceInfo(TestResult tr)
    {
        if (tr != null)
        {
            Logger.LogInformation("ID: {Id}, Title: {Title}, Status: {Status}", tr.GetIdentifier(), tr.GetTitle(), tr.GetStatus());
        }
    }

    async Task RunAsync(string webContextUrl, string user, string passwd, string projectArea)
    {
        try
        {

            //STEP 1: Create a new Form Auth client with the supplied user/password
            // ETM auth is on the same base URL usually
            JazzFormAuthClient client = new JazzFormAuthClient(webContextUrl, user, passwd, _loggerFactory.CreateLogger<OslcClient>());

            //STEP 2: Login in to Jazz Server
            if (await client.FormLoginAsync().ConfigureAwait(false) == HttpStatusCode.OK)
            {

                //STEP 3: Initialize a Jazz rootservices helper and indicate we're looking for the QualityManagement catalog
                //ETM contains both Quality and Change Management providers, so need to look for QM specifically
                //For Jazz servers, use the QM-specific property qmServiceProviders in the QM v1.0 namespace
                var rootServicesHelper = new RootServicesHelper(webContextUrl,
                    "http://open-services.net/xmlns/qm/1.0/", "qmServiceProviders");
                var rootServices = await rootServicesHelper.DiscoverAsync(client.GetHttpClient()).ConfigureAwait(false);

                //STEP 4: Get the URL of the OSLC QualityManagement catalog
                String catalogUrl = rootServices.ServiceProviderCatalog;

                //STEP 5: Find the OSLC Service Provider for the project area we want to work with
                String serviceProviderUrl = await client.LookupServiceProviderUrl(catalogUrl, projectArea).ConfigureAwait(false);

                //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
                String queryCapability = await client.LookupQueryCapabilityAsync(serviceProviderUrl,
                                                                      OSLCConstants.OSLC_QM_V2,
                                                                      OSLCConstants.QM_TEST_RESULT_QUERY).ConfigureAwait(false);

                //SCENARIO A: Run a query for all TestResults with a status of passed with OSLC paging of 10 items per
                //page turned on and list the members of the result
                OslcQueryParameters queryParams = new OslcQueryParameters();
                queryParams.SetWhere("oslc_qm:status=\"com.ibm.rqm.execution.common.state.passed\"");
                OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);

                OslcQueryResult result = await query.Submit().ConfigureAwait(false);

                bool processAsDotNetObjects = true;
                await ProcessPagedQueryResultsAsync(result, client, processAsDotNetObjects).ConfigureAwait(false);

                Logger.LogInformation("\n------------------------------\n");

                //SCENARIO B:  Run a query for a specific TestResult selecting only certain 
                //attributes and then print it as raw XML.  Change the dcterms:title below to match a 
                //real TestResult in your ETM project area
                OslcQueryParameters queryParams2 = new OslcQueryParameters();
                queryParams2.SetWhere("dcterms:title=\"Consistent_display_of_currency_Firefox_DB2_WAS_Windows_S1\"");
                queryParams2.SetSelect("dcterms:identifier,dcterms:title,dcterms:creator,dcterms:created,oslc_qm:status");
                OslcQuery query2 = new OslcQuery(client, queryCapability, queryParams2);

                OslcQueryResult result2 = await query2.Submit().ConfigureAwait(false);
                HttpResponseMessage rawResponse = result2.GetRawResponse();
                await ProcessRawResponseAsync(rawResponse).ConfigureAwait(false);
                rawResponse.ConsumeContent();

                //SCENARIO C:  ETM TestCase creation and update
                TestCase testcase = new TestCase();
                testcase.SetTitle("Accessibility verification using a screen reader");
                testcase.SetDescription("This test case uses a screen reader application to ensure that the web browser content fully complies with accessibility standards");
                testcase.AddTestsChangeRequest(new Link(new Uri("http://cmprovider/changerequest/1"), "Implement accessibility in Pet Store application"));

                //Get the Creation Factory URL for test cases so that we can create a test case
                String testcaseCreation = await client.LookupCreationFactoryAsync(
                        serviceProviderUrl, OSLCConstants.OSLC_QM_V2,
                        testcase.Types.First().ToString()).ConfigureAwait(false);

                //Create the test case
                HttpResponseMessage creationResponse = await client.CreateResourceRawAsync(
                        testcaseCreation, testcase,
                        OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false);
                creationResponse.ConsumeContent();
                String testcaseLocation = creationResponse.Headers.Location.ToString();
                Logger.LogInformation("Test Case created a location {Location}", testcaseLocation);

                //Get the test case from the service provider and update its title property 
                testcase = await (await client.GetResourceRawAsync(testcaseLocation,
                        OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false)).Content.ReadAsAsync<TestCase>(client.GetFormatters()).ConfigureAwait(false);
                testcase.SetTitle(testcase.GetTitle() + " (updated)");

                //Create a partial update URL so that only the title will be updated.
                //Assuming (for readability) that the test case URL does not already contain a '?'
                String updateUrl = testcase.GetAbout() + "?oslc.properties=dcterms:title";

                //Update the test case at the service provider
                (await client.UpdateResourceRawAsync(new Uri(updateUrl), testcase,
                    OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false)).ConsumeContent();

            }
            else
            {
                throw new InvalidOperationException("Authentication failed");
            }
        }
        catch (RootServicesException re)
        {
            Logger.LogError(re, "Unable to access the Jazz rootservices document at: {Url}", webContextUrl + "/rootservices");
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{Message}", e.Message);
            throw;
        }
    }
}
