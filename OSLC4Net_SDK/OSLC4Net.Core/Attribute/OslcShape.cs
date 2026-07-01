// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

namespace OSLC4Net.Core.Attribute;

/// <summary>
/// Marks a partial resource type as an OSLC shape generation target.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class OslcShape : System.Attribute
{
    public OslcShape(string uri)
    {
        Uri = uri;
    }

    public string Uri { get; }

    public string? Title { get; set; }
}
