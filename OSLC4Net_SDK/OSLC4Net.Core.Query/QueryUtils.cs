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
using System.IO;
using System.Linq;
using System.Text;

using ParseException = OSLC4Net.Core.Query.ParseException;

using OSLC4Net.Core.Query.Impl;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query
{
    public class QueryUtils
    {
        /// <summary>
        ///  Parse a oslc.prefix clause into a map between prefixes
        /// and corresponding URIs 
        /// <p><b>Note</b>: {@link Object#toString()} of result has been overridden to
        /// return input expression.
        /// </summary>
        /// <param name="prefixExpression">the oslc.prefix expression</param>
        /// <returns>the prefix map</returns>
        public static IDictionary<String, String>
        ParsePrefixes(
            String prefixExpression
        )
        {
            if (prefixExpression == null) {
                return new Dictionary<String, String>();
            }
        
            OslcPrefixParser parser = new OslcPrefixParser(prefixExpression);
            CommonTree rawTree = (CommonTree)parser.Result;        
 
            IList<ITree> rawPrefixes = rawTree.Children;
            PrefixMap prefixMap =
                new PrefixMap(rawPrefixes.Count);
           
             foreach (CommonTree rawPrefix in rawPrefixes) {

                if (rawPrefix.Token == Tokens.Skip || rawPrefix is CommonErrorNode) {
                     throw new ParseException(rawPrefix.ToString());
                 }            
                
                 String pn = rawPrefix.GetChild(0).Text;
                 String uri = rawPrefix.GetChild(1).Text;
                 
                 uri = uri.Substring(1, uri.Length - 2);
                 
                 prefixMap.Add(pn, uri);
             }
             
            return prefixMap;
        }

        /// <summary>
        /// Parse a oslc.select expression
        /// </summary>
        /// <param name="selectExpression">contents of an oslc.select HTTP query
        /// parameter</param>
        /// <param name="prefixMap">map between XML namespace prefixes and
        /// associated URLs</param>
        /// <returns>the parsed select clause</returns>
        public static SelectClause
        ParseSelect(
            String selectExpression,
            IDictionary<String, String> prefixMap
        )
        {
            try
            {
                OslcSelectParser parser = new OslcSelectParser(selectExpression);        
                CommonTree rawTree = (CommonTree)parser.Result;
                ITree child = rawTree.GetChild(0);

                if (child is CommonErrorNode)
                {
                    throw new ParseException(child.ToString());
                }

                return new SelectClauseImpl(rawTree, prefixMap);
            
            } catch (RecognitionException e) {
                throw new ParseException(e);
            }
        }
    
        /// <summary>
        /// Parse a oslc.properties expression
        /// </summary>
        /// <param name="propertiesExpression">contents of an oslc.properties HTTP query
        /// parameter</param>
        /// <param name="prefixMap"map between XML namespace prefixes and
        ///associated URLs></param>
        /// <returns>the parsed properties clause</returns>
        public static PropertiesClause
        parseProperties(
            String propertiesExpression,
            IDictionary<String, String> prefixMap
        )
        {
            try
            {
                OslcSelectParser parser = new OslcSelectParser(propertiesExpression);
                CommonTree rawTree = (CommonTree)parser.Result;
                ITree child = rawTree.GetChild(0);

                if (child is CommonErrorNode)
                {
                    throw new ParseException(child.ToString());
                }

                return new PropertiesClauseImpl(rawTree, prefixMap);

            }
            catch (RecognitionException e)
            {
                throw new ParseException(e);
            }
        }
    
        /// <summary>
        /// Parse a oslc.orderBy expression
        /// </summary>
        /// <param name="orderByExpression">contents of an oslc.orderBy HTTP query
        /// parameter</param>
        /// <param name="prefixMap">map between XML namespace prefixes and
        /// associated URLs</param>
        /// <returns></returns>
        public static OrderByClause
        ParseOrderBy(
            String orderByExpression,
            IDictionary<String, String> prefixMap
        )
        {
            try {

                OslcOrderByParser parser = new OslcOrderByParser(orderByExpression);
                CommonTree rawTree = (CommonTree)parser.Result;
                ITree child = rawTree.GetChild(0);
            
                if (child is CommonErrorNode) {
                    throw new ParseException(child.ToString());
                }

                return (OrderByClause)new SortTermsImpl(rawTree, prefixMap);
            
            } catch (RecognitionException e) {
                throw new ParseException(e);
            }
        }

        /// <summary>
        /// Parse a oslc.searchTerms expression
        /// 
        /// <p><b>Note</b>: {@link Object#toString()} of result has been overridden to
        /// return input expression.
        /// </summary>
        /// <param name="searchTermsExpression">contents of an oslc.searchTerms HTTP query
        /// parameter</param>
        /// <returns>the parsed search terms clause</returns>
        public static SearchTermsClause
        ParseSearchTerms(
            String searchTermsExpression
        )
        {
            try {
            
                OslcSearchTermsParser parser = new OslcSearchTermsParser(searchTermsExpression);
                CommonTree rawTree = (CommonTree)parser.Result;
                CommonTree child = (CommonTree)rawTree.GetChild(0);
            
                if (child is CommonErrorNode) {
                    throw ((CommonErrorNode)child).trappedException;
                }

                IList<ITree> rawList = rawTree.Children;
                StringList stringList = new StringList(rawList.Count);
            
                foreach (CommonTree str in rawList) {
                
                    String rawString = str.Text;
                
                    stringList.Add(rawString.Substring(1, rawString.Length-2));
                }
            
                return stringList;
            
            } catch (RecognitionException e) {
                throw new ParseException(e);
            }
        }
    
        /// <summary>
        /// Implementation of a IDictionary<String, String> prefixMap
        /// </summary>
        private class PrefixMap : Dictionary<String, String>
        {
            public
            PrefixMap(int size)
                : base(size)
            {
            }
            
            public override String
            ToString()
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;

                foreach (String key in Keys)
                {

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        buffer.Append(',');
                    }

                    buffer.Append(key);
                    buffer.Append('=');
                    buffer.Append('<');
                    buffer.Append(this[key]);
                    buffer.Append('>');
                }

                return buffer.ToString();
            }
        }

        /// <summary>
        /// Implementation of a SearchTermsClause interface
        /// </summary>
        private class StringList : List<string>, SearchTermsClause
        {
            public
            StringList(int size) : base(size)
            {
            }

            public override string
            ToString()
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;
            
                foreach (String str in this) {
                
                    if (first) {
                        first = false;
                    } else {
                        buffer.Append(',');
                    }

                    buffer.Append('"');
                    buffer.Append(str);
                    buffer.Append('"');
                }
            
                return buffer.ToString();
            }
        }

        /**
         * Check list of errors from parsing some expression, generating
         * @{link {@link ParseException} if there are any.
         * 
         * @param errors list of errors, hopefully empty
         * 
         * @throws ParseException
         */
        private static void
        CheckErrors(Parser parser)
        {
            if (! parser.Failed) {
                return;
            }
        
            throw new ParseException("error");
        }
    }
}
