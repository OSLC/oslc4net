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
using System.Linq;
using System.Text;

namespace OSLC4Net.Core.Model
{
    /// <summary>
    /// A resource that can hold unknown properties and content. If a setter is not
    /// found for a property when reading a resource, it is added as an extended
    /// property. These extended properties are preserved when writing the resource
    /// back out, for instance on a PUT request. In OSLC, clients MUST preserve
    /// unknown content when performing updates of resources.
    /// 
    /// see <a href="http://open-services.net/bin/view/Main/OslcCoreSpecification?sortcol=table;up=#Unknown_properties_and_content">OSLC Core 2.0: Unknown properties and content</a>
    
    /// </summary>
    public interface IExtendedResource : IResource
    {
        /// <summary>
        /// Gets the RDF types of this resource. These types will be added to the
        /// serialization of the resource in addition to the
        /// OslcResourceShape#describes() annotation
        /// </summary>
        /// <returns></returns>
        ICollection<Uri> GetTypes();

        /// <summary>
        /// Sets the RDF types of this resource. These types will be added to the
        /// serialization of the resource in addition to the
        /// OslcResourceShape#describes() annotation.
        /// </summary>
        /// <param name="types"></param>
        void SetTypes(ICollection<Uri> types);

        /// <summary>
        /// Adds an RDF type to this resource. These types will be added to the
        /// serialization of the resource in addition to the
        /// OslcResourceShape#describes() annotation.
        /// </summary>
        /// <param name="type"></param>
        void AddType(Uri type);

	    /// <summary>
        ///  Sets extended properties not defined in the bean.
	    /// </summary>
	    /// <param name="properties"></param>
	    void SetExtendedProperties(IDictionary<QName, Object> properties);
	
	    /// <summary>
        /// Gets back the list of extended properties not defined in this bean.
	    /// </summary>
	    /// <returns></returns>
        IDictionary<QName, Object> GetExtendedProperties();
    }
}
