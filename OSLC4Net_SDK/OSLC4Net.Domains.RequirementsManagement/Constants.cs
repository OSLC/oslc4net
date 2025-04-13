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

namespace OSLC4Net.Domains.RequirementsManagement;

public static class Constants
{
    public static class Domains
    {
        public static class FOAF
        {
            public const string NS = "http://xmlns.com/foaf/0.1/";
            public const string Prefix = "foaf";

            public const string Person = NS + "Person";
        }

        /// <summary>
        ///     OSLC Requirements Management specific constants
        /// </summary>
        public static class RM
        {
            public const string NS = "http://open-services.net/ns/rm#";
            public const string Prefix = "oslc_rm";

            public const string Requirement = NS + "Requirement";
        }
    }
}
