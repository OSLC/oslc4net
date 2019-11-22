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
    /// OSLC Preview attribute
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC Preview Resource Shape", describes = new string[] { OslcConstants.TYPE_PREVIEW })]
    public class Preview : AbstractResource
    {
        private Uri document;

        private string hintHeight;

        private string hintWidth;

        private string initialHeight;

        public Preview()
            : base()
        {
        }

        [OslcDescription("The Uri of an HTML document to be used for the preview")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "document")]
        [OslcReadOnly]
        [OslcTitle("Document")]
        public Uri GetDocument()
        {
            return document;
        }

        [OslcDescription(
            "Recommended height of the preview. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size).")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hintHeight")]
        [OslcReadOnly]
        [OslcTitle("Hint Height")]
        public string GetHintHeight()
        {
            return hintHeight;
        }

        [OslcDescription(
            "Recommended width of the preview. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size).")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hintWidth")]
        [OslcReadOnly]
        [OslcTitle("Hint Width")]
        public string GetHintWidth()
        {
            return hintWidth;
        }

        [OslcDescription(
            "Recommended initial height of the preview. The presence of this property indicates that the preview supports dynamically computing its size. Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1). Em and ex units are interpreted relative to the default system font (at 100% size).")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "initialHeight")]
        [OslcReadOnly]
        [OslcTitle("Initial Height")]
        public string GetInitialHeight()
        {
            return initialHeight;
        }

        public void SetDocument(Uri document)
        {
            this.document = document;
        }

        public void SetHintHeight(string hintHeight)
        {
            this.hintHeight = hintHeight;
        }

        public void SetHintWidth(string hintWidth)
        {
            this.hintWidth = hintWidth;
        }

        public void SetInitialHeight(string initialHeight)
        {
            this.initialHeight = initialHeight;
        }
    }
}