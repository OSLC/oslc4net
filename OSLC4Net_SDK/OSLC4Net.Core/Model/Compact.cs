/*
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
 *     Steve Pitschke  - initial API and implementation
 */

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

/// <summary>
///     OSLC Compact resource representation.
/// </summary>
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Compact Resource Shape",
    describes = new[] { OslcConstants.TYPE_COMPACT })]
public class Compact : AbstractResource
{
    public Compact()
    {
    }

    public Compact(Uri about)
    {
        SetAbout(about);
    }

    private Uri? icon;
    private Preview? largePreview;
    private string? shortTitle;
    private Preview? smallPreview;
    private string? title;
    private string? iconTitle;
    private string? iconAltLabel;
    private string? iconSrcSet;

    /// <summary>
    /// Uri of an image which may be used in the display of a link to the resource. The image SHOULD be 16x16 pixels in size.
    /// </summary>
    [OslcDescription(
        "Uri of an image which may be used in the display of a link to the resource. The image SHOULD be 16x16 pixels in size.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "icon")]
    [OslcReadOnly]
    [OslcTitle("Icon")]
    public Uri? Icon
    {
        get => icon;
        set => icon = value;
    }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a large preview.
    /// </summary>
    [OslcDescription(
        "Uri and sizing properties for an HTML document to be used for a large preview.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "largePreview")]
    [OslcRange(OslcConstants.TYPE_PREVIEW)]
    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
    [OslcTitle("Large Preview")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PREVIEW)]
    [OslcValueType(ValueType.LocalResource)]
    public Preview? LargePreview
    {
        get => largePreview;
        set => largePreview = value;
    }

    /// <summary>
    /// Abbreviated title which may be used in the display of a link to the resource.
    /// </summary>
    [OslcDescription(
        "Abbreviated title which may be used in the display of a link to the resource.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "shortTitle")]
    [OslcReadOnly]
    [OslcTitle("Short Title")]
    public string? ShortTitle
    {
        get => shortTitle;
        set => shortTitle = value;
    }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a small preview.
    /// </summary>
    [OslcDescription(
        "Uri and sizing properties for an HTML document to be used for a small preview.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "smallPreview")]
    [OslcRange(OslcConstants.TYPE_PREVIEW)]
    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
    [OslcTitle("Small Preview")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PREVIEW)]
    [OslcValueType(ValueType.LocalResource)]
    public Preview? SmallPreview
    {
        get => smallPreview;
        set => smallPreview = value;
    }

    /// <summary>
    /// Title which may be used in the display of a link to the resource.
    /// </summary>
    [OslcDescription("Title which may be used in the display of a link to the resource.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "title")]
    [OslcReadOnly]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string? Title
    {
        get => title;
        set => title = value;
    }

    /// <summary>
    /// Alternative text for the icon image. It should be used if the icon fails to load, or for screen readers.
    /// </summary>
    [OslcDescription("Alternative text for the icon image. It should be used if the icon fails to load, or for screen readers.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "iconTitle")]
    [OslcReadOnly]
    [OslcTitle("Icon Title")]
    public string? IconTitle
    {
        get => iconTitle;
        set => iconTitle = value;
    }

    /// <summary>
    /// Alternative label text for the icon image, e.g. for accessibility. This property is deprecated. Programmatic users should use oslc:iconTitle instead.
    /// </summary>
    [OslcDescription("Alternative label text for the icon image, e.g. for accessibility. This property is deprecated. Programmatic users should use oslc:iconTitle instead.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "iconAltLabel")]
    [OslcReadOnly]
    [OslcTitle("Icon Alternative Label")]
    public string? IconAltLabel
    {
        get => iconAltLabel;
        set => iconAltLabel = value;
    }

    /// <summary>
    /// A list of alternate icon images, e.g. for high-resolution displays. The syntax of this property's value is the same as the srcset attribute of the HTML img element.
    /// </summary>
    [OslcDescription("A list of alternate icon images, e.g. for high-resolution displays. The syntax of this property's value is the same as the srcset attribute of the HTML img element.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "iconSrcSet")]
    [OslcReadOnly]
    [OslcTitle("Icon Source Set")]
    public string? IconSrcSet
    {
        get => iconSrcSet;
        set => iconSrcSet = value;
    }

    /// <summary>
    /// Uri of an image which may be used in the display of a link to the resource.
    /// </summary>
    [Obsolete("Use Icon property instead")]
    public Uri GetIcon()
    {
        return Icon!;
    }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a large preview.
    /// </summary>
    [Obsolete("Use LargePreview property instead")]
    public Preview GetLargePreview()
    {
        return LargePreview!;
    }

    /// <summary>
    /// Abbreviated title which may be used in the display of a link to the resource.
    /// </summary>
    [Obsolete("Use ShortTitle property instead")]
    public string GetShortTitle()
    {
        return ShortTitle!;
    }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a small preview.
    /// </summary>
    [Obsolete("Use SmallPreview property instead")]
    public Preview GetSmallPreview()
    {
        return SmallPreview!;
    }

    /// <summary>
    /// Title which may be used in the display of a link to the resource.
    /// </summary>
    [Obsolete("Use Title property instead")]
    public string GetTitle()
    {
        return Title!;
    }

    /// <summary>
    /// Uri of an image which may be used in the display of a link to the resource.
    /// </summary>
    [Obsolete("Use Icon property instead")]
    public void SetIcon(Uri icon)
    {
        Icon = icon;
    }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a large preview.
    /// </summary>
    [Obsolete("Use LargePreview property instead")]
    public void SetLargePreview(Preview largePreview)
    {
        LargePreview = largePreview;
    }

    /// <summary>
    /// Abbreviated title which may be used in the display of a link to the resource.
    /// </summary>
    [Obsolete("Use ShortTitle property instead")]
    public void SetShortTitle(string shortTitle)
    {
        ShortTitle = shortTitle;
    }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a small preview.
    /// </summary>
    [Obsolete("Use SmallPreview property instead")]
    public void SetSmallPreview(Preview smallPreview)
    {
        SmallPreview = smallPreview;
    }

    /// <summary>
    /// Title which may be used in the display of a link to the resource.
    /// </summary>
    [Obsolete("Use Title property instead")]
    public void SetTitle(string title)
    {
        Title = title;
    }
}
