/*******************************************************************************
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.ChangeManagement;
using OSLC4Net.Client;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagementTest;

[TestClass]
[TestCategory("RunningOslcServerRequired")]
public abstract class TestBase
{
    private static readonly ISet<MediaTypeFormatter> FORMATTERS = new HashSet<MediaTypeFormatter>();

    static TestBase()
    {
        FORMATTERS.Add(new RdfXmlMediaTypeFormatter());
    }

    private static Uri? CREATED_CHANGE_REQUEST_URI;

    protected TestBase()
    {
    }

    private static string GetCreation(string mediaType,
                                      string type)
    {
        ServiceProvider[] serviceProviders = new ServiceProviderRegistryClient(FORMATTERS, mediaType).GetServiceProviders();

        foreach (var serviceProvider in serviceProviders)
        {
            Service[] services = serviceProvider.GetServices();

            foreach (var service in services)
            {
                if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                {
                    CreationFactory[] creationFactories = service.GetCreationFactories();

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

    private static string GetQueryBase(string mediaType,
                                       string type)
    {
        ServiceProvider[] serviceProviders = new ServiceProviderRegistryClient(FORMATTERS, mediaType).GetServiceProviders();

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
                                return queryCapability.GetQueryBase().ToString();
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve queryBase for type '" + type + "'");
    }

    private static ResourceShape GetResourceShape(string mediaType,
                                                  string type)
    {
        ServiceProvider[] serviceProviders = new ServiceProviderRegistryClient(FORMATTERS, mediaType).GetServiceProviders();

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
                                        FORMATTERS, resourceShape, mediaType);
                                    return oslcRestClient.GetOslcResource<ResourceShape>();
                                }
                            }
                        }
                    }
                }
            }
        }

        throw new AssertFailedException("Unable to retrieve resource shape for type '" + type + "'");
   }

    private static void VerifyChangeRequest(string mediaType,
                                            ChangeRequest changeRequest,
                                            bool          recurse)
    {
        Assert.IsNotNull(changeRequest);

        var       aboutURI           = changeRequest.GetAbout();
        var createdDate        = changeRequest.GetCreated();
        var identifierString   = changeRequest.GetIdentifier();
        var modifiedDate       = changeRequest.GetModified();
        Uri[]     rdfTypesURIs       = changeRequest.GetRdfTypes();
        var       serviceProviderURI = changeRequest.GetServiceProvider();

        Assert.IsNotNull(aboutURI);
        Assert.IsNotNull(createdDate);
        Assert.IsNotNull(identifierString);
        Assert.IsNotNull(modifiedDate);
        Assert.IsNotNull(rdfTypesURIs);
        Assert.IsNotNull(serviceProviderURI);

        Assert.IsTrue(aboutURI.ToString().EndsWith(identifierString));
        Assert.IsTrue(modifiedDate.Equals(createdDate) || modifiedDate > createdDate);
        Assert.IsTrue(rdfTypesURIs.Contains(new Uri(Constants.TYPE_CHANGE_REQUEST)));

        if (recurse)
        {
            OslcRestClient aboutOSLCRestClient = new(FORMATTERS,
                                                    aboutURI,
                                                    mediaType);

            VerifyChangeRequest(mediaType,
                                aboutOSLCRestClient.GetOslcResource<ChangeRequest>(),
                                recurse: false);

            OslcRestClient serviceProviderOSLCRestClient = new(FORMATTERS,
                                                            serviceProviderURI,
                                                            mediaType);

            var serviceProvider = serviceProviderOSLCRestClient.GetOslcResource<ServiceProvider>();

            Assert.IsNotNull(serviceProvider);
        }
    }

    private static void VerifyCompact(string mediaType,
                                      Compact compact)
    {
        Assert.IsNotNull(compact);

        var    aboutURI         = compact.GetAbout();
        var shortTitleString = compact.GetShortTitle();
        var titleString      = compact.GetTitle();

        Assert.IsNotNull(aboutURI);
        Assert.IsNotNull(shortTitleString);
        Assert.IsNotNull(titleString);

        OslcRestClient aboutOSLCRestClient = new(FORMATTERS,
                                                aboutURI,
                                                mediaType);

        VerifyChangeRequest(mediaType,
                            aboutOSLCRestClient.GetOslcResource<ChangeRequest>(),
                            false);
    }

    private static void VerifyResourceShape(ResourceShape resourceShape,
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
            var name               = property.GetName();
            var    propertyDefinition = property.GetPropertyDefinition();

            Assert.IsNotNull(property.GetDescription());
            Assert.IsNotNull(name);
            Assert.IsNotNull(property.GetOccurs());
            Assert.IsNotNull(propertyDefinition);
            Assert.IsNotNull(property.GetTitle());
            Assert.IsNotNull(property.GetValueType());

            Assert.IsTrue(propertyDefinition.ToString().EndsWith(name),
                          "propertyDefinition [" + propertyDefinition.ToString() + "], name [" + name + "]");
        }
    }

    protected void TestResourceShape(string mediaType)
    {
        var resourceShape = GetResourceShape(mediaType,
                                                       Constants.TYPE_CHANGE_REQUEST);

        VerifyResourceShape(resourceShape,
                            Constants.TYPE_CHANGE_REQUEST);
    }

    protected void TestCompact(string compactMediaType,
                               string normalMediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(FORMATTERS,
                                                           CREATED_CHANGE_REQUEST_URI,
                                                           compactMediaType);

        var compact = oslcRestClient.GetOslcResource<Compact>();

        VerifyCompact(normalMediaType,
                      compact);
    }

    protected ChangeRequest MakeChangeRequest(string mediaType)
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

        var creation = GetCreation(mediaType, Constants.TYPE_CHANGE_REQUEST);

        OslcRestClient oslcRestClient = new(FORMATTERS,
                                            creation,
                                            mediaType);

        var addedChangeRequest = oslcRestClient.AddOslcResource(changeRequest);

        CREATED_CHANGE_REQUEST_URI = addedChangeRequest.GetAbout();

        return addedChangeRequest;
    }

    protected void TestCreate(string mediaType)
    {
        Assert.IsNull(CREATED_CHANGE_REQUEST_URI);

        var addedChangeRequest = MakeChangeRequest(mediaType);

        VerifyChangeRequest(mediaType,
                            addedChangeRequest,
                            true);
    }

    protected HttpResponseMessage? DeleteChangeRequest(string mediaType)
    {
        try
        {
            OslcRestClient oslcRestClient = new(FORMATTERS,
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

    protected void TestDelete(string mediaType)
    {
        OslcRestClient oslcRestClient = new(FORMATTERS,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        var clientResponse = DeleteChangeRequest(mediaType);

        Assert.IsNotNull(clientResponse);
        Assert.AreEqual(HttpStatusCode.NoContent, clientResponse?.StatusCode);

        Assert.IsNull(oslcRestClient.GetOslcResource<ChangeRequest>());
    }

    protected void TestRetrieve(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(FORMATTERS,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        var changeRequest = oslcRestClient.GetOslcResource<ChangeRequest>();

        VerifyChangeRequest(mediaType,
                            changeRequest,
                            recurse: true);
    }

    protected void TestRetrieves(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        var queryBase = GetQueryBase(mediaType,
                                        Constants.TYPE_CHANGE_REQUEST);

        Assert.IsNotNull(queryBase);

        OslcRestClient oslcRestClient = new(FORMATTERS,
                                            queryBase,
                                            mediaType);

        ChangeRequest[] changeRequests = oslcRestClient.GetOslcResources<ChangeRequest>();

        Assert.IsNotNull(changeRequests);
        Assert.IsTrue(changeRequests.Length > 0);

        var found = false;

        foreach (var changeRequest in changeRequests)
        {
            VerifyChangeRequest(mediaType, changeRequest, recurse: true);

            if (CREATED_CHANGE_REQUEST_URI.Equals(changeRequest.GetAbout()))
            {
                found = true;
            }
        }

        Assert.IsTrue(found);
    }

    protected static void TestUpdate(string mediaType)
    {
        Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

        OslcRestClient oslcRestClient = new(FORMATTERS,
                                            CREATED_CHANGE_REQUEST_URI,
                                            mediaType);

        var changeRequest = oslcRestClient.GetOslcResource<ChangeRequest>();

        VerifyChangeRequest(mediaType, changeRequest, recurse: true);

        Assert.IsNull(changeRequest.IsApproved());
        Assert.IsNull(changeRequest.GetCloseDate());

        var closeDate = DateTime.Now;

        changeRequest.SetApproved(true);
        changeRequest.SetCloseDate(closeDate);

        var clientResponse = oslcRestClient.UpdateOslcResourceReturnClientResponse(changeRequest);

        Assert.IsNotNull(clientResponse);
        Assert.AreEqual(HttpStatusCode.OK, clientResponse.StatusCode);

        var updatedChangeRequest = oslcRestClient.GetOslcResource<ChangeRequest>();

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
