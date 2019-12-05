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
using System.Text;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    /// <summary>
    /// Static utilities to return the qualified name of a type
    /// </summary>
    public static class TypeFactory
    {

        public static string GetQualifiedName(Type objectType)
        {
            return GetNamespace(objectType) +
                   GetName(objectType);
        }

        public static string GetNamespace(Type objectType)
        {
            OslcNamespace[] oslcNamespaceAnnotation = (OslcNamespace[])(objectType.GetCustomAttributes(typeof(OslcNamespace), false));

            return oslcNamespaceAnnotation.Length > 0 ? oslcNamespaceAnnotation[0].value : OslcConstants.OSLC_DATA_NAMESPACE;
        }

        public static string GetName(Type objectType)
        {
            OslcName[] oslcNameAnnotation = (OslcName[])(objectType.GetCustomAttributes(typeof(OslcName), false));

            return oslcNameAnnotation.Length > 0 ? oslcNameAnnotation[0].value : objectType.Name;
        }
    }
}
