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
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OSLC4Net.ChangeManagement;
using OSLC4Net.Client;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;
using Xunit;
using Xunit.Sdk;
using Type = OSLC4Net.ChangeManagement.Type;

namespace OSLC4Net.ChangeManagementTest;

public abstract class TestBase
{
    private OslcClient? _testClient;

    protected string ServiceProviderCatalogUri;
    protected readonly IConfigurationRoot Config;
    protected IHost AppHost { get; set; }
    protected ILoggerFactory LoggerFactory { get; set; }

    protected TestBase(ITestOutputHelper testOutputHelper)
    {
        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            //  .AddEnvironmentVariables()
            .Build();
        if (Config["serviceProviderCatalog:auth:type"] is not null
            && Config["serviceProviderCatalog:auth:type"]!.Equals("basic",
                StringComparison.InvariantCultureIgnoreCase))
        {
            Username = Config["serviceProviderCatalog:auth:user"];
            Password = Config["serviceProviderCatalog:auth:password"];
        }

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureLogging(
                builder =>
                {
                    builder.Services.AddSingleton<ILoggerProvider>(
                        new XUnitLoggerProvider(testOutputHelper, false));
                }).Build();
        LoggerFactory = AppHost.Services.GetRequiredService<ILoggerFactory>();
    }



    protected virtual IEnumerable<MediaTypeFormatter> Formatters { get; } =
        OslcRestClient.DEFAULT_FORMATTERS;

    protected Uri? ChangeRequestUri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public OslcClient TestClient => _testClient ??= GetTestClient();

    private ServiceProviderRegistryClient GetTestSPCClient()
    {
        ServiceProviderRegistryClient registryClient;

        if (Password is not null && Username is not null)
        {
            registryClient =
                ServiceProviderRegistryClient.WithBasicAuth(ServiceProviderCatalogUri, Username,
                    Password, LoggerFactory);
        }
        else
        {
            registryClient =
                new ServiceProviderRegistryClient(ServiceProviderCatalogUri, LoggerFactory);
        }

        return registryClient;
    }

    protected OslcClient GetTestClient()
    {
        OslcClient client;

        if (Password is not null && Username is not null)
        {
            client = OslcClient.ForBasicAuth(Username, Password,
                LoggerFactory.CreateLogger<OslcClient>());
        }
        else
        {
            client = new OslcClient(LoggerFactory.CreateLogger<OslcClient>());
        }

        return client;
    }

