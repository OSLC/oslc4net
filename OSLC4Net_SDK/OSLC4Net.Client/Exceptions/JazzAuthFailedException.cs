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
using System.Net;

namespace OSLC4Net.Client.Exceptions
{
    /// <summary>
    /// Exceptions indicating a Jazz authentication or credentials problem
    /// </summary>
    public class JazzAuthFailedException : OslcClientApplicationException
    {
	    private const string MESSAGE_KEY = "JazzAuthFailedException";
	
	    private readonly string user;
	    private readonly string jazzUrl;
	
	    public JazzAuthFailedException(string user, string jazzUrl) :
		    base(MESSAGE_KEY, new object[] {user, jazzUrl})
        {
		    this.user = user;
		    this.jazzUrl = jazzUrl;
	    }
	
	    public string GetUser() {
		    return user;
	    }
	
	    public string GetJazzUrl() {
		    return jazzUrl;
	    }
    }
}
