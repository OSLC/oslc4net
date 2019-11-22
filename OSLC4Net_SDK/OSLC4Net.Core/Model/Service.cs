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
    /// OSLC Service attribute
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(title = "OSLC Service Resource Shape", describes = new string[] { OslcConstants.TYPE_SERVICE })]
    public class Service : AbstractResource
    {
        private IList<Dialog> creationDialogs = new List<Dialog>();

        private IList<CreationFactory> creationFactories = new List<CreationFactory>();

        private Uri domain;

        private IList<QueryCapability> queryCapabilities = new List<QueryCapability>();

        private IList<Dialog> selectionDialogs = new List<Dialog>();

        public Service()
            : base()
        {
        }

        public Service(Uri domain)
            : this()
        {
            this.domain = domain;
        }

        public void AddCreationDialog(Dialog dialog)
        {
            creationDialogs.Add(dialog);
        }

        public void AddCreationFactory(CreationFactory creationFactory)
        {
            creationFactories.Add(creationFactory);
        }

        public void AddQueryCapability(QueryCapability queryCapability)
        {
            queryCapabilities.Add(queryCapability);
        }

        public void AddSelectionDialog(Dialog dialog)
        {
            selectionDialogs.Add(dialog);
        }

        [OslcDescription("Enables clients to create a resource via UI")]
        [OslcName("creationDialog")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "creationDialog")]
        [OslcRange(OslcConstants.TYPE_DIALOG)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Creation Dialogs")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_DIALOG)]
        [OslcValueType(ValueType.LocalResource)]
        public Dialog[] GetCreationDialogs()
        {
            return creationDialogs.ToArray();
        }

        [OslcDescription("Enables clients to create new resources")]
        [OslcName("creationFactory")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "creationFactory")]
        [OslcRange(OslcConstants.TYPE_CREATION_FACTORY)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Creation Factories")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_CREATION_FACTORY)]
        [OslcValueType(ValueType.LocalResource)]
        public CreationFactory[] GetCreationFactories()
        {
            return creationFactories.ToArray();
        }

        [OslcDescription("Namespace Uri of the OSLC domain specification that is implemented by this service")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "domain")]
        [OslcReadOnly]
        [OslcTitle("Domain")]
        public Uri GetDomain()
        {
            return domain;
        }

        [OslcDescription("Enables clients query across a collection of resources")]
        [OslcName("queryCapability")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "queryCapability")]
        [OslcRange(OslcConstants.TYPE_QUERY_CAPABILITY)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Query Capabilities")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_QUERY_CAPABILITY)]
        [OslcValueType(ValueType.LocalResource)]
        public QueryCapability[] GetQueryCapabilities()
        {
            return queryCapabilities.ToArray();
        }

        [OslcDescription("Enables clients to select a resource via UI")]
        [OslcName("selectionDialog")]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "selectionDialog")]
        [OslcRange(OslcConstants.TYPE_DIALOG)]
        [OslcReadOnly]
        [OslcRepresentation(Representation.Inline)]
        [OslcTitle("Selection Dialogs")]
        [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_DIALOG)]
        [OslcValueType(ValueType.LocalResource)]
        public Dialog[] GetSelectionDialogs()
        {
            return selectionDialogs.ToArray();
        }

        public void SetCreationDialogs(Dialog[] creationDialogs)
        {
            this.creationDialogs.Clear();
            if (creationDialogs != null)
            {
                this.creationDialogs.AddAll(creationDialogs);
            }
        }

        public void SetCreationFactories(CreationFactory[] creationFactories)
        {
            this.creationFactories.Clear();
            if (creationFactories != null)
            {
                this.creationFactories.AddAll(creationFactories);
            }
        }

        public void SetDomain(Uri domain)
        {
            this.domain = domain;
        }

        public void SetQueryCapabilities(QueryCapability[] queryCapabilities)
        {
            this.queryCapabilities.Clear();
            if (queryCapabilities != null)
            {
                this.queryCapabilities.AddAll(queryCapabilities);
            }
        }

        public void SetSelectionDialogs(Dialog[] selectionDialogs)
        {
            this.selectionDialogs.Clear();
            if (selectionDialogs != null)
            {
                this.selectionDialogs.AddAll(selectionDialogs);
            }
        }
    }
}