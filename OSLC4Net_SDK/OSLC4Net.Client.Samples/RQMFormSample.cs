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
using log4net;
using Microsoft.Test.CommandLineParsing;
using OSLC4Net.Client.Oslc.Jazz;
using OSLC4Net.Client.Oslc;
using System.Net;
using OSLC4Net.Client.Oslc.Resources;
using System.Net.Http;
using OSLC4Net.Core.Model;
using OSLC4Net.Client.Exceptions;
using System.IO;

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
        private static ILog logger = LogManager.GetLogger(typeof(RQMFormSample));

        /// <summary>
        /// Login to the RQM server and perform some OSLC actions
        /// </summary>
        /// <param name="args"></param>
	    static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

		    CommandLineDictionary cmd = CommandLineDictionary.FromArguments(args);

		    if (!ValidateOptions(cmd)) {		
			    logger.Error("Syntax:  /url=https://<server>:port/<context>/ /user=<user> /password=<password> /project=\"<project_area>\"");
			    logger.Error("Example: /url=https://exmple.com:9443/qm /user=ADMIN /password=ADMIN /project=\"JKE Banking (Quality Management)\"");
			    return;
		    }
			
		    String webContextUrl = cmd["url"];
		    String user = cmd["user"];
		    String passwd = cmd["password"];
		    String projectArea = cmd["project"];		
		
		    try {
		
			    //STEP 1: Initialize a Jazz rootservices helper and indicate we're looking for the QualityManagement catalog
			    //RQM contains both Quality and Change Management providers, so need to look for QM specifically
			    JazzRootServicesHelper helper = new JazzRootServicesHelper(webContextUrl,OSLCConstants.OSLC_QM_V2);
			
			    //STEP 2: Create a new Form Auth client with the supplied user/password
			    JazzFormAuthClient client = helper.InitFormClient(user, passwd);
			
			    //STEP 3: Login in to Jazz Server
			    if (client.FormLogin() == HttpStatusCode.OK) {
				
				    //STEP 4: Get the URL of the OSLC QualityManagement catalog
				    String catalogUrl = helper.GetCatalogUrl();
				
				    //STEP 5: Find the OSLC Service Provider for the project area we want to work with
				    String serviceProviderUrl = client.LookupServiceProviderUrl(catalogUrl, projectArea);
				
				    //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
				    String queryCapability = client.LookupQueryCapability(serviceProviderUrl,
																	      OSLCConstants.OSLC_QM_V2,
																	      OSLCConstants.QM_TEST_RESULT_QUERY);
				
				    //SCENARIO A: Run a query for all TestResults with a status of passed with OSLC paging of 10 items per
				    //page turned on and list the members of the result
				    OslcQueryParameters queryParams = new OslcQueryParameters();
				    queryParams.SetWhere("oslc_qm:status=\"com.ibm.rqm.execution.common.state.passed\"");
				    OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);
				
				    OslcQueryResult result = query.Submit();
				
				    bool processAsDotNetObjects = true;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				
				    Console.WriteLine("\n------------------------------\n");
				
				    //SCENARIO B:  Run a query for a specific TestResult selecting only certain 
				    //attributes and then print it as raw XML.  Change the dcterms:title below to match a 
				    //real TestResult in your RQM project area
				    OslcQueryParameters queryParams2 = new OslcQueryParameters();
				    queryParams2.SetWhere("dcterms:title=\"Consistent_display_of_currency_Firefox_DB2_WAS_Windows_S1\"");
				    queryParams2.SetSelect("dcterms:identifier,dcterms:title,dcterms:creator,dcterms:created,oslc_qm:status");
				    OslcQuery query2 = new OslcQuery(client, queryCapability, queryParams2);
				
				    OslcQueryResult result2 = query2.Submit();
				    HttpResponseMessage rawResponse = result2.GetRawResponse();
				    ProcessRawResponse(rawResponse);
				    rawResponse.ConsumeContent();
				
				    //SCENARIO C:  RQM TestCase creation and update
				    TestCase testcase = new TestCase();
				    testcase.SetTitle("Accessibility verification using a screen reader");
				    testcase.SetDescription("This test case uses a screen reader application to ensure that the web browser content fully complies with accessibility standards");
				    testcase.AddTestsChangeRequest(new Link(new Uri("http://cmprovider/changerequest/1"), "Implement accessibility in Pet Store application"));
				
				    //Get the Creation Factory URL for test cases so that we can create a test case
				    String testcaseCreation = client.LookupCreationFactory(
						    serviceProviderUrl, OSLCConstants.OSLC_QM_V2,
						    testcase.GetRdfTypes()[0].ToString());

				    //Create the test case
				    HttpResponseMessage creationResponse = client.CreateResource(
						    testcaseCreation, testcase,
						    OslcMediaType.APPLICATION_RDF_XML);
				    creationResponse.ConsumeContent();
				    String testcaseLocation = creationResponse.Headers.Location.ToString();
				    Console.WriteLine("Test Case created a location " + testcaseLocation);
				
				    //Get the test case from the service provider and update its title property 
				    testcase = client.GetResource(testcaseLocation,
						    OslcMediaType.APPLICATION_RDF_XML).Content.ReadAsAsync<TestCase>(client.GetFormatters()).Result;
				    testcase.SetTitle(testcase.GetTitle() + " (updated)");

				    //Create a partial update URL so that only the title will be updated.
				    //Assuming (for readability) that the test case URL does not already contain a '?'
				    String updateUrl = testcase.GetAbout() + "?oslc.properties=dcterms:title";
				
				    //Update the test case at the service provider
				    client.UpdateResource(updateUrl, testcase,
						    OslcMediaType.APPLICATION_RDF_XML).ConsumeContent();				
				
							
			    }
		    } catch (RootServicesException re) {
			    logger.Error("Unable to access the Jazz rootservices document at: " + webContextUrl + "/rootservices", re);
		    } catch (Exception e) {
			    logger.Error(e.Message,e);
		    }
	    }
	
	    private static void ProcessPagedQueryResults(OslcQueryResult result, OslcClient client, bool asDotNetObjects) {
		    int page = 1;
		    do {
			    Console.WriteLine("\nPage " + page + ":\n");
			    ProcessCurrentPage(result,client,asDotNetObjects);
			    if (result.MoveNext()) {
				    result = result.Current;
				    page++;
			    } else {
				    break;
			    }
		    } while(true);
	    }
	
	    private static void ProcessCurrentPage(OslcQueryResult result, OslcClient client, bool asDotNetObjects) {
		
		    foreach (String resultsUrl in result.GetMembersUrls()) {
			    Console.WriteLine(resultsUrl);
			
			    HttpResponseMessage response = null;
			    try {
				
				    //Get a single artifact by its URL 
				    response = client.GetResource(resultsUrl, OSLCConstants.CT_RDF);
		
				    if (response != null) {
					    //De-serialize it as a .NET object 
					    if (asDotNetObjects) {
						       TestResult tr = response.Content.ReadAsAsync<TestResult>(client.GetFormatters()).Result;
						       PrintTestResultInfo(tr);   //print a few attributes
					    } else {
						
						    //Just print the raw RDF/XML (or process the XML as desired)
						    ProcessRawResponse(response);
						
					    }
				    }
			    } catch (Exception e) {
				    logger.Error( "Unable to process artifact at url: " + resultsUrl, e);
			    }
			
		    }
		
	    }
	
	    private static void ProcessRawResponse(HttpResponseMessage response)
        {
		    Stream inStream = response.Content.ReadAsStreamAsync().Result;
		    StreamReader streamReader = new StreamReader(new BufferedStream(inStream), Encoding.UTF8);
		
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
	
	    private static bool ValidateOptions(CommandLineDictionary cmd) {
		    bool isValid = true;
		
		    if (! (cmd.ContainsKey("url") &&
                   cmd.ContainsKey("user") &&
                   cmd.ContainsKey("password") &&
                   cmd.ContainsKey("project") &&
                   cmd.Count == 4))
            {			  
			    isValid = false;
		    }
		    return isValid;		
	    }
    }
}
