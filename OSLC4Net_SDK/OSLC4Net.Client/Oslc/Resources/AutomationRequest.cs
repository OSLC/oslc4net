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
using OSLC4Net.Core.Model;
using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Client.Oslc.Resources
{
    [OslcResourceShape(title = "Automation Request Resource Shape", describes = new string[] {AutomationConstants.TYPE_AUTOMATION_REQUEST})]
    [OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_AutomationRequest
    /// </summary>
    public class AutomationRequest : AbstractResource
    {
	    private readonly ISet<Uri>      contributors                = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      creators                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      rdfTypes                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<string>   subjects                    = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      states                      = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<ParameterInstance> inputParameters    = new HashSet<ParameterInstance>(); // XXX - TreeSet<> in Java
    
        private DateTime?   created;
        private string description;
        private string identifier;
        private Uri         instanceShape;
        private DateTime?   modified;
        private Uri         serviceProvider;
        private string title;
        private Uri         desiredState;
        private Link        executesAutomationPlan;

	    public AutomationRequest() : base()
	    {
		    rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_REQUEST));
	    }
	
        public AutomationRequest(Uri about) : base(about)
         {
		    rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_REQUEST));
         }

        protected Uri GetRdfType()
        {
    	    return new Uri(AutomationConstants.TYPE_AUTOMATION_REQUEST);
        }
    
        public void AddContributor(Uri contributor)
        {
            contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            creators.Add(creator);
        }
    
        public void AddRdfType(Uri rdfType)
        {
            rdfTypes.Add(rdfType);
        }

        public void AddSubject(string subject)
        {
            subjects.Add(subject);
        }

        public void AddState(Uri state)
        {
            states.Add(state);
        }
    
        public void AddInputParameter(ParameterInstance parameter)
        {
            inputParameters.Add(parameter);
        }
    
        [OslcDescription("The person(s) who are responsible for the work needed to complete the automation request.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return contributors.ToArray();
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return created;
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

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return identifier;
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return instanceShape;
        }

        [OslcDescription("Timestamp last latest resource modification.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
        [OslcReadOnly]
        [OslcTitle("Modified")]
        public DateTime? GetModified()
        {
            return modified;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return rdfTypes.ToArray();
        }

        [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return serviceProvider;
        }

        [OslcDescription("Used to indicate the desired state of the Automation Request based on values defined by the service provider.")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "desiredState")]
        [OslcName("desiredState")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        [OslcTitle("Desired State")]
        [OslcAllowedValue(new string[] {
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_NEW,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_IN_PROGRESS,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_QUEUED,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_CANCELING,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_CANCELED,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_COMPLETE})]
        public Uri GetDesiredState()
        {
            return desiredState;
        }
    
        [OslcDescription("Automation Plan run by the Automation Request.")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "executesAutomationPlan")]
        [OslcName("executesAutomationPlan")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcTitle("Executes Automation Plan")]
        public Link GetExecutesAutomationPlan()
        {
            return executesAutomationPlan;
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

        [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcTitle("Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return title;
        }
    
        [OslcDescription("Used to indicate the state of the automation request based on values defined by the service provider.")]
        [OslcOccurs(Occurs.OneOrMany)]
        [OslcReadOnly(true)]
        [OslcName("state")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "state")]
        [OslcTitle("States")]
        [OslcAllowedValue(new string[] {
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_NEW,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_IN_PROGRESS,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_QUEUED,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_CANCELING,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_CANCELED,
		    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.STATE_COMPLETE})]
        public Uri[] GetStates()
        {
            return states.ToArray();
        }

    
        [OslcDescription("Parameters provided when Automation Requests are created.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("inputParameter")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "inputParameter")]
        [OslcReadOnly(false)]
        [OslcTitle("Input Parameter")]
        public ParameterInstance[] GetInputParameters()
        {
            return inputParameters.ToArray();
        }

        public void SetContributors(Uri[] contributors)
        {
            this.contributors.Clear();

            if (contributors != null)
            {
                this.contributors.AddAll(contributors);
            }
        }

        public void SetCreated(DateTime? created)
        {
            this.created = created;
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

        public void SetIdentifier(string identifier)
        {
            this.identifier = identifier;
        }
    
        public void SetInstanceShape(Uri instanceShape)
        {
            this.instanceShape = instanceShape;
        }

        public void SetModified(DateTime? modified)
        {
            this.modified = modified;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            this.rdfTypes.Clear();

            if (rdfTypes != null)
            {
                this.rdfTypes.AddAll(rdfTypes);
            }
        }

        public void SetServiceProvider(Uri serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void SetDesiredState(Uri desiredState)
        {
            this.desiredState = desiredState;
        }

        public void SetExecutesAutomationPlan(Link executesAutomationPlan)
        {
            this.executesAutomationPlan = executesAutomationPlan;
        }
    
        public void SetSubjects(string[] subjects)
        {
            this.subjects.Clear();

            if (subjects != null)
            {
                this.subjects.AddAll(subjects);
            }
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void SetStates(Uri[] states)
        {
            this.states.Clear();

            if (states != null)
            {
                this.states.AddAll(states);
            }
        }
    
        public void SetInputParameters(ParameterInstance[] parameters)
        {
            inputParameters.Clear();

            if (parameters != null)
            {
                inputParameters.AddAll(parameters);
            }
        }
    }
}
