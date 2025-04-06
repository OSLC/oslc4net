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
        IResource? requestResource = null, Error? errorResource = null) : base(responseMessage
        ?.ReasonPhrase)
    {
        ResponseMessage = responseMessage;
        StatusCode = statusCode;
        ErrorResource = errorResource;
        RequestResource = requestResource;
    }

    public int StatusCode { get; set; }
    public HttpResponseMessage? ResponseMessage { get; set; }
    public IResource? RequestResource { get; private set; }
    public Error? ErrorResource { get; private set; }
}
