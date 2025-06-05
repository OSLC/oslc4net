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

using System.Globalization;
using System.Net;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     OSLC4Net Core exception
/// </summary>
// TODO: accept HttpResponseMessage in a static ctor but do not retain it (@berezovskyi 2025-06)
public class OslcCoreRequestException(
    HttpStatusCode? statusCode,
    string responseMessage,
    IResource? requestResource = null,
    Error? errorResource = null)
    : Exception($"HTTP {ToErrorCode(statusCode, -1)} {responseMessage}")
{
    public HttpStatusCode? StatusCode { get; } = statusCode;
    public string ResponseMessage { get; } = responseMessage;
    public IResource? RequestResource { get; } = requestResource;
    public Error? ErrorResource { get; } = errorResource;

    private static string ToErrorCode(HttpStatusCode? status, int code)
    {
        return status switch
        {
            null => code.ToString(CultureInfo.InvariantCulture),
            _ => ((int)status).ToString(CultureInfo.InvariantCulture)
        };
    }

}
