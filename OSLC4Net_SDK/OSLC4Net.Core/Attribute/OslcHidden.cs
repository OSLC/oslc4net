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

namespace OSLC4Net.Core.Attribute;

/// <summary>
///     OSLC Hidden attribute
/// </summary>
/// <remarks>See http://open-services.net/bin/view/Main/OSLCCoreSpecAppendixA </remarks>
[AttributeUsage(AttributeTargets.Method)
]
public class OslcHidden : System.Attribute
{
    /**
     * A hint that indicates that property MAY be hidden when presented in a user interface.
     */
    public readonly bool value;

    public OslcHidden(bool value)
    {
        this.value = value;
    }
}
