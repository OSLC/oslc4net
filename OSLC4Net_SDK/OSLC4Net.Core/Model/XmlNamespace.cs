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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    public static class XmlNamespace
    {
        private static readonly OslcNamespaceDefinition[] namespaces = new OslcNamespaceDefinition[]
        {
                 new OslcNamespaceDefinition(prefix: OslcConstants.DCTERMS_NAMESPACE_PREFIX, namespaceURI: OslcConstants.DCTERMS_NAMESPACE),
                 new OslcNamespaceDefinition(prefix: OslcConstants.OSLC_CORE_NAMESPACE_PREFIX, namespaceURI: OslcConstants.OSLC_CORE_NAMESPACE),
                 new OslcNamespaceDefinition(prefix: OslcConstants.RDF_NAMESPACE_PREFIX, namespaceURI: OslcConstants.RDF_NAMESPACE),
                 new OslcNamespaceDefinition(prefix: OslcConstants.RDFS_NAMESPACE_PREFIX, namespaceURI: OslcConstants.RDFS_NAMESPACE)
        };

        public static OslcNamespaceDefinition[] GetNamespaces() { return namespaces; }
    }
}
