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
/// OSLC Dialog attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class OslcDialog : System.Attribute
{
    /// <summary>
    /// Values MUST be expressed in relative length units.  Em and ex units are interpreted relative to the default system font (at 100% size).
    /// </summary>
    public string hintHeight = "";

    /// <summary>
    /// Values MUST be expressed in relative length units.  Em and ex units are interpreted relative to the default system font (at 100% size).
    /// </summary>
    public string hintWidth = "";

    /// <summary>
    /// Very short label for use in menu items
    /// </summary>
    public string label = "";

    /// <summary>
    /// Resource types
    /// </summary>
    public string[] resourceTypes = Array.Empty<string>();

    /// <summary>
    /// Title string that could be used for display
    /// </summary>
    public string title;

    /// <summary>
    /// The URI of the dialog
    /// </summary>
    public string uri;

    /// <summary>
    /// Usages
    /// </summary>
    public string[] usages = Array.Empty<string>();
}
