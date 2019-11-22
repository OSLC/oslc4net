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

    using OSLC4Net.Core.Attribute;
    using OSLC4Net.Core.Model;

    public class OslcCoreInvalidOccursException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "InvalidOccursException";

        private MethodInfo _method;

        private OslcOccurs _oslcOccurs;

        private Type _resourceType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="method"></param>
        /// <param name="oslcOccurs"></param>
        public OslcCoreInvalidOccursException(Type resourceType, MethodInfo method, OslcOccurs oslcOccurs)
            : base(
                MESSAGE_KEY,
                new object[] { resourceType.Name, method.Name, OccursExtension.ToString(oslcOccurs.value) })
        {
            _method = method;
            _oslcOccurs = oslcOccurs;
            _resourceType = resourceType;
        }

        public MethodInfo GetMethod()
        {
            return _method;
        }

        public OslcOccurs GetOslcOccurs()
        {
            return _oslcOccurs;
        }

        public Type GetResourceType()
        {
            return _resourceType;
        }
    }
}