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
            _oslcClient = oslcClient;
            _capabilityUrl = capabilityUrl;
            _pageSize = (pageSize < 1) ? 0 : pageSize;

            //make a local copy of any query parameters
            if (oslcQueryParams != null)
            {
                _where = oslcQueryParams.GetWhere();
                _select = oslcQueryParams.GetSelect();
                _orderBy = oslcQueryParams.GetOrderBy();
                _searchTerms = oslcQueryParams.GetSearchTerms();
                _prefix = oslcQueryParams.GetPrefix();
            }
            else
            {
                _where = _select = _orderBy = _searchTerms = _prefix = null;
            }

            _uriBuilder = new UriBuilder(capabilityUrl);
            ApplyPagination();
            ApplyOslcQueryParams();
            _queryUrl = GetQueryUrl();
        }

        internal OslcQuery(OslcQueryResult previousResult) :
            this(previousResult.GetQuery(), previousResult.GetNextPageUrl())
        {
        }

        private OslcQuery(OslcQuery previousQuery, string nextPageUrl) :
            this(previousQuery._oslcClient, previousQuery._capabilityUrl, previousQuery._pageSize)
        {
            _queryUrl = nextPageUrl;
            _uriBuilder = new UriBuilder(nextPageUrl);
        }

        /**
         * @return the number of entries to return for each page,
         * 		if zero, the remote system's (or full query's) default is used
         */

        public int GetPageSize()
        {
            return _pageSize;
        }

        /**
         * @return the base query capability URL
         */

        public string GetCapabilityUrl()
        {
            return _capabilityUrl;
        }

        /**
         * @return the complete query URL
         */

        public string GetQueryUrl()
        {
            if (_queryUrl == null)
            {
                _queryUrl = _uriBuilder.ToString();
            }
            return _queryUrl;
        }

        public OslcQueryResult Submit()
        {
            return new OslcQueryResult(this, GetResponse());
        }

        internal HttpResponseMessage GetResponse()
        {
            return _oslcClient.GetResource(GetQueryUrl(), OSLCConstants.CT_RDF);
        }

        private void ApplyPagination()
        {
            if (_pageSize > 0)
            {
                QueryParam("oslc.paging", "true");
                QueryParam("oslc.pageSize", _pageSize.ToString());
            }
        }

        private void ApplyOslcQueryParams()
        {
            if (_where != null && _where.Length != 0)
            {
                QueryParam("oslc.where", _where);
            }
            if (_select != null && _select.Length != 0)
            {
                QueryParam("oslc.select", _select);
            }
            if (_orderBy != null && _orderBy.Length != 0)
            {
                QueryParam("oslc.orderBy", _orderBy);
            }
            if (_searchTerms != null && _searchTerms.Length != 0)
            {
                QueryParam("oslc.searchTerms", _searchTerms);
            }
            if (_prefix != null && _prefix.Length != 0)
            {
                QueryParam("oslc.prefix", _prefix);
            }
        }

        private void QueryParam(string name, string value)
        {
            string content = name + '=' + value;

            if (_uriBuilder.Query != null && _uriBuilder.Query.Length > 1)
            {
                _uriBuilder.Query = _uriBuilder.Query.Substring(1) + '&' + content;
            }
            else
            {
                _uriBuilder.Query = content;
            }
        }
    }
}
