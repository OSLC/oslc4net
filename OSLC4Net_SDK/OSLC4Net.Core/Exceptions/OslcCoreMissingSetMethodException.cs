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

using System.Reflection;

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     Exception thrown for beans without required setters
/// </summary>
public class OslcCoreMissingSetMethodException : OslcCoreApplicationException
{
    private static readonly string MESSAGE_KEY = "MissingSetMethodException";
    private readonly MethodInfo getMethod;

    private readonly Type resourceType;

    /// <summary>
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="getMethod"></param>
    /// <param name="exception"></param>
    public OslcCoreMissingSetMethodException(Type resourceType, MethodInfo getMethod,
        Exception exception) :
        base(MESSAGE_KEY, new object[] { resourceType.Name, getMethod.Name, exception })
    {
        this.getMethod = getMethod;
        this.resourceType = resourceType;
    }

    /// <summary>
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="getMethod"></param>
    public OslcCoreMissingSetMethodException(Type resourceType, MethodInfo getMethod) :
        base(MESSAGE_KEY, new object[] { resourceType.Name, getMethod.Name, null })
    {
        this.getMethod = getMethod;
        this.resourceType = resourceType;
    }
}
