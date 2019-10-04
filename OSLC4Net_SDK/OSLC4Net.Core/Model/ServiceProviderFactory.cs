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
 *     Michael Fiedler  - initial API and implementation
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;

namespace OSLC4Net.Core.Model
{
    public class ServiceProviderFactory
    {

        private ServiceProviderFactory() : base()
        {          
        } 

	    public static ServiceProvider CreateServiceProvider(string baseURI, string title, string description, Publisher publisher, Type[] resourceTypes)
        {
		    return InitServiceProvider(new ServiceProvider(), baseURI, title, description, publisher, resourceTypes, null);
	    }
	
	    public static ServiceProvider CreateServiceProvider(string baseURI, string title, string description, Publisher publisher, Type[] resourceTypes, Dictionary<string,object> pathParameterValues)
        {
		    return InitServiceProvider(new ServiceProvider(), baseURI, title, description, publisher, resourceTypes, pathParameterValues);
	    }

	    public static ServiceProvider InitServiceProvider(ServiceProvider serviceProvider, string baseURI, string title, string description, Publisher publisher, Type[] resourceTypes, Dictionary<string,object> pathParameterValues) 
        {
            
		    serviceProvider.SetTitle(title);
		    serviceProvider.SetDescription(description);
		    serviceProvider.SetPublisher(publisher);

            var serviceMap = new Dictionary<string, Service>();

		    foreach (var resourceType in resourceTypes) 
            {
		        OslcService[] serviceAttribute = (OslcService[]) resourceType.GetCustomAttributes(typeof(OslcService),false);

		        if (serviceAttribute == null || serviceAttribute.Length == 0) 
                {
		            throw new OslcCoreMissingAttributeException(resourceType, typeof(OslcService));
		        } else if (serviceAttribute.Length > 1)
                {
                    throw new OslcCoreInvalidAttributeException(resourceType, typeof(OslcService));
                }

                string domain = serviceAttribute[0].value;
                Service service = null;

                bool serviceExists = serviceMap.TryGetValue(domain, out service);
                if (!serviceExists && service == null) {
                    service = new Service(new Uri(domain));
                    serviceMap[domain] =  service;
                }

			    HandleResourceType(baseURI, resourceType, service, pathParameterValues);
		    }

		    // add the services to the provider
		    foreach (var service in serviceMap.Values)
            {
		        serviceProvider.AddService(service);
		    }

		    return serviceProvider;
	    }

        /// <summary>
        /// Examine service methods to determine URLs for QueryCapability, CreationFactory and Dialogs
        /// For more info on how routing works on MVC 4:  http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
        /// </summary>
        /// <param name="baseURI"></param>
        /// <param name="resourceType"></param>
        /// <param name="service"></param>
        /// <param name="pathParameterValues"></param>
	    private static void HandleResourceType(string baseURI, Type resourceType,
			    Service service, Dictionary<string,object> pathParameterValues) 
        {
		    foreach (var method in resourceType.GetMethods()) 
            {
			   
			    if (method.Name.StartsWith("Get")) 
                {
				    OslcQueryCapability [] queryCapabilityAttribute = (OslcQueryCapability [])method.GetCustomAttributes(typeof(OslcQueryCapability), false);
				    string[] resourceShapes = null;
				    if (queryCapabilityAttribute != null && queryCapabilityAttribute.Length > 0) 
                    {
					    service.AddQueryCapability(CreateQueryCapability(baseURI, method, pathParameterValues));
					    string resourceShape = queryCapabilityAttribute[0].resourceShape;
					    if ((resourceShape != null) && (resourceShape.Length > 0)) 
                        {
					        resourceShapes = new string[] {resourceShape};
					    }
				    }
				    OslcDialogs [] dialogsAttribute = (OslcDialogs [])method.GetCustomAttributes(typeof(OslcDialogs), false);
				    if (dialogsAttribute != null && dialogsAttribute.Length > 0) 
                    {
				        OslcDialog [] dialogs = dialogsAttribute[0].value;
				        foreach (var dialog in dialogs) 
                        {
				            if (dialog != null) {
				                service.AddSelectionDialog(CreateSelectionDialog(baseURI, method, dialog, resourceShapes, pathParameterValues));
				            }
				        }
				    }
				    else
				    {
                        OslcDialog [] dialogAttribute = (OslcDialog [])method.GetCustomAttributes(typeof(OslcDialog), false);
                        if (dialogAttribute != null && dialogAttribute.Length > 0) 
                        {
                            service.AddSelectionDialog(CreateSelectionDialog(baseURI, method, dialogAttribute[0], resourceShapes, pathParameterValues));
                        }
				    }
			    } else {
				    if (method.Name.StartsWith("Post")) {
					    OslcCreationFactory [] creationFactoryAttribute =(OslcCreationFactory []) method.GetCustomAttributes(typeof(OslcCreationFactory), false);
					    string[] resourceShapes = null;
					    if (creationFactoryAttribute != null && creationFactoryAttribute.Length > 0) 
                        {
						    service.AddCreationFactory(CreateCreationFactory(baseURI, method, pathParameterValues));
						    resourceShapes = creationFactoryAttribute[0].resourceShapes;
					    }
	                    OslcDialogs [] dialogsAttribute = (OslcDialogs []) method.GetCustomAttributes(typeof(OslcDialogs), false);
	                    if (dialogsAttribute != null && dialogsAttribute.Length > 0) 
                        {
	                        OslcDialog[] dialogs = dialogsAttribute[0].value;
	                        foreach (var dialog in dialogs) 
                            {
	                            if (dialog != null) 
                                {
	                                service.AddCreationDialog(CreateCreationDialog(baseURI, method, dialog, resourceShapes, pathParameterValues));
	                            }
	                        }
	                    }
	                    else
	                    {
	                        OslcDialog [] dialogAttribute = (OslcDialog []) method.GetCustomAttributes(typeof(OslcDialog), false);
	                        if (dialogAttribute != null && dialogAttribute.Length > 0) 
                            {
	                            service.AddCreationDialog(CreateCreationDialog(baseURI, method, dialogAttribute[0], resourceShapes, pathParameterValues));
	                        }
	                    }
				    }
			    }
		    }
	    }

