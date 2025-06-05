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
/// Exception thrown when an error occurs processing the root services document.
/// </summary>
public class RootServicesException(string endpoint, Exception exception)
    : OslcClientApplicationException($"OSLC2003: An error occurred processing the root services document. Server location: {endpoint}.", exception)
{
    /// <summary>
    /// OSLC server URL.
    /// </summary>
    public string Endpoint { get; } = endpoint;
}
