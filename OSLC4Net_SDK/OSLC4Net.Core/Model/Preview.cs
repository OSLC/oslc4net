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
///     OSLC Preview resource representation
/// </summary>
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(
    title = "OSLC Preview Resource Shape",
    describes = new[] { OslcConstants.TYPE_PREVIEW }
)]
public class Preview : AbstractResource
{
    private Uri? document;
    private string? hintHeight;
    private string? hintWidth;
    private string? initialHeight;

    public Preview() { }

    public Preview(Uri about)
    {
        SetAbout(about);
    }

    /// <summary>
    /// The Uri of an HTML document to be used for the preview.
    /// </summary>
    [OslcDescription("The Uri of an HTML document to be used for the preview")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "document")]
    [OslcReadOnly]
    [OslcTitle("Document")]
    public Uri? Document
    {
        get => document;
        set => document = value;
    }

    /// <summary>
    /// Recommended height of the preview. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size).
    /// </summary>
    [OslcDescription(
        "Recommended height of the preview. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size)."
    )]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hintHeight")]
    [OslcReadOnly]
    [OslcTitle("Hint Height")]
    public string? HintHeight
    {
        get => hintHeight;
        set => hintHeight = value;
    }

    /// <summary>
    /// Recommended width of the preview. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size).
    /// </summary>
    [OslcDescription(
        "Recommended width of the preview. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size)."
    )]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hintWidth")]
    [OslcReadOnly]
    [OslcTitle("Hint Width")]
    public string? HintWidth
    {
        get => hintWidth;
        set => hintWidth = value;
    }

    /// <summary>
    /// Recommended initial height of the preview. The presence of this property indicates that the preview supports dynamically computing its size. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size).
    /// </summary>
    [OslcDescription(
        "Recommended initial height of the preview. The presence of this property indicates that the preview supports dynamically computing its size. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size)."
    )]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "initialHeight")]
    [OslcReadOnly]
    [OslcTitle("Initial Height")]
    public string? InitialHeight
    {
        get => initialHeight;
        set => initialHeight = value;
    }

    /// <summary>
    /// The Uri of an HTML document to be used for the preview.
    /// </summary>
    [Obsolete("Use Document property instead")]
    public Uri GetDocument()
    {
        return Document!;
    }

    /// <summary>
    /// Recommended height of the preview.
    /// </summary>
    [Obsolete("Use HintHeight property instead")]
    public string GetHintHeight()
    {
        return HintHeight!;
    }

    /// <summary>
    /// Recommended width of the preview.
    /// </summary>
    [Obsolete("Use HintWidth property instead")]
    public string GetHintWidth()
    {
        return HintWidth!;
    }

    /// <summary>
    /// Recommended initial height of the preview.
    /// </summary>
    [Obsolete("Use InitialHeight property instead")]
    public string GetInitialHeight()
    {
        return InitialHeight!;
    }

    /// <summary>
    /// The Uri of an HTML document to be used for the preview.
    /// </summary>
    [Obsolete("Use Document property instead")]
    public void SetDocument(Uri document)
    {
        Document = document;
    }

    /// <summary>
    /// Recommended height of the preview.
    /// </summary>
    [Obsolete("Use HintHeight property instead")]
    public void SetHintHeight(string hintHeight)
    {
        HintHeight = hintHeight;
    }

    /// <summary>
    /// Recommended width of the preview.
    /// </summary>
    [Obsolete("Use HintWidth property instead")]
    public void SetHintWidth(string hintWidth)
    {
        HintWidth = hintWidth;
    }

    /// <summary>
    /// Recommended initial height of the preview.
    /// </summary>
    [Obsolete("Use InitialHeight property instead")]
    public void SetInitialHeight(string initialHeight)
    {
        InitialHeight = initialHeight;
    }
}
