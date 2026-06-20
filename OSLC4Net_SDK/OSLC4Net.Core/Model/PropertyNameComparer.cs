namespace OSLC4Net.Core.Model;

public sealed class PropertyNameComparer : IComparer<Property>
{
    public int Compare(Property? x, Property? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        var nameComparison = string.Compare(x.GetName(), y.GetName(), StringComparison.Ordinal);
        if (nameComparison != 0)
        {
            return nameComparison;
        }

        var propertyDefComparison = Uri.Compare(x.GetPropertyDefinition(), y.GetPropertyDefinition(),
            UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal);

        if (propertyDefComparison != 0)
        {
            return propertyDefComparison;
        }

        // To comply with SortedSet requirements, elements must have a total order.
        // If names and property definitions are identical, we should try to compare About URIs to avoid dropping distinct objects.
        var xAbout = x.GetAbout();
        var yAbout = y.GetAbout();

        if (xAbout != null && yAbout != null)
        {
            var aboutComparison = Uri.Compare(xAbout, yAbout, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.Ordinal);
            if (aboutComparison != 0)
            {
                return aboutComparison;
            }
        }
        else if (xAbout != null)
        {
            return 1;
        }
        else if (yAbout != null)
        {
            return -1;
        }

        // Lastly, fallback to object identity hash code for stable sorting of BNodes
        return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(x).CompareTo(System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(y));
    }
}
