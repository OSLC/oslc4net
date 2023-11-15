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
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.ChangeManagement;
using OSLC4Net.Client;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagementTest;

[TestClass]
[TestCategory("RunningOslcServerRequired")]
public abstract class TestBase
{
    protected virtual IEnumerable<MediaTypeFormatter> Formatters { get; } = OslcRestClient.DEFAULT_FORMATTERS;
    protected Uri? CREATED_CHANGE_REQUEST_URI;
    protected readonly IConfigurationRoot _config;
    protected readonly string _serviceProviderCatalogURI;

    protected TestBase()
    {
        _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                //  .AddEnvironmentVariables()
                 .Build();
        _serviceProviderCatalogURI = _config["serviceProviderCatalog:uri"]!;
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

            foreach (Service service in services)
            {
                if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                {
                    QueryCapability[] queryCapabilities = service.GetQueryCapabilities();

                    foreach (QueryCapability queryCapability in queryCapabilities)
                    {
                        Uri[] resourceTypes = queryCapability.GetResourceTypes();

                        foreach (Uri resourceType in resourceTypes)
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
            foreach (Service service in services)
            {
                if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                {
                    QueryCapability[] queryCapabilities = service.GetQueryCapabilities();
                    foreach (QueryCapability queryCapability in queryCapabilities)
                    {
                        Uri[] resourceTypes = queryCapability.GetResourceTypes();
                        foreach (Uri resourceType in resourceTypes)
                        {
                            if (resourceType.ToString().Equals(type))
                            {
                                Uri resourceShape = queryCapability.GetResourceShape();
                                if (resourceShape != null)
                                {
                                    OslcRestClient oslcRestClient = new(
                                        Formatters, resourceShape, mediaType);
                                    return await oslcRestClient.GetOslcResourceAsync<ResourceShape>();
                                }
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve resource shape for type '" + type + "'");
    }

    protected async void VerifyChangeRequest(string mediaType,
                                            ChangeRequest changeRequest,
                                            bool recurse)
    {
        Assert.IsNotNull(changeRequest);

        Uri aboutURI = changeRequest.GetAbout();
        DateTime? createdDate = changeRequest.GetCreated();
        string identifierString = changeRequest.GetIdentifier();
        DateTime? modifiedDate = changeRequest.GetModified();
        Uri[] rdfTypesURIs = changeRequest.GetRdfTypes();
        Uri serviceProviderURI = changeRequest.GetServiceProvider();

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

            VerifyChangeRequest(mediaType,
                                await aboutOSLCRestClient.GetOslcResourceAsync<ChangeRequest>(),
                                recurse: false);
            if(serviceProviderURI != null) {
                OslcRestClient serviceProviderOSLCRestClient = new(Formatters,
                                                                serviceProviderURI,
                                                                mediaType);

                var serviceProvider = await serviceProviderOSLCRestClient.GetOslcResourceAsync<ServiceProvider>();

                Assert.IsNotNull(serviceProvider);
            }
        }
    }

    protected async void VerifyCompact(string mediaType,
                                      Compact compact)
    {
        Assert.IsNotNull(compact);

        Uri aboutURI = compact.GetAbout();
        string shortTitleString = compact.GetShortTitle();
        string titleString = compact.GetTitle();

        Assert.IsNotNull(aboutURI);
        // TODO: check OSLC Core and print warning otherwise
        // Assert.IsNotNull(shortTitleString);
        Assert.IsNotNull(titleString);

        OslcRestClient aboutOSLCRestClient = new(Formatters,
                                                aboutURI,
                                                mediaType);

        VerifyChangeRequest(mediaType,
                            await aboutOSLCRestClient.GetOslcResourceAsync<ChangeRequest>(),
                            false);
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
            string name = property.GetName();
            Uri propertyDefinition = property.GetPropertyDefinition();

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
        ResourceShape resourceShape = await GetResourceShapeAsync(mediaType,
                                                       Constants.TYPE_CHANGE_REQUEST);

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

        Compact compact = await oslcRestClient.GetOslcResourceAsync<Compact>();

        VerifyCompact(normalMediaType,
                      compact);
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

        string creation = await GetCreationAsync(mediaType, Constants.TYPE_CHANGE_REQUEST);

        OslcRestClient oslcRestClient = new(Formatters,
                                            creation,
                                            mediaType);

        ChangeRequest addedChangeRequest = await oslcRestClient.AddOslcResourceAsync(changeRequest);

        CREATED_CHANGE_REQUEST_URI = addedChangeRequest.GetAbout();

        return addedChangeRequest;
    }

    protected async Task TestCreateAsync(string mediaType)
    {
        // Assert.IsNull(CREATED_CHANGE_REQUEST_URI);

        ChangeRequest addedChangeRequest = await MakeChangeRequestAsync(mediaType);

        VerifyChangeRequest(mediaType,
                            addedChangeRequest,
                            true);
    }

    protected HttpResponseMessage? DeleteChangeRequest(string mediaType)
    {
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

        HttpResponseMessage? clientResponse = DeleteChangeRequest(mediaType);

        Assert.IsNotNull(clientResponse);
        // OSLC 3.0 allows 200 OK or 204 No Content
        // TODO: confirm an exact CC
        CollectionAssert.Contains(new[] { HttpStatusCode.NoContent, HttpStatusCode.OK },
            clientResponse?.StatusCode);
        // Assert.Equals(HttpStatusCode.NoContent, clientResponse?.StatusCode);

        Assert.IsNull(await oslcRestClient.GetOslcResourceAsync<ChangeRequest>());
    }

    protected async Task TestRetrieveAsync(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(Formatters,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        ChangeRequest changeRequest = await oslcRestClient.GetOslcResourceAsync<ChangeRequest>();

        VerifyChangeRequest(mediaType,
                            changeRequest,
                            recurse: true);
    }

    protected async Task TestRetrievesAsync(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        string queryBase = await GetQueryBaseAsync(mediaType, Constants.TYPE_CHANGE_REQUEST);

        Assert.IsNotNull(queryBase);

        var client = new OslcRestClient(Formatters, queryBase, mediaType);

        ICollection<ChangeRequest> changeRequests = await client.GetOslcResourcesAsync<ChangeRequest>();

        Assert.IsNotNull(changeRequests);
        Assert.IsTrue(changeRequests.Count > 0);

        bool found = false;

        // FIXME add paging
        foreach (ChangeRequest changeRequest in changeRequests)
        {
            VerifyChangeRequest(mediaType, changeRequest, recurse: true);

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

        ChangeRequest changeRequest = await oslcRestClient.GetOslcResourceAsync<ChangeRequest>();

        VerifyChangeRequest(mediaType, changeRequest, recurse: true);

        Assert.IsNull(changeRequest.IsApproved());
        Assert.IsNull(changeRequest.GetCloseDate());

        DateTime closeDate = DateTime.Now;

        changeRequest.SetApproved(true);
        changeRequest.SetCloseDate(closeDate);

        HttpResponseMessage clientResponse = oslcRestClient.UpdateOslcResourceReturnClientResponse(changeRequest);

        Assert.IsNotNull(clientResponse);
        Assert.AreEqual(HttpStatusCode.OK, clientResponse.StatusCode);

        var updatedChangeRequest = await oslcRestClient.GetOslcResourceAsync<ChangeRequest>();

        VerifyChangeRequest(mediaType,
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
