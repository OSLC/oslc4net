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
/// Samples of logging in to Enterprise Workflow Management (EWM) and running OSLC operations
/// 
/// 
/// - run an OLSC ChangeRequest query and retrieve OSLC ChangeRequests and de-serialize them as .NET objects
/// - retrieve an OSLC ChangeRequest and print it as XML
/// - create a new ChangeRequest
/// - update an existing ChangeRequest
/// </summary>
sealed class EWMSample : SampleBase<ChangeRequest>
{
    private readonly ILoggerFactory _loggerFactory;

    public EWMSample(ILoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<EWMSample>())
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Entry point for EWM Sample
    /// </summary>
    /// <param name="args"></param>
    public static async Task Run(string[] args)
    {
        await RunSample(args).ConfigureAwait(false);
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
        var logger = loggerFactory.CreateLogger<EWMSample>();
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

        await RunAsync(url, user, password, project, loggerFactory).ConfigureAwait(false);
    }

    static async Task RunAsync(string webContextUrl, string user, string passwd, string projectArea, ILoggerFactory loggerFactory)
    {
        var sample = new EWMSample(loggerFactory);
        await sample.RunScenarioAsync(webContextUrl, user, passwd, projectArea).ConfigureAwait(false);
    }

    public async Task RunScenarioAsync(string webContextUrl, string user, string passwd, string projectArea)
    {
        try
        {

            //STEP 1: Create a new Form Auth client with the supplied user/password
            JazzFormAuthClient client = new JazzFormAuthClient(webContextUrl, user, passwd, _loggerFactory.CreateLogger<OslcClient>());

            //STEP 2: Login in to Jazz Server
            if (await client.FormLoginAsync().ConfigureAwait(false) == HttpStatusCode.OK)
            {

                Logger.LogInformation("[EWM Debug] Entered RunAsync with latest build");

                //STEP 3: Initialize a Jazz rootservices helper and indicate we're looking for the ChangeManagement catalog
                //EWM contains a service provider for CM and SCM, so we need to indicate our interest in CM
                //For Jazz servers, use the CM-specific property cmServiceProviders in the CM v1.0 namespace
                var rootServicesHelper = new RootServicesHelper(webContextUrl,
                    "http://open-services.net/xmlns/cm/1.0/", "cmServiceProviders");
                var rootServices = await rootServicesHelper.DiscoverAsync(client.GetHttpClient()).ConfigureAwait(false);

                //STEP 4: Get the URL of the OSLC ChangeManagement catalog
                String catalogUrl = rootServices.ServiceProviderCatalog;

                //STEP 5: Find the OSLC Service Provider for the project area we want to work with
                String serviceProviderUrl = await client.LookupServiceProviderUrl(catalogUrl, projectArea).ConfigureAwait(false);

                //STEP 6: Get the Query Capabilities URL so that we can run some OSLC queries
                String queryCapability = await client.LookupQueryCapabilityAsync(serviceProviderUrl,
                                                                                                                          OSLCConstants.OSLC_CM_V2,
                                                                                                                          OSLCConstants.CM_CHANGE_REQUEST_TYPE).ConfigureAwait(false);

                //SCENARIO A: Run a query for all open ChangeRequests with OSLC paging of 10 items per
                //page turned on and list the members of the result
                OslcQueryParameters queryParams = new OslcQueryParameters();
                queryParams.SetWhere("oslc_cm:closed=false");
                queryParams.SetSelect("dcterms:identifier,dcterms:title,oslc_cm:status");
                OslcQuery query = new OslcQuery(client, queryCapability, 10, queryParams);

                OslcQueryResult result = await query.Submit().ConfigureAwait(false);

                bool processAsJavaObjects = true;
                await ProcessPagedQueryResultsAsync(result, client, processAsJavaObjects).ConfigureAwait(false);

                Logger.LogInformation("\n------------------------------\n");

                //SCENARIO B:  Run a query for a specific ChangeRequest selecting only certain
                //attributes and then print it as raw XML.  Change the dcterms:identifier below to match a
                //real workitem in your EWM project area
                OslcQueryParameters queryParams2 = new OslcQueryParameters();
                queryParams2.SetWhere("dcterms:identifier=\"10\"");
                queryParams2.SetSelect("dcterms:identifier,dcterms:title,dcterms:creator,dcterms:created,oslc_cm:status");
                OslcQuery query2 = new OslcQuery(client, queryCapability, queryParams2);

                OslcQueryResult result2 = await query2.Submit().ConfigureAwait(false);
                HttpResponseMessage rawResponse = result2.GetRawResponse();
                await ProcessRawResponseAsync(rawResponse).ConfigureAwait(false);
                rawResponse.ConsumeContent();

                //SCENARIO C:  EWM Workitem creation and update
                ChangeRequest changeRequest = new ChangeRequest();
                changeRequest.SetTitle("Implement accessibility in Pet Store application");
                changeRequest.SetDescription("Image elements must provide a description in the 'alt' attribute for consumption by screen readers.");
                changeRequest.AddTestedByTestCase(new Link(new Uri("http://qmprovider/testcase/1"), "Accessibility verification using a screen reader"));
                changeRequest.AddDctermsType("task");

                // Populate required Filed Against/category using allowed values from the creation factory shape
                Logger.LogInformation("[FiledAgainst] Starting resolution");
                Uri? filedAgainstValue = await ResolveFiledAgainstAsync(client, serviceProviderUrl, changeRequest).ConfigureAwait(false);
                if (filedAgainstValue != null)
                {
                    changeRequest.GetExtendedProperties()[new QName(JazzConstants.RTC_CM, "filedAgainst")] = filedAgainstValue;
                    Logger.LogInformation("[FiledAgainst] Using value {Value}", filedAgainstValue);
                }
                else
                {
                    Logger.LogWarning("Could not resolve Filed Against allowed values; creation may fail with 403");
                    Logger.LogWarning("[FiledAgainst] Resolution returned null");
                }

                //Get the Creation Factory URL for change requests so that we can create one
                String changeRequestCreation = await client.LookupCreationFactoryAsync(
                                serviceProviderUrl, OSLCConstants.OSLC_CM_V2,
                                changeRequest.GetTypes().First().ToString()).ConfigureAwait(false);

                //Create the change request
                HttpResponseMessage creationResponse = await client.CreateResourceRawAsync(
                                changeRequestCreation, changeRequest,
                                OslcMediaType.APPLICATION_RDF_XML,
                                OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false);

                if (creationResponse.StatusCode != HttpStatusCode.Created)
                {
                    String errorString = await creationResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new InvalidOperationException($"Failed to create change request (HTTP {creationResponse.StatusCode}): {errorString}");
                }

                String changeRequestLocation = creationResponse.Headers.Location.ToString();
                creationResponse.ConsumeContent();
                Logger.LogInformation("Change Request created a location {Location}", changeRequestLocation);

                //Get the change request from the service provider and update its title property
                changeRequest = await (await client.GetResourceRawAsync(changeRequestLocation,
                                OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false)).Content.ReadAsAsync<ChangeRequest>(client.GetFormatters()).ConfigureAwait(false);
                changeRequest.SetTitle(changeRequest.GetTitle() + " (updated)");

                //Create a partial update URL so that only the title will be updated.
                //Assuming (for readability) that the change request URL does not already contain a '?'
                String updateUrl = changeRequest.GetAbout() + "?oslc.properties=dcterms:title";

                //Update the change request at the service provider
                HttpResponseMessage updateResponse = await client.UpdateResourceRawAsync(
                                new Uri(updateUrl), changeRequest,
                                OslcMediaType.APPLICATION_RDF_XML,
                                OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false);

                updateResponse.ConsumeContent();

            }
            else
            {
                throw new InvalidOperationException("Authentication failed");
            }
        }
        catch (RootServicesException re)
        {
            Logger.LogError(re, "Unable to access the Jazz rootservices document at: " + webContextUrl + "/rootservices");
            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
            throw;
        }
    }

