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

using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Attribute;

/// <summary>
///     OSLC Representation attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)
]
public class OslcRepresentation : System.Attribute
{
    /**
     * Specify how the resource will be represented (for properties with a resource value-type).
     */
    public readonly Representation value;

    public OslcRepresentation(Representation value)
    {
        this.value = value;
    }
}
