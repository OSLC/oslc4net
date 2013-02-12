using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.StockQuoteSample.Models
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
        new OslcNamespaceDefinition(prefix: Constants.STOCK_QUOTE_NAMESPACE_PREFIX,             namespaceURI: Constants.STOCK_QUOTE_NAMESPACE),

    };

        public static OslcNamespaceDefinition[] GetNamespaces() { return namespaces; }
    }
}