    protected override void PrintResourceInfo(ChangeRequest cr)
    {
        if (cr != null)
        {
            Logger.LogInformation("id: {Id}, title: {Title}, status: {Status}", cr.GetIdentifier(), cr.GetTitle(), cr.GetStatus());
        }
    }

    /// <summary>
    /// Resolves the "Filed Against" (rtc_cm:filedAgainst) allowed value from the creation factory's resource shape.
    /// This is required by EWM to satisfy the Filed Against precondition when creating work items.
    /// </summary>
    private async Task<Uri?> ResolveFiledAgainstAsync(
        OslcClient client,
        string serviceProviderUrl,
        ChangeRequest changeRequest)
    {
        try
        {
            // Step 1: Get the creation factory URL
            string creationFactoryUrl = await client.LookupCreationFactoryAsync(
                serviceProviderUrl,
                OSLCConstants.OSLC_CM_V2,
                changeRequest.GetTypes().First().ToString()).ConfigureAwait(false);

            if (string.IsNullOrEmpty(creationFactoryUrl))
            {
                Logger.LogWarning("Could not find creation factory URL for Filed Against resolution");
                return null;
            }

            Logger.LogInformation("Creation factory for Filed Against: {FactoryUrl}", creationFactoryUrl);

            // Step 2: Fetch the ServiceProvider to get the CreationFactory object
            // Since LookupCreationFactoryAsync only returns URL string, we need to fetch the service provider
            // and extract the creation factory's resource shapes
            var serviceProviderResponse = await client.GetResourceAsync<ServiceProvider>(serviceProviderUrl).ConfigureAwait(false);
            var serviceProvider = serviceProviderResponse.Resources?.SingleOrDefault();
            if (serviceProvider == null)
            {
                Logger.LogWarning("Could not fetch ServiceProvider for Filed Against resolution");
                return null;
            }

            // Step 3: Find the creation factory and its resource shapes
            CreationFactory? targetFactory = null;
            foreach (var service in serviceProvider.GetServices())
            {
                if (string.Equals(service.GetDomain()?.ToString(), OSLCConstants.OSLC_CM_V2, StringComparison.Ordinal))
                {
                    foreach (var factory in service.GetCreationFactories())
                    {
                        if (string.Equals(factory.GetCreation()?.ToString(), creationFactoryUrl, StringComparison.Ordinal))
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
                Logger.LogWarning("Could not find CreationFactory object for Filed Against resolution. Looking for {FactoryUrl}", creationFactoryUrl);
                return null;
            }

            Logger.LogInformation("Found CreationFactory with {ShapeCount} resource shapes", targetFactory.GetResourceShapes()?.Length ?? 0);

            // Step 4: Get the resource shapes from the factory
            Uri[]? shapeUris = targetFactory.GetResourceShapes();
            if (shapeUris == null || shapeUris.Length == 0)
            {
                Logger.LogWarning("CreationFactory has no resource shapes for Filed Against resolution");
                return null;
            }

            // Try all shapes until we find a filedAgainst property with allowed values
            foreach (var shapeUri in shapeUris)
            {
                Logger.LogInformation("Inspecting ResourceShape {ShapeUri}", shapeUri);
                var resourceShapeResponse = await client.GetResourceAsync<ResourceShape>(shapeUri.ToString()).ConfigureAwait(false);
                var resourceShape = resourceShapeResponse.Resources?.SingleOrDefault();
                if (resourceShape == null)
                {
                    Logger.LogWarning("Could not fetch ResourceShape for Filed Against resolution. Shape URI: {ShapeUri}", shapeUri);
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
                        Logger.LogInformation("Found filedAgainst property definition {Definition}", defString);
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
                    Logger.LogWarning("filedAgainst property has no allowed values reference (shape {ShapeUri})", shapeUri);
                    continue;
                }

                Logger.LogInformation("Fetching Filed Against allowed values from {AllowedValuesRef}", allowedValuesRef);

                var allowedValuesResponse = await client.GetResourceRawAsync(allowedValuesRef.ToString(), OslcMediaType.APPLICATION_RDF_XML).ConfigureAwait(false);
                if (!allowedValuesResponse.IsSuccessStatusCode)
                {
                    Logger.LogError("Failed to fetch allowed values: {StatusCode}", allowedValuesResponse.StatusCode);
                    continue;
                }

                try
                {
#pragma warning disable OSLCEXP001
                    var allowedValues = await allowedValuesResponse.Content.ReadAsAsync<AllowedValuesResource<Uri>>(client.GetFormatters()).ConfigureAwait(false);
#pragma warning restore OSLCEXP001

                    if (allowedValues != null && allowedValues.AllowedValues.Count > 0)
                    {
                        var values = allowedValues.AllowedValues;
                        Logger.LogInformation("Found {Count} allowed values", values.Count);

                        foreach (var val in values)
                        {
                            Logger.LogInformation("Found allowed value >{}<", val);
                            // Skip empty or whitespace values                                
                            // if (!val.Contains("Unassigned", StringComparison.OrdinalIgnoreCase))
                            // {
                            //     Logger.LogInformation("Resolved Filed Against category: {Category}", val);
                            //     return new Uri(val);
                            // }
                        }

                        // Fallback to first non-empty value if all seem unassigned or check failed
                        var firstNonEmpty = values.FirstOrDefault(v => v is not null);
                        if (firstNonEmpty != null)
                        {
                            Logger.LogInformation("Using fallback Filed Against category: {Category}", firstNonEmpty);
                            return firstNonEmpty;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("No allowed values found in the response");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to deserialize AllowedValues");
                }
            }

            Logger.LogWarning("No allowed values found for Filed Against across all resource shapes");
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving Filed Against allowed values");
            return null;
        }
    }
}

