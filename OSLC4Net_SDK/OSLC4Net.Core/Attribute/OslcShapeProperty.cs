// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

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
