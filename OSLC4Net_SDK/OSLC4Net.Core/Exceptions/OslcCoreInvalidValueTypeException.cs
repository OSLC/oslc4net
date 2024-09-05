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
using System.Reflection;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Exceptions;

public class OslcCoreInvalidValueTypeException : OslcCoreApplicationException
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="method"></param>
    /// <param name="valueType"></param>
    public OslcCoreInvalidValueTypeException(Type resourceType, MethodInfo method, Model.ValueType valueType) :
        base(MESSAGE_KEY, new object[] { resourceType.Name, method.Name, ValueTypeExtension.ToString(valueType) })
    {
        this.method = method;
        this.valueType = valueType;
        this.resourceType = resourceType;
    }

    public MethodInfo GetMethod()
    {
        return method;
    }

    public Model.ValueType GetValueType()
    {
        return valueType;
    }

    public Type GetResourceType()
    {
        return resourceType;
    }

    private static readonly string MESSAGE_KEY = "InvalidValueTypeException";

    private readonly MethodInfo method;
    private readonly Type resourceType;
    private readonly Model.ValueType valueType;
}
