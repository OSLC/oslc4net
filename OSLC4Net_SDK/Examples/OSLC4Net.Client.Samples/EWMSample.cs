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
using System.CommandLine.Invocation;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Oslc.Jazz;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Resources;
using OSLC4Net.Client.Oslc.Helpers;
using OSLC4Net.Core.Model;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Core.Attribute;
using VDS.RDF;
using OSLC4Net.Core.DotNetRdfProvider;

namespace OSLC4Net.Client.Samples
{
    /// <summary>
    /// Samples of logging in to Enterprise Workflow Management (EWM) and running OSLC operations
    /// 
    /// 
    /// - run an OLSC ChangeRequest query and retrieve OSLC ChangeRequests and de-serialize them as .NET objects
    /// - retrieve an OSLC ChangeRequest and print it as XML
    /// - create a new ChangeRequest
    /// - update an existing ChangeRequest
    /// </summary>
    class EWMSample
    {
        private static ILogger logger;

        /// <summary>
        /// Entry point for EWM Sample
        /// </summary>
        /// <param name="args"></param>
        public static async Task Run(string[] args)
        {
            await RunSample(args);
        }

        private static async Task RunSample(string[] args)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
            });
            logger = loggerFactory.CreateLogger<EWMSample>();
            var urlOption = new System.CommandLine.Option<string>("--url")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var userOption = new System.CommandLine.Option<string>("--user")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var passwordOption = new System.CommandLine.Option<string>("--password")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var projectOption = new System.CommandLine.Option<string>("--project")
            {
                Arity = ArgumentArity.ExactlyOne
            };

            var rootCommand = new System.CommandLine.RootCommand("EWM Sample");
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

            await RunAsync(url, user, password, project, loggerFactory);
        }

        static async Task RunAsync(string webContextUrl, string user, string passwd, string projectArea, ILoggerFactory loggerFactory)
        {

            try {

                    //STEP 1: Create a new Form Auth client with the supplied user/password
                    JazzFormAuthClient client = new JazzFormAuthClient(webContextUrl, user, passwd, loggerFactory.CreateLogger<OslcClient>());

                    //STEP 2: Login in to Jazz Server
                    if (await client.FormLoginAsync() == HttpStatusCode.OK) {

                    Console.WriteLine("[EWM Debug] Entered RunAsync with latest build");

                    //STEP 3: Initialize a Jazz rootservices helper and indicate we're looking for the ChangeManagement catalog
                    //EWM contains a service provider for CM and SCM, so we need to indicate our interest in CM
                    //For Jazz servers, use the CM-specific property cmServiceProviders in the CM v1.0 namespace
                    var rootServicesHelper = new RootServicesHelper(webContextUrl,
                        "http://open-services.net/xmlns/cm/1.0/", "cmServiceProviders");
                    var rootServices = await rootServicesHelper.DiscoverAsync(client.GetHttpClient());

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
                                    //real workitem in your EWM project area
                                    OslcQueryParameters queryParams2 = new OslcQueryParameters();
                                    queryParams2.SetWhere("dcterms:identifier=\"10\"");
                                    queryParams2.SetSelect("dcterms:identifier,dcterms:title,dcterms:creator,dcterms:created,oslc_cm:status");
                                    OslcQuery query2 = new OslcQuery(client, queryCapability, queryParams2);

                                    OslcQueryResult result2 = await query2.Submit();
                                    HttpResponseMessage rawResponse = result2.GetRawResponse();
                                    await ProcessRawResponseAsync(rawResponse);
                                    rawResponse.ConsumeContent();

                                    //SCENARIO C:  EWM Workitem creation and update
                                    ChangeRequest changeRequest = new ChangeRequest();
                                    changeRequest.SetTitle("Implement accessibility in Pet Store application");
                                    changeRequest.SetDescription("Image elements must provide a description in the 'alt' attribute for consumption by screen readers.");
                                    changeRequest.AddTestedByTestCase(new Link(new Uri("http://qmprovider/testcase/1"), "Accessibility verification using a screen reader"));
                                    changeRequest.AddDctermsType("task");

                                    // Populate required Filed Against/category using allowed values from the creation factory shape
                                    Console.WriteLine("[FiledAgainst] Starting resolution");
                                    Uri? filedAgainstValue = await ResolveFiledAgainstAsync(client, serviceProviderUrl, changeRequest, logger);
                                    if (filedAgainstValue != null)
                                    {
                                        changeRequest.GetExtendedProperties()[new QName(JazzConstants.RTC_CM, "filedAgainst")] = filedAgainstValue;
                                        Console.WriteLine($"[FiledAgainst] Using value {filedAgainstValue}");
                                    }
                                    else
                                    {
                                        logger.LogWarning("Could not resolve Filed Against allowed values; creation may fail with 403");
                                        Console.WriteLine("[FiledAgainst] Resolution returned null");
                                    }

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

            private static void PrintChangeRequestInfo(ChangeRequest cr) {
                    //See the OSLC4J ChangeRequest class for a full list of attributes you can access.
                    if (cr != null) {
                            Console.WriteLine("ID: " + cr.GetIdentifier() + ", Title: " + cr.GetTitle() + ", Status: " + cr.GetStatus());
                    }
            }

            /// <summary>
            /// Resolves the "Filed Against" (rtc_cm:filedAgainst) allowed value from the creation factory's resource shape.
            /// This is required by EWM to satisfy the Filed Against precondition when creating work items.
            /// </summary>
            private static async Task<Uri?> ResolveFiledAgainstAsync(
                OslcClient client,
                string serviceProviderUrl,
                ChangeRequest changeRequest,
                ILogger logger)
            {
                try
                {
                    // Step 1: Get the creation factory URL
                    string creationFactoryUrl = await client.LookupCreationFactoryAsync(
                        serviceProviderUrl,
                        OSLCConstants.OSLC_CM_V2,
                        changeRequest.GetRdfTypes()[0].ToString());

                    if (string.IsNullOrEmpty(creationFactoryUrl))
                    {
                        logger.LogWarning("Could not find creation factory URL for Filed Against resolution");
                        return null;
                    }

                    logger.LogInformation("Creation factory for Filed Against: {FactoryUrl}", creationFactoryUrl);

                    // Step 2: Fetch the ServiceProvider to get the CreationFactory object
                    // Since LookupCreationFactoryAsync only returns URL string, we need to fetch the service provider
                    // and extract the creation factory's resource shapes
                    var serviceProviderResponse = await client.GetResourceAsync<ServiceProvider>(serviceProviderUrl);
                    var serviceProvider = serviceProviderResponse.Resources?.SingleOrDefault();
                    if (serviceProvider == null)
                    {
                        logger.LogWarning("Could not fetch ServiceProvider for Filed Against resolution");
                        return null;
                    }

                    // Step 3: Find the creation factory and its resource shapes
                    CreationFactory? targetFactory = null;
                    foreach (var service in serviceProvider.GetServices())
                    {
                        if (service.GetDomain()?.ToString() == OSLCConstants.OSLC_CM_V2)
                        {
                            foreach (var factory in service.GetCreationFactories())
                            {
                                if (factory.GetCreation()?.ToString() == creationFactoryUrl)
                                {
                                    targetFactory = factory;
                                    break;
                                }
                            }
                        }

                        if (targetFactory != null)
                        {
                            break;
                        }
                    }

                    if (targetFactory == null)
                    {
                        logger.LogWarning("Could not find CreationFactory object for Filed Against resolution. Looking for {FactoryUrl}", creationFactoryUrl);
                        return null;
                    }

                    logger.LogInformation("Found CreationFactory with {ShapeCount} resource shapes", targetFactory.GetResourceShapes()?.Length ?? 0);

                    // Step 4: Get the resource shapes from the factory
                    Uri[]? shapeUris = targetFactory.GetResourceShapes();
                    if (shapeUris == null || shapeUris.Length == 0)
                    {
                        logger.LogWarning("CreationFactory has no resource shapes for Filed Against resolution");
                        return null;
                    }

                    // Try all shapes until we find a filedAgainst property with allowed values
                    foreach (var shapeUri in shapeUris)
                    {
                        logger.LogInformation("Inspecting ResourceShape {ShapeUri}", shapeUri);
                        var resourceShapeResponse = await client.GetResourceAsync<ResourceShape>(shapeUri.ToString());
                        var resourceShape = resourceShapeResponse.Resources?.SingleOrDefault();
                        if (resourceShape == null)
                        {
                            logger.LogWarning("Could not fetch ResourceShape for Filed Against resolution. Shape URI: {ShapeUri}", shapeUri);
                            continue;
                        }

                        Property? filedAgainstProperty = null;
                        foreach (var property in resourceShape.GetProperties())
                        {
                            var propertyDefinition = property.GetPropertyDefinition();
                            var defString = propertyDefinition?.ToString() ?? string.Empty;
                            if (defString.EndsWith("filedAgainst", StringComparison.OrdinalIgnoreCase) || defString.Contains("filedAgainst", StringComparison.OrdinalIgnoreCase))
                            {
                                filedAgainstProperty = property;
                                logger.LogInformation("Found filedAgainst property definition {Definition}", defString);
                                break;
                            }
                        }

                        if (filedAgainstProperty == null)
                        {
                            continue;
                        }

                        Uri? allowedValuesRef = filedAgainstProperty.GetAllowedValuesRef();
                        if (allowedValuesRef == null)
                        {
                            logger.LogWarning("filedAgainst property has no allowed values reference (shape {ShapeUri})", shapeUri);
                            continue;
                        }

                        logger.LogInformation("Fetching Filed Against allowed values from {AllowedValuesRef}", allowedValuesRef);

                        var allowedValuesResponse = await client.GetResourceRawAsync(allowedValuesRef.ToString(), OslcMediaType.APPLICATION_RDF_XML);
                        if (!allowedValuesResponse.IsSuccessStatusCode)
                        {
                            logger.LogWarning("Failed to fetch allowed values: {StatusCode}", allowedValuesResponse.StatusCode);
                            continue;
                        }

                        try
                        {
                            var allowedValues = await allowedValuesResponse.Content.ReadAsAsync<AllowedValues>(client.GetFormatters());

                            if (allowedValues != null && allowedValues.GetAllowedValues().Length > 0)
                            {
                                var values = allowedValues.GetAllowedValues();
                                logger.LogInformation("Found {Count} allowed values", values.Length);

                                foreach (var val in values)
                                {
                                    if (!val.ToString().Contains("Unassigned", StringComparison.OrdinalIgnoreCase))
                                    {
                                        logger.LogInformation("Resolved Filed Against category: {Category}", val);
                                        return val;
                                    }
                                }

                                // Fallback to first if all seem unassigned or check failed
                                if (values.Length > 0)
                                {
                                    logger.LogInformation("Using fallback Filed Against category: {Category}", values[0]);
                                    return values[0];
                                }
                            }
                            else
                            {
                                logger.LogWarning("No allowed values found in the response");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to deserialize AllowedValues");
                        }
                    }

                    logger.LogWarning("No allowed values found for Filed Against across all resource shapes");
                    return null;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error resolving Filed Against allowed values");
                    return null;
                }
            }
    }

    [OslcNamespace(OSLC4Net.Core.Model.OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC Allowed Values Resource Shape", describes = new[] { OSLC4Net.Core.Model.OslcConstants.TYPE_ALLOWED_VALUES })]
    public class AllowedValues : AbstractResource
    {
        private List<Uri> allowedValues = new List<Uri>();

        [OslcDescription("Value allowed for a property")]
        [OslcName("allowedValue")]
        [OslcPropertyDefinition(OSLC4Net.Core.Model.OslcConstants.OSLC_CORE_NAMESPACE + "allowedValue")]
        [OslcTitle("Allowed Values")]
        public Uri[] GetAllowedValues()
        {
            return allowedValues.ToArray();
        }

        public void SetAllowedValues(Uri[] allowedValues)
        {
            this.allowedValues.Clear();
            if (allowedValues != null)
            {
                this.allowedValues.AddRange(allowedValues);
            }
        }
    }
}
