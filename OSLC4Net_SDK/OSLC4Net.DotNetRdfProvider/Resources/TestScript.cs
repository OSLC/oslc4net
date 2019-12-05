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

namespace OSLC4Net.Core.Resources
{
    [OslcResourceShape(title = "Quality Management Resource Shape", describes = new string[] { QmConstants.TYPE_TEST_SCRIPT })]
    [OslcNamespace(QmConstants.QUALITY_MANAGEMENT_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/bin/view/Main/QmSpecificationV2#Resource_TestScript
    /// </summary>
    public class TestScript : QmResource
    {
        private readonly ISet<Uri> contributors = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> creators = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Link> relatedChangeRequests = new HashSet<Link>();
        private readonly ISet<Link> validatesRequirements = new HashSet<Link>();

        private Uri executionInstructions;
        private string description;

        public TestScript() : base()
        {
        }

        protected override Uri GetRdfType()
        {
            return new Uri(QmConstants.TYPE_TEST_SCRIPT);
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

        public void AddValidatesRequirement(Link requirement)
        {
            validatesRequirements.Add(requirement);
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

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return description;
        }

        [OslcDescription("Instructions for executing the test script.")]
        [OslcPropertyDefinition(QmConstants.QUALITY_MANAGEMENT_NAMESPACE + "executionInstructions")]
        [OslcTitle("Execution Instructions")]
        public Uri GetExecutionInstructions()
        {
            return executionInstructions;
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

        public void setContributors(Uri[] contributors)
        {
            this.contributors.Clear();

            if (contributors != null)
            {
                this.contributors.AddAll(contributors);
            }
        }

        public void setCreators(Uri[] creators)
        {
            this.creators.Clear();

            if (creators != null)
            {
                this.creators.AddAll(creators);
            }
        }

        public void setDescription(string description)
        {
            this.description = description;
        }

        public void setExecutionInstructions(Uri executionInstructions)
        {
            this.executionInstructions = executionInstructions;
        }

        public void setRelatedChangeRequests(Link[] relatedChangeRequests)
        {
            this.relatedChangeRequests.Clear();

            if (relatedChangeRequests != null)
            {
                this.relatedChangeRequests.AddAll(relatedChangeRequests);
            }
        }

        public void setValidatesRequirements(Link[] validatesRequirements)
        {
            this.validatesRequirements.Clear();

            if (validatesRequirements != null)
            {
                this.validatesRequirements.AddAll(validatesRequirements);
            }
        }
    }
}
