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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSLC4Net.Core.Model;
using System.Net.Http;
using System.Net.Http.Formatting;
using OSLC4Net.Client.Exceptions;
using System.Xml.Linq;

namespace OSLC4Net.Client.Oslc.Resources
{
    public static class RmUtil
    {
	    public static ResourceShape LookupRequirementsInstanceShapes(string serviceProviderUrl, string oslcDomain, string oslcResourceType, OslcClient client, string requiredInstanceShape) 
	    {
		    HttpResponseMessage response = client.GetResource(serviceProviderUrl, OSLCConstants.CT_RDF);
            ISet<MediaTypeFormatter> formatters = client.GetFormatters();
		    ServiceProvider serviceProvider = response.Content.ReadAsAsync<ServiceProvider>(formatters).Result;
				
		    if (serviceProvider != null) {
			    foreach (Service service in serviceProvider.GetServices()) {
				    Uri domain = service.GetDomain();				
				    if (domain != null  && domain.ToString().Equals(oslcDomain)) {
					    CreationFactory [] creationFactories = service.GetCreationFactories();
					    if (creationFactories != null && creationFactories.Length > 0) {
						    foreach  (CreationFactory creationFactory in creationFactories) {
							    foreach  (Uri resourceType in creationFactory.GetResourceTypes()) {
								    if (resourceType.ToString() != null && resourceType.ToString().Equals(oslcResourceType)) {
									    Uri[] instanceShapes = creationFactory.GetResourceShapes();
									    if (instanceShapes != null ){
										    foreach ( Uri typeURI in instanceShapes) {
											    response = client.GetResource(typeURI.ToString(),OSLCConstants.CT_RDF);
											    ResourceShape resourceShape =  response.Content.ReadAsAsync<ResourceShape>(formatters).Result;
                                                string typeTitle = resourceShape.GetTitle();
											    if ( ( typeTitle != null) && (string.Compare(typeTitle, requiredInstanceShape, true) == 0) ) {
												    return resourceShape;	
											    }
										    }
									    }
								    }							
							    }
						    }
					    }
				    }
			    }
		    }		 
		
		    throw new ResourceNotFoundException(serviceProviderUrl, "InstanceShapes");
        }		
	
	    public static XElement ConvertStringToHTML(string text) {

            XDocument document = new XDocument();
		    XElement divElement = new XElement(XName.Get("div", RmConstants.NAMESPACE_URI_XHTML));

            document.Add(divElement);
		    divElement.SetValue(text);

		    return divElement;
	    }
    }
}
