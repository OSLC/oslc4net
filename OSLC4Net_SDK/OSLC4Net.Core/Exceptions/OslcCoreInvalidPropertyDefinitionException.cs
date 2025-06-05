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
using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Exceptions;

public class OslcCoreInvalidPropertyDefinitionException(
    Type resourceType,
    MemberInfo? member,
    OslcPropertyDefinition oslcPropertyDefinition) : OslcCoreApplicationException(
    $"OSLC1004: Invalid property definition annotation {oslcPropertyDefinition.value} for method {member?.Name} of class {resourceType.Name}")
{
    public Type ResourceType { get; } = resourceType;
    public MemberInfo? Member { get; } = member;
    public OslcPropertyDefinition OslcPropertyDefinition { get; } = oslcPropertyDefinition;
}
