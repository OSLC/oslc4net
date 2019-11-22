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

    using OSLC4Net.Core.Attribute;

    #endregion

    /// <summary>
    /// OSLC Publisher resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(
        title = "OSLC Publisher Resource Shape",
        describes = new string[] { OslcConstants.TYPE_PUBLISHER })]
    public class Publisher : AbstractResource
    {
        private Uri _icon;

        private string _identifier;

        private string _label;

        private string _title;

        public Publisher()
            : base()
        {
        }

        public Publisher(string title, string identifier)
            : this()
        {
            _title = title;
            _identifier = identifier;
        }

        [OslcDescription(
            "URL to an icon file that represents the provider. This icon should be a favicon format and 16x16 pixels in size")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "icon")]
        [OslcReadOnly]
        [OslcTitle("Icon")]
        public Uri GetIcon()
        {
            return _icon;
        }

        [OslcDescription("A URN that uniquely identifies the implementation")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
        [OslcReadOnly] // TODO - Marked as unspecified in the spec, but is this correct?
        [OslcTitle("Identifier")]
        public string GetIdentifier()
        {
            return _identifier;
        }

        [OslcDescription("Very short label for use in menu items")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "label")]
        [OslcReadOnly]
        [OslcTitle("Label")]
        public string GetLabel()
        {
            return _label;
        }

        [OslcDescription("Title string that could be used for display")]
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

        public void SetIdentifier(string identifier)
        {
            _identifier = identifier;
        }

        public void SetLabel(string label)
        {
            _label = label;
        }

        public void SetTitle(string title)
        {
            _title = title;
        }
    }
}