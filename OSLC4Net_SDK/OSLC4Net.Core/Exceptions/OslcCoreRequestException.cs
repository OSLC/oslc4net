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

using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     OSLC4Net Core exception
/// </summary>
public class OslcCoreRequestException : Exception
{
    public OslcCoreRequestException(int statusCode, HttpResponseMessage? responseMessage,
        IResource? requestResource = null, Error? errorResource = null) : base($"{(int)responseMessage
            ?.StatusCode} {responseMessage?.ReasonPhrase}")
    {
        ResponseMessage = responseMessage;
        StatusCode = statusCode;
        ErrorResource = errorResource;
        RequestResource = requestResource;
    }

    public int StatusCode { get; }
    public HttpResponseMessage? ResponseMessage { get; }
    public IResource? RequestResource { get; }
    public Error? ErrorResource { get; }
}
