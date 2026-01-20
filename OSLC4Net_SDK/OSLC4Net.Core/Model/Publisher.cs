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
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

/// <summary>
///     OSLC Publisher resource
/// </summary>
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Publisher Resource Shape",
    describes = new[] { OslcConstants.TYPE_PUBLISHER })]
public class Publisher : AbstractResource
{
    private Uri icon;
    private string identifier;
    private string label;
    private string title;

    public Publisher()
    {
    }

    public Publisher(string title, string identifier) : this()
    {
        this.title = title;
        this.identifier = identifier;
    }

    /// <summary>
    /// URL to an icon file that represents the provider. This icon should be a favicon format and 16x16 pixels in size
    /// </summary>
    [OslcDescription(
        "URL to an icon file that represents the provider. This icon should be a favicon format and 16x16 pixels in size")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "icon")]
    [OslcReadOnly]
    [OslcTitle("Icon")]
    public Uri GetIcon()
    {
        return icon;
    }

    /// <summary>
    /// A URN that uniquely identifies the implementation
    /// </summary>
    [OslcDescription("A URN that uniquely identifies the implementation")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "identifier")]
    [OslcReadOnly] // TODO - Marked as unspecified in the spec, but is this correct?
    [OslcTitle("Identifier")]
    public string GetIdentifier()
    {
        return identifier;
    }

    /// <summary>
    /// Very short label for use in menu items
    /// </summary>
    [OslcDescription("Very short label for use in menu items")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "label")]
    [OslcReadOnly]
    [OslcTitle("Label")]
    public string GetLabel()
    {
        return label;
    }

    /// <summary>
    /// Title string that could be used for display
    /// </summary>
    [OslcDescription("Title string that could be used for display")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "title")]
    [OslcReadOnly]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle()
    {
        return title;
    }

    public void SetIcon(Uri icon)
    {
        this.icon = icon;
    }

    public void SetIdentifier(string identifier)
    {
        this.identifier = identifier;
    }

    public void SetLabel(string label)
    {
        this.label = label;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }
}