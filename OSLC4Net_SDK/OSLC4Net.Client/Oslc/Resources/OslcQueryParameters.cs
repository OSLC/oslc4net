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
using log4net;

namespace OSLC4Net.Client.Oslc.Resources
{
    /// <summary>
    /// A container for OSLC Query parameters which can be associated with an OslcQuery object.
    /// </summary>
    public class OslcQueryParameters
    {
	    private string where;
	    private string select;
	    private string searchTerms;
	    private string orderBy;
	    private string prefix;
	
        private static ILog logger = LogManager.GetLogger(typeof(OslcQuery));

	    public OslcQueryParameters()
	    {		
	    }

        /// <summary>
        /// Initialize an OSLC Parameter using the supplied terms
        /// </summary>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <param name="searchTerms"></param>
        /// <param name="orderBy"></param>
        /// <param name="prefix"></param>
	    public OslcQueryParameters (string where, string select, string searchTerms, string orderBy, string prefix) {
		    this.where       = where;
		    this.select      = select;
		    this.searchTerms = searchTerms;
		    this.orderBy     = orderBy;
		    this.prefix      = prefix;
	    }

	    public string GetWhere() {
		    return where;
	    }

	    public void SetWhere(string where) {
		    this.where = encodeQueryParams(where);
	    }

	    public string GetSelect() {
		    return select;
	    }

	    public void SetSelect(string select) {
		    this.select = encodeQueryParams(select);
	    }

	    public string GetSearchTerms() {
		    return searchTerms;
	    }

	    public void SetSearchTerms(string searchTerms) {
		    this.searchTerms = encodeQueryParams(searchTerms);
	    }

	    public string GetOrderBy() {
		    return orderBy;
	    }

	    public void SetOrderBy(string orderBy) {
		    this.orderBy = encodeQueryParams(orderBy);
	    }

	    public string GetPrefix() {
		    return prefix;
	    }

	    public void SetPrefix(string prefix) {
		    this.prefix = encodeQueryParams(prefix);
	    }
	
	    private string encodeQueryParams(string oslcQueryParam) {

            string encodedQueryParms = null;
		    try {
			    encodedQueryParms = Uri.EscapeUriString(oslcQueryParam);
		    } catch (Exception e) {
			    //Should not occur
			    logger.Error("Could not UTF-8 encode query parameters: " + oslcQueryParam, e);
		    } 
		
            // XXX - CLM is picky about encoding and native .NET URL encoder doesn't
            // encode these extra substitutions
		    return encodedQueryParms.Replace("#", "%23").Replace("/", "%2F").Replace(":", "%3A").Replace("=", "%3D");
	    }
    }
}
