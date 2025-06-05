/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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

namespace OSLC4Net.Client.Exceptions;

/// <summary>
/// Exception thrown when a requested resource could not be found.
/// </summary>
public class ResourceNotFoundException(string resource, string value)
    : OslcClientApplicationException($"OSLC2004: The requested resource could not be found. Resource: {resource} Value: {value}.")
{
    public string Resource { get; } = resource;

    public string Value { get; } = value;
}
