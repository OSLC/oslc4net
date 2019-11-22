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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OSLC4Net.Core.Attribute;
    using OSLC4Net.Core.Model;

    [OslcResourceShape(title = "Automation Result Resource Shape", describes = new string[] { AutomationConstants.TYPE_AUTOMATION_RESULT })]
    [OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_AutomationResult
    /// </summary>
    public class AutomationResult : AbstractResource
    {
        private readonly ISet<Uri> _contributors = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _creators = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<string> _subjects = new HashSet<string>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _states = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _verdicts = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<Uri> _contributions = new HashSet<Uri>(); // XXX - TreeSet<> in Java
        private readonly ISet<ParameterInstance> _inputParameters = new HashSet<ParameterInstance>(); // XXX - TreeSet<> in Java
        private readonly ISet<ParameterInstance> _outputParameters = new HashSet<ParameterInstance>(); // XXX - TreeSet<> in Java

        private DateTime? _created;
        private string _identifier;
        private Uri _instanceShape;
        private DateTime? _modified;
        private Uri _serviceProvider;
        private string _title;
        private Uri _desiredState;
        private Link _producedByAutomationRequest;
        private Link _reportsOnAutomationPlan;

        public AutomationResult() : base()
        {
            _rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_RESULT));
        }

        public AutomationResult(Uri about) : base(about)
        {
            _rdfTypes.Add(new Uri(AutomationConstants.TYPE_AUTOMATION_RESULT));
        }

        public void AddContributor(Uri contributor)
        {
            _contributors.Add(contributor);
        }

        public void AddCreator(Uri creator)
        {
            _creators.Add(creator);
        }

        public void AddRdfType(Uri rdfType)
        {
            _rdfTypes.Add(rdfType);
        }

        public void AddSubject(string subject)
        {
            _subjects.Add(subject);
        }

        public void AddState(Uri state)
        {
            _states.Add(state);
        }

        public void AddVerdict(Uri verdict)
        {
            _verdicts.Add(verdict);
        }

        public void AddContribution(Uri contribution)
        {
            _contributions.Add(contribution);
        }

        public void AddInputParameter(ParameterInstance parameter)
        {
            _inputParameters.Add(parameter);
        }

        public void AddOutputParameter(ParameterInstance parameter)
        {
            _outputParameters.Add(parameter);
        }

        [OslcDescription("The person(s) who are responsible for the work needed to complete the automation result.")]
        [OslcName("contributor")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "contributor")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Contributors")]
        public Uri[] GetContributors()
        {
            return _contributors.ToArray();
        }

        [OslcDescription("Timestamp of resource creation.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "created")]
        [OslcReadOnly]
        [OslcTitle("Created")]
        public DateTime? GetCreated()
        {
            return _created;
        }

        [OslcDescription("Creator or creators of resource.")]
        [OslcName("creator")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "creator")]
        [OslcRange(QmConstants.TYPE_PERSON)]
        [OslcTitle("Creators")]
        public Uri[] GetCreators()
        {
            return _creators.ToArray();
        }

        [OslcDescription("A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly]
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return _identifier;
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return _instanceShape;
        }

        [OslcDescription("Timestamp last latest resource modification.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "modified")]
        [OslcReadOnly]
        [OslcTitle("Modified")]
        public DateTime? GetModified()
        {
            return _modified;
        }

        [OslcDescription("The resource type URIs.")]
        [OslcName("type")]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
        [OslcTitle("Types")]
        public Uri[] GetRdfTypes()
        {
            return _rdfTypes.ToArray();
        }

        [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
        [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
        [OslcTitle("Service Provider")]
        public Uri GetServiceProvider()
        {
            return _serviceProvider;
        }

        [OslcDescription("Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")]
        [OslcName("subject")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "subject")]
        [OslcReadOnly(false)]
        [OslcTitle("Subjects")]
        public string[] GetSubjects()
        {
            return _subjects.ToArray();
        }

        [OslcDescription("Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcTitle("Title")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return _title;
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
            return _states.ToArray();
        }

        [OslcDescription("A result contribution associated with this automation result.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("contribution")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "contribution")]
        [OslcTitle("Contribution")]
        public Uri[] GetContributions()
        {
            return _contributions.ToArray();
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
            return _verdicts.ToArray();
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
            return _desiredState;
        }

        [OslcDescription("Automation Request which produced the Automation Result.")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "producedByAutomationRequest")]
        [OslcName("producedByAutomationRequest")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        [OslcTitle("Produced By Automation Request")]
        public Link GetProducedByAutomationRequest()
        {
            return _producedByAutomationRequest;
        }

        [OslcDescription("Automation Plan which the Automation Result reports on.")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "reportsOnAutomationPlan")]
        [OslcName("reportsOnAutomationPlan")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcTitle("Reports On Automation Plan")]
        public Link GetReportsOnAutomationPlan()
        {
            return _reportsOnAutomationPlan;
        }

        [OslcDescription("A copy of the parameters provided during creation of the Automation Request which produced this Automation Result.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("inputParameter")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "inputParameter")]
        [OslcReadOnly(true)]
        [OslcTitle("Input Parameter")]
        public ParameterInstance[] GetInputParameters()
        {
            return _inputParameters.ToArray();
        }

        [OslcDescription("Automation Result output parameters are parameters associated with the automation execution which produced this Result. This includes the value of all parameters used to initiate the execution and any additional parameters which may have been created during automation execution by the service provider or external agents.")]
        [OslcOccurs(Occurs.ZeroOrMany)]
        [OslcName("outputParameter")]
        [OslcPropertyDefinition(AutomationConstants.AUTOMATION_NAMESPACE + "outputParameter")]
        [OslcTitle("Output Parameter")]
        public ParameterInstance[] GetOutputParameters()
        {
            return _outputParameters.ToArray();
        }

        public void SetContributors(Uri[] contributors)
        {
            _contributors.Clear();

            if (contributors != null)
            {
                _contributors.AddAll(contributors);
            }
        }

        public void SetCreated(DateTime? created)
        {
            _created = created;
        }

        public void SetCreators(Uri[] creators)
        {
            _creators.Clear();

            if (creators != null)
            {
                _creators.AddAll(creators);
            }
        }

        public void SetIdentifier(string identifier)
        {
            _identifier = identifier;
        }

        public void SetInstanceShape(Uri instanceShape)
        {
            _instanceShape = instanceShape;
        }

        public void SetModified(DateTime? modified)
        {
            _modified = modified;
        }

        public void SetRdfTypes(Uri[] rdfTypes)
        {
            _rdfTypes.Clear();

            if (rdfTypes != null)
            {
                _rdfTypes.AddAll(rdfTypes);
            }
        }

        public void SetServiceProvider(Uri serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetSubjects(string[] subjects)
        {
            _subjects.Clear();

            if (subjects != null)
            {
                _subjects.AddAll(subjects);
            }
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public void SetStates(Uri[] states)
        {
            _states.Clear();

            if (states != null)
            {
                _states.AddAll(states);
            }
        }

        public void SetVerdicts(Uri[] verdicts)
        {
            _verdicts.Clear();

            if (verdicts != null)
            {
                _verdicts.AddAll(verdicts);
            }
        }

        public void SetContributions(Uri[] contributions)
        {
            _contributions.Clear();

            if (contributions != null)
            {
                _contributions.AddAll(contributions);
            }
        }

        public void SetDesiredState(Uri desiredState)
        {
            _desiredState = desiredState;
        }

        public void SetProducedByAutomationRequest(Link producedByAutomationRequest)
        {
            _producedByAutomationRequest = producedByAutomationRequest;
        }

        public void SetReportsOnAutomationPlan(Link reportsOnAutomationPlan)
        {
            _reportsOnAutomationPlan = reportsOnAutomationPlan;
        }

        public void SetInputParameters(ParameterInstance[] parameters)
        {
            _inputParameters.Clear();

            if (parameters != null)
            {
                _inputParameters.AddAll(parameters);
            }
        }

        public void SetOutputParameters(ParameterInstance[] parameters)
        {
            _outputParameters.Clear();

            if (parameters != null)
            {
                _outputParameters.AddAll(parameters);
            }
        }

        protected Uri GetRdfType()
        {
            return new Uri(AutomationConstants.TYPE_AUTOMATION_RESULT);
        }
    }
}
