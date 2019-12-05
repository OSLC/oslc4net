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
using System.Text;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    /// <summary>
    /// An OSLC link - inculdes a Uri and a label.
    /// </summary>
    public class Link : AbstractReifiedResource<Uri>
    {
	    private string label;

	    public Link()
	    {	
	    }
	
	    public Link(Uri resource)
	    {
		    SetValue(resource);
	    }
	
	    public Link(Uri resource, string label)
	    {
		    SetValue(resource);
		    this.label = label;
	    }

        /// <summary>
        /// Gets the link label.
        /// </summary>
        /// <returns></returns>
	    [OslcName("title")]
	    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
	    public string GetLabel()
	    {
		    return label;
	    }

	    /// <summary>
        /// Sets the link label.
	    /// </summary>
	    /// <param name="label"></param>
	    public void SetLabel(string label)
	    {
		    this.label = label;
	    }
    }
}
