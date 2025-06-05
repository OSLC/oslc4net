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
/// Exceptions indicating a Jazz authentication or credentials problem
/// </summary>
public class JazzAuthFailedException(string user, string jazzUrl)
    : OslcClientApplicationException(
        $"OSLC2001: Unable to authenticate user {user} with the Jazz server at {jazzUrl}.")
{
    /// <summary>
    ///     User that failed authentication.
    /// </summary>
    public string User { get; } = user;

    /// <summary>
    ///     Jazz server URL.
    /// </summary>
    public string JazzUrl { get; } = jazzUrl;
}
