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

using System.Reflection;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;

namespace OSLC4Net.Core.Model;

public class ServiceProviderFactory
{
    private ServiceProviderFactory()
    {
    }

    public static ServiceProvider CreateServiceProvider(string baseURI, string title,
        string description, Publisher publisher, Type[] resourceTypes)
    {
        return InitServiceProvider(new ServiceProvider(), baseURI, title, description, publisher,
            resourceTypes, null);
    }

    public static ServiceProvider CreateServiceProvider(string baseURI, string title,
        string description, Publisher publisher, Type[] resourceTypes,
        Dictionary<string, object> pathParameterValues)
    {
        return InitServiceProvider(new ServiceProvider(), baseURI, title, description, publisher,
            resourceTypes, pathParameterValues);
    }

    public static ServiceProvider InitServiceProvider(ServiceProvider serviceProvider,
        string baseURI, string title, string description, Publisher publisher, Type[] resourceTypes,
        Dictionary<string, object> pathParameterValues)
    {
        serviceProvider.SetTitle(title);
        serviceProvider.SetDescription(description);
        serviceProvider.SetPublisher(publisher);

        var serviceMap = new Dictionary<string, Service>(StringComparer.Ordinal);

        foreach (var resourceType in resourceTypes)
        {
            var serviceAttribute =
                (OslcService[])resourceType.GetCustomAttributes(typeof(OslcService), false);

            if (serviceAttribute == null || serviceAttribute.Length == 0)
            {
                throw new OslcCoreMissingAttributeException(resourceType, typeof(OslcService));
            }

            if (serviceAttribute.Length > 1)
            {
                throw new OslcCoreInvalidAttributeException(resourceType, typeof(OslcService));
            }

            var domain = serviceAttribute[0].value;
            Service service;
            var serviceExists = serviceMap.TryGetValue(domain, out service);
            if (!serviceExists && service == null)
            {
                service = new Service(new Uri(domain));
                serviceMap[domain] = service;
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
    ///     Examine service methods to determine URLs for QueryCapability, CreationFactory and Dialogs
    ///     For more info on how routing works on MVC 4:
    ///     http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
    /// </summary>
    /// <param name="baseURI"></param>
    /// <param name="resourceType"></param>
    /// <param name="service"></param>
    /// <param name="pathParameterValues"></param>
    private static void HandleResourceType(string baseURI, Type resourceType,
        Service service, Dictionary<string, object> pathParameterValues)
    {
        foreach (var method in resourceType.GetMethods())
        {
            if (method.Name.StartsWith("Get", StringComparison.Ordinal))
            {
                var queryCapabilityAttribute =
                    method.GetCustomAttributes(typeof(OslcQueryCapability),
                        false) as OslcQueryCapability[];
                string[]? resourceShapes = null;
                if (queryCapabilityAttribute?.Length > 0)
                {
                    service.AddQueryCapability(CreateQueryCapability(baseURI, method,
                        pathParameterValues));
                    var resourceShape = queryCapabilityAttribute[0].ResourceShape;
                    if (resourceShape != null && resourceShape.Length > 0)
                    {
                        resourceShapes = new[] { resourceShape };
                    }
                }

                var dialogsAttribute =
                    (OslcDialogs[])method.GetCustomAttributes(typeof(OslcDialogs), false);
                if (dialogsAttribute != null && dialogsAttribute.Length > 0)
                {
                    var dialogs = dialogsAttribute[0].Value;
                    if (dialogs != null)
                    {
                        foreach (var dialog in dialogs)
                        {
                            service.AddSelectionDialog(CreateSelectionDialog(baseURI, method,
                                dialog, resourceShapes, pathParameterValues));
                        }
                    }
                }
                else
                {
                    var dialogAttribute =
                        method.GetCustomAttributes(typeof(OslcDialog), false) as OslcDialog[];
                    if (dialogAttribute is { Length: > 0 })
                    {
                        service.AddSelectionDialog(CreateSelectionDialog(baseURI, method,
                            dialogAttribute[0], resourceShapes, pathParameterValues));
                    }
                }
            }
            else
            {
                if (method.Name.StartsWith("Post", StringComparison.Ordinal))
                {
                    var creationFactoryAttribute =
                        method.GetCustomAttributes(
                            typeof(OslcCreationFactory), false) as OslcCreationFactory[];
                    string[]? resourceShapes = null;
                    if (creationFactoryAttribute is { Length: > 0 })
                    {
                        service.AddCreationFactory(CreateCreationFactory(baseURI, method,
                            pathParameterValues));
                        resourceShapes = creationFactoryAttribute[0].resourceShapes;
                    }

                    var dialogsAttribute =
                        method.GetCustomAttributes(typeof(OslcDialogs), false) as OslcDialogs[];
                    if (dialogsAttribute is { Length: > 0 })
                    {
                        var dialogs = dialogsAttribute[0].Value;
                        if (dialogs != null)
                        {
                            foreach (var dialog in dialogs)
                            {
                                service.AddCreationDialog(CreateCreationDialog(baseURI, method,
                                    dialog, resourceShapes, pathParameterValues));
                            }
                        }
                    }
                    else
                    {
                        var dialogAttribute =
                            (OslcDialog[])method.GetCustomAttributes(typeof(OslcDialog), false);
                        if (dialogAttribute != null && dialogAttribute.Length > 0)
                        {
                            service.AddCreationDialog(CreateCreationDialog(baseURI, method,
                                dialogAttribute[0], resourceShapes, pathParameterValues));
                        }
                    }
                }
            }
        }
    }

    private static CreationFactory CreateCreationFactory(string baseURI, MethodInfo method,
        Dictionary<string, object> pathParameterValues)
    {
        var creationFactoryAttribute =
            (OslcCreationFactory[])method.GetCustomAttributes(typeof(OslcCreationFactory), false);

        var title = creationFactoryAttribute[0].title;
        var label = creationFactoryAttribute[0].label;
        var resourceShapes = creationFactoryAttribute[0].resourceShapes;
        var resourceTypes = creationFactoryAttribute[0].resourceTypes;
        var usages = creationFactoryAttribute[0].usages;

        var typeName = method.DeclaringType.Name;
        //controller names must end with Controller
        var pos = typeName.IndexOf("Controller", StringComparison.Ordinal);
        var controllerName = typeName.Substring(0, pos);

        var creation =
            ResolvePathParameters(baseURI, controllerName.ToLower(), pathParameterValues);
        /* TODO Path methodPathAttribute = method.getAttribute(Path.class);
        if (methodPathAttribute != null) {
            creation = creation + '/' + methodPathAttribute.value();
        }
         */

        // TODO: normalise URI
        var creationFactory = new CreationFactory(title, new Uri(creation));

        if (label != null && label.Length > 0)
        {
            creationFactory.SetLabel(label);
        }

        foreach (var resourceShape in resourceShapes)
        {
            creationFactory.AddResourceShape(new Uri(baseURI + "/" + resourceShape));
        }

        foreach (var resourceType in resourceTypes)
        {
            creationFactory.AddResourceType(new Uri(resourceType));
        }

        foreach (var usage in usages)
        {
            creationFactory.AddUsage(new Uri(usage));
        }

        return creationFactory;
    }

    private static QueryCapability CreateQueryCapability(string baseURI, MethodInfo method,
        Dictionary<string, object> pathParameterValues)
    {
        var queryCapabilityAttribute =
            (OslcQueryCapability[])method.GetCustomAttributes(typeof(OslcQueryCapability), false);

        var title = queryCapabilityAttribute[0].Title;
        var label = queryCapabilityAttribute[0].Label;
        var resourceShape = queryCapabilityAttribute[0].ResourceShape;
        var resourceTypes = queryCapabilityAttribute[0].ResourceTypes;
        var usages = queryCapabilityAttribute[0].Usages;

        var typeName = method.DeclaringType.Name;
        //controller names must end with Controller
        var pos = typeName.IndexOf("Controller", StringComparison.Ordinal);
        var controllerName = typeName.Substring(0, pos);

        var query = ResolvePathParameters(baseURI, controllerName.ToLower(), pathParameterValues);
        /* TODO
        Path methodPathAttribute = method.getAttribute(Path.class);
        if (methodPathAttribute != null) {
            query = query + '/' + methodPathAttribute.value();
        } */

        var queryCapability = new QueryCapability(title, new Uri(query));

        if (label != null && label.Length > 0)
        {
            queryCapability.SetLabel(label);
        }

        if (resourceShape != null && resourceShape.Length > 0)
        {
            queryCapability.SetResourceShape(new Uri(baseURI + resourceShape));
        }

        foreach (var resourceType in resourceTypes)
        {
            queryCapability.AddResourceType(new Uri(resourceType));
        }

        foreach (var usage in usages)
        {
            queryCapability.AddUsage(new Uri(usage));
        }

        return queryCapability;
    }

    private static Dialog CreateCreationDialog(string baseURI, MethodInfo method,
        OslcDialog dialogAttribute, string[] resourceShapes,
        Dictionary<string, object> pathParameterValues)
    {
        return CreateDialog(baseURI, "Creation", "creation", method, dialogAttribute,
            resourceShapes, pathParameterValues);
    }

    private static Dialog CreateSelectionDialog(string baseURI, MethodInfo method,
        OslcDialog dialogAttribute, string[] resourceShapes,
        Dictionary<string, object> pathParameterValues)
    {
        return CreateDialog(baseURI, "Selection", "queryBase", method, dialogAttribute,
            resourceShapes, pathParameterValues);
    }

    private static Dialog CreateDialog(string baseURI, string dialogType, string parameterName,
        MethodInfo method, OslcDialog dialogAttribute, string[] resourceShapes,
        Dictionary<string, object> pathParameterValues)
    {
        var title = dialogAttribute.title;
        var label = dialogAttribute.label;
        var dialogURI = dialogAttribute.uri;
        var hintWidth = dialogAttribute.hintWidth;
        var hintHeight = dialogAttribute.hintHeight;
        var resourceTypes = dialogAttribute.resourceTypes;
        var usages = dialogAttribute.usages;
        var typeName = method.DeclaringType.Name;
        //controller names must end with Controller
        var pos = typeName.IndexOf("Controller", StringComparison.Ordinal);
        _ = typeName.Substring(0, pos);

        string uri;
        if (dialogURI.Length > 0)
        {
            // TODO: Do we chop off everything after the port and then append the dialog URI?
            //       For now just assume that the dialog URI builds on top of the baseURI.
            uri = ResolvePathParameters(baseURI, dialogURI, pathParameterValues);
        }
        else
        {
            throw new OslcCoreInvalidAttributeException(typeof(OslcDialog), typeof(OslcDialog));
        }

        var dialog = new Dialog(title, new Uri(uri));

        if (label != null && label.Length > 0)
        {
            dialog.SetLabel(label);
        }

        if (hintWidth != null && hintWidth.Length > 0)
        {
            dialog.SetHintWidth(hintWidth);
        }

        if (hintHeight != null && hintHeight.Length > 0)
        {
            dialog.SetHintHeight(hintHeight);
        }

        foreach (var resourceType in resourceTypes)
        {
            dialog.AddResourceType(new Uri(resourceType));
        }

        foreach (var usage in usages)
        {
            dialog.AddUsage(new Uri(usage));
        }

        return dialog;
    }

    private static string ResolvePathParameters(string basePath, string pathAttribute,
        Dictionary<string, object> pathParameterValues)
    {
        string returnUri;

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
