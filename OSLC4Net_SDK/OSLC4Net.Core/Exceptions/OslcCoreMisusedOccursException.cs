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
///     Exception thrown for an incorrect use of the OSLC Occurs attribute
/// </summary>
public class OslcCoreMisusedOccursException : OslcCoreApplicationException
{
    private const string MessageKey = "MisusedOccursException";

    public MemberInfo Method { get; }
    private Type ResourceType { get; }

    /// <summary>
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="method"></param>
    public OslcCoreMisusedOccursException(Type resourceType, MemberInfo method) :
        base(MessageKey, [resourceType.Name, method.Name])
    {
        Method = method;
        ResourceType = resourceType;
    }
}
