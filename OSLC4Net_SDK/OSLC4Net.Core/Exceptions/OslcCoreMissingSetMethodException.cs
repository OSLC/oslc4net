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
///     Exception thrown for beans without required setters
/// </summary>
public class OslcCoreMissingSetMethodException(
    Type resourceType,
    MethodInfo getMethod,
    Exception? exception = null) : OslcCoreApplicationException(
    $"OSLC1011: Missing corresponding set method for method {getMethod.Name} of class {resourceType.Name}")
{
    public Type ResourceType { get; } = resourceType;
    public MethodInfo GetMethod { get; } = getMethod;
    public Exception? Exception { get; } = exception;
}
