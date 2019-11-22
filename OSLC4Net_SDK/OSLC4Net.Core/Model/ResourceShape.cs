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

namespace OSLC4Net.Core.Model
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OSLC4Net.Core.Attribute;

    #endregion

    /// <summary>
    /// OSLC ResourceShape resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(
        title = "OSLC Resource Shape Resource Shape",
        describes = new string[] { OslcConstants.TYPE_RESOURCE_SHAPE })]
    public class ResourceShape : AbstractResource
    {
        private SortedSet<Uri> describes = new SortedUriSet();

        private SortedSet<Property> properties = new SortedSet<Property>();

        private string title;

        public ResourceShape()
            : base()
        {
        }

        public ResourceShape(Uri about)
            : base(about)
        {
        }

        public void AddDescribeItem(Uri describeItem)
        {
            describes.Add(describeItem);
        }

        public void AddProperty(Property property)
        {
            properties.Add(property);
        }

        [OslcDescription("Type or types of resource described by this shape")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "describes")]
        [OslcReadOnly]
        [OslcTitle("Describes")]
        public Uri[] GetDescribes()
        {
            return describes.ToArray();
        }

        [OslcDescription("The properties that are allowed or required by this shape")]
        [OslcName("property")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "property")]
        [OslcRange(OslcConstants.TYPE_PROPERTY)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Properties")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PROPERTY)]
        [OslcValueType(ValueType.LocalResource)]
        public Property[] GetProperties()
        {
            return properties.ToArray();
        }

        [OslcDescription(
            "Title of the resource shape. SHOULD include only content that is valid and suitable inside an XHTML <div> element")]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcReadOnly]
        [OslcTitle("Title")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return title;
        }

        public void SetDescribes(Uri[] describes)
        {
            this.describes.Clear();
            if (describes != null)
            {
                foreach (var desc in describes)
                {
                    this.describes.Add(desc);
                }
            }
        }

        public void SetProperties(Property[] properties)
        {
            this.properties.Clear();
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    this.properties.Add(prop);
                }
            }
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }
    }
}