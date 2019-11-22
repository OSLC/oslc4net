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
    [OslcResourceShape(title = "Quality Management Resource Shape", describes = new string[] {QmConstants.TYPE_TEST_EXECUTION_RECORD})]
    [OslcNamespace(QmConstants.QUALITY_MANAGEMENT_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/bin/view/Main/QmSpecificationV2#Resource_TestExecutionRecord
    /// </summary>
    public class TestExecutionRecord : QmResource
    {
        private readonly ISet<Link>     blockedByChangeRequests     = new HashSet<Link>();
        private readonly ISet<Uri>      contributors                = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      creators                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Link>     relatedChangeRequests       = new HashSet<Link>();

        private Link     reportsOnTestPlan;
        private Uri      runsOnTestEnvironment;
        private Link     runsTestCase;

        public TestExecutionRecord() : base()
        {
        }
    
        protected override Uri GetRdfType()
        {
    	    return new Uri(QmConstants.TYPE_TEST_EXECUTION_RECORD);
        }
    
        public void AddBlockedByChangeRequest(Link blockingChangeRequest)
        {
            blockedByChangeRequests.Add(blockingChangeRequest);
        }

        public void AddContributor(Uri contributor)
        {
            contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            creators.Add(creator);
        }

        public void AddRelatedChangeRequest(Link relatedChangeRequest)
        {
            relatedChangeRequests.Add(relatedChangeRequest);
        }

        [OslcDescription("The person(s) who are responsible for the work needed to complete the change request.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return contributors.ToArray();
        }

        [OslcDescription("Creator or creators of resource.")]
        [OslcName("creator")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return creators.ToArray();
        }

        [OslcDescription("Change Request that prevents execution of the Test Execution Record.")]
        [OslcName("blockedByChangeRequest")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "blockedByChangeRequest")]
        [OslcRange(QmConstants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Blocked By Change Request")]
        public Link[] GetBlockedByChangeRequests()
        {
            return blockedByChangeRequests.ToArray();
        }
    
        [OslcDescription("This relationship is loosely coupled and has no specific meaning.")]
        [OslcName("relatedChangeRequest")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "relatedChangeRequest")]
        [OslcRange(QmConstants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Change Requests")]
        public Link[] GetRelatedChangeRequests()
        {
            return relatedChangeRequests.ToArray();
        }

        [OslcDescription("Test Plan that the Test Execution Record reports on.")]
        [OslcName("reportsOnTestPlan")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "reportsOnTestPlan")]
        [OslcRange(QmConstants.TYPE_TEST_PLAN)]
        [OslcReadOnly(false)]
        [OslcTitle("Reports On Test Plan")]
        public Link GetReportsOnTestPlan()
        {
            return reportsOnTestPlan;
        }
    
        [OslcDescription("Indicates the environment details of the test case for this execution record.")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "runsOnTestEnvironment")]
        [OslcTitle("Runs On Test Environment")]
        public Uri GetRunsOnTestEnvironment()
        {
            return runsOnTestEnvironment;
        }
    
        [OslcDescription("Test Case run by the Test Execution Record.")]
        [OslcName("runsTestCase")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "runsTestCase")]
        [OslcRange(QmConstants.TYPE_TEST_CASE)]
        [OslcReadOnly(false)]
        [OslcTitle("Runs Test Case")]
        public Link GetRunsTestCase()
        {
            return runsTestCase;
        }
    
        public void SetBlockedByChangeRequests(Link[] blockedByChangeRequests)
        {
            this.blockedByChangeRequests.Clear();

            if (blockedByChangeRequests != null)
            {
                this.blockedByChangeRequests.AddAll(blockedByChangeRequests);
            }
        }
    
        public void SetContributors(Uri[] contributors)
        {
            this.contributors.Clear();

            if (contributors != null)
            {
                this.contributors.AddAll(contributors);
            }
        }

        public void SetCreators(Uri[] creators)
        {
            this.creators.Clear();

            if (creators != null)
            {
                this.creators.AddAll(creators);
            }
        }

        public void SetRelatedChangeRequests(Link[] relatedChangeRequests)
        {
            this.relatedChangeRequests.Clear();

            if (relatedChangeRequests != null)
            {
                this.relatedChangeRequests.AddAll(relatedChangeRequests);
            }
        }

        public void SetReportsOnTestPlan(Link reportsOnTestPlan)
        {
            this.reportsOnTestPlan = reportsOnTestPlan;
        }
    
        public void SetRunsOnTestEnvironment(Uri runsOnTestEnvironment)
        {
            this.runsOnTestEnvironment = runsOnTestEnvironment;
        }

        public void SetRunsTestCase(Link runsTestCase)
        {
            this.runsTestCase = runsTestCase;
        }
    }
}
