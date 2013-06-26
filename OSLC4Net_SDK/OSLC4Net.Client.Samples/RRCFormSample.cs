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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using log4net;
using Microsoft.Test.CommandLineParsing;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Jazz;
using OSLC4Net.Client.Oslc.Resources;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Samples
{
    /// <summary>
    /// Samples of logging in to Rational Requirements Composer and running OSLC operations
    /// 
    /// 
    /// - run an OLSC Requirement query and retrieve OSLC Requirements and de-serialize them as .NET objects
    /// - TODO:  Add more requirement sample scenarios
    /// </summary>
    class RRCFormSample
    {
        private static ILog logger = LogManager.GetLogger(typeof(RRCFormSample));
	
	    // Following is a workaround for primaryText issue in DNG ( it is PrimaryText instead of primaryText 
	    private static readonly QName PROPERTY_PRIMARY_TEXT_WORKAROUND   = new QName(RmConstants.JAZZ_RM_NAMESPACE, "PrimaryText");

        /// <summary>
        /// Login to the RRC server and perform some OSLC actions
        /// </summary>
        /// <param name="args"></param>
	    static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

		    CommandLineDictionary cmd = CommandLineDictionary.FromArguments(args);

		    if (!ValidateOptions(cmd)) {		
			    logger.Error("Syntax:  /url=https://<server>:port/<context>/ /user=<user> /password=<password> /project=\"<project_area>\"");
			    logger.Error("Example: /url=https://exmple.com:9443/rm /user=ADMIN /password=ADMIN /project=\"JKE Banking (Requirements Management)\"");
			    return;
		    }
			
		    String webContextUrl = cmd["url"];
		    String user = cmd["user"];
		    String passwd = cmd["password"];
		    String projectArea = cmd["project"];
		
		    try {
		
			    //STEP 1: Initialize a Jazz rootservices helper and indicate we're looking for the RequirementManagement catalog
			    JazzRootServicesHelper helper = new JazzRootServicesHelper(webContextUrl,OSLCConstants.OSLC_RM_V2);
			
			    //STEP 2: Create a new Form Auth client with the supplied user/password
			    //RRC is a fronting server, so need to use the initForm() signature which allows passing of an authentication URL.
			    //For RRC, use the JTS for the authorization URL
			
			    //This is a bit of a hack for readability.  It is assuming RRC is at context /rm.  Could use a regex or UriBuilder instead.
			    String authUrl = webContextUrl.Replace("/rm","/jts"); // XXX - should be ReplaceFirst(), if it existed
			    JazzFormAuthClient client = helper.InitFormClient(user, passwd, authUrl);
			
			    //STEP 3: Login in to Jazz Server
			    if (client.FormLogin() == HttpStatusCode.OK) {
				
				    //STEP 4: Get the URL of the OSLC ChangeManagement catalog
				    String catalogUrl = helper.GetCatalogUrl();
				
				    //STEP 5: Find the OSLC Service Provider for the project area we want to work with
				    String serviceProviderUrl = client.LookupServiceProviderUrl(catalogUrl, projectArea);
				
				    //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
				    String queryCapability = client.LookupQueryCapability(serviceProviderUrl,
																	      OSLCConstants.OSLC_RM_V2,
																	      OSLCConstants.RM_REQUIREMENT_TYPE);
				    //STEP 7: Create base requirements
				    //Get the Creation Factory URL for change requests so that we can create one
				    Requirement requirement = new Requirement();
				    String requirementFactory = client.LookupCreationFactory(
						    serviceProviderUrl, OSLCConstants.OSLC_RM_V2,
						    requirement.GetRdfTypes()[0].ToString());
				
				    //Get Feature Requirement Type URL
				    ResourceShape featureInstanceShape = RmUtil.LookupRequirementsInstanceShapes(
						    serviceProviderUrl, OSLCConstants.OSLC_RM_V2,
						    requirement.GetRdfTypes()[0].ToString(), client, "Feature");
				
				    Uri rootFolder = null;
				    //Get Collection Type URL
				    RequirementCollection collection = new RequirementCollection();
                    ResourceShape collectionInstanceShape = RmUtil.LookupRequirementsInstanceShapes(
                            serviceProviderUrl, OSLCConstants.OSLC_RM_V2,
                            collection.GetRdfTypes()[0].ToString(), client, "Personal Collection");
				
				    String req01URL=null;
				    String req02URL=null;
				    String req03URL=null;
				    String req04URL=null;
				    String reqcoll01URL=null;
				
				    String primaryText = null;
				    if (( featureInstanceShape != null ) && (requirementFactory != null ) ){
					
					    // Create REQ01
					    requirement.SetInstanceShape(featureInstanceShape.GetAbout());
					    requirement.SetTitle("Req01");
					
					    // Decorate the PrimaryText
					    primaryText = "My Primary Text";
					    XElement obj = RmUtil.ConvertStringToHTML(primaryText);
					    requirement.GetExtendedProperties().Add(RmConstants.PROPERTY_PRIMARY_TEXT, obj);
					
					    requirement.SetDescription("Created By OSLC4Net");
					    requirement.AddImplementedBy(new Link(new Uri("http://google.com"), "Link in REQ01"));
					    //Create the change request
					    HttpResponseMessage creationResponse = client.CreateResource(
							    requirementFactory, requirement,
							    OslcMediaType.APPLICATION_RDF_XML,
							    OslcMediaType.APPLICATION_RDF_XML);
					    req01URL = creationResponse.Headers.Location.ToString();
                        creationResponse.ConsumeContent();
					
					    // Create REQ02	
					    requirement = new Requirement();
					    requirement.SetInstanceShape(featureInstanceShape.GetAbout());
					    requirement.SetTitle("Req02");
					    requirement.SetDescription("Created By OSLC4Net");
					    requirement.AddValidatedBy(new Link(new Uri("http://bancomer.com"), "Link in REQ02"));
					    //Create the change request
					    creationResponse = client.CreateResource(
							    requirementFactory, requirement,
							    OslcMediaType.APPLICATION_RDF_XML,
							    OslcMediaType.APPLICATION_RDF_XML);
					
					    req02URL = creationResponse.Headers.Location.ToString();
                        creationResponse.ConsumeContent();
					
					    // Create REQ03	
					    requirement = new Requirement();
					    requirement.SetInstanceShape(featureInstanceShape.GetAbout());
					    requirement.SetTitle("Req03");
					    requirement.SetDescription("Created By OSLC4Net");
					    requirement.AddValidatedBy(new Link(new Uri("http://outlook.com"), "Link in REQ03"));
					    //Create the change request
					    creationResponse = client.CreateResource(
							    requirementFactory, requirement,
							    OslcMediaType.APPLICATION_RDF_XML,
							    OslcMediaType.APPLICATION_RDF_XML);
					    req03URL = creationResponse.Headers.Location.ToString();
                        creationResponse.ConsumeContent();
					
					    // Create REQ04	
					    requirement = new Requirement();
					    requirement.SetInstanceShape(featureInstanceShape.GetAbout());
					    requirement.SetTitle("Req04");
					    requirement.SetDescription("Created By OSLC4Net");
					
					    //Create the Requirement
					    creationResponse = client.CreateResource(
							    requirementFactory, requirement,
							    OslcMediaType.APPLICATION_RDF_XML,
							    OslcMediaType.APPLICATION_RDF_XML);
					    req04URL = creationResponse.Headers.Location.ToString();
                        creationResponse.ConsumeContent();
					
					    // Now create a collection 
					    // Create REQ04	
					    collection = new RequirementCollection();
					
					    collection.AddUses(new Uri(req03URL));
					    collection.AddUses(new Uri(req04URL));
					
					    collection.SetInstanceShape(collectionInstanceShape.GetAbout());
					    collection.SetTitle("Collection01");
					    collection.SetDescription("Created By OSLC4Net");
					    //Create the change request
					    creationResponse = client.CreateResource(
							    requirementFactory, collection,
							    OslcMediaType.APPLICATION_RDF_XML,
							    OslcMediaType.APPLICATION_RDF_XML);
					    reqcoll01URL = creationResponse.Headers.Location.ToString();
                        creationResponse.ConsumeContent();
					
				    }
				
				    // Check that everything was properly created
				     if ( req01URL == null ||
					      req02URL == null ||
					      req03URL == null ||
					      req04URL == null ||
					     reqcoll01URL == null ) {
					     throw new Exception("Failed to create an artifact");
				     }

				    // GET the root folder based on First requirement created
				    HttpResponseMessage getResponse = client.GetResource(req01URL,OslcMediaType.APPLICATION_RDF_XML);
				    requirement = getResponse.Content.ReadAsAsync<Requirement>(client.GetFormatters()).Result;
				    String etag1 = getResponse.Headers.ETag.ToString();
				    // May not be needed getResponse.ConsumeContent();
				    //Save the Uri of the root folder in order to used it easily 
				    rootFolder = (Uri) requirement.GetExtendedProperties()[RmConstants.PROPERTY_PARENT_FOLDER];
				
				    String changedPrimaryText =  (String ) requirement.GetExtendedProperties()[RmConstants.PROPERTY_PRIMARY_TEXT];
				    if ( changedPrimaryText == null ){
					    // Check with the workaround
					     changedPrimaryText =  (String ) requirement.GetExtendedProperties()[PROPERTY_PRIMARY_TEXT_WORKAROUND];
				    }
				
				    if ( ( changedPrimaryText != null) && (! changedPrimaryText.Contains(primaryText)) ) {
					    logger.Error("Error getting primary Text");
				    }

				    //QUERIES
				    // SCENARIO 01  Do a query for type= Requirements
				    OslcQueryParameters queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("rdf=<http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
				    queryParams.SetWhere("rdf:type=<http://open-services.net/ns/rm#Requirement>");
				    OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);
				    OslcQueryResult result = query.Submit();
				    bool processAsDotNetObjects = false;
				    int resultsSize = result.GetMembersUrls().Length;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 01 = " + resultsSize + "\n");
				
				
				    // SCENARIO 02 	Do a query for type= Requirements and for it folder container = rootFolder
				    queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("nav=<http://com.ibm.rdm/navigation#>,rdf=<http://www.w3.org/1999/02/22-rdf-syntax-ns#>");
				    queryParams.SetWhere("rdf:type=<http://open-services.net/ns/rm#Requirement> and nav:parent=<" + rootFolder + ">");
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    processAsDotNetObjects = false;
				    resultsSize = result.GetMembersUrls().Length;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 02 = " + resultsSize + "\n");
				
				    // SCENARIO 03	Do a query for title
				    queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("dcterms=<http://purl.org/dc/terms/>");
				    queryParams.SetWhere("dcterms:title=\"Req04\"");
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    resultsSize = result.GetMembersUrls().Length;
				    processAsDotNetObjects = false;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 03 = " + resultsSize + "\n");
				
				    // SCENARIO 04	Do a query for the link that is implemented
				    queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("oslc_rm=<http://open-services.net/ns/rm#>");
				    queryParams.SetWhere("oslc_rm:implementedBy=<http://google.com>");
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    resultsSize = result.GetMembersUrls().Length;
				    processAsDotNetObjects = false;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 04 = " + resultsSize + "\n");
				
				    // SCENARIO 05	Do a query for the links that is validated 
				    queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("oslc_rm=<http://open-services.net/ns/rm#>");
				    queryParams.SetWhere("oslc_rm:validatedBy in [<http://bancomer.com>,<http://outlook.com>]");
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    resultsSize = result.GetMembersUrls().Length;
				    processAsDotNetObjects = false;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 05 = " + resultsSize + "\n");
				
				    // SCENARIO 06 Do a query for it container folder and for the link that is implemented
				    queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("nav=<http://com.ibm.rdm/navigation#>,oslc_rm=<http://open-services.net/ns/rm#>");
				    queryParams.SetWhere("nav:parent=<"+rootFolder+"> and oslc_rm:validatedBy=<http://bancomer.com>"); 
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    resultsSize = result.GetMembersUrls().Length;
				    processAsDotNetObjects = false;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 06 = " + resultsSize + "\n");
				

				    // GET resources from req03 in order edit its values
				    getResponse = client.GetResource(req03URL,OslcMediaType.APPLICATION_RDF_XML);
				    requirement = getResponse.Content.ReadAsAsync<Requirement>(client.GetFormatters()).Result;
				    // Get the eTAG, we need it to update
				    String etag = getResponse.Headers.ETag.ToString();
                    getResponse.ConsumeContent();
				    requirement.SetTitle("My new Title");
				    requirement.AddImplementedBy(new Link(new Uri("http://google.com"), "Link created by an Eclipse Lyo user"));
				
				    // Update the requirement with the proper etag 
				    HttpResponseMessage updateResponse = client.UpdateResource(req03URL, 
						    requirement, OslcMediaType.APPLICATION_RDF_XML, OslcMediaType.APPLICATION_RDF_XML, etag);

                    updateResponse.ConsumeContent();
				
				    /*Do a query in order to see if the requirement have changed*/
				    // SCENARIO 07 Do a query for the new title just changed
				    queryParams = new OslcQueryParameters();
				    queryParams.SetPrefix("dcterms=<http://purl.org/dc/terms/>");
				    queryParams.SetWhere("dcterms:title=\"My new Title\"");
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    resultsSize = result.GetMembersUrls().Length;
				    processAsDotNetObjects = false;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 07 = " + resultsSize + "\n");
				
				    // SCENARIO 08	Do a query for implementedBy links
				    queryParams = new OslcQueryParameters();							
				    queryParams.SetPrefix("oslc_rm=<http://open-services.net/ns/rm#>");
				    queryParams.SetWhere("oslc_rm:implementedBy=<http://google.com>");
				    query = new OslcQuery(client, queryCapability, 10, queryParams);
				    result = query.Submit();
				    resultsSize = result.GetMembersUrls().Length;
				    processAsDotNetObjects = false;
				    ProcessPagedQueryResults(result,client, processAsDotNetObjects);
				    Console.WriteLine("\n------------------------------\n");
				    Console.WriteLine("Number of Results for SCENARIO 08 = " + resultsSize + "\n");
				

			    }
		    } catch (RootServicesException re) {
			    logger.Error("Unable to access the Jazz rootservices document at: " + webContextUrl + "/rootservices", re);
		    } catch (Exception e) {
			    logger.Error(e.Message,e);
		    }
	    }
	
	    private static void ProcessPagedQueryResults(OslcQueryResult result, OslcClient client, bool asDotNetObjects) {
		    int page = 1;
		    //For now, just show first 5 pages
		    do {
			    Console.WriteLine("\nPage " + page + ":\n");
			    ProcessCurrentPage(result,client,asDotNetObjects);
			    if (result.MoveNext() && page < 5) {
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
						       //Requirement req = response.getEntity(Requirement.class);
						       //printRequirementInfo(req);   //print a few attributes
					    } else {
						
						    //Just print the raw RDF/XML (or process the XML as desired)
						    processRawResponse(response);
						
					    }
				    }
			    } catch (Exception e) {
				    logger.Error("Unable to process artfiact at url: " + resultsUrl, e);
			    }
			
		    }
		
	    }
	
	    private static void processRawResponse(HttpResponseMessage response) 
        {
		    Stream inStream = response.Content.ReadAsStreamAsync().Result;
		    StreamReader streamReader = new StreamReader(new BufferedStream(inStream), System.Text.Encoding.UTF8);
		
		    String line = null;
            while ((line = streamReader.ReadLine()) != null)
            {
		      Console.WriteLine(line);
		    }
		    Console.WriteLine();
            response.ConsumeContent();
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
