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
///     OSLC CreationFactory attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method)
]
public class OslcCreationFactory : System.Attribute
{
    /**
     * Very short label for use in menu items
     */
    public string label = "";

    /**
     * Resource shapes
     */
    public string[] resourceShapes = { };

    /**
     * Resource types
     */
    public string[] resourceTypes = { };

    /**
     * Title string that could be used for display
     */
    public string title;

    /**
     * Usages
     */
    public string[] usages = { };
}
