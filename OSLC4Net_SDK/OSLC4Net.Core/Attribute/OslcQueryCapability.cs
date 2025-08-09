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
///     OSLC QueryCapability attribute
/// </summary>
/// <remarks>See http://open-services.net/bin/view/Main/OSLCCoreSpecAppendixA </remarks>
[AttributeUsage(AttributeTargets.Method)
]
public class OslcQueryCapability(string? title) : System.Attribute
{
    /**
     * Very short label for use in menu items
     */
    public readonly string Label = "";

    /**
 * Resource shapes
 */
    public readonly string ResourceShape = "";

    /**
     * Resource types
     */
    public readonly string[] ResourceTypes = Array.Empty<string>();

    /**
     * Title string that could be used for display
     */
    public readonly string? Title = title;

    /**
     * Usages
     */
    public readonly string[] Usages = Array.Empty<string>();
}
