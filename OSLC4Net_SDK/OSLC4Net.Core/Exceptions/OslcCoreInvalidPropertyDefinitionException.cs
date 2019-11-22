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

    public class OslcCoreInvalidPropertyDefinitionException : OslcCoreApplicationException
    {
        private const string MESSAGE_KEY = "InvalidPropertyDefinitionException";

        private readonly MethodInfo _method;

        private readonly OslcPropertyDefinition _oslcPropertyDefinition;

        private readonly PropertyInfo _property;

        private readonly Type _resourceType;

        public OslcCoreInvalidPropertyDefinitionException(
            Type resourceType,
            MethodInfo method,
            OslcPropertyDefinition oslcPropertyDefinition)
            : base(MESSAGE_KEY, new object[] { resourceType.Name, method.Name, oslcPropertyDefinition.value })
        {
            _method = method;
            _oslcPropertyDefinition = oslcPropertyDefinition;
            _resourceType = resourceType;
        }

        public OslcCoreInvalidPropertyDefinitionException(
            Type resourceType,
            PropertyInfo property,
            OslcPropertyDefinition oslcPropertyDefinition)
            : base(MESSAGE_KEY, new object[] { resourceType.Name, property.Name, oslcPropertyDefinition.value })
        {
            _property = property;
            _oslcPropertyDefinition = oslcPropertyDefinition;
            _resourceType = resourceType;
        }
    }
}