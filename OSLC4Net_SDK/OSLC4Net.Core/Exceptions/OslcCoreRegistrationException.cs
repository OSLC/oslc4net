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
    using OSLC4Net.Core.Model;

    /// <summary>
    /// Exception thrown for a service provider registration failure
    /// </summary>
    public class OslcCoreRegistrationException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "RegistrationException";

        private string responseMessage;

        private ServiceProvider serviceProvider;

        private int statusCode;

        public OslcCoreRegistrationException(ServiceProvider serviceProvider, int statusCode, string responseMessage)
            : base(MESSAGE_KEY, new object[] { serviceProvider.GetTitle(), statusCode, responseMessage })
        {
            this.responseMessage = responseMessage;
            this.serviceProvider = serviceProvider;
            this.statusCode = statusCode;
        }

        public string GetResponseMessage()
        {
            return responseMessage;
        }

        public ServiceProvider GetServiceProvider()
        {
            return serviceProvider;
        }

        public int GetStatusCode()
        {
            return statusCode;
        }
    }
}