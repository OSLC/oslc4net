/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

/// <summary>
/// OSLC Property attributes
/// </summary>
/// <remarks>See http://open-services.net/bin/view/Main/OSLCCoreSpecAppendixA </remarks>
///
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Property Resource Shape", describes = new string[] { OslcConstants.TYPE_PROPERTY })]
public sealed class Property : AbstractResource, IComparable<Property>
{
    private readonly IList<string> allowedValues = new List<string>();
    private readonly List<Uri> range = new List<Uri>();

    private Uri allowedValuesRef;
    private string defaultValue;
    private string description;
    private bool hidden;
    private int maxSize;
    private bool memberProperty;
    private string name;
    private Occurs occurs;
    private Uri propertyDefinition;
    private bool readOnly;
    private Representation representation;
    private string title;
    private Uri valueShape;
    private ValueType valueType;

    public Property() : base()
    {
    }

    public Property(string name,
            Occurs occurs,
            Uri propertyDefinition,
            ValueType valueType) : this()
    {
        this.name = name;
        this.occurs = occurs;
        this.propertyDefinition = propertyDefinition;
        this.valueType = valueType;
    }

    public void AddAllowedValue(string allowedValue)
    {
        this.allowedValues.Add(allowedValue);
    }

    public void AddRange(Uri range)
    {
        this.range.Add(range);
    }

    public int CompareTo(Property o)
    {
        return name.CompareTo(o.GetName());
    }

