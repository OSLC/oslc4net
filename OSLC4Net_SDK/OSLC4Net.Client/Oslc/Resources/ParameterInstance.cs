/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 * Copyright (c) 2023 Andrii Berezovskyi and OSLC4Net contributors.
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

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Client.Oslc.Resources;

/// <summary>
/// http://open-services.net/wiki/automation/OSLC-Automation-Specification-Version-2.0/#Resource_ParameterInstance
/// </summary>
[OslcResourceShape(title = "Parameter Instance Resource Shape",
    describes = new string[] { AutomationConstants.TYPE_PARAMETER_INSTANCE })]
[OslcNamespace(AutomationConstants.AUTOMATION_NAMESPACE)]
public class ParameterInstance : AbstractResource
{


    private string name;
    private string value;
    private string description;
    private Uri instanceShape;
    private Uri serviceProvider;

    public ParameterInstance() : base()
    {
        AddType(new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE));
    }

    public ParameterInstance(Uri about) : base(about)
    {
        AddType(new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE));
    }

    protected Uri GetRdfType()
    {
        return new Uri(AutomationConstants.TYPE_PARAMETER_INSTANCE);
    }

    public void addRdfType(Uri rdfType)
    {
        AddType(rdfType);
    }

    [OslcDescription(
        "Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content.")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
    [OslcTitle("Description")]
    [OslcValueType(ValueType.XMLLiteral)]
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

    [Obsolete("User GetTypes() or .Types instead")]
    public Uri[] GetRdfTypes()
    {
        return GetTypes().ToArray();
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

    [Obsolete("User SetTypes() or .Types instead")]
    public void SetRdfTypes(Uri[] rdfTypes)
    {
        SetTypes(rdfTypes);
    }

    public void SetServiceProvider(Uri serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public int CompareTo(ParameterInstance o)
    {
        return o.GetName().CompareTo(name);
    }
}
