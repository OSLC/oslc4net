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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.CommandLine;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Oslc.Jazz;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Resources;
using OSLC4Net.Client.Oslc.Helpers;
using OSLC4Net.Core.Model;
using OSLC4Net.Client.Exceptions;

namespace OSLC4Net.Client.Samples
{
    /// <summary>
    /// Samples of logging in to Rational Quality Manager and running OSLC operations
    /// 
    /// - run an OLSC TestResult query and retrieve OSLC TestResults and de-serialize them as .NET objects
    /// - retrieve an OSLC TestResult and print it as XML
    /// - create a new TestCase
    /// - update an existing TestCase
    /// </summary>
    class RQMFormSample
    {
        private static ILogger logger;

        /// <summary>
        /// Login to the RQM server and perform some OSLC actions
        /// </summary>
        /// <param name="args"></param>
	    static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            logger = loggerFactory.CreateLogger<RQMFormSample>();

            var urlOption = new Option<string>(
                name: "--url",
                description: "The URL of the server") { IsRequired = true };
            var userOption = new Option<string>(
                name: "--user",
                description: "The user name") { IsRequired = true };
            var passwordOption = new Option<string>(
                name: "--password",
                description: "The password") { IsRequired = true };
            var projectOption = new Option<string>(
                name: "--project",
                description: "The project area") { IsRequired = true };

            var rootCommand = new RootCommand("RQM Sample");
            rootCommand.AddOption(urlOption);
            rootCommand.AddOption(userOption);
            rootCommand.AddOption(passwordOption);
            rootCommand.AddOption(projectOption);

            rootCommand.SetHandler(async (string url, string user, string password, string project) =>
            {
                await RunAsync(url, user, password, project, loggerFactory);
            }, urlOption, userOption, passwordOption, projectOption);

            await rootCommand.InvokeAsync(args);
	    }