    protected async Task<string> GetCreationAsync(string mediaType,
        string type)
    {
        var registryClient = GetTestSPCClient();
        var serviceProviders =
            await registryClient.GetServiceProvidersAsync().ConfigureAwait(true);

        foreach (var serviceProvider in serviceProviders)
        {
            var services = serviceProvider.GetServices();
            foreach (var service in services)
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

        throw FailException.ForFailure("Unable to retrieve creation for type '" + type + "'");
    }

    protected async Task<string> GetQueryBaseAsync(string mediaType,
        string type)
    {
        var registryClient = GetTestSPCClient();
        var serviceProviders =
            await registryClient.GetServiceProvidersAsync().ConfigureAwait(true);

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

        throw FailException.ForFailure("Unable to retrieve queryBase for type '" + type + "'");
    }

    protected async Task<ResourceShape> GetResourceShapeAsync(string mediaType,
        string type)
    {
        var registryClient = GetTestSPCClient();
        var serviceProviders =
            await registryClient.GetServiceProvidersAsync().ConfigureAwait(true);

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
                                        .ConfigureAwait(true);
                                    return response.Resources!.Single();
                                }
                            }
                        }
                    }
                }
            }
        }

        throw FailException.ForFailure("Unable to retrieve resource shape for type '" + type +
                                       "'");
    }

    protected async Task VerifyChangeRequestAsync(string mediaType,
        ChangeRequest? changeRequest,
        bool recurse)
    {
        Assert.NotNull(changeRequest);

        var aboutURI = changeRequest.GetAbout();
        var createdDate = changeRequest.GetCreated();
        var identifierString = changeRequest.GetIdentifier();
        _ = changeRequest.GetModified();
        Uri[] rdfTypesURIs = changeRequest.GetRdfTypes();
        var serviceProviderURI = changeRequest.GetServiceProvider();

        Assert.NotNull(aboutURI);
        Assert.NotNull(createdDate);
        Assert.NotNull(identifierString);
        // TODO: check the spec and refimpl if shall be set
        // Assert.NotNull(modifiedDate);
        Assert.NotNull(rdfTypesURIs);
        // TODO: check the spec and refimpl if shall be set
        // Assert.NotNull(serviceProviderURI);
        // Assert.True(modifiedDate.Equals(createdDate) || modifiedDate > createdDate);

        // FIXME: says who?
        // Assert.True(aboutURI.ToString().EndsWith(identifierString));

        Assert.Contains(new Uri(Constants.TYPE_CHANGE_REQUEST), rdfTypesURIs);

        if (recurse)
        {
            var aboutResponse = await TestClient
                .GetResourceAsync<ChangeRequest>(aboutURI.ToString(), mediaType)
                .ConfigureAwait(true);

            await VerifyChangeRequestAsync(mediaType,
                aboutResponse.Resources!.Single(),
                false).ConfigureAwait(true);
            if (serviceProviderURI != null)
            {
                var serviceProviderResponse = await TestClient
                    .GetResourceAsync<ChangeRequest>(serviceProviderURI.ToString(), mediaType)
                    .ConfigureAwait(true);
                var serviceProvider = serviceProviderResponse.Resources?.SingleOrDefault();

                Assert.NotNull(serviceProvider);
            }
        }
    }

    protected async Task VerifyCompact(string mediaType,
        Compact? compact)
    {
        Assert.NotNull(compact);

        var aboutURI = compact.GetAbout();
        _ = compact.GetShortTitle();
        var titleString = compact.GetTitle();

        Assert.NotNull(aboutURI);
        // TODO: check OSLC Core and print warning otherwise
        // Assert.NotNull(shortTitleString);
        Assert.NotNull(titleString);

        var aboutResponse = await TestClient
            .GetResourceAsync<ChangeRequest>(aboutURI.ToString(), mediaType).ConfigureAwait(true);


        await VerifyChangeRequestAsync(mediaType,
            aboutResponse.Resources?.SingleOrDefault(),
            false).ConfigureAwait(true);
    }

    protected void VerifyResourceShape(ResourceShape resourceShape,
        string type)
    {
        Assert.NotNull(resourceShape);

        Uri[] describes = resourceShape.GetDescribes();
        Assert.NotNull(describes);
        Assert.True(describes.Length > 0);

        if (type != null)
        {
            Assert.True(describes.Contains(new Uri(type)));
        }

        var properties = resourceShape.GetProperties();

        Assert.NotNull(properties);
        Assert.True(properties.Length > 0);

        foreach (var property in properties)
        {
            var name = property.GetName();
            var propertyDefinition = property.GetPropertyDefinition();

            // not mandatory according OSLC CM 3.0
            // Assert.NotNull(property.GetDescription());
            Assert.NotNull(name);
            Assert.NotNull(property.GetOccurs());
            Assert.NotNull(propertyDefinition);
            Assert.NotNull(property.GetTitle());
            Assert.NotNull(property.GetValueType());

            Assert.True(propertyDefinition.ToString().EndsWith(name),
                $"propertyDefinition [{propertyDefinition}], name [{name}]");
        }
    }

    protected async Task TestResourceShapeAsync(string mediaType)
    {
        var resourceShape = await GetResourceShapeAsync(mediaType,
            Constants.TYPE_CHANGE_REQUEST).ConfigureAwait(true);

        VerifyResourceShape(resourceShape,
            Constants.TYPE_CHANGE_REQUEST);
    }

    protected async Task TestCompactAsync(string compactMediaType,
        string normalMediaType)
    {
        Assert.NotNull(ChangeRequestUri);

        //OslcRestClient oslcRestClient = new(Formatters,
        //                                    CREATED_CHANGE_REQUEST_URI,
        //compactMediaType);

        var compactResponse = await TestClient
            .GetResourceAsync<Compact>(ChangeRequestUri.ToString(), compactMediaType)
            .ConfigureAwait(true);

        var compact = compactResponse.Resources?.SingleOrDefault();

        await VerifyCompact(normalMediaType,
            compact).ConfigureAwait(true);
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
            .ConfigureAwait(true);

        //OslcRestClient oslcRestClient = new(Formatters,
        //                                    creation,
        //                                    mediaType);
        var client = GetTestClient();
        var addedChangeRequestResponse = await TestClient
            .CreateResourceAsync(creation, changeRequest, mediaType)
            .ConfigureAwait(true);
        var createdResource = addedChangeRequestResponse.Resources?.SingleOrDefault();
        ChangeRequestUri = createdResource?.GetAbout();

        return createdResource ?? throw new OslcCoreRequestException(
            (int)addedChangeRequestResponse.StatusCode, addedChangeRequestResponse.ResponseMessage);
    }

    protected async Task TestCreateAsync(string mediaType)
    {
        // Assert.IsNull(CREATED_CHANGE_REQUEST_URI);

        var addedChangeRequest = await MakeChangeRequestAsync(mediaType).ConfigureAwait(true);

        await VerifyChangeRequestAsync(mediaType,
            addedChangeRequest,
            true).ConfigureAwait(true);
    }

    protected async Task<HttpResponseMessage?> DeleteChangeRequestAsync(string mediaType)
    {
        ArgumentNullException.ThrowIfNull(ChangeRequestUri);
        try
        {
            return await TestClient.DeleteResourceAsync(ChangeRequestUri).ConfigureAwait(true);
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
        Assert.NotNull(ChangeRequestUri);
        var resourceToBeDeleted = ChangeRequestUri;
        //var oslcRestClient = new OslcRestClient(Formatters,
        //                                    CREATED_CHANGE_REQUEST_URI,
        //                                    mediaType);

        var clientResponse = await DeleteChangeRequestAsync(mediaType).ConfigureAwait(true);

        Assert.NotNull(clientResponse);
        // OSLC 3.0 allows 200 OK or 204 No Content
        // TODO: confirm an exact CC
        HashSet<HttpStatusCode?> allowedStatuses = [HttpStatusCode.NoContent, HttpStatusCode.OK];
        Assert.Contains(clientResponse?.StatusCode, allowedStatuses);
        // Assert.Equals(HttpStatusCode.NoContent, clientResponse?.StatusCode);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(resourceToBeDeleted.ToString(), mediaType)
            .ConfigureAwait(true);

        Assert.True(response.StatusCode == HttpStatusCode.NotFound ||
                    response.StatusCode == HttpStatusCode.Gone);
        Assert.Null(response.Resources?.FirstOrDefault());
    }

    protected async Task TestRetrieveAsync(string mediaType)
    {
        Assert.NotNull(ChangeRequestUri);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(ChangeRequestUri.ToString(), mediaType)
            .ConfigureAwait(true);

        await VerifyChangeRequestAsync(mediaType,
            response.Resources?.SingleOrDefault(),
            true).ConfigureAwait(true);
    }

    protected async Task TestRetrievesAsync(string mediaType)
    {
        Assert.NotNull(ChangeRequestUri);

        var queryBase = await GetQueryBaseAsync(mediaType, Constants.TYPE_CHANGE_REQUEST)
            .ConfigureAwait(true);

        Assert.NotNull(queryBase);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(queryBase, mediaType).ConfigureAwait(true);

        Assert.NotNull(response.Resources?.FirstOrDefault());
        Assert.True(response.Resources.Count > 0);

        var found = false;

        // FIXME add paging
        foreach (var changeRequest in response.Resources)
        {
            await VerifyChangeRequestAsync(mediaType, changeRequest, true)
                .ConfigureAwait(true);

            if (ChangeRequestUri.Equals(changeRequest.GetAbout()))
            {
                found = true;
            }
        }

        Assert.True(found);
    }

    protected async Task TestUpdateAsync(string mediaType)
    {
        Assert.NotNull(ChangeRequestUri);

        var response = await TestClient
            .GetResourceAsync<ChangeRequest>(ChangeRequestUri.ToString(), mediaType)
            .ConfigureAwait(true);

        var changeRequest = response.Resources?.SingleOrDefault();
        await VerifyChangeRequestAsync(mediaType, changeRequest, true)
            .ConfigureAwait(true);

        Assert.Null(changeRequest!.IsApproved());
        Assert.Null(changeRequest!.GetCloseDate());

        var closeDate = DateTime.Now;

        changeRequest.SetApproved(true);
        changeRequest.SetCloseDate(closeDate);

        var clientResponse = await TestClient
            .UpdateResourceRawAsync(ChangeRequestUri, changeRequest, mediaType)
            .ConfigureAwait(true);

        Assert.NotNull(clientResponse);
        Assert.Equal(HttpStatusCode.OK, clientResponse.StatusCode);

        var updatedResponse = await TestClient
            .GetResourceAsync<ChangeRequest>(ChangeRequestUri.ToString(), mediaType)
            .ConfigureAwait(true);

        var updatedChangeRequest = updatedResponse.Resources!.Single();

        await VerifyChangeRequestAsync(mediaType,
            updatedChangeRequest,
            true).ConfigureAwait(true);
        Assert.Equal(changeRequest.GetAbout(), updatedChangeRequest.GetAbout());
        Assert.Equal(true, updatedChangeRequest.IsApproved());
        Assert.Equal(closeDate.ToShortDateString() + " - " + closeDate.ToShortTimeString(),
            updatedChangeRequest.GetCloseDate()?.ToShortDateString() + " - " +
            updatedChangeRequest.GetCloseDate()?.ToShortTimeString());
        Assert.False(changeRequest.GetModified().Equals(updatedChangeRequest.GetModified()));
        Assert.True(updatedChangeRequest.GetModified() > updatedChangeRequest.GetCreated());
    }
}
