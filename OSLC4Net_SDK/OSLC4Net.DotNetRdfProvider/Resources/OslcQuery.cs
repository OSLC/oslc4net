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
	
	    private readonly String capabilityUrl;
	
	    private String queryUrl;
	
	    private readonly int pageSize;

        private readonly UriBuilder uriBuilder;
	
	    //query parameters
	    private readonly String where;
	    private readonly String select;
	    private readonly String orderBy;
	    private readonly String searchTerms;
	    private readonly String prefix;

        /// <summary>
        /// Create an OSLC query that uses the remote system's default page size.
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">the URL that is the base </param>
	    public OslcQuery(OslcClient oslcClient, String capabilityUrl) : 
		    this(oslcClient, capabilityUrl, 0)
        {
	    }

        /// <summary>
        /// Create an OSLC query with query parameters that uses the default page size
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">capabilityUrl capabilityUrl the URL that is the base</param>
        /// <param name="oslcQueryParams">an OslcQueryParameters object</param>
	    public OslcQuery(OslcClient oslcClient, String capabilityUrl, OslcQueryParameters oslcQueryParams) :
		    this(oslcClient, capabilityUrl, 0, oslcQueryParams)
        {
	    }

        /// <summary>
        /// Create an OSLC query that uses the given page size
        /// </summary>
        /// <param name="oslcClient">the authenticated OSLC client</param>
        /// <param name="capabilityUrl">the URL that is the base</param>
        /// <param name="pageSize">the number of results to include on each page (OslcQueryResult)</param>
	    public OslcQuery(OslcClient oslcClient, String capabilityUrl, int pageSize) :
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
	    public OslcQuery(OslcClient oslcClient, String capabilityUrl,
					     int pageSize, OslcQueryParameters oslcQueryParams)
        {
		    this.oslcClient = oslcClient;
		    this.capabilityUrl = capabilityUrl;
		    this.pageSize = (pageSize < 1) ? 0 : pageSize;
		
		    //make a local copy of any query parameters
		    if (oslcQueryParams != null)
		    {
			    this.where = oslcQueryParams.GetWhere();
			    this.select = oslcQueryParams.GetSelect();
			    this.orderBy = oslcQueryParams.GetOrderBy();
			    this.searchTerms = oslcQueryParams.GetSearchTerms();
			    this.prefix = oslcQueryParams.GetPrefix();
		    } else {
			    this.where = this.select = this.orderBy = this.searchTerms = this.prefix = null;
		    }

            this.uriBuilder = new UriBuilder(capabilityUrl);
            ApplyPagination();
            ApplyOslcQueryParams();
            this.queryUrl = this.GetQueryUrl();				
	    }
	
	    internal OslcQuery(OslcQueryResult previousResult) :
		    this(previousResult.GetQuery(), previousResult.GetNextPageUrl())
        {
	    }
	
	    private OslcQuery(OslcQuery previousQuery, String nextPageUrl) :
		    this(previousQuery.oslcClient, previousQuery.capabilityUrl, previousQuery.pageSize)
        {
		    this.queryUrl = nextPageUrl;
		    this.uriBuilder = new UriBuilder(nextPageUrl);
	    }
	
	    private void ApplyPagination() {
		    if (pageSize > 0) {
			    QueryParam("oslc.paging", "true");
			    QueryParam("oslc.pageSize", pageSize.ToString());
		    }
	    }
	
	    private void ApplyOslcQueryParams() {
		    if (this.where != null && this.where.Length != 0) {
			    QueryParam("oslc.where", this.where);
		    }
            if (this.select != null && this.select.Length != 0)
            {
			    QueryParam("oslc.select", this.select);
		    }
		    if (this.orderBy != null && this.orderBy.Length != 0) {
			    QueryParam("oslc.orderBy", this.orderBy);
		    }
            if (this.searchTerms != null && this.searchTerms.Length != 0)
            {
			    QueryParam("oslc.searchTerms", this.searchTerms);
		    }
            if (this.prefix != null && this.prefix.Length != 0)
            {
			    QueryParam("oslc.prefix", this.prefix);
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
	    public String GetCapabilityUrl() {
		    return capabilityUrl;
	    }
	
	    /**
	     * @return the complete query URL
	     */
	    public String GetQueryUrl() {
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
            String content = name + '=' + value;

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
