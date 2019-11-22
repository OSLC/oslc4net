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
    /// Exception thrown for an invalid property type
    /// </summary>
    public class OslcCoreInvalidPropertyTypeException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "InvalidPropertyTypeException";

        private MethodInfo _method;

        private Type _resourceType;

        private Type _returnType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="method"></param>
        /// <param name="returnType"></param>
        public OslcCoreInvalidPropertyTypeException(Type resourceType, MethodInfo method, Type returnType)
            : base(MESSAGE_KEY, new object[] { resourceType.Name, method.Name, returnType.Name })
        {
            _method = method;
            _resourceType = resourceType;
            _returnType = returnType;
        }

        public MethodInfo GetMethod()
        {
            return _method;
        }

        public Type GetResourceClass()
        {
            return _resourceType;
        }
    }
}