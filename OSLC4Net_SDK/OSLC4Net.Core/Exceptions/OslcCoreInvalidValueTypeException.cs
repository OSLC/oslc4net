/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 * Copyright (c) 2025 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *******************************************************************************/

using System.Reflection;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Core.Exceptions;

public class OslcCoreInvalidValueTypeException(
    Type resourceType,
    MemberInfo method,
    ValueType valueType) : OslcCoreApplicationException(
    $"OSLC1007: Invalid value type {ValueTypeExtension.ToString(valueType)} defined for method {method.Name} of class {resourceType.Name}")
{
    public Type ResourceType { get; } = resourceType;
    public MemberInfo Method { get; } = method;
    public ValueType ValueType { get; } = valueType;
}
