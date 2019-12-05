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
    [OslcResourceShape(title = "Parameter Instance Resource Shape", describes = new string[] {AutomationConstants.TYPE_PARAMETER_INSTANCE})]
    [OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
    /// <summary>
    /// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_ParameterInstance
    /// </summary>
    public class ParameterInstance : AbstractResource
    {
        private readonly ISet<Uri>      rdfTypes                    = new HashSet<Uri>(); // XXX - TreeSet<> in Java
    
        private string name;
        private string value;
        private string description;
        private Uri      instanceShape;
        private Uri      serviceProvider;

	    public ParameterInstance() : base()
	    {
		    rdfTypes.Add(new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE));
	    }
	
        public ParameterInstance(Uri about) : base(about)
        {
		    rdfTypes.Add(new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE));
         }

        protected Uri GetRdfType()
        {
    	    return new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE);
        }
    
        public void addRdfType(Uri rdfType)
        {
            rdfTypes.Add(rdfType);
        }

        [OslcDescription("Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
        [OslcTitle("Description")]
        [OslcValueType(Core.Model.ValueType.XMLLiteral)]
        public string GetDescription()
        {
            return description;
        }

        [OslcDescription("The name of the parameter instance.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "name")]
        [OslcTitle("Name")]
        public string GetName()
        {
            return name;
        }

        [OslcDescription("The value of the parameter.")]
        [OslcOccurs(Occurs.ZeroOrOne)]
        [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "value")]
        [OslcTitle("Value")]
        public string GetValue()
        {
            return value;
        }
    
        [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcTitle("Instance Shape")]
        public Uri GetInstanceShape()
        {
            return instanceShape;
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

        public void SetDescription(string description)
        {
            this.description = description;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    
        public void SetValue(string value)
        {
            this.value = value;
        }
    
        public void SetInstanceShape(Uri instanceShape)
        {
            this.instanceShape = instanceShape;
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

	    public int CompareTo(ParameterInstance o) {
		    return o.GetName().CompareTo(name);
	    }
    }
}
