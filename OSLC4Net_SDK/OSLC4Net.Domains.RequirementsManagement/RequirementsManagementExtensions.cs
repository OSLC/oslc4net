/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

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