        static async Task RunAsync(string webContextUrl, string user, string passwd, string projectArea, ILoggerFactory loggerFactory)
        {
		    try {
		
			    //STEP 1: Initialize a Jazz rootservices helper and indicate we're looking for the QualityManagement catalog
			    //RQM contains both Quality and Change Management providers, so need to look for QM specifically
                // Replaced JazzRootServicesHelper with RootServicesHelper
			    var rootServicesHelper = new RootServicesHelper(webContextUrl, OSLCConstants.OSLC_QM_V2);
                var rootServices = await rootServicesHelper.DiscoverAsync();
			
			    //STEP 2: Create a new Form Auth client with the supplied user/password
                // RQM auth is on the same base URL usually
			    JazzFormAuthClient client = new JazzFormAuthClient(webContextUrl, user, passwd, loggerFactory.CreateLogger<OslcClient>());
			
			    //STEP 3: Login in to Jazz Server
			    if (await client.FormLoginAsync() == HttpStatusCode.OK) {
				
				    //STEP 4: Get the URL of the OSLC QualityManagement catalog
				    String catalogUrl = rootServices.ServiceProviderCatalog;
				
				    //STEP 5: Find the OSLC Service Provider for the project area we want to work with
				    String serviceProviderUrl = await client.LookupServiceProviderUrl(catalogUrl, projectArea);
				
				    //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
				    String queryCapability = await client.LookupQueryCapabilityAsync(serviceProviderUrl,
																	      OSLCConstants.OSLC_QM_V2,
																	      OSLCConstants.QM_TEST_RESULT_QUERY);
				
				    //SCENARIO A: Run a query for all TestResults with a status of passed with OSLC paging of 10 items per
				    //page turned on and list the members of the result
				    OslcQueryParameters queryParams = new OslcQueryParameters();
				    queryParams.SetWhere("oslc_qm:status=\"com.ibm.rqm.execution.common.state.passed\"");
				    OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);
				
				    OslcQueryResult result = await query.Submit();
				
				    bool processAsDotNetObjects = true;
				    await ProcessPagedQueryResultsAsync(result,client, processAsDotNetObjects);
				
				    Console.WriteLine("\n------------------------------\n");
				
				    //SCENARIO B:  Run a query for a specific TestResult selecting only certain 
				    //attributes and then print it as raw XML.  Change the dcterms:title below to match a 
				    //real TestResult in your RQM project area
				    OslcQueryParameters queryParams2 = new OslcQueryParameters();
				    queryParams2.SetWhere("dcterms:title=\"Consistent_display_of_currency_Firefox_DB2_WAS_Windows_S1\"");
				    queryParams2.SetSelect("dcterms:identifier,dcterms:title,dcterms:creator,dcterms:created,oslc_qm:status");
				    OslcQuery query2 = new OslcQuery(client, queryCapability, queryParams2);
				
				    OslcQueryResult result2 = await query2.Submit();
				    HttpResponseMessage rawResponse = result2.GetRawResponse();
				    await ProcessRawResponseAsync(rawResponse);
				    rawResponse.ConsumeContent();
				
				    //SCENARIO C:  RQM TestCase creation and update
				    TestCase testcase = new TestCase();
				    testcase.SetTitle("Accessibility verification using a screen reader");
				    testcase.SetDescription("This test case uses a screen reader application to ensure that the web browser content fully complies with accessibility standards");
				    testcase.AddTestsChangeRequest(new Link(new Uri("http://cmprovider/changerequest/1"), "Implement accessibility in Pet Store application"));
				
				    //Get the Creation Factory URL for test cases so that we can create a test case
				    String testcaseCreation = await client.LookupCreationFactoryAsync(
						    serviceProviderUrl, OSLCConstants.OSLC_QM_V2,
						    testcase.GetRdfTypes()[0].ToString());

				    //Create the test case
				    HttpResponseMessage creationResponse = await client.CreateResourceRawAsync(
						    testcaseCreation, testcase,
						    OslcMediaType.APPLICATION_RDF_XML);
				    creationResponse.ConsumeContent();
				    String testcaseLocation = creationResponse.Headers.Location.ToString();
				    Console.WriteLine("Test Case created a location " + testcaseLocation);
				
				    //Get the test case from the service provider and update its title property 
				    testcase = await (await client.GetResourceRawAsync(testcaseLocation,
						    OslcMediaType.APPLICATION_RDF_XML)).Content.ReadAsAsync<TestCase>(client.GetFormatters());
				    testcase.SetTitle(testcase.GetTitle() + " (updated)");

				    //Create a partial update URL so that only the title will be updated.
				    //Assuming (for readability) that the test case URL does not already contain a '?'
				    String updateUrl = testcase.GetAbout() + "?oslc.properties=dcterms:title";
				
				    //Update the test case at the service provider
				    (await client.UpdateResourceRawAsync(new Uri(updateUrl), testcase,
						    OslcMediaType.APPLICATION_RDF_XML)).ConsumeContent();
				
							
			    }
		    } catch (RootServicesException re) {
			    logger.LogError(re, "Unable to access the Jazz rootservices document at: " + webContextUrl + "/rootservices");
		    } catch (Exception e) {
			    logger.LogError(e, e.Message);
		    }
        }
	
	    private static async Task ProcessPagedQueryResultsAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects) {
		    int page = 1;
		    do {
			    Console.WriteLine("\nPage " + page + ":\n");
			    await ProcessCurrentPageAsync(result,client,asDotNetObjects);
			    if (result.MoveNext()) {
				    result = result.Current;
				    page++;
			    } else {
				    break;
			    }
		    } while(true);
	    }
	
	    private static async Task ProcessCurrentPageAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects) {
		
		    foreach (String resultsUrl in result.GetMembersUrls()) {
			    Console.WriteLine(resultsUrl);
			
			    HttpResponseMessage response = null;
			    try {
				
				    //Get a single artifact by its URL 
				    response = await client.GetResourceRawAsync(resultsUrl, OSLCConstants.CT_RDF);
		
				    if (response != null) {
					    //De-serialize it as a .NET object 
					    if (asDotNetObjects) {
						       TestResult tr = await response.Content.ReadAsAsync<TestResult>(client.GetFormatters());
						       PrintTestResultInfo(tr);   //print a few attributes
					    } else {
						
						    //Just print the raw RDF/XML (or process the XML as desired)
						    await ProcessRawResponseAsync(response);
						
					    }
				    }
			    } catch (Exception e) {
				    logger.LogError(e, "Unable to process artifact at url: " + resultsUrl);
			    }
			
		    }
		
	    }
	
	    private static async Task ProcessRawResponseAsync(HttpResponseMessage response)
        {
		    Stream inStream = await response.Content.ReadAsStreamAsync();
		    StreamReader streamReader = new StreamReader(new BufferedStream(inStream), System.Text.Encoding.UTF8);
		
		    String line = null;
            while ((line = streamReader.ReadLine()) != null)
            {
		      Console.WriteLine(line);
		    }
		    Console.WriteLine();
		    response.ConsumeContent();
	    }
	
	    private static void PrintTestResultInfo(TestResult tr) {
		    //See the OSLC4J TestResult class for a full list of attributes you can access.
		    if (tr != null) {
			    Console.WriteLine("ID: " + tr.GetIdentifier() + ", Title: " + tr.GetTitle() + ", Status: " + tr.GetStatus());
		    }
	    }
    }
}
