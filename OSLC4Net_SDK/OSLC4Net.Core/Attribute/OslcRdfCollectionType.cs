/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
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

using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Attribute;

[AttributeUsage(AttributeTargets.Method)
]
public class OslcRdfCollectionType : System.Attribute
{
    /**
     * Prefix for the namespace.
     */
    public string CollectionType { get; } = "List";

    /**
     * Namespace URI.
     */
    public string NamespaceUri { get; } = OslcConstants.RDF_NAMESPACE;

    public OslcRdfCollectionType(string namespaceURI, string collectionType)
    {
        NamespaceUri = namespaceURI;
        CollectionType = collectionType;
    }
}
