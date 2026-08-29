/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */
using System.Text.Json.Serialization;

namespace OSLC4Net.Core.Model;

/// <summary>
/// DTO representing the OSLC Core 3.0 Compact JSON representation.
/// </summary>
public class CompactDto
{
    /// <summary>
    /// Title which may be used in the display of a link to the resource.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Abbreviated title which may be used in the display of a link to the resource.
    /// </summary>
    [JsonPropertyName("shortTitle")]
    public string? ShortTitle { get; set; }

    /// <summary>
    /// Uri of an image which may be used in the display of a link to the resource.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Alternative text for the icon image. It should be used if the icon fails to load, or for screen readers.
    /// </summary>
    [JsonPropertyName("iconTitle")]
    public string? IconTitle { get; set; }

    /// <summary>
    /// Alternative label text for the icon image, e.g. for accessibility. This property is deprecated.
    /// </summary>
    [JsonPropertyName("iconAltLabel")]
    public string? IconAltLabel { get; set; }

    /// <summary>
    /// A list of alternate icon images, e.g. for high-resolution displays.
    /// </summary>
    [JsonPropertyName("iconSrcSet")]
    public string? IconSrcSet { get; set; }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a small preview.
    /// </summary>
    [JsonPropertyName("smallPreview")]
    public PreviewDto? SmallPreview { get; set; }

    /// <summary>
    /// Uri and sizing properties for an HTML document to be used for a large preview.
    /// </summary>
    [JsonPropertyName("largePreview")]
    public PreviewDto? LargePreview { get; set; }
}
