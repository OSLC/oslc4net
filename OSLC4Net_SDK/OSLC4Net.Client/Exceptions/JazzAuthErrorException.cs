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

using System.Net;

namespace OSLC4Net.Client.Exceptions;

/// <summary>
/// Exceptions indicating a Jazz authentication or credentials problem
/// </summary>
public class JazzAuthErrorException(HttpStatusCode status, string jazzUrl)
    : OslcClientApplicationException(
        $"OSLC2002: An error occurred attempting to login to the Jazz server. Status code {status}. Server location: {jazzUrl}.")
{
    /// <summary>
    ///     HTTP status code returned by the Jazz server.
    /// </summary>
    public HttpStatusCode Status { get; } = status;

    /// <summary>
    ///     Jazz endpoint URL.
    /// </summary>
    public string JazzUrl { get; } = jazzUrl;
}
