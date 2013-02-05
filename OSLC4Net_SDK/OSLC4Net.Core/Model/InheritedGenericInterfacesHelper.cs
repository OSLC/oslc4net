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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    public static class InheritedGenericInterfacesHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="typeToTest"></param>
        /// <returns></returns>
        public static bool ImplementsGenericInterface(Type genericType, Type typeToTest)
        {
            Type[] interfaces = typeToTest.GetInterfaces();

            foreach (Type interfac in interfaces) {
                if (interfac.IsGenericType && genericType == interfac.GetGenericTypeDefinition())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
