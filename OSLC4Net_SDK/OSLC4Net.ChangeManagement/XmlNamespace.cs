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

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.ChangeManagement
{
    public static class XmlNamespace
    {
        private static readonly OslcNamespaceDefinition[] namespaces = new OslcNamespaceDefinition[]
        {
            new OslcNamespaceDefinition(prefix: OslcConstants.DCTERMS_NAMESPACE_PREFIX,             namespaceURI: OslcConstants.DCTERMS_NAMESPACE),
            new OslcNamespaceDefinition(prefix: OslcConstants.OSLC_CORE_NAMESPACE_PREFIX,           namespaceURI: OslcConstants.OSLC_CORE_NAMESPACE),
            new OslcNamespaceDefinition(prefix: OslcConstants.OSLC_DATA_NAMESPACE_PREFIX,           namespaceURI: OslcConstants.OSLC_DATA_NAMESPACE),
            new OslcNamespaceDefinition(prefix: OslcConstants.RDF_NAMESPACE_PREFIX,                 namespaceURI: OslcConstants.RDF_NAMESPACE),
            new OslcNamespaceDefinition(prefix: OslcConstants.RDFS_NAMESPACE_PREFIX,                namespaceURI: OslcConstants.RDFS_NAMESPACE),
            new OslcNamespaceDefinition(prefix: Constants.CHANGE_MANAGEMENT_NAMESPACE_PREFIX,       namespaceURI: Constants.CHANGE_MANAGEMENT_NAMESPACE),
            new OslcNamespaceDefinition(prefix: Constants.FOAF_NAMESPACE_PREFIX,                    namespaceURI: Constants.FOAF_NAMESPACE),
            new OslcNamespaceDefinition(prefix: Constants.QUALITY_MANAGEMENT_PREFIX,                namespaceURI: Constants.QUALITY_MANAGEMENT_NAMESPACE),
            new OslcNamespaceDefinition(prefix: Constants.REQUIREMENTS_MANAGEMENT_PREFIX,           namespaceURI: Constants.REQUIREMENTS_MANAGEMENT_NAMESPACE),
            new OslcNamespaceDefinition(prefix: Constants.SOFTWARE_CONFIGURATION_MANAGEMENT_PREFIX, namespaceURI: Constants.SOFTWARE_CONFIGURATION_MANAGEMENT_NAMESPACE)
        };

        public static OslcNamespaceDefinition[] GetNamespaces()
        {
            return namespaces;
        }
    }
}