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
 *
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSLC4Net.Core.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Method)
    ]
    public class OslcQueryCapability : System.Attribute
    {
	    /**
	     * Title string that could be used for display
	     */
	    public readonly string title;

	    /**
	     * Very short label for use in menu items
	     */
	    public readonly string label = "";

	    /**
         * Resource shapes
         */
        public readonly string resourceShape = "";

        /**
         * Resource types
         */
        public readonly string[] resourceTypes = { };

        /**
         * Usages
         */
        public readonly string[] usages = { };

        public OslcQueryCapability(
            string title
        )
        {
            this.title = title;
        }
    }
}
