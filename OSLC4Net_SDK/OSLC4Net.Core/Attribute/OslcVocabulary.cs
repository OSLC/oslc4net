// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

namespace OSLC4Net.Core.Attribute;

/// <summary>
/// Marks a partial static class as an OSLC vocabulary generation target.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class OslcVocabulary : System.Attribute
{
    public OslcVocabulary(string uri)
    {
        Uri = uri;
    }

    public string Uri { get; }
}
