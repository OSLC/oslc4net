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
 *
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System;

using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)
    ]
    public class OslcRdfCollectionType : System.Attribute
    {
        /**
         * Namespace URI.
         */
        public readonly string namespaceURI = OslcConstants.RDF_NAMESPACE;

        /**
         * Prefix for the namespace.
         */
        public readonly string collectionType = "List";

        public OslcRdfCollectionType(string namespaceURI, string collectionType)
        {
            this.namespaceURI = namespaceURI;
            this.collectionType = collectionType;
        }
    }
}