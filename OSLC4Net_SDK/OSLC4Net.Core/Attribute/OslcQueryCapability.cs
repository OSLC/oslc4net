/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
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
/// <remarks>See https://docs.oasis-open-projects.org/oslc-op/core/v3.0/os/core-vocab.html </remarks>
[AttributeUsage(AttributeTargets.Method)
]
public class OslcQueryCapability(string? title) : System.Attribute
{
    /**
     * Very short label for use in menu items
     */
    public string Label { get; } = "";

    /**
 * Resource shapes
 */
    public string ResourceShape { get; } = "";

    /**
     * Resource types
     */
    public string[] ResourceTypes { get; } = Array.Empty<string>();

    /**
     * Title string that could be used for display
     */
    public string? Title { get; } = title;

    /**
     * Usages
     */
    public string[] Usages { get; } = Array.Empty<string>();
}
