/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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

namespace OSLC4Net.Core.Resources
{
    using OSLC4Net.Core.Model;

    public static class RmConstants
    {
        public const string REQUIREMENTS_MANAGEMENT_DOMAIN = "http://open-services.net/ns/rm#";
        public const string REQUIREMENTS_MANAGEMENT_NAMESPACE = "http://open-services.net/ns/rm#";

        public const string REQUIREMENTS_MANAGEMENT_PREFIX = "oslc_rm";
        public const string SOFTWARE_CONFIGURATION_MANAGEMENT_PREFIX = "oslc_scm";
        public const string QUALITY_MANAGEMENT_PREFIX = "oslc_qm";

        public const string FOAF_NAMESPACE = "http://xmlns.com/foaf/0.1/";
        public const string FOAF_NAMESPACE_PREFIX = "foaf";

        public const string TYPE_DISCUSSION = OslcConstants.OSLC_CORE_NAMESPACE + "Discussion";
        public const string TYPE_PERSON = FOAF_NAMESPACE + "Person";
        public const string TYPE_REQUIREMENT = REQUIREMENTS_MANAGEMENT_NAMESPACE + "Requirement";
        public const string TYPE_REQUIREMENT_COLLECTION = REQUIREMENTS_MANAGEMENT_NAMESPACE + "RequirementCollection";

        public const string JAZZ_RM_NAMESPACE = "http://jazz.net/ns/rm#";
        public const string JAZZ_RM_NAV_NAMESPACE = "http://jazz.net/ns/rm/navigation#";
        public const string JAZZ_RM_ACCESS_NAMESPACE = "http://jazz.net/ns/acp#";

        public static readonly QName PROPERTY_PRIMARY_TEXT = new QName(JAZZ_RM_NAMESPACE, "primaryText");
        public static readonly QName PROPERTY_PARENT_FOLDER = new QName(JAZZ_RM_NAV_NAMESPACE, "parent");
        public static readonly QName PROPERTY_ACCESS_CONTROL = new QName(JAZZ_RM_ACCESS_NAMESPACE, "accessControl");

        public const string NAMESPACE_URI_XHTML = "http://www.w3.org/1999/xhtml"; //$NON-NLS-1$
    }
}
