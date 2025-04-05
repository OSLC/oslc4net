/*******************************************************************************
 * Copyright (c) 2023 Andrii Berezovskyi and OSLC4Net contributors.
 * Copyright (c) 2012 IBM Corporation.
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

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSLC4Net.ChangeManagement;
using OSLC4Net.Client;

using OSLC4Net.Core.Model;
using System.Net;
using System.Net.Http.Formatting;
using ServiceProvider = OSLC4Net.Core.Model.ServiceProvider;

namespace OSLC4Net.ChangeManagementTest;

[TestClass]
[TestCategory("RunningOslcServerRequired")]
public abstract class TestBase
{
    protected virtual IEnumerable<MediaTypeFormatter> Formatters { get; } = OslcRestClient.DEFAULT_FORMATTERS;
    protected Uri? CREATED_CHANGE_REQUEST_URI;
    protected readonly IConfigurationRoot _config;
    protected static string _serviceProviderCatalogURI;

    protected TestBase()
    {
        _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                 //  .AddEnvironmentVariables()
                 .Build();
        //_serviceProviderCatalogURI = _config["serviceProviderCatalog:uri"]!;
    }

    
    protected static async Task<DistributedApplication> SetupAspireAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.OSLC4Net_Test_AspireHost>().ConfigureAwait(false);

        var refimplCM = builder.AddDockerfile("refimpl-rm", "../../../../refimpl/src/", "server-cm/Dockerfile")
             .WithEndpoint(port: 8801, targetPort: 8080, isExternal:true, isProxied: false, scheme: "http", name: "http");

        //builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        //{
        //    clientBuilder.AddStandardResilienceHandler();
        //});

        // To output logs to the xUnit.net ITestOutputHelper, 
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        await using var app = await builder.BuildAsync().ConfigureAwait(false);

        await app.StartAsync().ConfigureAwait(false);

        var cmEndpoint = refimplCM.GetEndpoint("http");

        _serviceProviderCatalogURI = cmEndpoint.Url + "/services/catalog/singleton";


        return app;
    }

    protected async Task<string> GetCreationAsync(string mediaType,
                                      string type)
    {
        var registryClient = new ServiceProviderRegistryClient(_serviceProviderCatalogURI);
        var serviceProviders = await registryClient.GetServiceProvidersAsync();

        foreach (var serviceProvider in serviceProviders)
        {
            var services = serviceProvider.GetServices();
            foreach (var service in services)
            {
                if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                {
                    var creationFactories = service.GetCreationFactories();
                    foreach (var creationFactory in creationFactories)
                    {
                        Uri[] resourceTypes = creationFactory.GetResourceTypes();
                        foreach (var resourceType in resourceTypes)
                        {
                            if (resourceType.ToString().Equals(type))
                            {
                                return creationFactory.GetCreation().ToString();
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve creation for type '" + type + "'");
    }

    protected async Task<string> GetQueryBaseAsync(string mediaType,
                                       string type)
    {
        var registryClient = new ServiceProviderRegistryClient(_serviceProviderCatalogURI, Formatters, mediaType);
        var serviceProviders = await registryClient.GetServiceProvidersAsync();

        foreach (var serviceProvider in serviceProviders)
        {
            var services = serviceProvider.GetServices();

            foreach (var service in services)
            {
                if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                {
                    QueryCapability[] queryCapabilities = service.GetQueryCapabilities();

                    foreach (var queryCapability in queryCapabilities)
                    {
                        Uri[] resourceTypes = queryCapability.GetResourceTypes();

                        foreach (var resourceType in resourceTypes)
                        {
                            if (resourceType.ToString().Equals(type))
                            {
                                return queryCapability.GetQueryBase().ToString();
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve queryBase for type '" + type + "'");
    }

    protected async Task<ResourceShape> GetResourceShapeAsync(string mediaType,
                                                  string type)
    {
        var registryClient = new ServiceProviderRegistryClient(_serviceProviderCatalogURI,
                    Formatters, mediaType);
        var serviceProviders = await registryClient.GetServiceProvidersAsync();

        foreach (var serviceProvider in serviceProviders)
        {
            Service[] services = serviceProvider.GetServices();
            foreach (var service in services)
            {
                if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                {
                    QueryCapability[] queryCapabilities = service.GetQueryCapabilities();
                    foreach (var queryCapability in queryCapabilities)
                    {
                        Uri[] resourceTypes = queryCapability.GetResourceTypes();
                        foreach (var resourceType in resourceTypes)
                        {
                            if (resourceType.ToString().Equals(type))
                            {
                                var resourceShape = queryCapability.GetResourceShape();
                                if (resourceShape != null)
                                {
                                    OslcRestClient oslcRestClient = new(
                                        Formatters, resourceShape, mediaType);
                                    return await oslcRestClient.GetOslcResourceAsync<ResourceShape>().ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve resource shape for type '" + type + "'");
    }

    protected async Task VerifyChangeRequestAsync(string mediaType,
                                            ChangeRequest changeRequest,
                                            bool recurse)
    {
        Assert.IsNotNull(changeRequest);

        var aboutURI = changeRequest.GetAbout();
        var createdDate = changeRequest.GetCreated();
        var identifierString = changeRequest.GetIdentifier();
        _ = changeRequest.GetModified();
        Uri[] rdfTypesURIs = changeRequest.GetRdfTypes();
        var serviceProviderURI = changeRequest.GetServiceProvider();

        Assert.IsNotNull(aboutURI);
        Assert.IsNotNull(createdDate);
        Assert.IsNotNull(identifierString);
        // TODO: check the spec and refimpl if shall be set
        // Assert.IsNotNull(modifiedDate);
        Assert.IsNotNull(rdfTypesURIs);
        // TODO: check the spec and refimpl if shall be set
        // Assert.IsNotNull(serviceProviderURI);
        // Assert.IsTrue(modifiedDate.Equals(createdDate) || modifiedDate > createdDate);

        // FIXME: says who?
        // Assert.IsTrue(aboutURI.ToString().EndsWith(identifierString));

        Assert.IsTrue(rdfTypesURIs.Contains(new Uri(Constants.TYPE_CHANGE_REQUEST)));

        if (recurse)
        {
            OslcRestClient aboutOSLCRestClient = new(Formatters,
                                                    aboutURI,
                                                    mediaType);

            await VerifyChangeRequestAsync(mediaType,
                                await aboutOSLCRestClient.GetOslcResourceAsync<ChangeRequest>().ConfigureAwait(false),
                                recurse: false).ConfigureAwait(false);
            if (serviceProviderURI != null)
            {
                OslcRestClient serviceProviderOSLCRestClient = new(Formatters,
                                                                serviceProviderURI,
                                                                mediaType);

                var serviceProvider = await serviceProviderOSLCRestClient.GetOslcResourceAsync<ServiceProvider>().ConfigureAwait(false);

                Assert.IsNotNull(serviceProvider);
            }
        }
    }

    protected async Task VerifyCompact(string mediaType,
                                      Compact compact)
    {
        Assert.IsNotNull(compact);

        var aboutURI = compact.GetAbout();
        _ = compact.GetShortTitle();
        var titleString = compact.GetTitle();

        Assert.IsNotNull(aboutURI);
        // TODO: check OSLC Core and print warning otherwise
        // Assert.IsNotNull(shortTitleString);
        Assert.IsNotNull(titleString);

        OslcRestClient aboutOSLCRestClient = new(Formatters,
                                                aboutURI,
                                                mediaType);

        await VerifyChangeRequestAsync(mediaType,
                            await aboutOSLCRestClient.GetOslcResourceAsync<ChangeRequest>().ConfigureAwait(false),
                            false).ConfigureAwait(false);
    }

    protected void VerifyResourceShape(ResourceShape resourceShape,
                                            string type)
    {
        Assert.IsNotNull(resourceShape);

        Uri[] describes = resourceShape.GetDescribes();
        Assert.IsNotNull(describes);
        Assert.IsTrue(describes.Length > 0);

        if (type != null)
        {
            Assert.IsTrue(describes.Contains(new Uri(type)));
        }

        Property[] properties = resourceShape.GetProperties();

        Assert.IsNotNull(properties);
        Assert.IsTrue(properties.Length > 0);

        foreach (var property in properties)
        {
            var name = property.GetName();
            var propertyDefinition = property.GetPropertyDefinition();

            // not mandatory according OSLC CM 3.0
            // Assert.IsNotNull(property.GetDescription());
            Assert.IsNotNull(name);
            Assert.IsNotNull(property.GetOccurs());
            Assert.IsNotNull(propertyDefinition);
            Assert.IsNotNull(property.GetTitle());
            Assert.IsNotNull(property.GetValueType());

            Assert.IsTrue(propertyDefinition.ToString().EndsWith(name),
                          $"propertyDefinition [{propertyDefinition}], name [{name}]");
        }
    }

    protected async Task TestResourceShapeAsync(string mediaType)
    {
        var resourceShape = await GetResourceShapeAsync(mediaType,
                                                       Constants.TYPE_CHANGE_REQUEST).ConfigureAwait(false);

        VerifyResourceShape(resourceShape,
                            Constants.TYPE_CHANGE_REQUEST);
    }

    protected async Task TestCompactAsync(string compactMediaType,
                               string normalMediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(Formatters,
                                            CREATED_CHANGE_REQUEST_URI,
                                            compactMediaType);

        var compact = await oslcRestClient.GetOslcResourceAsync<Compact>().ConfigureAwait(false);

        await VerifyCompact(normalMediaType,
                      compact).ConfigureAwait(false);
    }

    protected async Task<ChangeRequest> MakeChangeRequestAsync(string mediaType)
    {
        CREATED_CHANGE_REQUEST_URI = null;

        ChangeRequest changeRequest = new();

        changeRequest.AddContributor(new Uri("http://myserver/mycmapp/users/bob"));
        changeRequest.AddCreator(new Uri("http://myserver/mycmapp/users/bob"));
        changeRequest.AddDctermsType(ChangeManagement.Type.Defect.ToString());
        changeRequest.SetDescription("Invalid installation instructions indicating invalid patches to be applied.");
        changeRequest.SetDiscussedBy(new Uri("http://example.com/bugs/2314/discussion"));
        changeRequest.SetInstanceShape(new Uri("http://example.com/shapes/defect"));
        changeRequest.AddRelatedChangeRequest(new Link(new Uri("http://myserver/mycmapp/bugs/1235"), "Bug 1235"));
        changeRequest.SetSeverity(Severity.Major.ToString());
        changeRequest.SetShortTitle("Bug 2314");
        changeRequest.SetStatus("InProgress");
        changeRequest.AddSubject("doc");
        changeRequest.AddSubject("install");
        changeRequest.SetTitle("Invalid installation instructions");
        changeRequest.AddTracksRequirement(new Link(new Uri("http://myserver/reqtool/req/34ef31af")));
        changeRequest.AddTracksRequirement(new Link(new Uri("http://remoteserver/reqrepo/project1/req456"), "Requirement 456"));

        var creation = await GetCreationAsync(mediaType, Constants.TYPE_CHANGE_REQUEST).ConfigureAwait(false);

        OslcRestClient oslcRestClient = new(Formatters,
                                            creation,
                                            mediaType);

        var addedChangeRequest = await oslcRestClient.AddOslcResourceAsync(changeRequest).ConfigureAwait(false);

        CREATED_CHANGE_REQUEST_URI = addedChangeRequest.GetAbout();

        return addedChangeRequest;
    }

    protected async Task TestCreateAsync(string mediaType)
    {
        // Assert.IsNull(CREATED_CHANGE_REQUEST_URI);

        var addedChangeRequest = await MakeChangeRequestAsync(mediaType);

        await VerifyChangeRequestAsync(mediaType,
                            addedChangeRequest,
                            true).ConfigureAwait(false);
    }

    protected HttpResponseMessage? DeleteChangeRequest(string mediaType)
    {
        ArgumentNullException.ThrowIfNull(CREATED_CHANGE_REQUEST_URI);
        try
        {
            OslcRestClient oslcRestClient = new(Formatters,
                                                CREATED_CHANGE_REQUEST_URI,
                                                mediaType);
            return oslcRestClient.RemoveOslcResourceReturnClientResponse();
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            CREATED_CHANGE_REQUEST_URI = null;
        }
    }

    protected async Task TestDeleteAsync(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);
        var oslcRestClient = new OslcRestClient(Formatters,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        var clientResponse = DeleteChangeRequest(mediaType);

        Assert.IsNotNull(clientResponse);
        // OSLC 3.0 allows 200 OK or 204 No Content
        // TODO: confirm an exact CC
        CollectionAssert.Contains(new[] { HttpStatusCode.NoContent, HttpStatusCode.OK },
            clientResponse?.StatusCode);
        // Assert.Equals(HttpStatusCode.NoContent, clientResponse?.StatusCode);

        Assert.IsNull(await oslcRestClient.GetOslcResourceAsync<ChangeRequest>().ConfigureAwait(false));
    }

    protected async Task TestRetrieveAsync(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(Formatters,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        var changeRequest = await oslcRestClient.GetOslcResourceAsync<ChangeRequest>().ConfigureAwait(false);

        await VerifyChangeRequestAsync(mediaType,
                            changeRequest,
                            recurse: true).ConfigureAwait(false);
    }

    protected async Task TestRetrievesAsync(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        var queryBase = await GetQueryBaseAsync(mediaType, Constants.TYPE_CHANGE_REQUEST).ConfigureAwait(false);

        Assert.IsNotNull(queryBase);

        var client = new OslcRestClient(Formatters, queryBase, mediaType);

        ICollection<ChangeRequest> changeRequests = await client.GetOslcResourcesAsync<ChangeRequest>().ConfigureAwait(false);

        Assert.IsNotNull(changeRequests);
        Assert.IsTrue(changeRequests.Count > 0);

        var found = false;

        // FIXME add paging
        foreach (var changeRequest in changeRequests)
        {
            await VerifyChangeRequestAsync(mediaType, changeRequest, recurse: true).ConfigureAwait(false);

            if (CREATED_CHANGE_REQUEST_URI.Equals(changeRequest.GetAbout()))
            {
                found = true;
            }
        }

        Assert.IsTrue(found);
    }

    protected async Task TestUpdateAsync(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(Formatters,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        var changeRequest = await oslcRestClient.GetOslcResourceAsync<ChangeRequest>();

        VerifyChangeRequestAsync(mediaType, changeRequest, recurse: true);

        Assert.IsNull(changeRequest.IsApproved());
        Assert.IsNull(changeRequest.GetCloseDate());

        var closeDate = DateTime.Now;

        changeRequest.SetApproved(true);
        changeRequest.SetCloseDate(closeDate);

        var clientResponse = oslcRestClient.UpdateOslcResourceReturnClientResponse(changeRequest);

        Assert.IsNotNull(clientResponse);
        Assert.AreEqual(HttpStatusCode.OK, clientResponse.StatusCode);

        var updatedChangeRequest = await oslcRestClient.GetOslcResourceAsync<ChangeRequest>();

        VerifyChangeRequestAsync(mediaType,
                            updatedChangeRequest,
                            true);

        Assert.AreEqual(changeRequest.GetAbout(), updatedChangeRequest.GetAbout());
        Assert.AreEqual(true, updatedChangeRequest.IsApproved());
        Assert.AreEqual(closeDate.ToShortDateString() + " - " + closeDate.ToShortTimeString(),
                        updatedChangeRequest.GetCloseDate()?.ToShortDateString() + " - " + updatedChangeRequest.GetCloseDate()?.ToShortTimeString());
        Assert.IsFalse(changeRequest.GetModified().Equals(updatedChangeRequest.GetModified()));
        Assert.IsTrue(updatedChangeRequest.GetModified() > updatedChangeRequest.GetCreated());
    }
}
