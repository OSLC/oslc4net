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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Exceptions
{
    public class OslcCoreDuplicatePropertyDefinitionException : OslcCoreApplicationException
    {
        public OslcCoreDuplicatePropertyDefinitionException(Type resourceType, OslcPropertyDefinition oslcPropertyDefinition) :
            base(MESSAGE_KEY, new Object[] {resourceType.Name, oslcPropertyDefinition.value})
        {
            this.oslcPropertyDefinition = oslcPropertyDefinition;
            this.resourceType          = resourceType;
        }

        public OslcPropertyDefinition GetOslcPropertyDefinition()
        {
            return oslcPropertyDefinition;
        }

        private static readonly String MESSAGE_KEY = "DuplicatePropertyDefinitionException";

        private OslcPropertyDefinition oslcPropertyDefinition;
        private Type               resourceType;
    }
}
