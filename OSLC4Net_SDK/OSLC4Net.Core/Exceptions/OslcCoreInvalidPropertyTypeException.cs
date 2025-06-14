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

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     Exception thrown for an invalid property type
/// </summary>
public class OslcCoreInvalidPropertyTypeException(
    Type resourceType,
    MethodInfo method,
    Type returnType) : OslcCoreApplicationException(
    $"OSLC1005: Invalid property type {returnType.Name} returned by method {method.Name} of class {resourceType.Name}")
{
    public Type ResourceType { get; } = resourceType;
    public MethodInfo Method { get; } = method;
    public Type ReturnType { get; } = returnType;
}
