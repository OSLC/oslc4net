// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

namespace OSLC4Net.Core.Model;

/// <summary>
/// Compares RDF URI references, including fragment identifiers.
/// </summary>
public sealed class OslcUriEqualityComparer : IEqualityComparer<Uri>
{
    public static OslcUriEqualityComparer Instance { get; } = new();

    private OslcUriEqualityComparer() { }

    public bool Equals(Uri? x, Uri? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return string.Equals(ToRdfUriString(x), ToRdfUriString(y), StringComparison.Ordinal);
    }

    public int GetHashCode(Uri obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return StringComparer.Ordinal.GetHashCode(ToRdfUriString(obj));
    }

    private static string ToRdfUriString(Uri uri)
    {
        return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
    }
}
