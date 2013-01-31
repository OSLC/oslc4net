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

namespace OSLC4Net.Core.Model
{
    public class ResponseInfo<T> : FilteredResource<T>
    {
        /**
         * Total count of resource
         */
        public readonly int totalCount;

        /**
         * Next page in paged output
         */
        public readonly String nextPage;

        public
        ResponseInfo(
            T resource,
            IDictionary<String, Object> properties,
            int totalCount,
            String nextPage
        ) : base(resource, properties)
        {
            this.totalCount = totalCount;
            this.nextPage = nextPage;
        }
    
        public
        ResponseInfo(
            T resource,
            IDictionary<String, Object> properties,
            int totalCount,
            Uri nextPage
        ) : this(resource, properties, totalCount,
                 nextPage == null ? null : nextPage.ToString())
        {
        }
    }
}
