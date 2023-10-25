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

namespace OSLC4Net.Core.Attribute
{
    /// <summary>
    /// OSLC ValueType attribue
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)
    ]
    public class OslcValueType : System.Attribute
    {
        /**
         * Value-type of the property.
         */
        public readonly OSLC4Net.Core.Model.ValueType value;

        public OslcValueType(OSLC4Net.Core.Model.ValueType value)
        {
            this.value = value;
        }
    }
}
