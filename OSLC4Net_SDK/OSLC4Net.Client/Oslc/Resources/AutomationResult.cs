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
    [OslcResourceShape(title = "Automation Result Resource Shape", describes = new string[] {AutomationConstants.TYPE_AUTOMATION_RESULT})]
    [OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_AutomationResult
    /// </summary>
    public class AutomationResult : AbstractResource
    {
	    private readonly ISet<Uri>      contributors                = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      creators                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      rdfTypes                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<string>   subjects                    = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      states                      = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      verdicts                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri>      contributions               = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<ParameterInstance> inputParameters    = new HashSet<ParameterInstance>(); // XXX - TreeSet<> in Java
        private readonly ISet<ParameterInstance> outputParameters   = new HashSet<ParameterInstance>(); // XXX - TreeSet<> in Java
    
        private DateTime?   created;
        private string identifier;
        private Uri         instanceShape;
        private DateTime?   modified;
        private Uri         serviceProvider;
        private string title;
        private Uri         desiredState;
        private Link        producedByAutomationRequest;
        private Link        reportsOnAutomationPlan;

	    public AutomationResult() : base()
	    {
		    rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_RESULT));
	    }
	
        public AutomationResult(Uri about) : base(about)
         {
		    rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_RESULT));
         }

        protected Uri GetRdfType() {
    	    return new Uri(AutomationConstants.TYPE_AUTOMATION_RESULT);
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
    
        public void AddVerdict(Uri verdict)
        {
            verdicts.Add(verdict);
        }
    
        public void AddContribution(Uri contribution)
        {
            contributions.Add(contribution);
        }
    
        public void AddInputParameter(ParameterInstance parameter)
        {
            inputParameters.Add(parameter);
        }
    
        public void AddOutputParameter(ParameterInstance parameter)
        {
            outputParameters.Add(parameter);
        }
    
        [OslcDescription("The person(s) who are responsible for the work needed to complete the automation result.")]
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
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return serviceProvider;
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
    
        [OslcDescription("Used to indicate the state of the automation result based on values defined by the service provider.")]
        [OslcOccurs(Occurs.OneOrMany)]
        [OslcReadOnly(true)]
        [OslcName("state")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "state")]
        [OslcTitle("State")]
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
    
        [OslcDescription("A result contribution associated with this automation result.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("contribution")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "contribution")]
        [OslcTitle("Contribution")]
        public Uri[] GetContributions()
        {
            return contributions.ToArray();
        }
    
        [OslcDescription("Used to indicate the verdict of the automation result based on values defined by the service provider.")]
        [OslcOccurs(Occurs.OneOrMany)]
        [OslcName("verdict")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "verdict")]
        [OslcTitle("Verdict")]
        [OslcAllowedValue(new string[] {
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.VERDICT_PASSED,
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.VERDICT_FAILED,
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.VERDICT_WARNING,
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.VERDICT_ERROR,
    	    AutomationConstants.AUTOMATION_NAMESPACE + AutomationConstants.VERDICT_UNAVAILABLE})]
        public Uri[] GetVerdicts()
        {
            return verdicts.ToArray();
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
    
        [OslcDescription("Automation Request which produced the Automation Result.")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "producedByAutomationRequest")]
        [OslcName("producedByAutomationRequest")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        [OslcTitle("Produced By Automation Request")]
        public Link GetProducedByAutomationRequest()
        {
            return producedByAutomationRequest;
        }
    
        [OslcDescription("Automation Plan which the Automation Result reports on.")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "reportsOnAutomationPlan")]
        [OslcName("reportsOnAutomationPlan")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcTitle("Reports On Automation Plan")]
        public Link GetReportsOnAutomationPlan()
        {
            return reportsOnAutomationPlan;
        }
    
        [OslcDescription("A copy of the parameters provided during creation of the Automation Request which produced this Automation Result.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("inputParameter")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "inputParameter")]
        [OslcReadOnly(true)]
        [OslcTitle("Input Parameter")]
        public ParameterInstance[] GetInputParameters()
        {
            return inputParameters.ToArray();
        }

        [OslcDescription("Automation Result output parameters are parameters associated with the automation execution which produced this Result. This includes the value of all parameters used to initiate the execution and any additional parameters which may have been created during automation execution by the service provider or external agents.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("outputParameter")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "outputParameter")]
        [OslcTitle("Output Parameter")]
        public ParameterInstance[] GetOutputParameters()
        {
            return outputParameters.ToArray();
        }
    
        public void setContributors(Uri[] contributors)
        {
            this.contributors.Clear();

            if (contributors != null)
            {
                this.contributors.AddAll(contributors);
            }
        }

        public void setCreated(DateTime? created)
        {
            this.created = created;
        }

        public void setCreators(Uri[] creators)
        {
            this.creators.Clear();

            if (creators != null)
            {
                this.creators.AddAll(creators);
            }
        }

        public void setIdentifier(string identifier)
        {
            this.identifier = identifier;
        }
    
        public void setInstanceShape(Uri instanceShape)
        {
            this.instanceShape = instanceShape;
        }

        public void setModified(DateTime? modified)
        {
            this.modified = modified;
        }

        public void setRdfTypes(Uri[] rdfTypes)
        {
            this.rdfTypes.Clear();

            if (rdfTypes != null)
            {
                this.rdfTypes.AddAll(rdfTypes);
            }
        }

        public void setServiceProvider(Uri serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void setSubjects(string[] subjects)
        {
            this.subjects.Clear();

            if (subjects != null)
            {
                this.subjects.AddAll(subjects);
            }
        }

        public void setTitle(string title)
        {
            this.title = title;
        }

        public void setStates(Uri[] states)
        {
            this.states.Clear();

            if (states != null)
            {
                this.states.AddAll(states);
            }
        }
    
        public void setVerdicts(Uri[] verdicts)
        {
            this.verdicts.Clear();

            if (verdicts != null)
            {
                this.verdicts.AddAll(verdicts);
            }
        }
    
        public void setContributions(Uri[] contributions)
        {
            this.contributions.Clear();

            if (contributions != null)
            {
                this.contributions.AddAll(contributions);
            }
        }
    
        public void setDesiredState(Uri desiredState)
        {
            this.desiredState = desiredState;
        }

        public void setProducedByAutomationRequest(Link producedByAutomationRequest)
        {
            this.producedByAutomationRequest = producedByAutomationRequest;
        }
    
        public void setReportsOnAutomationPlan(Link reportsOnAutomationPlan)
        {
            this.reportsOnAutomationPlan = reportsOnAutomationPlan;
        }
    
        public void setInputParameters(ParameterInstance[] parameters)
        {
            inputParameters.Clear();

            if (parameters != null)
            {
                inputParameters.AddAll(parameters);
            }
        }
    
        public void setOutputParameters(ParameterInstance[] parameters)
        {
            outputParameters.Clear();

            if (parameters != null)
            {
                outputParameters.AddAll(parameters);
            }
        }
    }
}
