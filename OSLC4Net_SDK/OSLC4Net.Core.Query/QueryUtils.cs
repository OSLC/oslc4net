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
        public static IDictionary<string, string>
        ParsePrefixes(
            string prefixExpression
        )
        {
            if (prefixExpression == null) {
                return new Dictionary<string, string>();
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

                string pn = rawPrefix.GetChild(0).Text;
                string uri = rawPrefix.GetChild(1).Text;
                 
                 uri = uri.Substring(1, uri.Length - 2);
                 
                 prefixMap.Add(pn, uri);
             }
             
            return prefixMap;
        }

        /// <summary>
        /// Parse a oslc.where expression
        /// </summary>
        /// <param name="whereExpression"contents of an oslc.where HTTP query
        /// parameter></param>
        /// <param name="prefixMap">map between XML namespace prefixes and
        /// associated URLs</param>
        /// <returns>the parsed where clause</returns>
        public static WhereClause
        ParseWhere(
            string whereExpression,
            IDictionary<string, string> prefixMap
        )
        {
            try
            {
                OslcWhereParser parser = new OslcWhereParser(whereExpression);
                CommonTree rawTree = (CommonTree)parser.Result;
                ITree child = rawTree.GetChild(0);

                if (child is CommonErrorNode)
                {
                    throw new ParseException(child.ToString());
                }
            
                return (WhereClause) new WhereClauseImpl(rawTree, prefixMap);
            
            } catch (RecognitionException e) {
                throw new ParseException(e);
            }
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
            string selectExpression,
            IDictionary<string, string> prefixMap
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
            string propertiesExpression,
            IDictionary<string, string> prefixMap
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
            string orderByExpression,
            IDictionary<string, string> prefixMap
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
        /// Create a map representation of the Properties returned
        /// from parsing oslc.properties or olsc.select URL query
        /// parameters suitable for generating a property result from an
        /// HTTP GET request.<p/>
        /// 
        /// The map keys are the property names; i.e. the local name of the
        /// property concatenated to the XML namespace of the property.  The
        /// values of the map are:<p/>
        /// 
        /// <ul>
        /// <li> OSLC4NetConstants.OSLC4NET_PROPERTY_WILDCARD - if all
        /// properties at this level are to be output.  No recursion
        /// below this level is to be done.</li>
        /// <li> OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON - if only
        /// the named property is to be output, without recursion</li>
        /// <li> a nested property list to recurse through</li>
        /// </ul>
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>the property map</returns>
        public static IDictionary<string, object>
        InvertSelectedProperties(Properties properties)
        {
            IList<Property> children = properties.Children;
            IDictionary<string, object> result = new Dictionary<string, object>(children.Count);
        
            foreach (Property property in children) {
            
                PName pname = null;
                string propertyName = null;
            
                if (! property.IsWildcard) {
                    pname = property.Identifier;
                    propertyName = pname.ns + pname.local;
                }
            
                switch (property.Type) {
                case PropertyType.IDENTIFIER:
                    if (property.IsWildcard) {
                    
                        if (result is SingletonWildcardProperties) {
                            break;
                        }
                    
                        if (result is NestedWildcardProperties) {
                            result = new BothWildcardPropertiesImpl(
                                    (NestedWildcardPropertiesImpl)result);
                        } else {
                            result = new SingletonWildcardPropertiesImpl();
                        }
                    
                        break;
                    
                    } else {
                    
                        if (result is SingletonWildcardProperties) {
                            break;
                        }
                    }
                
                    result.Add(propertyName,
                               OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON);
                
                    break;
                
                case PropertyType.NESTED_PROPERTY:
                    if (property.IsWildcard) {
                    
                        if (! (result is NestedWildcardProperties)) {                        
                            if (result is SingletonWildcardProperties) {
                                result = new BothWildcardPropertiesImpl();
                            } else {
                                result = new NestedWildcardPropertiesImpl(result);
                            }
                        
                            ((NestedWildcardPropertiesImpl)result).commonNestedProperties =
                                InvertSelectedProperties((NestedProperty)property);
                        
                       } else {
                            MergePropertyMaps(
                                ((NestedWildcardProperties)result).CommonNestedProperties(),
                                InvertSelectedProperties((NestedProperty)property));
                       }
                    
                        break;
                    }
                
                    result.Add(propertyName,
                               InvertSelectedProperties(
                                       (NestedProperty)property));
                
                    break;
                }
            }
        
            if (! (result is NestedWildcardProperties)) {
                return result;
            }
        
            IDictionary<String, Object> commonNestedProperties =
                ((NestedWildcardProperties)result).CommonNestedProperties();
        
            foreach (string propertyName in result.Keys) {
            
                IDictionary<String, Object> nestedProperties =
                    (IDictionary<string, object>)result[propertyName];
            
                if (nestedProperties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON) {
                    result.Add(propertyName, commonNestedProperties);
                } else {
                    MergePropertyMaps(nestedProperties, commonNestedProperties);
                }
            }
        
            return result;
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
            string searchTermsExpression
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

                    string rawString = str.Text;
                
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
        private class PrefixMap : Dictionary<string, string>
        {
            public
            PrefixMap(int size)
                : base(size)
            {
            }
            
            public override string
            ToString()
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;

                foreach (string key in Keys)
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
            
                foreach (string str in this) {
                
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

        /// <summary>
        /// Implementation of SingletonWildcardProperties
        /// </summary>
        private class SingletonWildcardPropertiesImpl :
            Dictionary<string, object>,
            SingletonWildcardProperties
        {
            public
            SingletonWildcardPropertiesImpl() : base(0)
            {
            }
       }

        /// <summary>
        /// Implementation of NestedWildcardProperties
        /// </summary>
       private class NestedWildcardPropertiesImpl :
            Dictionary<string, object>,
            NestedWildcardProperties
        {
            public
            NestedWildcardPropertiesImpl(IDictionary<string, object> accumulated) : base(accumulated)
            {
            }
        
            protected
            NestedWildcardPropertiesImpl()
            {
            }
        
            public IDictionary<string, object>
            CommonNestedProperties()
            {
                return commonNestedProperties;
            }
        
            internal IDictionary<string, object> commonNestedProperties =
                new Dictionary<string, object>();
        }

        /// <summary>
        /// Implementation of both SingletonWildcardProperties and
        /// NestedWildcardProperties
        /// </summary>
        private class BothWildcardPropertiesImpl :
            NestedWildcardPropertiesImpl,
            SingletonWildcardProperties
        {
            public
            BothWildcardPropertiesImpl()
            {
            }
        
            public
            BothWildcardPropertiesImpl(NestedWildcardPropertiesImpl accumulated) : this()
            {
                commonNestedProperties = accumulated.CommonNestedProperties();
            }
        }

        /// <summary>
        /// Merge into lhs properties those of rhs property
        /// map, merging any common, nested property maps
        /// </summary>
        /// <param name="lhs">target of property map merge</param>
        /// <param name="rhs">source of property map merge</param>
        private static void
        MergePropertyMaps(
            IDictionary<string, object> lhs,
            IDictionary<string, object> rhs
        )
        {
            ICollection<String> propertyNames = rhs.Keys;
        
            foreach (string propertyName in propertyNames) {
            
                IDictionary<String, Object> lhsNestedProperties =
                    (IDictionary<string, object>)lhs[propertyName];
                IDictionary<String, Object> rhsNestedProperties =
                    (IDictionary<string, object>)rhs[propertyName];
            
                if (lhsNestedProperties == rhsNestedProperties) {
                    continue;
                }
            
                if (lhsNestedProperties == null ||
                    lhsNestedProperties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON) {
                
                    lhs.Add(propertyName, rhsNestedProperties);
                
                    continue;
                }
            
                MergePropertyMaps(lhsNestedProperties, rhsNestedProperties);
            }
        }

        /// <summary>
        /// Check list of errors from parsing some expression, generating
        /// @{link {@link ParseException} if there are any.
        /// </summary>
        /// <param name="parser"></param>
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
