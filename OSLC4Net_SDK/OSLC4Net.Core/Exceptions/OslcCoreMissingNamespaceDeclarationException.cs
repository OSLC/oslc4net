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

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     Exception thrown when a required OSLC attribute definition is missing.
/// </summary>
public class OslcCoreMissingNamespaceDeclarationException(
    string ns) : OslcCoreApplicationException(
    $"OSLC1009: Namespace declaration not defined for namespace {ns}")
{
    public string Namespace { get; } = ns;
}