	    private static CreationFactory CreateCreationFactory(string baseURI, MethodInfo method, Dictionary<string,object> pathParameterValues) 
        {
		    OslcCreationFactory [] creationFactoryAttribute = (OslcCreationFactory [])method.GetCustomAttributes(typeof(OslcCreationFactory), false);

		    string title = creationFactoryAttribute[0].title;
		    string label = creationFactoryAttribute[0].label;
		    string[] resourceShapes = creationFactoryAttribute[0].resourceShapes;
		    string[] resourceTypes = creationFactoryAttribute[0].resourceTypes;
		    string[] usages = creationFactoryAttribute[0].usages;


            string typeName = method.DeclaringType.Name;
            //controller names must end with Controller
            int pos = typeName.IndexOf("Controller");
            string controllerName = typeName.Substring(0, pos);
            
            string creation = ResolvePathParameters(baseURI, controllerName.ToLower(), pathParameterValues);
            
            /* TODO Path methodPathAttribute = method.getAttribute(Path.class);
            if (methodPathAttribute != null) {
			    creation = creation + '/' + methodPathAttribute.value();
		    }
             */

		    CreationFactory creationFactory = null;
		    creationFactory = new CreationFactory(title, new Uri(creation)); //TODO:  Normalize the URI

		    if ((label != null) && (label.Length > 0)) 
            {
		        creationFactory.SetLabel(label);
		    }

		    foreach (string resourceShape in resourceShapes) 
            {
                creationFactory.AddResourceShape(new Uri(baseURI + "/" + resourceShape));
            }

		    foreach (string resourceType in resourceTypes) 
            {
                creationFactory.AddResourceType(new Uri(resourceType));
            }

		    foreach (string usage in usages) 
            {
                creationFactory.AddUsage(new Uri(usage));
            }

		    return creationFactory;
	    }

