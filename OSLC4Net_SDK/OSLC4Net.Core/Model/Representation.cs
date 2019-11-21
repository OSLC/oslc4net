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

namespace OSLC4Net.Core.Model
{
    #region

    using System;

    #endregion

    /// <summary>
    /// OSLC Representation attribute
    /// </summary>
    /// <remarks>see http://open-services.net/bin/view/Main/OslcCoreSpecification#OSLC_Defined_Resources</remarks>
    public enum Representation
    {
        [URI(OslcConstants.OSLC_CORE_NAMESPACE + "Reference")]
        Reference,

        [URI(OslcConstants.OSLC_CORE_NAMESPACE + "Inline")]
        Inline,

        [URI("")]
        Unknown
    }

    public static class RepresentationExtension
    {
        public static Representation FromString(string value)
        {
            foreach (Representation representation in Enum.GetValues(typeof(Representation)))
            {
                var uri = ToString(representation);

                if (uri.Equals(value))
                {
                    return representation;
                }
            }

            throw new ArgumentException();
        }

        public static Representation FromURI(URI uri)
        {
            return FromString(uri.ToString());
        }

        public static string ToString(Representation representation)
        {
            var attributes = (URI[])representation.GetType().GetField(representation.ToString())
                .GetCustomAttributes(typeof(URI), false);

            return attributes.Length > 0 ? attributes[0].uri : string.Empty;
        }
    }
}