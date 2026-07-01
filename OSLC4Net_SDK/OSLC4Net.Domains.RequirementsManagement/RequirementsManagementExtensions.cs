// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Domains.RequirementsManagement;

public static partial class RM
{
    public static partial class P
    {
        public const string ManagedBy = NS + "managedBy";
    }

    public static partial class Q
    {
        public static QName ManagedBy => QNameFor("managedBy");
    }
}

public partial record RequirementCollection
{
    [OslcDescription("Resource, such as a change request, which manages this requirement collection.")]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition(RM.P.ManagedBy)]
    [OslcName("managedBy")]
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)]
    [OslcTitle("managedBy")]
    public HashSet<Uri> ManagedBy { get; set; } = new(OslcUriEqualityComparer.Instance);
}
