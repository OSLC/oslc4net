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
    public class OslcResponseInfoCollection<T> : OslcResponseInfo<IEnumerable<T>>
    {
        /**
         * Collection of resources
         */
        public readonly IEnumerable<T> collection() { return resource; }

        public
        OslcResponseInfoCollection(
            IEnumerable<T> collection,
            IDictionary<String, Object> properties,
            int totalCount,
            String nextPage
        ) : base(collection, properties, totalCount, nextPage)
        {
        }
    
        public
        OslcResponseInfoCollection(
            IEnumerable<T> collection,
            IDictionary<String, Object> properties,
            int totalCount,
            Uri nextPage
        ) : this(collection, properties, totalCount, nextPage.ToString())
        {
        }
    }
}
