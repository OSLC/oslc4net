/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using OSLC4Net.Core.Model;

namespace OSLC4Net.Domains.RequirementsManagement;

public static class Constants
{
    public static class Domains
    {
        /// <summary>
        ///     OSLC Requirements Management specific constants
        /// </summary>
        public static class RM
        {
            public const string NS = "http://open-services.net/ns/rm#";
            public const string Prefix = "oslc_rm";

            public const string Requirement = NS + "Requirement";
            public const string RequirementCollection = NS + "RequirementCollection";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string AffectedBy = NS + "affectedBy";
                public const string ConstrainedBy = NS + "constrainedBy";
                public const string Constrains = NS + "constrains";
                public const string DecomposedBy = NS + "decomposedBy";
                public const string Decomposes = NS + "decomposes";
                public const string ElaboratedBy = NS + "elaboratedBy";
                public const string Elaborates = NS + "elaborates";
                public const string ImplementedBy = NS + "implementedBy";
                public const string SatisfiedBy = NS + "satisfiedBy";
                public const string Satisfies = NS + "satisfies";
                public const string SpecifiedBy = NS + "specifiedBy";
                public const string Specifies = NS + "specifies";
                public const string TrackedBy = NS + "trackedBy";
                public const string Uses = NS + "uses";
                public const string ValidatedBy = NS + "validatedBy";
            }


            public static class Q
            {
                public static QName AffectedBy => QNameFor("affectedBy");
                public static QName ConstrainedBy => QNameFor("constrainedBy");
                public static QName Constrains => QNameFor("constrains");
                public static QName DecomposedBy => QNameFor("decomposedBy");
                public static QName Decomposes => QNameFor("decomposes");
                public static QName ElaboratedBy => QNameFor("elaboratedBy");
                public static QName Elaborates => QNameFor("elaborates");
                public static QName ImplementedBy => QNameFor("implementedBy");
                public static QName SatisfiedBy => QNameFor("satisfiedBy");
                public static QName Satisfies => QNameFor("satisfies");
                public static QName SpecifiedBy => QNameFor("specifiedBy");
                public static QName Specifies => QNameFor("specifies");
                public static QName TrackedBy => QNameFor("trackedBy");
                public static QName Uses => QNameFor("uses");
                public static QName ValidatedBy => QNameFor("validatedBy");
            }
        }
    }
}
