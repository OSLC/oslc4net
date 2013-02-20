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
using System.Reflection;
using System.Text;

namespace OSLC4Net.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a required OSLC attribute definition is missing.
    /// </summary>
    public class OslcCoreMissingNamespaceDeclarationException : OslcCoreApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="annotationType"></param>
        public OslcCoreMissingNamespaceDeclarationException(String ns) :
            base(MESSAGE_KEY, new object[] { ns })
        {
            this.ns = ns;
         }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    public String GetNamespace() {
            return ns;
	    }

        private static readonly String MESSAGE_KEY = "MissingNamespaceDeclarationException";

	    private String ns;
    }
}
