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

    using log4net;

    /// <summary>
    /// A container for OSLC Query parameters which can be associated with an OslcQuery object.
    /// </summary>
    public class OslcQueryParameters
    {
        private static ILog logger = LogManager.GetLogger(typeof(OslcQuery));

        private string _where;
        private string _select;
        private string _searchTerms;
        private string _orderBy;
        private string _prefix;

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
        public OslcQueryParameters(string where, string select, string searchTerms, string orderBy, string prefix)
        {
            _where = where;
            _select = select;
            _searchTerms = searchTerms;
            _orderBy = orderBy;
            _prefix = prefix;
        }

        public string GetWhere()
        {
            return _where;
        }

        public void SetWhere(string where)
        {
            _where = EncodeQueryParams(where);
        }

        public string GetSelect()
        {
            return _select;
        }

        public void SetSelect(string select)
        {
            _select = EncodeQueryParams(select);
        }

        public string GetSearchTerms()
        {
            return _searchTerms;
        }

        public void SetSearchTerms(string searchTerms)
        {
            _searchTerms = EncodeQueryParams(searchTerms);
        }

        public string GetOrderBy()
        {
            return _orderBy;
        }

        public void SetOrderBy(string orderBy)
        {
            _orderBy = EncodeQueryParams(orderBy);
        }

        public string GetPrefix()
        {
            return _prefix;
        }

        public void SetPrefix(string prefix)
        {
            _prefix = EncodeQueryParams(prefix);
        }

        private string EncodeQueryParams(string oslcQueryParam)
        {
            string encodedQueryParms = null;
            try
            {
                encodedQueryParms = Uri.EscapeUriString(oslcQueryParam);
            }
            catch (Exception e)
            {
                // Should not occur
                logger.Error("Could not UTF-8 encode query parameters: " + oslcQueryParam, e);
            }

            // XXX - CLM is picky about encoding and native .NET URL encoder doesn't
            // encode these extra substitutions
            return encodedQueryParms.Replace("#", "%23").Replace("/", "%2F").Replace(":", "%3A").Replace("=", "%3D");
        }
    }
}
