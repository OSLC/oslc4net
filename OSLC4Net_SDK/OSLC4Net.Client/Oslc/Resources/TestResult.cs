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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Oslc.Resources
{
    [OslcResourceShape(title = "Quality Management Resource Shape", describes = new string[] {QmConstants.TYPE_TEST_RESULT})]
    [OslcNamespace(QmConstants.QUALITY_MANAGEMENT_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/bin/view/Main/QmSpecificationV2#Resource_TestResult
    /// </summary>
    public class TestResult : QmResource
    {
        private readonly ISet<Link>     affectedByChangeRequests       = new HashSet<Link>();

        private Link     executesTestScript;
        private Link     reportsOnTestCase;
        private Link     reportsOnTestPlan;
        private Link     producedByTestExecutionRecord;    
        private string status;

        public TestResult() : base()
        {
        }

        protected override Uri GetRdfType()
        {
    	    return new Uri(QmConstants.TYPE_TEST_RESULT);
        }
    
        public void AddAffectedByChangeRequest(Link affectingChangeRequest)
        {
            affectedByChangeRequests.Add(affectingChangeRequest);
        }

        [OslcDescription("Change request that affects the Test Result.")]
        [OslcName("affectedByChangeRequest")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "affectedByChangeRequest")]
        [OslcRange(QmConstants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Affected By Change Request")]
        public Link[] GetAffectedByChangeRequests()
        {
            return affectedByChangeRequests.ToArray();
        }
    
        [OslcDescription("Test Plan that the Test Result reports on.")]
        [OslcName("reportsOnTestPlan")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "reportsOnTestPlan")]
        [OslcRange(QmConstants.TYPE_TEST_PLAN)]
        [OslcReadOnly(false)]
        [OslcTitle("Reports On Test Plan")]
        public Link GetReportsOnTestPlan()
        {
            return reportsOnTestPlan;
        }
    
        [OslcDescription("Test Case that the Test Result reports on.")]
        [OslcName("reportsOnTestCase")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "reportsOnTestCase")]
        [OslcRange(QmConstants.TYPE_TEST_CASE)]
        [OslcReadOnly(false)]
        [OslcTitle("Reports On Test Case")]
        public Link GetReportsOnTestCase()
        {
            return reportsOnTestCase;
        }
    
        [OslcDescription("Test Script executed to produce the Test Result.")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "executesTestScript")]
        [OslcTitle("Executes Test Script")]
        public Link GetExecutesTestScript()
        {
            return executesTestScript;
        }
    
        [OslcDescription("Test Execution Record that the Test Result was produced by.")]
        [OslcName("producedByTestExecutionRecord")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "producedByTestExecutionRecord")]
        [OslcRange(QmConstants.TYPE_TEST_EXECUTION_RECORD)]
        [OslcReadOnly(false)]
        [OslcTitle("Produced By Test Execution Record")]
        public Link GetProducedByTestExecutionRecord()
        {
            return producedByTestExecutionRecord;
        }
    
        [OslcDescription("Used to indicate the state of the Test Result based on values defined by the service provider.")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "status")]
        [OslcTitle("Status")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetStatus()
        {
            return status;
        }
    
        public void SetAffectedByChangeRequests(Link[] affectedByChangeRequests)
        {
            this.affectedByChangeRequests.Clear();

            if (affectedByChangeRequests != null)
            {
                this.affectedByChangeRequests.AddAll(affectedByChangeRequests);
            }
        }
    
        public void SetReportsOnTestPlan(Link reportsOnTestPlan)
        {
            this.reportsOnTestPlan = reportsOnTestPlan;
        }
    
        public void SetReportsOnTestCase(Link reportsOnTestCase)
        {
            this.reportsOnTestCase = reportsOnTestCase;
        }
    
        public void SetProducedByTestExecutionRecord(Link producedByTestExecutionRecord)
        {
            this.producedByTestExecutionRecord = producedByTestExecutionRecord;
        }
    
        public void SetExecutesTestScript(Link executesTestScript)
        {
            this.executesTestScript = executesTestScript;
        }
    
        public void SetStatus(string status)
        {
            this.status = status;
        }
    }
}
