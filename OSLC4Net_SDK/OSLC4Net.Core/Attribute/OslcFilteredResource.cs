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
 *
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Method)
    ]
    public class OslcFilteredResource<T> : System.Attribute
    {
        /**
         * Resource.
         */
        public readonly T resource;

        /**
         * properties
         */
        public readonly IDictionary<String, Object> properties;

        public OslcFilteredResource(
            T resource,
            IDictionary<String, Object> properties
        )
        {
            this.resource = resource;
            this.properties = properties;
        }
    }
}
