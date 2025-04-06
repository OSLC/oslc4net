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

using System.Net;
using System.Net.Http.Formatting;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSLC4Net.ChangeManagement;
using OSLC4Net.Client;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;
using Projects;
using Type = OSLC4Net.ChangeManagement.Type;

namespace OSLC4Net.ChangeManagementTest;

[TestClass]
[TestCategory("RunningOslcServerRequired")]
public abstract class TestBase
{
    private const int TIMEOUT_HEALTHCHECK = 1; // seconds

    // between retries, seconds
    private const int DELAY_HEALTHCHECK = 1;
    protected static string _serviceProviderCatalogURI;
    protected readonly IConfigurationRoot _config;

    private OslcClient _testClient;

    protected TestBase()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            //  .AddEnvironmentVariables()
            .Build();
        if (_config["serviceProviderCatalog:auth:type"] is not null
            && _config["serviceProviderCatalog:auth:type"]!.Equals("basic",
                StringComparison.InvariantCultureIgnoreCase))
        {
            Username = _config["serviceProviderCatalog:auth:user"];
            Password = _config["serviceProviderCatalog:auth:password"];
        }
    }

    protected virtual IEnumerable<MediaTypeFormatter> Formatters { get; } =
        OslcRestClient.DEFAULT_FORMATTERS;

    protected Uri? ChangeRequestUri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public OslcClient TestClient => _testClient ??= GetTestClient();


    protected static async Task<DistributedApplication> SetupAspireAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<OSLC4Net_Test_AspireHost>().ConfigureAwait(false);

        var refimplCM = builder
            .AddDockerfile("refimpl-rm", "../../../../refimpl/src/", "server-cm/Dockerfile")
            .WithEndpoint(8801, 8080, isExternal: true, isProxied: false,
                scheme: "http", name: "http");

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

        // Poll for server availability
        await AwaitRefimplStartupAsync().ConfigureAwait(false);

        return app;
    }

    private static async Task AwaitRefimplStartupAsync()
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(TIMEOUT_HEALTHCHECK);
        var maxRetries = 60;
        var retryDelay = TimeSpan.FromSeconds(DELAY_HEALTHCHECK);
        var serverUrl = new Uri(_serviceProviderCatalogURI);

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var response = await httpClient.GetAsync(serverUrl).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Server is ready
                    break;
                }
            }
            catch
            {
                // Server not ready yet
            }

            if (i < maxRetries - 1)
            {
                await Task.Delay(retryDelay).ConfigureAwait(false);
            }
            else
            {
                throw new TimeoutException(
                    $"Server at {serverUrl} did not start within {maxRetries * retryDelay.TotalSeconds} seconds");
            }
        }
    }

    private ServiceProviderRegistryClient GetTestSPCClient()
    {
        ServiceProviderRegistryClient registryClient;

        if (Password is not null && Username is not null)
        {
            registryClient =
                ServiceProviderRegistryClient.WithBasicAuth(_serviceProviderCatalogURI, Username,
                    Password);
        }
        else
        {
            registryClient = new ServiceProviderRegistryClient(_serviceProviderCatalogURI);
        }

        return registryClient;
    }

    private OslcClient GetTestClient()
    {
        OslcClient client;

        if (Password is not null && Username is not null)
        {
            client = OslcClient.ForBasicAuth(Username, Password);
        }
        else
        {
            client = new OslcClient();
        }

        return client;
    }

    protected async Task<string> GetCreationAsync(string mediaType,
        string type)
    {
        var registryClient = GetTestSPCClient();
        var serviceProviders =
            await registryClient.GetServiceProvidersAsync().ConfigureAwait(false);

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
        var registryClient = GetTestSPCClient();
        var serviceProviders =
            await registryClient.GetServiceProvidersAsync().ConfigureAwait(false);

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
        var registryClient = GetTestSPCClient();
        var serviceProviders =
            await registryClient.GetServiceProvidersAsync().ConfigureAwait(false);

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
                                    var response = await TestClient
                                        .GetResourceAsync<ResourceShape>(resourceShape)
                                        .ConfigureAwait(false);
                                    return response.Resources!.Single();
                                }
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve resource shape for type '" + type +
                                        "'");
    }

    protected async Task VerifyChangeRequestAsync(string mediaType,
        ChangeRequest? changeRequest,
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
            var aboutResponse = await TestClient
                .GetResourceAsync<ChangeRequest>(aboutURI.ToString(), mediaType)
                .ConfigureAwait(false);

            await VerifyChangeRequestAsync(mediaType,
                aboutResponse.Resources!.Single(),
                false).ConfigureAwait(false);
            if (serviceProviderURI != null)
            {
                var serviceProviderResponse = await TestClient
                    .GetResourceAsync<ChangeRequest>(serviceProviderURI.ToString(), mediaType)
                    .ConfigureAwait(false);
                var serviceProvider = serviceProviderResponse.Resources?.SingleOrDefault();

                Assert.IsNotNull(serviceProvider);
            }
        }
    }

    protected async Task VerifyCompact(string mediaType,
        Compact? compact)
    {
        Assert.IsNotNull(compact);

        var aboutURI = compact.GetAbout();
        _ = compact.GetShortTitle();
        var titleString = compact.GetTitle();

        Assert.IsNotNull(aboutURI);
        // TODO: check OSLC Core and print warning otherwise
        // Assert.IsNotNull(shortTitleString);
        Assert.IsNotNull(titleString);

        var aboutResponse = await TestClient
            .GetResourceAsync<ChangeRequest>(aboutURI.ToString(), mediaType).ConfigureAwait(false);


        await VerifyChangeRequestAsync(mediaType,
            aboutResponse.Resources?.SingleOrDefault(),
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

        var properties = resourceShape.GetProperties();

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
        Assert.IsNotNull(ChangeRequestUri);

        //OslcRestClient oslcRestClient = new(Formatters,
        //                                    CREATED_CHANGE_REQUEST_URI,
        //compactMediaType);

        var compactResponse = await TestClient
            .GetResourceAsync<Compact>(ChangeRequestUri.ToString(), compactMediaType)
            .ConfigureAwait(false);

        var compact = compactResponse.Resources?.SingleOrDefault();

        await VerifyCompact(normalMediaType,
            compact).ConfigureAwait(false);
    }

    protected async Task<ChangeRequest> MakeChangeRequestAsync(string mediaType)
    {
        //ChangeRequestUri = null;

        ChangeRequest changeRequest = new();

        changeRequest.AddContributor(new Uri("http://myserver/mycmapp/users/bob"));
        changeRequest.AddCreator(new Uri("http://myserver/mycmapp/users/bob"));
        changeRequest.AddDctermsType(Type.Defect.ToString());
        changeRequest.SetDescription(
            "Invalid installation instructions indicating invalid patches to be applied.");
        changeRequest.SetDiscussedBy(new Uri("http://example.com/bugs/2314/discussion"));
        changeRequest.SetInstanceShape(new Uri("http://example.com/shapes/defect"));
        changeRequest.AddRelatedChangeRequest(new Link(new Uri("http://myserver/mycmapp/bugs/1235"),
            "Bug 1235"));
        changeRequest.SetSeverity(Severity.Major.ToString());
        changeRequest.SetShortTitle("Bug 2314");
        changeRequest.SetStatus("InProgress");
        changeRequest.AddSubject("doc");
        changeRequest.AddSubject("install");
        changeRequest.SetTitle("Invalid installation instructions");
        changeRequest.AddTracksRequirement(
            new Link(new Uri("http://myserver/reqtool/req/34ef31af")));
        changeRequest.AddTracksRequirement(
            new Link(new Uri("http://remoteserver/reqrepo/project1/req456"), "Requirement 456"));

        var creation = await GetCreationAsync(mediaType, Constants.TYPE_CHANGE_REQUEST)
            .ConfigureAwait(false);

        //OslcRestClient oslcRestClient = new(Formatters,
        //                                    creation,
        //                                    mediaType);
        var client = GetTestClient();
        var addedChangeRequestResponse = await TestClient
            .CreateResourceAsync(creation, changeRequest, mediaType)
            .ConfigureAwait(false);
        var createdResource = addedChangeRequestResponse.Resources?.SingleOrDefault();
        ChangeRequestUri = createdResource?.GetAbout();

        return createdResource ?? throw new OslcCoreRequestException(
            (int)addedChangeRequestResponse.StatusCode, addedChangeRequestResponse.ResponseMessage);
    }

    protected async Task TestCreateAsync(string mediaType)
    {
        // Assert.IsNull(CREATED_CHANGE_REQUEST_URI);

        var addedChangeRequest = await MakeChangeRequestAsync(mediaType).ConfigureAwait(false);

        await VerifyChangeRequestAsync(mediaType,
            addedChangeRequest,
            true).ConfigureAwait(false);
    }

    protected async Task<HttpResponseMessage?> DeleteChangeRequestAsync(string mediaType)
    {
        ArgumentNullException.ThrowIfNull(ChangeRequestUri);
        try
        {
            return await TestClient.DeleteResourceAsync(ChangeRequestUri).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            ChangeRequestUri = null;
        }
    }

    protected async Task TestDeleteAsync(string mediaType)
    {
        Assert.IsNotNull(ChangeRequestUri);
        var resourceToBeDeleted = ChangeRequestUri;
        //var oslcRestClient = new OslcRestClient(Formatters,
        //                                    CREATED_CHANGE_REQUEST_URI,
        //                                    mediaType);

        var clientResponse = await DeleteChangeRequestAsync(mediaType).ConfigureAwait(false);

        Assert.IsNotNull(clientResponse);
        // OSLC 3.0 allows 200 OK or 204 No Content
        // TODO: confirm an exact CC
        CollectionAssert.Contains(new[] { HttpStatusCode.NoContent, HttpStatusCode.OK },
            clientResponse?.StatusCode);
        // Assert.Equals(HttpStatusCode.NoContent, clientResponse?.StatusCode);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(resourceToBeDeleted.ToString(), mediaType)
            .ConfigureAwait(false);

        Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound ||
                      response.StatusCode == HttpStatusCode.Gone);
        Assert.IsNull(response.Resources?.FirstOrDefault());
    }

    protected async Task TestRetrieveAsync(string mediaType)
    {
        Assert.IsNotNull(ChangeRequestUri);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(ChangeRequestUri.ToString(), mediaType)
            .ConfigureAwait(false);

        await VerifyChangeRequestAsync(mediaType,
            response.Resources?.SingleOrDefault(),
            true).ConfigureAwait(false);
    }

    protected async Task TestRetrievesAsync(string mediaType)
    {
        Assert.IsNotNull(ChangeRequestUri);

        var queryBase = await GetQueryBaseAsync(mediaType, Constants.TYPE_CHANGE_REQUEST)
            .ConfigureAwait(false);

        Assert.IsNotNull(queryBase);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(queryBase, mediaType).ConfigureAwait(false);

        Assert.IsNotNull(response.Resources?.FirstOrDefault());
        Assert.IsTrue(response.Resources.Count > 0);

        var found = false;

        // FIXME add paging
        foreach (var changeRequest in response.Resources)
        {
            await VerifyChangeRequestAsync(mediaType, changeRequest, true)
                .ConfigureAwait(false);

            if (ChangeRequestUri.Equals(changeRequest.GetAbout()))
            {
                found = true;
            }
        }

        Assert.IsTrue(found);
    }

    protected async Task TestUpdateAsync(string mediaType)
    {
        Assert.IsNotNull(ChangeRequestUri);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(ChangeRequestUri.ToString(), mediaType)
            .ConfigureAwait(false);

        var changeRequest = response.Resources?.SingleOrDefault();
        await VerifyChangeRequestAsync(mediaType, changeRequest, true)
            .ConfigureAwait(false);

        Assert.IsNull(changeRequest!.IsApproved());
        Assert.IsNull(changeRequest!.GetCloseDate());

        var closeDate = DateTime.Now;

        changeRequest.SetApproved(true);
        changeRequest.SetCloseDate(closeDate);

        var clientResponse = await TestClient
            .UpdateResourceRawAsync(ChangeRequestUri, changeRequest, mediaType)
            .ConfigureAwait(false);

        Assert.IsNotNull(clientResponse);
        Assert.AreEqual(HttpStatusCode.OK, clientResponse.StatusCode);

        var updatedResponse = await TestClient
            .GetResourceAsync<ChangeRequest>(ChangeRequestUri.ToString(), mediaType)
            .ConfigureAwait(false);

        var updatedChangeRequest = updatedResponse.Resources!.Single();

        await VerifyChangeRequestAsync(mediaType,
            updatedChangeRequest,
            true).ConfigureAwait(false);
        Assert.AreEqual(changeRequest.GetAbout(), updatedChangeRequest.GetAbout());
        Assert.AreEqual(true, updatedChangeRequest.IsApproved());
        Assert.AreEqual(closeDate.ToShortDateString() + " - " + closeDate.ToShortTimeString(),
            updatedChangeRequest.GetCloseDate()?.ToShortDateString() + " - " +
            updatedChangeRequest.GetCloseDate()?.ToShortTimeString());
        Assert.IsFalse(changeRequest.GetModified().Equals(updatedChangeRequest.GetModified()));
        Assert.IsTrue(updatedChangeRequest.GetModified() > updatedChangeRequest.GetCreated());
    }
}
