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

using System.Xml.Linq;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Oslc.Resources;

public static class RmUtil
{
    public static async Task<ResourceShape> LookupRequirementsInstanceShapesAsync(string serviceProviderUrl,
        string oslcDomain, string oslcResourceType, OslcClient client, string requiredInstanceShape)
    {
        var response = await client.GetResourceAsync<ServiceProvider>(serviceProviderUrl).ConfigureAwait(false);
        //var formatters = client.GetFormatters();
        var serviceProvider = response.Resources.SingleOrDefault();

        if (serviceProvider != null)
            foreach (var service in serviceProvider.GetServices())
            {
                var domain = service.GetDomain();
                if (domain != null && domain.ToString().Equals(oslcDomain))
                {
                    var creationFactories = service.GetCreationFactories();
                    if (creationFactories != null && creationFactories.Length > 0)
                        foreach (var creationFactory in creationFactories)
                        foreach (var resourceType in creationFactory.GetResourceTypes())
                            if (resourceType.ToString() != null &&
                                resourceType.ToString().Equals(oslcResourceType))
                            {
                                var instanceShapes = creationFactory.GetResourceShapes();
                                if (instanceShapes != null)
                                    foreach (var typeURI in instanceShapes)
                                    {
                                        var typeResponse = await client.GetResourceAsync<ResourceShape>(typeURI)
                                            .ConfigureAwait(false);
                                        var resourceShape = typeResponse.Resources.SingleOrDefault();
                                        var typeTitle = resourceShape.GetTitle();
                                        if (typeTitle != null && string.Compare(typeTitle,
                                                requiredInstanceShape, true) == 0)
                                            return resourceShape;
                                    }
                            }
                }
            }

        throw new ResourceNotFoundException(serviceProviderUrl, "InstanceShapes");
    }

    public static XElement ConvertStringToHTML(string text)
    {
        var document = new XDocument();
        var divElement = new XElement(XName.Get("div", RmConstants.NAMESPACE_URI_XHTML));

        document.Add(divElement);
        divElement.SetValue(text);

        return divElement;
    }
}