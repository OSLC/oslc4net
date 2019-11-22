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

    [OslcResourceShape(title = "Parameter Instance Resource Shape", describes = new string[] { AutomationConstants.TYPE_PARAMETER_INSTANCE })]
    [OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_ParameterInstance
    /// </summary>
    public class ParameterInstance : AbstractResource
    {
        private readonly ISet<Uri> _rdfTypes = new HashSet<Uri>(); // XXX - TreeSet<> in Java

        private string _name;
        private string _value;
        private string _description;
        private Uri _instanceShape;
        private Uri _serviceProvider;

        public ParameterInstance() : base()
        {
            _rdfTypes.Add(new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE));
        }

        public ParameterInstance(Uri about) : base(about)
        {
            _rdfTypes.Add(new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE));
        }

        public void AddRdfType(Uri rdfType)
        {
            _rdfTypes.Add(rdfType);
        }

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return _description;
        }

        [OslcDescription("The name of the parameter instance.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "name")]
        [OslcTitle("Name")]
        public string GetName()
        {
            return _name;
        }

        [OslcDescription("The value of the parameter.")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "value")]
        [OslcTitle("Value")]
        public string GetValue()
        {
            return _value;
        }

        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return _instanceShape;
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

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public void SetValue(string value)
        {
            _value = value;
        }

        public void SetInstanceShape(Uri instanceShape)
        {
            _instanceShape = instanceShape;
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

        public int CompareTo(ParameterInstance o)
        {
            return o.GetName().CompareTo(_name);
        }

        protected Uri GetRdfType()
        {
            return new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE);
        }
    }
}