    [OslcDescription("A value allowed for property, inlined into property definition. If there are both oslc:allowedValue elements and an oslc:allowedValue resource, then the full-set of allowed values is the union of both")]
    [OslcName("allowedValue")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "allowedValue")]
    [OslcReadOnly]
    [OslcTitle("Allowed Values")]
    public string[] GetAllowedValues()
    {
        return allowedValues.ToArray();
    }

    [OslcDescription("Resource with allowed values for the property being defined")]
    [OslcName("allowedValues")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "allowedValues")]
    [OslcRange(OslcConstants.TYPE_ALLOWED_VALUES)]
    [OslcReadOnly]
    [OslcTitle("Allowed Value Reference")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_ALLOWED_VALUES)]
    public Uri GetAllowedValuesRef()
    {
        return allowedValuesRef;
    }

    [OslcDescription("A default value for property, inlined into property definition")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "defaultValue")]
    [OslcReadOnly]
    [OslcTitle("Default Value")]
    public string GetDefaultValue()
    {
        return defaultValue;
    }

    [OslcDescription("Description of the property. SHOULD include only content that is valid and suitable inside an XHTML <div> element")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "description")]
    [OslcReadOnly]
    [OslcTitle("Description")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetDescription()
    {
        return description;
    }

    [OslcDescription("For string properties only, specifies maximum characters allowed. If not set, then there is no maximum or maximum is specified elsewhere")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "maxSize")]
    [OslcReadOnly]
    [OslcTitle("Maximum Size")]
    public int getMaxSize()
    {
        return maxSize;
    }

    [OslcDescription("Name of property being defined, i.e. second part of property's Prefixed Name")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "name")]
    [OslcReadOnly]
    [OslcTitle("Name")]
    public string GetName()
    {
        return name;
    }

    [OslcAllowedValue(OslcConstants.OSLC_CORE_NAMESPACE + "Exactly-one",
                  OslcConstants.OSLC_CORE_NAMESPACE + "Zero-or-one",
                  OslcConstants.OSLC_CORE_NAMESPACE + "Zero-or-many",
                  OslcConstants.OSLC_CORE_NAMESPACE + "One-or-many")]
    [OslcDescription("MUST be either http://open-services.net/ns/core#Exactly-one, http://open-services.net/ns/core#Zero-or-one, http://open-services.net/ns/core#Zero-or-many or http://open-services.net/ns/core#One-or-many")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "occurs")]
    [OslcReadOnly]
    [OslcTitle("Occurs")]
    public Uri GetOccurs()
    {
        if (occurs != Occurs.Unknown)
        {
            try
            {
                return new Uri(OccursExtension.ToString(occurs));
            }
            catch (UriFormatException exception)
            {
                // This should never happen since we control the possible values of the Occurs enum.
                throw new SystemException(exception.Message, exception);
            }
        }

        return null;
    }

    [OslcDescription("Uri of the property whose usage is being described")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "propertyDefinition")]
    [OslcReadOnly]
    [OslcTitle("Property Definition")]
    public Uri GetPropertyDefinition()
    {
        return propertyDefinition;
    }

    [OslcDescription("For properties with a resource value-type, Providers MAY also specify the range of possible resource classes allowed, each specified by Uri. The default range is http://open-services.net/ns/core#Any")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "range")]
    [OslcReadOnly]
    [OslcTitle("Ranges")]
    public Uri[] GetRange()
    {
        return range.ToArray();
    }

    [OslcAllowedValue(OslcConstants.OSLC_CORE_NAMESPACE + "Reference",
                      OslcConstants.OSLC_CORE_NAMESPACE + "Inline",
                      OslcConstants.OSLC_CORE_NAMESPACE + "Either")]
    [OslcDescription("Should be http://open-services.net/ns/core#Reference, http://open-services.net/ns/core#Inline or http://open-services.net/ns/core#Either")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "representation")]
    [OslcReadOnly]
    [OslcTitle("Representation")]
    public Uri GetRepresentation()
    {
        if (representation != null)
        {
            try
            {
                return new Uri(RepresentationExtension.ToString(representation));
            }
            catch (UriFormatException exception)
            {
                // This should never happen since we control the possible values of the Representation enum.
                throw new SystemException(exception.Message, exception);
            }
        }

        return null;
    }

    [OslcDescription("Title of the property. SHOULD include only content that is valid and suitable inside an XHTML <div> element")]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
    [OslcReadOnly]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle()
    {
        return title;
    }

    [OslcDescription("if the value-type is a resource type, then Property MAY provide a shape value to indicate the Resource Shape that applies to the resource")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "valueShape")]
    [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
    [OslcReadOnly]
    [OslcTitle("Value Shape")]
    public Uri GetValueShape()
    {
        return valueShape;
    }

    [OslcAllowedValue(OslcConstants.XML_NAMESPACE + "boolean",
                      OslcConstants.XML_NAMESPACE + "dateTime",
                      OslcConstants.XML_NAMESPACE + "decimal",
                      OslcConstants.XML_NAMESPACE + "double",
                      OslcConstants.XML_NAMESPACE + "float",
                      OslcConstants.XML_NAMESPACE + "integer",
                      OslcConstants.XML_NAMESPACE + "string",
                      OslcConstants.RDF_NAMESPACE + "XMLLiteral",
                      OslcConstants.OSLC_CORE_NAMESPACE + "Resource",
                      OslcConstants.OSLC_CORE_NAMESPACE + "LocalResource",
                      OslcConstants.OSLC_CORE_NAMESPACE + "AnyResource")]
    [OslcDescription("See list of allowed values for oslc:valueType")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "valueType")]
    [OslcReadOnly]
    [OslcTitle("Value Type")]
    public Uri GetValueType()
    {
        if (valueType != ValueType.Unknown)
        {
            try
            {
                return new Uri(ValueTypeExtension.ToString(valueType));
            }
            catch (UriFormatException exception)
            {
                // This should never happen since we control the possible values of the ValueType enum.
                throw new SystemException(exception.Message, exception);
            }
        }

        return null;
    }

    [OslcDescription("A hint that indicates that property MAY be hidden when presented in a user interface")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hidden")]
    [OslcReadOnly]
    [OslcTitle("Hidden")]
    public bool IsHidden()
    {
        return hidden;
    }

    [OslcDescription("If set to true, this indicates that the property is a membership property, as described in the Query Syntax Specification: Member List Patterns. This is useful when the resource whose shape is being defined is viewed as a container of other resources. For example, look at the last example in Appendix B's RDF/XML Representation Examples: Specifying the shape of a query result, where blog:comment is defined as a membership property and comment that matches the query is returned as value of that property.")]
    [OslcName("isMemberProperty")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "isMemberProperty")]
    [OslcReadOnly]
    [OslcTitle("Is Member Property")]
    public bool IsMemberProperty()
    {
        return memberProperty;
    }

    [OslcDescription("true if the property is read-only. If not set, or set to false, then the property is writable. Providers SHOULD declare a property read-only when changes to the value of that property will not be accepted on PUT. Consumers should note that the converse does not apply: Providers MAY reject a change to the value of a writable property.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "readOnly")]
    [OslcReadOnly]
    [OslcTitle("Read Only")]
    public bool IsReadOnly()
    {
        return readOnly;
    }

    public void SetAllowedValues(string[] allowedValues)
    {
        this.allowedValues.Clear();
        if (allowedValues != null)
        {

            foreach (var av in allowedValues)
            {
                this.allowedValues.Add(av);
            }
        }
    }

    public void SetAllowedValuesRef(Uri allowedValuesRef)
    {
        if (allowedValuesRef != null)
        {
            this.allowedValuesRef = allowedValuesRef;
        }
        else
        {
            this.allowedValuesRef = null;
        }
    }

    public void SetDefaultValue(string defaultValue)
    {
        this.defaultValue = defaultValue;
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }

    public void SetHidden(bool hidden)
    {
        this.hidden = hidden;
    }

    public void SetMaxSize(int maxSize)
    {
        this.maxSize = maxSize;
    }

    public void SetMemberProperty(bool memberProperty)
    {
        this.memberProperty = memberProperty;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetOccurs(Occurs occurs)
    {
        this.occurs = occurs;
    }

    public void SetOccurs(Uri occurs)
    {
        if (occurs != null)
        {
            this.occurs = OccursExtension.FromString(occurs.ToString());
        }
        else
        {
            this.occurs = Occurs.Unknown;
        }
    }

    public void SetPropertyDefinition(Uri propertyDefinition)
    {
        this.propertyDefinition = propertyDefinition;
    }

    public void SetRange(Uri[] ranges)
    {
        this.range.Clear();
        if (ranges != null)
        {
            foreach (var value in ranges)
            {
                this.range.Add(value);
            }
        }
    }

    public void SetReadOnly(bool readOnly)
    {
        this.readOnly = readOnly;
    }

    public void SetRepresentation(Representation representation)
    {
        this.representation = representation;
    }

    public void SetRepresentation(Uri representation)
    {
        if (representation != null)
        {
            this.representation = RepresentationExtension.FromString(representation.ToString());
        }
        else
        {
            this.representation = Representation.Unknown;
        }
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetValueShape(Uri valueShape)
    {
        this.valueShape = valueShape;
    }

    public void SetValueType(ValueType valueType)
    {
        this.valueType = valueType;
    }

    public void SetValueType(Uri valueType)
    {
        if (valueType != null)
        {
            this.valueType = ValueTypeExtension.FromString(valueType.ToString());
        }
        else
        {
            this.valueType = ValueType.Unknown;
        }
    }
}
