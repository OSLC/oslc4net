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
/// OSLC CreationFactory attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class OslcCreationFactory : System.Attribute
{
    /// <summary>
    /// Very short label for use in menu items
    /// </summary>
    public string label = "";

    /// <summary>
    /// Resource shapes
    /// </summary>
    public string[] resourceShapes = Array.Empty<string>();

    /// <summary>
    /// Resource types
    /// </summary>
    public string[] resourceTypes = Array.Empty<string>();

    /// <summary>
    /// Title string that could be used for display
    /// </summary>
    public string title;

    /// <summary>
    /// Usages
    /// </summary>
    public string[] usages = Array.Empty<string>();
}
