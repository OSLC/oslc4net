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
    /**
     * A resource that can hold unknown properties and content. If a setter is not
     * found for a property when reading a resource, it is added as an extended
     * property. These extended properties are preserved when writing the resource
     * back out, for instance on a PUT request. In OSLC, clients MUST preserve
     * unknown content when performing updates of resources.
     * 
     * @see <a href="http://open-services.net/bin/view/Main/OslcCoreSpecification?sortcol=table;up=#Unknown_properties_and_content">OSLC Core 2.0: Unknown properties and content</a>
     */
    public interface IExtendedResource : IResource
    {
        /**
         * Gets the RDF types of this resource. These types will be added to the
         * serialization of the resource in addition to the
         * {@link OslcResourceShape#describes()} annotation.
         * 
         * @return the collection of types
         */
        ICollection<Uri> GetTypes();

        /**
         * Sets the RDF types of this resource. These types will be added to the
         * serialization of the resource in addition to the
         * {@link OslcResourceShape#describes()} annotation.
         * 
         * @param types
         *            the collection of types
         */
        void SetTypes(ICollection<Uri> types);

        /**
         * Adds an RDF type to this resource. These types will be added to the
         * serialization of the resource in addition to the
         * {@link OslcResourceShape#describes()} annotation.
         * 
         * @param type
         *            the type URI
         */
        void AddType(Uri type);

	    /**
	     * Sets extended properties not defined in the bean.
	     * 
	     * @param properties
	     *            a map of properties where the key is the predicate qualified
	     *            name and the value is the object of the statement. Values are
	     *            collections if there are multiple statements for a predicate.
	     */
	    void SetExtendedProperties(IDictionary<QName, Object> properties);
	
	    /**
	     * Gets back the list of extended properties not defined in this bean.
	     * 
	     * @return the extended properties, a map of properties where the key is the
	     *         predicate qualified name and the value is the object of the
	     *         statement
	     */
        IDictionary<QName, Object> GetExtendedProperties();
    }
}
