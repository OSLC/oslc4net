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
    using System.Collections.Generic;
    using System.Linq;

    using OSLC4Net.Core.Attribute;

    #endregion

    /// <summary>
    /// OSLC QueryCapability resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(
        title = "OSLC Query Capability Resource Shape",
        describes = new string[] { OslcConstants.TYPE_QUERY_CAPABILITY })]
    public class QueryCapability : AbstractResource
    {
        private readonly SortedSet<Uri> _resourceTypes = new SortedUriSet();

        private readonly SortedSet<Uri> _usages = new SortedUriSet();

        private string _label;

        private Uri _queryBase;

        private Uri _resourceShape;

        private string _title;

        /// <summary>
        /// 
        /// </summary>
        public QueryCapability()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="queryBase"></param>
        public QueryCapability(string title, Uri queryBase)
            : this()
        {
            _title = title;
            _queryBase = queryBase;
        }

        public void AddResourceType(Uri resourceType)
        {
            _resourceTypes.Add(resourceType);
        }

        public void AddUsage(Uri usage)
        {
            _usages.Add(usage);
        }

        [OslcDescription("Very short label for use in menu items")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "label")]
        [OslcReadOnly]
        [OslcTitle("Label")]
        public string GetLabel()
        {
            return _label;
        }

        [OslcDescription(
            "The base Uri to use for queries. Queries are invoked via HTTP GET on a query Uri formed by appending a key=value pair to the base Uri, as described in Query Capabilities section")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "queryBase")]
        [OslcReadOnly]
        [OslcTitle("Query Base")]
        public Uri GetQueryBase()
        {
            return _queryBase;
        }

        [OslcDescription("The Query Capability SHOULD provide a Resource Shape that describes the query base Uri")]
        [OslcName("resourceShape")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "resourceShape")]
        [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
        [OslcReadOnly]
        [OslcTitle("Resource Shape")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_RESOURCE_SHAPE)]
        public Uri GetResourceShape()
        {
            return _resourceShape;
        }

        [OslcDescription(
            "The expected resource type Uri that will be returned with this query capability. These would be the URIs found in the result resource's rdf:type property")]
        [OslcName("resourceType")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "resourceType")]
        [OslcReadOnly]
        [OslcTitle("Resource Types")]
        public Uri[] GetResourceTypes()
        {
            return _resourceTypes.ToArray();
        }

        [OslcDescription("Title string that could be used for display")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
        [OslcReadOnly]
        [OslcTitle("Title")]
        [OslcValueType(ValueType.XMLLiteral)]
        public string GetTitle()
        {
            return _title;
        }

        [OslcDescription(
            "An identifier Uri for the domain specified usage of this query capability. If a service provides multiple query capabilities, it may designate the primary or default one that should be used with a property value of http://open-services/ns/core#default")]
        [OslcName("usage")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "usage")]
        [OslcReadOnly]
        [OslcTitle("Usages")]
        public Uri[] GetUsages()
        {
            return _usages.ToArray();
        }

        public void SetLabel(string label)
        {
            _label = label;
        }

        public void SetQueryBase(Uri queryBase)
        {
            _queryBase = queryBase;
        }

        public void SetResourceShape(Uri resourceShape)
        {
            _resourceShape = resourceShape;
        }

        public void SetResourceTypes(Uri[] resourceTypes)
        {
            _resourceTypes.Clear();
            if (resourceTypes != null)
            {
                _resourceTypes.AddAll(resourceTypes);
            }
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public void SetUsages(Uri[] usages)
        {
            _usages.Clear();
            if (usages != null)
            {
                _usages.AddAll(usages);
            }
        }
    }
}