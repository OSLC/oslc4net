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

    public static class QmConstants
    {
        public const string CHANGE_MANAGEMENT_DOMAIN = "http://open-services.net/ns/cm#";
        public const string CHANGE_MANAGEMENT_NAMESPACE = "http://open-services.net/ns/cm#";
        public const string CHANGE_MANAGEMENT_NAMESPACE_PREFIX = "oslc_cm";
        public const string FOAF_NAMESPACE = "http://xmlns.com/foaf/0.1/";
        public const string FOAF_NAMESPACE_PREFIX = "foaf";
        public const string QUALITY_MANAGEMENT_DOMAIN = "http://open-services.net/ns/qm#";
        public const string QUALITY_MANAGEMENT_NAMESPACE = "http://open-services.net/ns/qm#";
        public const string QUALITY_MANAGEMENT_PREFIX = "oslc_qm";
        public const string REQUIREMENTS_MANAGEMENT_NAMESPACE = "http://open-services.net/ns/rm#";
        public const string REQUIREMENTS_MANAGEMENT_PREFIX = "oslc_rm";
        public const string SOFTWARE_CONFIGURATION_MANAGEMENT_NAMESPACE = "http://open-services.net/ns/scm#";
        public const string SOFTWARE_CONFIGURATION_MANAGEMENT_PREFIX = "oslc_scm";

        public const string TYPE_CHANGE_REQUEST = CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest";
        public const string TYPE_CHANGE_SET = SOFTWARE_CONFIGURATION_MANAGEMENT_NAMESPACE + "ChangeSet";
        public const string TYPE_DISCUSSION = OslcConstants.OSLC_CORE_NAMESPACE + "Discussion";
        public const string TYPE_PERSON = FOAF_NAMESPACE + "Person";
        public const string TYPE_REQUIREMENT = REQUIREMENTS_MANAGEMENT_NAMESPACE + "Requirement";
        public const string TYPE_REQUIREMENT_COLLECTION = REQUIREMENTS_MANAGEMENT_NAMESPACE + "RequirementCollection";
        public const string TYPE_TEST_CASE = QUALITY_MANAGEMENT_NAMESPACE + "TestCase";
        public const string TYPE_TEST_EXECUTION_RECORD = QUALITY_MANAGEMENT_NAMESPACE + "TestExecutionRecord";
        public const string TYPE_TEST_PLAN = QUALITY_MANAGEMENT_NAMESPACE + "TestPlan";
        public const string TYPE_TEST_RESULT = QUALITY_MANAGEMENT_NAMESPACE + "TestResult";
        public const string TYPE_TEST_SCRIPT = QUALITY_MANAGEMENT_NAMESPACE + "TestScript";

        public const string PATH_TEST_PLAN = "testPlan";
        public const string PATH_TEST_CASE = "testCase";
        public const string PATH_TEST_SCRIPT = "testScript";
        public const string PATH_TEST_EXECUTION_RECORD = "testExecutionRecord";
        public const string PATH_TEST_RESULT = "testResult";

        public const string USAGE_LIST = QUALITY_MANAGEMENT_NAMESPACE + "list";
    }
}