	    private static QueryCapability CreateQueryCapability(string baseURI, MethodInfo method, Dictionary<string,object> pathParameterValues) 
        {
		    OslcQueryCapability [] queryCapabilityAttribute = (OslcQueryCapability [])method.GetCustomAttributes(typeof(OslcQueryCapability), false);

		    string title = queryCapabilityAttribute[0].title;
		    string label = queryCapabilityAttribute[0].label;
		    string resourceShape = queryCapabilityAttribute[0].resourceShape;
            string[] resourceTypes = queryCapabilityAttribute[0].resourceTypes;
            string[] usages = queryCapabilityAttribute[0].usages;

		    string typeName = method.DeclaringType.Name;
            //controller names must end with Controller
            int pos = typeName.IndexOf("Controller");
            string controllerName = typeName.Substring(0, pos);

            string query = ResolvePathParameters(baseURI, controllerName.ToLower(), pathParameterValues);
            /* TODO
            Path methodPathAttribute = method.getAttribute(Path.class);
            if (methodPathAttribute != null) {
                query = query + '/' + methodPathAttribute.value();
            } */

		    QueryCapability queryCapability = null;
		    queryCapability = new QueryCapability(title, new Uri(query));

		    if ((label != null) && (label.Length > 0))
            {
		        queryCapability.SetLabel(label);
		    }

		    if ((resourceShape != null) && (resourceShape.Length > 0)) 
            {
		        queryCapability.SetResourceShape(new Uri(baseURI + resourceShape));
            }

            foreach (string resourceType in resourceTypes) 
            {
                queryCapability.AddResourceType(new Uri(resourceType));
            }

            foreach (string usage in usages) 
            {
                queryCapability.AddUsage(new Uri(usage));
            }

		    return queryCapability;
	    }

        private static Dialog CreateCreationDialog(string baseURI, MethodInfo method, OslcDialog dialogAttribute, string[] resourceShapes,
    		    Dictionary<string,object> pathParameterValues)
        {
            return CreateDialog(baseURI, "Creation", "creation", method, dialogAttribute, resourceShapes, pathParameterValues);
        }

        private static Dialog CreateSelectionDialog(string baseURI, MethodInfo method, OslcDialog dialogAttribute, string[] resourceShapes,
    		    Dictionary<string,object> pathParameterValues)
        {
            return CreateDialog(baseURI, "Selection", "queryBase", method, dialogAttribute, resourceShapes, pathParameterValues);
        }

        private static Dialog CreateDialog(string baseURI, string dialogType, string parameterName, MethodInfo method, OslcDialog dialogAttribute, string[] resourceShapes,
    		    Dictionary<string,object> pathParameterValues)
        {

            string title = dialogAttribute.title;
            string label = dialogAttribute.label;
            string dialogURI = dialogAttribute.uri;
            string hintWidth = dialogAttribute.hintWidth;
            string hintHeight = dialogAttribute.hintHeight;
            string[] resourceTypes = dialogAttribute.resourceTypes;
            string[] usages = dialogAttribute.usages;

            string uri = "";

            string typeName = method.DeclaringType.Name;
            //controller names must end with Controller
            int pos = typeName.IndexOf("Controller");
            string controllerName = typeName.Substring(0, pos);

            if (dialogURI.Length > 0)
            {
                // TODO: Do we chop off everything after the port and then append the dialog URI?
                //       For now just assume that the dialog URI builds on top of the baseURI.
                uri = ResolvePathParameters(baseURI, dialogURI, pathParameterValues);
            }
            else
            {
                throw new OslcCoreInvalidAttributeException(typeof(OslcDialog),typeof(OslcDialog));
            }
            

            Dialog dialog = null;
            dialog = new Dialog(title, new Uri(uri));

            if ((label != null) && (label.Length > 0)) 
            {
                dialog.SetLabel(label);
            }

            if ((hintWidth != null) && (hintWidth.Length > 0)) 
            {
                dialog.SetHintWidth(hintWidth);
            }

            if ((hintHeight != null) && (hintHeight.Length > 0)) 
            {
                dialog.SetHintHeight(hintHeight);
            }

            foreach (string resourceType in resourceTypes) 
            {
                dialog.AddResourceType(new Uri(resourceType));
            }

            foreach (string usage in usages) 
            {
                dialog.AddUsage(new Uri(usage));
            }

            return dialog;
        }
    
        private static string ResolvePathParameters(string basePath, string pathAttribute, Dictionary<string, object> pathParameterValues)
        {
    	    string returnUri = null;
    	
    	    //Build the path from the @Path template + map of parameter value replacements
    	    if (pathParameterValues != null && pathParameterValues.Count > 0)
    	    {
                /* TODO - not supported yet
    		    UriBuilder builder = UriBuilder.fromUri(basePath);
    		    URI resolvedUri = builder.path(pathAttribute).buildFromMap(pathParameterValues);
    		    if (resolvedUri != null)
    		    {
    			    returnUri = resolvedUri.tostring();
    		    }
    		*/
                returnUri = basePath + "/" + pathAttribute;
    	    } 
    	    else
    	    {
    		    // no parameters supplied - assume @Path not templated
    		    returnUri = basePath + "/" + pathAttribute;
    	    }
    	    return returnUri;
    	
        }
    }
}
