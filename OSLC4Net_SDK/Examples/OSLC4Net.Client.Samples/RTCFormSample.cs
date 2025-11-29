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
using VDS.RDF;
using OSLC4Net.Core.DotNetRdfProvider;

namespace OSLC4Net.Client.Samples
{
    /// <summary>
    /// Samples of logging in to Rational Team Concert and running OSLC operations
    /// 
    /// 
    /// - run an OLSC ChangeRequest query and retrieve OSLC ChangeRequests and de-serialize them as .NET objects
    /// - retrieve an OSLC ChangeRequest and print it as XML
    /// - create a new ChangeRequest
    /// - update an existing ChangeRequest
    /// </summary>
    class RTCFormSample
    {
        private static ILogger logger;

        /// <summary>
        /// Login to the RTC server and perform some OSLC actions
        /// </summary>
        /// <param name="args"></param>
	    static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            logger = loggerFactory.CreateLogger<RTCFormSample>();

            var urlOption = new Option<string>("--url", "The URL of the server") { IsRequired = true };
            var userOption = new Option<string>("--user", "The user name") { IsRequired = true };
            var passwordOption = new Option<string>("--password", "The password") { IsRequired = true };
            var projectOption = new Option<string>("--project", "The project area") { IsRequired = true };

            var rootCommand = new RootCommand("RTC Sample");
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
		
			    //STEP 1: Initialize a Jazz rootservices helper and indicate we're looking for the ChangeManagement catalog
			    //RTC contains a service provider for CM and SCM, so we need to indicate our interest in CM
			    var rootServicesHelper = new RootServicesHelper(webContextUrl,OSLCConstants.OSLC_CM_V2);
                var rootServices = await rootServicesHelper.DiscoverAsync();
			
			    //STEP 2: Create a new Form Auth client with the supplied user/password
			    JazzFormAuthClient client = new JazzFormAuthClient(webContextUrl, user, passwd, loggerFactory.CreateLogger<OslcClient>());
			
			    //STEP 3: Login in to Jazz Server
			    if (await client.FormLoginAsync() == HttpStatusCode.OK) {
				
				    //STEP 4: Get the URL of the OSLC ChangeManagement catalog
				    String catalogUrl = rootServices.ServiceProviderCatalog;
				
				    //STEP 5: Find the OSLC Service Provider for the project area we want to work with
				    String serviceProviderUrl = await client.LookupServiceProviderUrl(catalogUrl, projectArea);
				
				    //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
				    String queryCapability = await client.LookupQueryCapabilityAsync(serviceProviderUrl,
																	      OSLCConstants.OSLC_CM_V2,
																	      OSLCConstants.CM_CHANGE_REQUEST_TYPE);
				
				    //SCENARIO A: Run a query for all open ChangeRequests with OSLC paging of 10 items per
				    //page turned on and list the members of the result
				    OslcQueryParameters queryParams = new OslcQueryParameters();
				    queryParams.SetWhere("oslc_cm:closed=false");
                    queryParams.SetSelect("dcterms:identifier,dcterms:title,oslc_cm:status");
                    OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);
				
				    OslcQueryResult result = await query.Submit();
				
				    bool processAsJavaObjects = true;
				    await ProcessPagedQueryResultsAsync(result,client, processAsJavaObjects);
				
				    Console.WriteLine("\n------------------------------\n");
				
				    //SCENARIO B:  Run a query for a specific ChangeRequest selecting only certain 
				    //attributes and then print it as raw XML.  Change the dcterms:identifier below to match a 
				    //real workitem in your RTC project area
				    OslcQueryParameters queryParams2 = new OslcQueryParameters();
				    queryParams2.SetWhere("dcterms:identifier=\"10\"");
				    queryParams2.SetSelect("dcterms:identifier,dcterms:title,dcterms:creator,dcterms:created,oslc_cm:status");
				    OslcQuery query2 = new OslcQuery(client, queryCapability, queryParams2);
				
				    OslcQueryResult result2 = await query2.Submit();
				    HttpResponseMessage rawResponse = result2.GetRawResponse();
				    await ProcessRawResponseAsync(rawResponse);
				    rawResponse.ConsumeContent();
				
				    //SCENARIO C:  RTC Workitem creation and update
				    ChangeRequest changeRequest = new ChangeRequest();
				    changeRequest.SetTitle("Implement accessibility in Pet Store application");
				    changeRequest.SetDescription("Image elements must provide a description in the 'alt' attribute for consumption by screen readers.");
				    changeRequest.AddTestedByTestCase(new Link(new Uri("http://qmprovider/testcase/1"), "Accessibility verification using a screen reader"));
				    changeRequest.AddDctermsType("task");
				
				    //Get the Creation Factory URL for change requests so that we can create one
				    String changeRequestCreation = await client.LookupCreationFactoryAsync(
						    serviceProviderUrl, OSLCConstants.OSLC_CM_V2,
						    changeRequest.GetRdfTypes()[0].ToString());

				    //Create the change request
				    HttpResponseMessage creationResponse = await client.CreateResourceRawAsync(
						    changeRequestCreation, changeRequest,
						    OslcMediaType.APPLICATION_RDF_XML,
						    OslcMediaType.APPLICATION_RDF_XML);

                    if (creationResponse.StatusCode != HttpStatusCode.Created)
                    {
                        String errorString = await creationResponse.Content.ReadAsStringAsync();
                        Console.Error.WriteLine("Failed to create change request: " + errorString);
                        return;
                    }

				    String changeRequestLocation = creationResponse.Headers.Location.ToString();
				    creationResponse.ConsumeContent();
				    Console.WriteLine("Change Request created a location " + changeRequestLocation);
				
				
				    //Get the change request from the service provider and update its title property 
				    changeRequest = await (await client.GetResourceRawAsync(changeRequestLocation,
						    OslcMediaType.APPLICATION_RDF_XML)).Content.ReadAsAsync<ChangeRequest>(client.GetFormatters());
				    changeRequest.SetTitle(changeRequest.GetTitle() + " (updated)");

				    //Create a partial update URL so that only the title will be updated.
				    //Assuming (for readability) that the change request URL does not already contain a '?'
				    String updateUrl = changeRequest.GetAbout() + "?oslc.properties=dcterms:title";
				
				    //Update the change request at the service provider
				    HttpResponseMessage updateResponse = await client.UpdateResourceRawAsync(
						    new Uri(updateUrl), changeRequest,
						    OslcMediaType.APPLICATION_RDF_XML,
						    OslcMediaType.APPLICATION_RDF_XML);
				
				    updateResponse.ConsumeContent();
							
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
	
	    private static Task ProcessCurrentPageAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects)
        {
            foreach (ChangeRequest cr in result.GetMembers<ChangeRequest>())
            {
			    Console.WriteLine("id: " + cr.GetIdentifier() + ", title: " + cr.GetTitle() + ", status: " + cr.GetStatus());			
		    }
            return Task.CompletedTask;
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
	
	    private static void PrintChangeRequestInfo(ChangeRequest cr) {
		    //See the OSLC4J ChangeRequest class for a full list of attributes you can access.
		    if (cr != null) {
			    Console.WriteLine("ID: " + cr.GetIdentifier() + ", Title: " + cr.GetTitle() + ", Status: " + cr.GetStatus());
		    }
	    }
    }
}
