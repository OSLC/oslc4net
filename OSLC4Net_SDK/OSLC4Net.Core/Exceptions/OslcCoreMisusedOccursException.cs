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
    using System;
    using System.Reflection;

    /// <summary>
    /// Exception thrown for an incorrect use of the OSLC Occurs attribute
    /// </summary>
    public class OslcCoreMisusedOccursException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "MisusedOccursException";

        private MethodInfo _method;

        private Type _resourceType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="method"></param>
        public OslcCoreMisusedOccursException(Type resourceType, MethodInfo method)
            : base(MESSAGE_KEY, new object[] { resourceType.Name, method.Name })
        {
            _method = method;
            _resourceType = resourceType;
        }

        public MethodInfo GetMethod()
        {
            return _method;
        }

        public Type GetResourceType()
        {
            return _resourceType;
        }
    }
}