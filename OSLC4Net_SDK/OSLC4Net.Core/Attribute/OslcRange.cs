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
    /// <summary>
    /// OSLC Range attribute
    /// </summary>
    /// <remarks>See http://open-services.net/bin/view/Main/OSLCCoreSpecAppendixA </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)
    ]
    public class OslcRange : System.Attribute
    {
        /**
         * Specify the range of possible resource types allowed (for properties with a resource value-type).
         */
        public readonly string[] value;

        public OslcRange(params string[] value)
        {
            this.value = new string[value.Length];

            value.CopyTo(this.value, 0);
        }
    }
}