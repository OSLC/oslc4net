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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.CommandLine;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Jazz;
using OSLC4Net.Client.Oslc.Helpers;
using OSLC4Net.Client.Oslc.Resources;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Core.Model;
using OSLC4Net.Domains.RequirementsManagement;
using Requirement = OSLC4Net.Domains.RequirementsManagement.Requirement;
using RequirementCollection = OSLC4Net.Domains.RequirementsManagement.RequirementCollection;

namespace OSLC4Net.Client.Samples
{
    /// <summary>
    /// Samples of logging in to Enterprise Requirements Management (ERM) and running OSLC operations
    /// 
    /// - run an OSLC Requirement query and retrieve OSLC Requirements and display results
    /// - demonstrate query result pagination and member enumeration
    /// </summary>
    class ERMSample
    {
        private static ILogger logger;

        /// <summary>
        /// Entry point for ERM Sample
        /// </summary>
        public static async Task Run(string[] args)
        {
            await RunSample(args);
        }

        private static async Task RunSample(string[] args)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            logger = loggerFactory.CreateLogger<ERMSample>();

            var urlOption = new System.CommandLine.Option<string>("--url") { Arity = ArgumentArity.ExactlyOne };
            var userOption = new System.CommandLine.Option<string>("--user") { Arity = ArgumentArity.ExactlyOne };
            var passwordOption = new System.CommandLine.Option<string>("--password") { Arity = ArgumentArity.ExactlyOne };
                var projectAreaOption = new System.CommandLine.Option<string>("--project") { Arity = ArgumentArity.ExactlyOne };

            var rootCommand = new System.CommandLine.RootCommand("ERM Sample");
            rootCommand.Add(urlOption);
            rootCommand.Add(userOption);
            rootCommand.Add(passwordOption);
            rootCommand.Add(projectAreaOption);

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
            var projectArea = parseResult.GetValue(projectAreaOption)!;

            await RunAsync(url, user, password, projectArea, loggerFactory);
        }

