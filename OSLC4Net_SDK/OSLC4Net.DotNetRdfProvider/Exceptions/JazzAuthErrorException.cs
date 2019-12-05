/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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
    using System.Net;

    /// <summary>
    /// Exceptions indicating a Jazz authentication or credentials problem
    /// </summary>
    public class JazzAuthErrorException : OslcClientApplicationException
    {
        private const string MESSAGE_KEY = "JazzAuthErrorException";

        private readonly HttpStatusCode _status;
        private readonly string _jazzUrl;


        public JazzAuthErrorException(HttpStatusCode status, string jazzUrl) :
            base(MESSAGE_KEY, new object[] { status.ToString(), jazzUrl })
        {
            _status = status;
            _jazzUrl = jazzUrl;
        }

        public HttpStatusCode GetStatus()
        {
            return _status;
        }

        public string GetJazzUrl()
        {
            return _jazzUrl;
        }
    }
}
