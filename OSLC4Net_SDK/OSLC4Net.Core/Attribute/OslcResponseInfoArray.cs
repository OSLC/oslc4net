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
    public class OslcResponseInfoArray<T> : OslcResponseInfo<T[]>
    {
        /**
         * Array of resources
         */
        public readonly T[] array() { return resource; }

        public
        OslcResponseInfoArray(
            T[] array,
            IDictionary<String, Object> properties,
            int totalCount,
            String nextPage
        ) : base(array, properties, totalCount, nextPage)
        {
        }
    
        public
        OslcResponseInfoArray(
            T[] array,
            IDictionary<String, Object> properties,
            int totalCount,
            Uri nextPage
        ) : this(array, properties, totalCount, nextPage.ToString())
        {
        }
    }
}