        static async Task RunAsync(string webContextUrl, string user, string passwd, string projectArea, ILoggerFactory loggerFactory)
        {
            try
            {
                //STEP 1: Create a new Form Auth client with the supplied user/password
                //ERM is a fronting server, so need to use the initForm() signature which allows passing of an authentication URL.
                //For ERM, use the JTS for the authorization URL
                //This assumes ERM is at context /rm
                String authUrl = webContextUrl.Replace("/rm", "/jts");
                JazzFormAuthClient client = new JazzFormAuthClient(webContextUrl, authUrl, user, passwd, loggerFactory.CreateLogger<OslcClient>());

                //STEP 2: Login to Jazz Server
                if (await client.FormLoginAsync() == HttpStatusCode.OK)
                {
                    //STEP 3: Initialize a Jazz rootservices helper and indicate we're looking for the RequirementManagement catalog
                    //For Jazz servers, use the RM-specific property rmServiceProviders in the RM v1.0 namespace
                    var rootServicesHelper = new RootServicesHelper(webContextUrl,
                        "http://open-services.net/xmlns/rm/1.0/", "rmServiceProviders");
                    var rootServices = await rootServicesHelper.DiscoverAsync(client.GetHttpClient());

                    //STEP 4: Get the URL of the OSLC RequirementManagement catalog
                    String catalogUrl = rootServices.ServiceProviderCatalog;

                    //STEP 5: Find the OSLC Service Provider for the project area we want to work with
                    String serviceProviderUrl = await client.LookupServiceProviderUrl(catalogUrl, projectArea);

                    //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
                    String queryCapability = await client.LookupQueryCapabilityAsync(serviceProviderUrl,
                        OSLCConstants.OSLC_RM_V2,
                        OSLCConstants.RM_REQUIREMENT_TYPE);

                    //STEP 7: Create base requirements using instance shapes and primary text
                    String requirementFactory = await client.LookupCreationFactoryAsync(serviceProviderUrl,
                        OSLCConstants.OSLC_RM_V2,
                        OSLCConstants.RM_REQUIREMENT_TYPE);

                    ResourceShape? featureInstanceShape = null;
                    ResourceShape? collectionInstanceShape = null;
                    try
                    {
                        try
                        {
                            featureInstanceShape = await RmUtil.LookupRequirementsInstanceShapesAsync(
                                serviceProviderUrl,
                                OSLCConstants.OSLC_RM_V2,
                                OSLCConstants.RM_REQUIREMENT_TYPE,
                                client,
                                "Feature");
                        }
                        catch (ResourceNotFoundException)
                        {
                            featureInstanceShape = await RmUtil.LookupRequirementsInstanceShapesAsync(
                                serviceProviderUrl,
                                OSLCConstants.OSLC_RM_V2,
                                OSLCConstants.RM_REQUIREMENT_TYPE,
                                client,
                                "User Requirement");
                        }

                        try
                        {
                            collectionInstanceShape = await RmUtil.LookupRequirementsInstanceShapesAsync(
                                serviceProviderUrl,
                                OSLCConstants.OSLC_RM_V2,
                                OSLCConstants.RM_REQUIREMENT_COLLECTION_TYPE,
                                client,
                                "Collection");
                        }
                        catch (ResourceNotFoundException ex)
                        {
                            logger.LogWarning(ex, "Collection instance shape not found; proceeding without it");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Unable to resolve instance shapes; creation may fail");
                    }

                    String req01URL = null;
                    String req02URL = null;
                    String req03URL = null;
                    String req04URL = null;
                    String reqcoll01URL = null;

                    if (!string.IsNullOrEmpty(requirementFactory))
                    {
                        try
                        {
                            // Create REQ01 with primaryText and implementedBy
                            Requirement requirement = new Requirement();
                            requirement.Title = "Req01";
                            requirement.Description = "Created By OSLC4Net";
                            requirement.InstanceShape = featureInstanceShape?.About;
                            requirement.ImplementedBy ??= new HashSet<Uri>();
                            requirement.ImplementedBy.Add(new Uri("http://google.com"));

                            XElement primaryText = RmUtil.ConvertStringToHTML("My Primary Text");
                            requirement.GetExtendedProperties()[RmConstants.PROPERTY_PRIMARY_TEXT] = primaryText;

                            HttpResponseMessage creationResponse = await client.CreateResourceRawAsync(
                                requirementFactory, requirement,
                                OslcMediaType.APPLICATION_RDF_XML,
                                OslcMediaType.APPLICATION_RDF_XML);

                            if (creationResponse.IsSuccessStatusCode)
                            {
                                req01URL = creationResponse.Headers.Location?.ToString();
                                logger.LogInformation($"REQ01 created: {req01URL}");
                                creationResponse.ConsumeContent();
                            }
                            else
                            {
                                logger.LogWarning($"Failed to create REQ01: {creationResponse.StatusCode}");
                                creationResponse.ConsumeContent();
                            }

                            // Create REQ02 with validatedBy
                            requirement = new Requirement();
                            requirement.Title = "Req02";
                            requirement.Description = "Created By OSLC4Net";
                            requirement.InstanceShape = featureInstanceShape?.About;
                            requirement.ValidatedBy ??= new HashSet<Uri>();
                            requirement.ValidatedBy.Add(new Uri("http://bancomer.com"));

                            creationResponse = await client.CreateResourceRawAsync(
                                requirementFactory, requirement,
                                OslcMediaType.APPLICATION_RDF_XML,
                                OslcMediaType.APPLICATION_RDF_XML);

                            if (creationResponse.IsSuccessStatusCode)
                            {
                                req02URL = creationResponse.Headers.Location?.ToString();
                                logger.LogInformation($"REQ02 created: {req02URL}");
                                creationResponse.ConsumeContent();
                            }
                            else
                            {
                                logger.LogWarning($"Failed to create REQ02: {creationResponse.StatusCode}");
                                creationResponse.ConsumeContent();
                            }

                            // Create REQ03 with validatedBy
                            requirement = new Requirement();
                            requirement.Title = "Req03";
                            requirement.Description = "Created By OSLC4Net";
                            requirement.InstanceShape = featureInstanceShape?.About;
                            requirement.ValidatedBy ??= new HashSet<Uri>();
                            requirement.ValidatedBy.Add(new Uri("http://outlook.com"));

                            creationResponse = await client.CreateResourceRawAsync(
                                requirementFactory, requirement,
                                OslcMediaType.APPLICATION_RDF_XML,
                                OslcMediaType.APPLICATION_RDF_XML);

                            if (creationResponse.IsSuccessStatusCode)
                            {
                                req03URL = creationResponse.Headers.Location?.ToString();
                                logger.LogInformation($"REQ03 created: {req03URL}");
                                creationResponse.ConsumeContent();
                            }
                            else
                            {
                                logger.LogWarning($"Failed to create REQ03: {creationResponse.StatusCode}");
                                creationResponse.ConsumeContent();
                            }

                            // Create REQ04
                            requirement = new Requirement();
                            requirement.Title = "Req04";
                            requirement.Description = "Created By OSLC4Net";
                            requirement.InstanceShape = featureInstanceShape?.About;

                            creationResponse = await client.CreateResourceRawAsync(
                                requirementFactory, requirement,
                                OslcMediaType.APPLICATION_RDF_XML,
                                OslcMediaType.APPLICATION_RDF_XML);

                            if (creationResponse.IsSuccessStatusCode)
                            {
                                req04URL = creationResponse.Headers.Location?.ToString();
                                logger.LogInformation($"REQ04 created: {req04URL}");
                                creationResponse.ConsumeContent();
                            }
                            else
                            {
                                logger.LogWarning($"Failed to create REQ04: {creationResponse.StatusCode}");
                                creationResponse.ConsumeContent();
                            }

                            // Create a collection using REQ03 and REQ04
                            if (!string.IsNullOrEmpty(req03URL) && !string.IsNullOrEmpty(req04URL))
                            {
                                RequirementCollection collection = new RequirementCollection();
                                collection.Title = "Collection01";
                                collection.Description = "Created By OSLC4Net";
                                collection.InstanceShape = collectionInstanceShape?.About;
                                collection.Uses = new HashSet<Uri>
                                {
                                    new Uri(req03URL),
                                    new Uri(req04URL)
                                };

                                creationResponse = await client.CreateResourceRawAsync(
                                    requirementFactory, collection,
                                    OslcMediaType.APPLICATION_RDF_XML,
                                    OslcMediaType.APPLICATION_RDF_XML);

                                if (creationResponse.IsSuccessStatusCode)
                                {
                                    reqcoll01URL = creationResponse.Headers.Location?.ToString();
                                    logger.LogInformation($"Collection01 created: {reqcoll01URL}");
                                    creationResponse.ConsumeContent();
                                }
                                else
                                {
                                    logger.LogWarning($"Failed to create Collection01: {creationResponse.StatusCode}");
                                    creationResponse.ConsumeContent();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Error during requirement creation");
                        }
                    }

                    //STEP 8: Query requirements
                    OslcQueryParameters queryParams = new OslcQueryParameters();
                    
                    OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);
                    OslcQueryResult queryResults = await query.Submit();
                    await ProcessPagedQueryResultsAsync(queryResults, client, false);

                    Console.WriteLine("\n------------------------------\n");
                    Console.WriteLine("Requirements query completed successfully.");
                }
                else
                {
                    Console.WriteLine("Authentication failed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error running ERM sample");
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static async Task ProcessPagedQueryResultsAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects)
        {
            int page = 1;
            //For now, just show first 5 pages
            do
            {
                Console.WriteLine("\nPage " + page + ":\n");
                await ProcessCurrentPageAsync(result, client, asDotNetObjects);
                if (result.MoveNext())
                {
                    result = result.Current;
                    page++;
                }
                else
                {
                    break;
                }
            } while (true);
        }

        private static async Task ProcessCurrentPageAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects)
        {
            foreach (String resultsUrl in result.GetMembersUrls())
            {
                Console.WriteLine(resultsUrl);

                HttpResponseMessage response = null;
                try
                {
                    //Get a single requirement by its URL 
                    response = await client.GetResourceRawAsync(resultsUrl, OSLCConstants.CT_RDF);

                    if (response != null)
                    {
                        //De-serialize it as a .NET object (commented out - requires specific RM classes)
                        if (asDotNetObjects)
                        {
                            //Requirement req = response.getEntity(Requirement.class);
                            //printRequirementInfo(req);   //print a few attributes
                        }
                        else
                        {
                            //Just print the raw RDF/XML (first 500 chars for readability)
                            await ProcessRawResponseAsync(response);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Unable to process requirement at url: " + resultsUrl);
                }
            }
        }

        private static async Task ProcessRawResponseAsync(HttpResponseMessage response)
        {
            if (!logger.IsEnabled(LogLevel.Trace))
            {
                response.ConsumeContent();
                return;
            }

            Stream inStream = await response.Content.ReadAsStreamAsync();
            StreamReader streamReader = new StreamReader(new BufferedStream(inStream), System.Text.Encoding.UTF8);

            String line = null;
            while ((line = streamReader.ReadLine()) != null)
            {
                logger.LogTrace(line);
            }
            response.ConsumeContent();
        }
    }
}
