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
/// DTO representing the OSLC Core 3.0 Preview JSON representation.
/// </summary>
public class PreviewDto
{
    /// <summary>
    /// The Uri of an HTML document to be used for the preview.
    /// </summary>
    [JsonPropertyName("document")]
    public string? Document { get; set; }

    /// <summary>
    /// Recommended width of the preview.
    /// </summary>
    [JsonPropertyName("hintWidth")]
    public string? HintWidth { get; set; }

    /// <summary>
    /// Recommended height of the preview.
    /// </summary>
    [JsonPropertyName("hintHeight")]
    public string? HintHeight { get; set; }

    /// <summary>
    /// Recommended initial height of the preview.
    /// </summary>
    [JsonPropertyName("initialHeight")]
    public string? InitialHeight { get; set; }
}
