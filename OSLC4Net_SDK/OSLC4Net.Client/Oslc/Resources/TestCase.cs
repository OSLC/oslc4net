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
    [OslcResourceShape(title = "Quality Management Resource Shape", describes = new string[] {QmConstants.TYPE_TEST_CASE})]
    [OslcNamespace(QmConstants.QUALITY_MANAGEMENT_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/bin/view/Main/QmSpecificationV2#Resource_TestCase
    /// </summary>
    public class TestCase : QmResource
    {
        private readonly ISet<Uri>      contributors                = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      creators                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Link>     relatedChangeRequests       = new HashSet<Link>();
        private readonly ISet<string>   subjects                    = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Link>     testsChangeRequests         = new HashSet<Link>();
        private readonly ISet<Link>     usesTestScripts             = new HashSet<Link>();
        private readonly ISet<Link>     validatesRequirements       = new HashSet<Link>();

        private string description;

        public TestCase() : base()
        {
        }

        protected override Uri GetRdfType()
        {
    	    return new Uri(QmConstants.TYPE_TEST_CASE);
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

        public void AddSubject(string subject)
        {
            subjects.Add(subject);
        }

        public void AddTestsChangeRequest(Link changeRequest)
        {
            testsChangeRequests.Add(changeRequest);
        }
    
        public void AddUsesTestScript(Link testscript)
        {
            usesTestScripts.Add(testscript);
        }

        public void AddValidatesRequirement(Link requirement)
        {
            validatesRequirements.Add(requirement);
        }
    
        [OslcDescription("The person(s) who are responsible for the work needed to complete the test case.")]
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

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return description;
        }

        [OslcDescription("A related change request.")]
        [OslcName("relatedChangeRequest")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "relatedChangeRequest")]
        [OslcRange(QmConstants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Related Change Requests")]
        public Link[] GetRelatedChangeRequests()
        {
            return relatedChangeRequests.ToArray();
        }

        [OslcDescription("Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")]
        [OslcName("subject")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "subject")]
        [OslcReadOnly(false)]
        [OslcTitle("Subjects")]
        public string[] GetSubjects()
        {
            return subjects.ToArray();
        }

        [OslcDescription("Change Request tested by the Test Case.")]
        [OslcName("testsChangeRequest")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "testsChangeRequest")]
        [OslcRange(QmConstants.TYPE_CHANGE_REQUEST)]
        [OslcReadOnly(false)]
        [OslcTitle("Tests Change Request")]
        public Link[] GetTestsChangeRequests()
        {
            return testsChangeRequests.ToArray();
        }
    
        [OslcDescription("Test Script used by the Test Case.")]
        [OslcName("usesTestScript")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "usesTestScript")]
        [OslcRange(QmConstants.TYPE_TEST_SCRIPT)]
        [OslcReadOnly(false)]
        [OslcTitle("Uses Test Script")]
        public Link[] GetUsesTestScripts()
        {
            return usesTestScripts.ToArray();
        }
 
        [OslcDescription("Requirement that is validated by the Test Case.")]
        [OslcName("validatesRequirement")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "validatesRequirement")]
        [OslcRange(QmConstants.TYPE_REQUIREMENT)]
        [OslcReadOnly(false)]
        [OslcTitle("Validates Requirement")]
        public Link[] GetValidatesRequirements()
        {
            return validatesRequirements.ToArray();
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

        public void SetDescription(string description)
        {
            this.description = description;
        }

        public void SetRelatedChangeRequests(Link[] relatedChangeRequests)
        {
            this.relatedChangeRequests.Clear();

            if (relatedChangeRequests != null)
            {
                this.relatedChangeRequests.AddAll(relatedChangeRequests);
           }
        }

        public void SetSubjects(string[] subjects)
        {
            this.subjects.Clear();

            if (subjects != null)
            {
                this.subjects.AddAll(subjects);
            }
        }

        public void SetTestsChangeRequests(Link[] testsChangeRequests)
        {
            this.testsChangeRequests.Clear();

            if (testsChangeRequests != null)
            {
                this.testsChangeRequests.AddAll(testsChangeRequests);
            }
        }
    
        public void SetUsesTestScripts(Link[] usesTestScripts)
        {
            this.usesTestScripts.Clear();

            if (usesTestScripts != null)
            {
                this.usesTestScripts.AddAll(usesTestScripts);
            }
        }

        public void SetValidatesRequirements(Link[] validatesRequirements)
        {
            this.validatesRequirements.Clear();

            if (validatesRequirements != null)
            {
                this.validatesRequirements.AddAll(validatesRequirements);
            }
        }
    }
}
