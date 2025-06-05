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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     Exception thrown when a property is defined more than once
/// </summary>
public class OslcCoreDuplicatePropertyDefinitionException(
    Type resourceType,
    OslcPropertyDefinition oslcPropertyDefinition) : OslcCoreApplicationException(
    $"OSLC1002: Duplicate property definition annotation {oslcPropertyDefinition.value} for class {resourceType.Name}")
{
    public Type ResourceType { get; } = resourceType;
    public OslcPropertyDefinition OslcPropertyDefinition { get; } = oslcPropertyDefinition;
}
