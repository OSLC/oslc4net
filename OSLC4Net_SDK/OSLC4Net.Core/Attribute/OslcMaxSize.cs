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
    /// OSLC MaxSize attribute
    /// </summary>
    /// <remarks>See http://open-services.net/bin/view/Main/OSLCCoreSpecAppendixA </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)
    ]
    public class OslcMaxSize : System.Attribute
    {
        /**
         * For String properties only, specifies maximum characters allowed.
         * If not set, then there is no maximum or maximum is specified elsewhere.
         */
        public readonly int value;

        public OslcMaxSize(int value)
        {
            this.value = value;
        }
    }
}