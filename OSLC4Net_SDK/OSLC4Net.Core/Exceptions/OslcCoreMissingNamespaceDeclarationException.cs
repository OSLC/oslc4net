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

namespace OSLC4Net.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a required OSLC attribute definition is missing.
    /// </summary>
    public class OslcCoreMissingNamespaceDeclarationException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "MissingNamespaceDeclarationException";

        private string _ns;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="annotationType"></param>
        public OslcCoreMissingNamespaceDeclarationException(string ns)
            : base(MESSAGE_KEY, new object[] { ns })
        {
            _ns = ns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetNamespace()
        {
            return _ns;
        }
    }
}