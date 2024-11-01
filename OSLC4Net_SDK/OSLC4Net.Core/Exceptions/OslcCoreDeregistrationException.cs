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

namespace OSLC4Net.Core.Exceptions;

public class OslcCoreDeregistrationException : OslcCoreApplicationException
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="serviceProviderURI"></param>
    /// <param name="statusCode"></param>
    /// <param name="responseMessage"></param>
    public OslcCoreDeregistrationException(Uri serviceProviderURI, int statusCode, string responseMessage) :
        base(MESSAGE_KEY, new object[] { serviceProviderURI.ToString(), statusCode, responseMessage })
    {
        this.responseMessage = responseMessage;
        this.serviceProviderURI = serviceProviderURI;
        this.statusCode = statusCode;
    }

    public string getResponseMessage()
    {
        return responseMessage;
    }

    public Uri getServiceProviderURI()
    {
        return serviceProviderURI;
    }

    public int getStatusCode()
    {
        return statusCode;
    }

    private static readonly string MESSAGE_KEY = "DeregistrationException";

    private readonly string responseMessage;
    private readonly Uri serviceProviderURI;
    private readonly int statusCode;
}
