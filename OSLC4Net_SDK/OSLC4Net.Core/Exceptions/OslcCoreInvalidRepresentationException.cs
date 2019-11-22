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
    using System.Reflection;

    using OSLC4Net.Core.Model;

    public class OslcCoreInvalidRepresentationException : OslcCoreApplicationException
    {
        private static readonly string MESSAGE_KEY = "InvalidRepresentationException";

        private MethodInfo _method;

        private Representation _representation;

        private Type _resourceType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="method"></param>
        /// <param name="representation"></param>
        public OslcCoreInvalidRepresentationException(
            Type resourceType,
            MethodInfo method,
            Representation representation)
            : base(
                MESSAGE_KEY,
                new object[] { resourceType.Name, method.Name, RepresentationExtension.ToString(representation) })
        {
            _method = method;
            _representation = representation;
            _resourceType = resourceType;
        }

        public MethodInfo GetMethod()
        {
            return _method;
        }

        public Representation GetRepresentation()
        {
            return _representation;
        }

        public Type GetResourceType()
        {
            return _resourceType;
        }
    }
}