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

    using OSLC4Net.Core.Attribute;

    /// <summary>
    /// Exception thrown when a property is defined more than once
    /// </summary>
    public class OslcCoreDuplicatePropertyDefinitionException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "DuplicatePropertyDefinitionException";

        private OslcPropertyDefinition oslcPropertyDefinition;

        private Type resourceType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="oslcPropertyDefinition"></param>
        public OslcCoreDuplicatePropertyDefinitionException(
            Type resourceType,
            OslcPropertyDefinition oslcPropertyDefinition)
            : base(MESSAGE_KEY, new object[] { resourceType.Name, oslcPropertyDefinition.value })
        {
            this.oslcPropertyDefinition = oslcPropertyDefinition;
            this.resourceType = resourceType;
        }

        public OslcPropertyDefinition GetOslcPropertyDefinition()
        {
            return oslcPropertyDefinition;
        }
    }
}