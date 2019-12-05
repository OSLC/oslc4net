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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSLC4Net.Core.Model;
using System.Net.Http;

namespace OSLC4Net.Client.Oslc.Resources
{
    /// <summary>
    /// Represents an OSLC query (HTTP GET) request to be made of a remote system.
    /// 
    /// Immutable.
    /// </summary>
    public class OslcQuery
    {
	    private readonly OslcClient oslcClient;
	
	    private readonly string capabilityUrl;
	
	    private string queryUrl;
	
	    private readonly int pageSize;

        private readonly UriBuilder uriBuilder;
	
	    //query parameters
	    private readonly string where;
	    private readonly string select;
	    private readonly string orderBy;
	    private readonly string searchTerms;
	    private readonly string prefix;

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
		    this.oslcClient = oslcClient;
		    this.capabilityUrl = capabilityUrl;
		    this.pageSize = (pageSize < 1) ? 0 : pageSize;
		
		    //make a local copy of any query parameters
		    if (oslcQueryParams != null)
		    {
			    where = oslcQueryParams.GetWhere();
			    select = oslcQueryParams.GetSelect();
			    orderBy = oslcQueryParams.GetOrderBy();
			    searchTerms = oslcQueryParams.GetSearchTerms();
			    prefix = oslcQueryParams.GetPrefix();
		    } else {
			    where = select = orderBy = searchTerms = prefix = null;
		    }

            uriBuilder = new UriBuilder(capabilityUrl);
            ApplyPagination();
            ApplyOslcQueryParams();
            queryUrl = GetQueryUrl();				
	    }
	
	    internal OslcQuery(OslcQueryResult previousResult) :
		    this(previousResult.GetQuery(), previousResult.GetNextPageUrl())
        {
	    }
	
	    private OslcQuery(OslcQuery previousQuery, string nextPageUrl) :
		    this(previousQuery.oslcClient, previousQuery.capabilityUrl, previousQuery.pageSize)
        {
		    queryUrl = nextPageUrl;
		    uriBuilder = new UriBuilder(nextPageUrl);
	    }
	
	    private void ApplyPagination() {
		    if (pageSize > 0) {
			    QueryParam("oslc.paging", "true");
			    QueryParam("oslc.pageSize", pageSize.ToString());
		    }
	    }
	
	    private void ApplyOslcQueryParams() {
		    if (where != null && where.Length != 0) {
			    QueryParam("oslc.where", where);
		    }
            if (select != null && select.Length != 0)
            {
			    QueryParam("oslc.select", select);
		    }
		    if (orderBy != null && orderBy.Length != 0) {
			    QueryParam("oslc.orderBy", orderBy);
		    }
            if (searchTerms != null && searchTerms.Length != 0)
            {
			    QueryParam("oslc.searchTerms", searchTerms);
		    }
            if (prefix != null && prefix.Length != 0)
            {
			    QueryParam("oslc.prefix", prefix);
		    }
	    }

	    /**
	     * @return the number of entries to return for each page, 
	     * 		if zero, the remote system's (or full query's) default is used
	     */
	    public int GetPageSize() {
		    return pageSize;
	    }

	    /**
	     * @return the base query capability URL
	     */
	    public string GetCapabilityUrl() {
		    return capabilityUrl;
	    }
	
	    /**
	     * @return the complete query URL
	     */
	    public string GetQueryUrl() {
		    if (queryUrl == null) {
			    queryUrl = uriBuilder.ToString();
		    }
		    return queryUrl;
	    }
	
	    public OslcQueryResult Submit() {
		    return new OslcQueryResult(this, GetResponse());
	    }
	
	    internal HttpResponseMessage GetResponse() {
            return oslcClient.GetResource(GetQueryUrl(), OSLCConstants.CT_RDF);
	    }

        private void QueryParam(string name, string value)
        {
            string content = name + '=' + value;

            if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
            {
                uriBuilder.Query = uriBuilder.Query.Substring(1) + '&' + content;
            }
            else
            {
                uriBuilder.Query = content;
            }
        }
    }
}
