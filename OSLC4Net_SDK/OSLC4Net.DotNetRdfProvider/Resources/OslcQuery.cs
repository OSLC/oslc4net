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
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

namespace OSLC4Net.Core.Resources
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// Represents an OSLC query (HTTP GET) request to be made of a remote system.
    ///
    /// Immutable.
    /// </summary>
    public class OslcQuery
    {
        private readonly OslcClient _oslcClient;

        private readonly string _capabilityUrl;

        private readonly int _pageSize;

        private readonly UriBuilder _uriBuilder;

        //query parameters
        private readonly string _where;

        private readonly string _select;
        private readonly string _orderBy;
        private readonly string _searchTerms;
        private readonly string _prefix;

        private string _queryUrl;

        /// <summary>
        /// Create an OSLC query that uses the remote system's default page size.
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">the URL that is the base </param>
        public OslcQuery(OslcClient oslcClient, string capabilityUrl) :
            this(oslcClient, capabilityUrl, 0)
        {
        }

        /// <summary>
        /// Create an OSLC query with query parameters that uses the default page size
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">capabilityUrl capabilityUrl the URL that is the base</param>
        /// <param name="oslcQueryParams">an OslcQueryParameters object</param>
        public OslcQuery(OslcClient oslcClient, string capabilityUrl, OslcQueryParameters oslcQueryParams) :
            this(oslcClient, capabilityUrl, 0, oslcQueryParams)
        {
        }

        /// <summary>
        /// Create an OSLC query that uses the given page size
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">the URL that is the base</param>
        /// <param name="pageSize">the number of results to include on each page (OslcQueryResult)</param>
        public OslcQuery(OslcClient oslcClient, string capabilityUrl, int pageSize) :
            this(oslcClient, capabilityUrl, pageSize, null)
        {
        }

        /// <summary>
        /// Create an OSLC query that uses OSLC query parameters and the given page size
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">the URL that is the base</param>
        /// <param name="pageSize">the number of results to include on each page (OslcQueryResult)</param>
        /// <param name="oslcQueryParams">an OslcQueryParameters object</param>
        public OslcQuery(OslcClient oslcClient, string capabilityUrl,
                         int pageSize, OslcQueryParameters oslcQueryParams)
        {
            this._oslcClient = oslcClient;
            this._capabilityUrl = capabilityUrl;
            this._pageSize = (pageSize < 1) ? 0 : pageSize;

            //make a local copy of any query parameters
            if (oslcQueryParams != null)
            {
                this._where = oslcQueryParams.GetWhere();
                this._select = oslcQueryParams.GetSelect();
                this._orderBy = oslcQueryParams.GetOrderBy();
                this._searchTerms = oslcQueryParams.GetSearchTerms();
                this._prefix = oslcQueryParams.GetPrefix();
            }
            else
            {
                this._where = this._select = this._orderBy = this._searchTerms = this._prefix = null;
            }

            this._uriBuilder = new UriBuilder(capabilityUrl);
            this.ApplyPagination();
            this.ApplyOslcQueryParams();
            this._queryUrl = this.GetQueryUrl();
        }

        internal OslcQuery(OslcQueryResult previousResult) :
            this(previousResult.GetQuery(), previousResult.GetNextPageUrl())
        {
        }

        private OslcQuery(OslcQuery previousQuery, string nextPageUrl) :
            this(previousQuery._oslcClient, previousQuery._capabilityUrl, previousQuery._pageSize)
        {
            this._queryUrl = nextPageUrl;
            this._uriBuilder = new UriBuilder(nextPageUrl);
        }

        /**
         * @return the number of entries to return for each page,
         * 		if zero, the remote system's (or full query's) default is used
         */

        public int GetPageSize()
        {
            return this._pageSize;
        }

        /**
         * @return the base query capability URL
         */

        public string GetCapabilityUrl()
        {
            return this._capabilityUrl;
        }

        /**
         * @return the complete query URL
         */

        public string GetQueryUrl()
        {
            if (this._queryUrl == null)
            {
                this._queryUrl = this._uriBuilder.ToString();
            }
            return this._queryUrl;
        }

        public OslcQueryResult Submit()
        {
            return new OslcQueryResult(this, this.GetResponse());
        }

        internal HttpResponseMessage GetResponse()
        {
            return this._oslcClient.GetResource(this.GetQueryUrl(), OSLCConstants.CT_RDF);
        }

        private void ApplyPagination()
        {
            if (this._pageSize > 0)
            {
                QueryParam("oslc.paging", "true");
                QueryParam("oslc.pageSize", this._pageSize.ToString());
            }
        }

        private void ApplyOslcQueryParams()
        {
            if (this._where != null && this._where.Length != 0)
            {
                QueryParam("oslc.where", this._where);
            }
            if (this._select != null && this._select.Length != 0)
            {
                QueryParam("oslc.select", this._select);
            }
            if (this._orderBy != null && this._orderBy.Length != 0)
            {
                QueryParam("oslc.orderBy", this._orderBy);
            }
            if (this._searchTerms != null && this._searchTerms.Length != 0)
            {
                QueryParam("oslc.searchTerms", this._searchTerms);
            }
            if (this._prefix != null && this._prefix.Length != 0)
            {
                QueryParam("oslc.prefix", this._prefix);
            }
        }

        private void QueryParam(string name, string value)
        {
            string content = name + '=' + value;

            if (this._uriBuilder.Query != null && this._uriBuilder.Query.Length > 1)
            {
                this._uriBuilder.Query = this._uriBuilder.Query.Substring(1) + '&' + content;
            }
            else
            {
                this._uriBuilder.Query = content;
            }
        }
    }
}
