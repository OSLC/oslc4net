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
    public static class AutomationConstants
    {
        public const string AUTOMATION_DOMAIN = "http://open-services.net/ns/auto#";
        public const string AUTOMATION_NAMESPACE = "http://open-services.net/ns/auto#";
        public const string AUTOMATION_PREFIX = "oslc_auto";

        public const string TYPE_AUTOMATION_PLAN = AUTOMATION_NAMESPACE + "AutomationPlan";
        public const string TYPE_AUTOMATION_REQUEST = AUTOMATION_NAMESPACE + "AutomationRequest";
        public const string TYPE_AUTOMATION_RESULT = AUTOMATION_NAMESPACE + "AutomationResult";
        public const string TYPE_PARAMETER_INSTANCE = AUTOMATION_NAMESPACE + "ParameterInstance";

        public const string STATE_NEW = AUTOMATION_NAMESPACE + "new";
        public const string STATE_QUEUED = AUTOMATION_NAMESPACE + "queued";
        public const string STATE_IN_PROGRESS = AUTOMATION_NAMESPACE + "inProgress";
        public const string STATE_CANCELING = AUTOMATION_NAMESPACE + "canceling";
        public const string STATE_CANCELED = AUTOMATION_NAMESPACE + "canceled";
        public const string STATE_COMPLETE = AUTOMATION_NAMESPACE + "complete";

        public const string VERDICT_UNAVAILABLE = AUTOMATION_NAMESPACE + "unavailable";
        public const string VERDICT_PASSED = AUTOMATION_NAMESPACE + "passed";
        public const string VERDICT_WARNING = AUTOMATION_NAMESPACE + "warning";
        public const string VERDICT_FAILED = AUTOMATION_NAMESPACE + "failed";
        public const string VERDICT_ERROR = AUTOMATION_NAMESPACE + "error";
    }
}
