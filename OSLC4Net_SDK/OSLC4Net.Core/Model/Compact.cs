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
    using System;

    using OSLC4Net.Core.Attribute;

    /// <summary>
    /// OSLC Compact resource representation
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC Compact Resource Shape", describes = new string[] { OslcConstants.TYPE_COMPACT })]
    public class Compact : AbstractResource
    {
        private Uri _icon;

        private Preview _largePreview;

        private string _shortTitle;

        private Preview _smallPreview;

        private string _title;

        public Compact()
            : base()
        {
        }

        [OslcDescription(
            "Uri of an image which may be used in the display of a link to the resource. The image SHOULD be 16x16 pixels in size.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "icon")]
        [OslcReadOnly]
        [OslcTitle("Icon")]
        public Uri GetIcon()
        {
            return _icon;
        }

        [OslcDescription("Uri and sizing properties for an HTML document to be used for a large preview.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "largePreview")]
        [OslcRange(OslcConstants.TYPE_PREVIEW)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Large Preview")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PREVIEW)]
        [OslcValueType(ValueType.LocalResource)]
        public Preview GetLargePreview()
        {
            return _largePreview;
        }

        [OslcDescription("Abbreviated title which may be used in the display of a link to the resource.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "shortTitle")]
        [OslcReadOnly]
        [OslcTitle("Short Title")]
        public string GetShortTitle()
        {
            return _shortTitle;
        }

        [OslcDescription("Uri and sizing properties for an HTML document to be used for a small preview.")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "smallPreview")]
        [OslcRange(OslcConstants.TYPE_PREVIEW)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Small Preview")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_PREVIEW)]
        [OslcValueType(ValueType.LocalResource)]
        public Preview GetSmallPreview()
        {
            return _smallPreview;
        }

        [OslcDescription("Title which may be used in the display of a link to the resource.")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcReadOnly]
        [OslcTitle("Title")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return _title;
        }

        public void SetIcon(Uri icon)
        {
            _icon = icon;
        }

        public void SetLargePreview(Preview largePreview)
        {
            _largePreview = largePreview;
        }

        public void SetShortTitle(string shortTitle)
        {
            _shortTitle = shortTitle;
        }

        public void SetSmallPreview(Preview smallPreview)
        {
            _smallPreview = smallPreview;
        }

        public void SetTitle(string title)
        {
            _title = title;
        }
    }
}