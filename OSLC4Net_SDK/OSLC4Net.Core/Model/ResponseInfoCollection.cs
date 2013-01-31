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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC ResponseInfo Resource Shape", describes = new string[] { OslcConstants.TYPE_RESPONSE_INFO })]
    public class ResponseInfoCollection<T> : ResponseInfo<IEnumerable<T>>
    {
        /**
         * Collection of resources
         */
        public IEnumerable<T> collection() { return resource; }

        public
        ResponseInfoCollection(
            IEnumerable<T> collection,
            IDictionary<String, Object> properties,
            int totalCount,
            String nextPage
        ) : base(collection, properties, totalCount, nextPage)
        {
        }
    
        public
        ResponseInfoCollection(
            IEnumerable<T> collection,
            IDictionary<String, Object> properties,
            int totalCount,
            Uri nextPage
        ) : this(collection, properties, totalCount, nextPage.ToString())
        {
        }
    }
}
