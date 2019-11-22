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

namespace OSLC4Net.Core.Exceptions
{
    using System;

    public class OslcCoreDeregistrationException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "DeregistrationException";

        private string responseMessage;

        private Uri serviceProviderURI;

        private int statusCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProviderURI"></param>
        /// <param name="statusCode"></param>
        /// <param name="responseMessage"></param>
        public OslcCoreDeregistrationException(Uri serviceProviderURI, int statusCode, string responseMessage)
            : base(MESSAGE_KEY, new object[] { serviceProviderURI.ToString(), statusCode, responseMessage })
        {
            this.responseMessage = responseMessage;
            this.serviceProviderURI = serviceProviderURI;
            this.statusCode = statusCode;
        }

        public string GetResponseMessage()
        {
            return responseMessage;
        }

        public Uri GetServiceProviderURI()
        {
            return serviceProviderURI;
        }

        public int GetStatusCode()
        {
            return statusCode;
        }
    }
}