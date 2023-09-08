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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.ChangeManagement;
using OSLC4Net.Client;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagementTest
{
    [TestClass]
    public abstract class TestBase
    {
        private static readonly ISet<MediaTypeFormatter> FORMATTERS = new HashSet<MediaTypeFormatter>();

        static TestBase()
        {
            FORMATTERS.Add(new RdfXmlMediaTypeFormatter());
            FORMATTERS.Add(new OSLC4Net.Core.JsonProvider.JsonMediaTypeFormatter());
        }

        private static Uri CREATED_CHANGE_REQUEST_URI;

        protected TestBase()
        {
        }

        private static String GetCreation(String mediaType,
                                          String type)
        {
            ServiceProvider[] serviceProviders = new ServiceProviderRegistryClient(FORMATTERS, mediaType).GetServiceProviders();

            foreach (ServiceProvider serviceProvider in serviceProviders)
            {
                Service[] services = serviceProvider.GetServices();

                foreach (Service service in services)
                {
                    if (Constants.CHANGE_MANAGEMENT_DOMAIN.Equals(service.GetDomain().ToString()))
                    {
                        CreationFactory[] creationFactories = service.GetCreationFactories();

                        foreach (CreationFactory creationFactory in creationFactories)
                        {
                            Uri[] resourceTypes = creationFactory.GetResourceTypes();

                            foreach (Uri resourceType in resourceTypes)
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

        private static String GetQueryBase(String mediaType,
                                           String type)
        {
            ServiceProvider[] serviceProviders = new ServiceProviderRegistryClient(FORMATTERS, mediaType).GetServiceProviders();

            foreach (ServiceProvider serviceProvider in serviceProviders)
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
                                    return queryCapability.GetQueryBase().ToString();
                                }
                            }
                        }
                    }
                }
            }

            throw new AssertFailedException("Unable to retrieve queryBase for type '" + type + "'");
        }

        private static ResourceShape GetResourceShape(String mediaType,
                                                      String type)
        {
            ServiceProvider[] serviceProviders = new ServiceProviderRegistryClient(FORMATTERS, mediaType).GetServiceProviders();

            foreach (ServiceProvider serviceProvider in serviceProviders)
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
                                        OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                                                                 resourceShape,
                                                                                                 mediaType);

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

        private static void VerifyChangeRequest(String        mediaType,
                                                ChangeRequest changeRequest,
                                                bool          recurse)
        {
            Assert.IsNotNull(changeRequest);

            Uri       aboutURI           = changeRequest.GetAbout();
            DateTime? createdDate        = changeRequest.GetCreated();
            String    identifierString   = changeRequest.GetIdentifier();
            DateTime? modifiedDate       = changeRequest.GetModified();
            Uri[]     rdfTypesURIs       = changeRequest.GetRdfTypes();
            Uri       serviceProviderURI = changeRequest.GetServiceProvider();

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
                OslcRestClient aboutOSLCRestClient = new OslcRestClient(FORMATTERS,
                                                                        aboutURI,
                                                                        mediaType);

                VerifyChangeRequest(mediaType,
                                    aboutOSLCRestClient.GetOslcResource<ChangeRequest>(),
                                    false);

                OslcRestClient serviceProviderOSLCRestClient = new OslcRestClient(FORMATTERS,
                                                                                  serviceProviderURI,
                                                                                  mediaType);

                ServiceProvider serviceProvider = serviceProviderOSLCRestClient.GetOslcResource<ServiceProvider>();

                Assert.IsNotNull(serviceProvider);
            }
        }

        private static void VerifyCompact(String  mediaType,
                                          Compact compact)
        {
            Assert.IsNotNull(compact);

            Uri    aboutURI         = compact.GetAbout();
            String shortTitleString = compact.GetShortTitle();
            String titleString      = compact.GetTitle();

            Assert.IsNotNull(aboutURI);
            Assert.IsNotNull(shortTitleString);
            Assert.IsNotNull(titleString);

            OslcRestClient aboutOSLCRestClient = new OslcRestClient(FORMATTERS,
                                                                    aboutURI,
                                                                    mediaType);

            VerifyChangeRequest(mediaType,
                                aboutOSLCRestClient.GetOslcResource<ChangeRequest>(),
                                false);
        }

        private static void VerifyResourceShape(ResourceShape resourceShape,
                                                String        type)
        {
            Assert.IsNotNull(resourceShape);

            Uri[] describes = resourceShape.GetDescribes();
            Assert.IsNotNull(describes);
            Assert.IsTrue(describes.Length > 0);

            if (type != null)
            {
                Assert.IsTrue(describes.Contains(new Uri(type)));
            }

            OSLC4Net.Core.Model.Property[] properties = resourceShape.GetProperties();

            Assert.IsNotNull(properties);
            Assert.IsTrue(properties.Length > 0);

            foreach (OSLC4Net.Core.Model.Property property in properties)
            {
                String name               = property.GetName();
                Uri    propertyDefinition = property.GetPropertyDefinition();

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

        protected void TestResourceShape(String mediaType)
        {
            ResourceShape resourceShape = GetResourceShape(mediaType,
                                                           Constants.TYPE_CHANGE_REQUEST);

            VerifyResourceShape(resourceShape,
                                Constants.TYPE_CHANGE_REQUEST);
        }

        protected void TestCompact(String compactMediaType,
                                   String normalMediaType)
        {
            Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

            OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                               CREATED_CHANGE_REQUEST_URI,
                                                               compactMediaType);

            Compact compact = oslcRestClient.GetOslcResource<Compact>();

            VerifyCompact(normalMediaType,
                          compact);
        }

        protected ChangeRequest MakeChangeRequest(String mediaType)
        {
            CREATED_CHANGE_REQUEST_URI = null;

            ChangeRequest changeRequest = new ChangeRequest();

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

            String creation = GetCreation(mediaType, Constants.TYPE_CHANGE_REQUEST);

            OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                               creation,
                                                               mediaType);

            ChangeRequest addedChangeRequest = oslcRestClient.AddOslcResource(changeRequest);

            CREATED_CHANGE_REQUEST_URI = addedChangeRequest.GetAbout();

            return addedChangeRequest;
        }

        protected void TestCreate(String mediaType)
        {
            Assert.IsNull(CREATED_CHANGE_REQUEST_URI);

            ChangeRequest addedChangeRequest = MakeChangeRequest(mediaType);

            VerifyChangeRequest(mediaType,
                                addedChangeRequest,
                                true);
        }

        protected HttpResponseMessage DeleteChangeRequest(String mediaType)
        {
            try
            {
                OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
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

        protected void TestDelete(String mediaType)
        {
            OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                               CREATED_CHANGE_REQUEST_URI,
                                                               mediaType);

            Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

            HttpResponseMessage clientResponse = DeleteChangeRequest(mediaType);

            Assert.IsNotNull(clientResponse);
            Assert.AreEqual(HttpStatusCode.NoContent, clientResponse.StatusCode);

            Assert.IsNull(oslcRestClient.GetOslcResource<ChangeRequest>());
        }

        protected void TestRetrieve(String mediaType)
        {
            Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

            OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                               CREATED_CHANGE_REQUEST_URI,
                                                               mediaType);

            ChangeRequest changeRequest = oslcRestClient.GetOslcResource<ChangeRequest>();

            VerifyChangeRequest(mediaType,
                                changeRequest,
                                true);
        }

        protected void TestRetrieves(String mediaType)
        {
            Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

            String queryBase = GetQueryBase(mediaType,
                                            Constants.TYPE_CHANGE_REQUEST);

            Assert.IsNotNull(queryBase);

            OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                               queryBase,
                                                               mediaType);

            ChangeRequest[] changeRequests = oslcRestClient.GetOslcResources<ChangeRequest>();

            Assert.IsNotNull(changeRequests);
            Assert.IsTrue(changeRequests.Length > 0);

            bool found = false;

            foreach (ChangeRequest changeRequest in changeRequests)
            {
                VerifyChangeRequest(mediaType,
                                    changeRequest,
                                    true);

                if (CREATED_CHANGE_REQUEST_URI.Equals(changeRequest.GetAbout()))
                {
                    found = true;
                }
            }

            Assert.IsTrue(found);
        }

        protected void TestUpdate(String mediaType)
        {
            Assert.IsNotNull(CREATED_CHANGE_REQUEST_URI);

            OslcRestClient oslcRestClient = new OslcRestClient(FORMATTERS,
                                                               CREATED_CHANGE_REQUEST_URI,
                                                               mediaType);

            ChangeRequest changeRequest = oslcRestClient.GetOslcResource<ChangeRequest>();

            VerifyChangeRequest(mediaType,
                                changeRequest,
                                true);

            Assert.IsNull(changeRequest.IsApproved());
            Assert.IsNull(changeRequest.GetCloseDate());

            DateTime closeDate = DateTime.Now;

            changeRequest.SetApproved(true);
            changeRequest.SetCloseDate(closeDate);

            HttpResponseMessage clientResponse = oslcRestClient.UpdateOslcResourceReturnClientResponse(changeRequest);

            Assert.IsNotNull(clientResponse);
            Assert.AreEqual(HttpStatusCode.OK, clientResponse.StatusCode);

            ChangeRequest updatedChangeRequest = oslcRestClient.GetOslcResource<ChangeRequest>();

            VerifyChangeRequest(mediaType,
                                updatedChangeRequest,
                                true);

            Assert.AreEqual(changeRequest.GetAbout(), updatedChangeRequest.GetAbout());
            Assert.AreEqual(true, updatedChangeRequest.IsApproved());
            Assert.AreEqual(closeDate.ToShortDateString() + " - " + closeDate.ToShortTimeString(),
                            updatedChangeRequest.GetCloseDate().Value.ToShortDateString() + " - " + updatedChangeRequest.GetCloseDate().Value.ToShortTimeString());
            Assert.IsFalse(changeRequest.GetModified().Equals(updatedChangeRequest.GetModified()));
            Assert.IsTrue(updatedChangeRequest.GetModified() > updatedChangeRequest.GetCreated());
        }
    }
}
