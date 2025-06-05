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
///     Exception thrown when relative URIs are encountered in an RDF model
/// </summary>
public class OslcCoreRelativeURIException(
    Type resourceType,
    string methodName,
    Uri relativeURI) : OslcCoreApplicationException(
    $"OSLC1014: Relative URI value {relativeURI} for method {methodName} of class {resourceType.Name}")
{
    public Type ResourceType { get; } = resourceType;
    public string MethodName { get; } = methodName;
    public Uri RelativeURI { get; } = relativeURI;
}
