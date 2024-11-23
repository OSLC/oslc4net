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
 *
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

namespace OSLC4Net.Core.Attribute;

/// <summary>
/// The definition of an OSLC namespace attribute
/// </summary>
[Serializable]
public class OslcNamespaceDefinition
{
    /**
     * Namespace URI.
     */
    public readonly string namespaceURI;

    /**
     * Prefix for the namespace.
     */
    public readonly string prefix;

    public OslcNamespaceDefinition(
        string namespaceURI,
        string prefix
    )
    {
        this.namespaceURI = namespaceURI;
        this.prefix = prefix;
    }
}
