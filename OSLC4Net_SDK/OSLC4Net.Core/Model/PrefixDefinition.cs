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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    /// <summary>
    /// OSLC PrefixDefinition resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC Prefix Definition Resource Shape", describes = new string[] { OslcConstants.TYPE_PREFIX_DEFINITION })]
    public class PrefixDefinition : AbstractResource
    {
	    private string prefix;
	    private Uri prefixBase;

	    public PrefixDefinition() : base()
        {
	    }

	    public PrefixDefinition(string prefix, Uri prefixBase) : this()
        {
		    this.prefix = prefix;
		    this.prefixBase = prefixBase;
	    }

	    [OslcDescription("Namespace prefix to be used for this namespace")]
	    [OslcOccurs(Occurs.ExactlyOne)]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "prefix")]
	    [OslcReadOnly]
        [OslcTitle("Prefix")]
	    public string GetPrefix() {
		    return prefix;
	    }

	    [OslcDescription("The base Uri of the namespace")]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "prefixBase")]
	    [OslcReadOnly]
        [OslcTitle("Prefix Base")]
	    public Uri GetPrefixBase() {
	        return prefixBase;
	    }

	    public void SetPrefix(string prefix) {
		    this.prefix = prefix;
	    }

	    public void SetPrefixBase(Uri prefixBase) {
	        this.prefixBase = prefixBase;
	    }
    }
}
