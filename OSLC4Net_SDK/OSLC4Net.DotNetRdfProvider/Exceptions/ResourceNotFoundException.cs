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

    /// <summary>
    /// Exceptions indicating a Jazz authentication or credentials problem
    /// </summary>
    public class ResourceNotFoundException : OslcClientApplicationException
    {
        private const string MESSAGE_KEY = "ResourceNotFoundException";

        private readonly string _resource;
        private readonly string _value;

        public ResourceNotFoundException(string resource, string value) :
            base(MESSAGE_KEY, new object[] { resource, value })
        {
            _resource = resource;
            _value = value;
        }

        public string GetResource()
        {
            return _resource;
        }

        public string GetValue()
        {
            return _value;
        }
    }
}
