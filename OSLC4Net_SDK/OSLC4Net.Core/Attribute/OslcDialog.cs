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
 *
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System;

namespace OSLC4Net.Core.Attribute
{
    /// <summary>
    /// OSLC Dialog attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class OslcDialog : System.Attribute
    {
        /**
	     * Title string that could be used for display
	     */
        public string title;

        /**
	     * Very short label for use in menu items
	     */
        public string label = "";

        /**
	     * The URI of the dialog
	     */
        public string uri;

        /**
         * Values MUST be expressed in relative length units.  Em and ex units are interpreted relative to the default system font (at 100% size).
         */
        public string hintWidth = "";

        /**
         * Values MUST be expressed in relative length units.  Em and ex units are interpreted relative to the default system font (at 100% size).
         */
        public string hintHeight = "";

        /**
         * Resource types
         */
        public string[] resourceTypes = { };

        /**
         * Usages
         */
        public string[] usages = { };
    }
}