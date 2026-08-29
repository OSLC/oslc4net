/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

namespace OSLC4Net.Core.Attribute;

/// <summary>
/// Selects one property from an OSLC shape for generation.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class OslcShapeProperty : System.Attribute
{
    public OslcShapeProperty(string uri)
    {
        Uri = uri;
    }

    public string Uri { get; }
}
