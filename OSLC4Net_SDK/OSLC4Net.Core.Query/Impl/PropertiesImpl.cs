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
using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query.Impl
{
    /// <summary>
    /// Implementation of Properties interface
    /// </summary>
    internal class PropertiesImpl : Properties
    {
        public
        PropertiesImpl(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        )
        {
            this.tree = tree;
            this.prefixMap = prefixMap;
            // this.children = CreateChildren(tree, prefixMap);
        }
    
        /**
         * Construct a {@link Properties} proxy that has a single
         * {@link Wildcard} child
         */
        public
        PropertiesImpl()
        {
            tree = null;
            prefixMap = null;
        
            children = new List<Property>(1);
        
            children.Add(new WildcardImpl());
        }

        public IList<Property> Children
        {
            get
            {
                if (children == null)
                {
                    children = CreateChildren(tree, prefixMap);
                }

                return children;
            }
        }
    
        public override string ToString()
        {
            StringBuilder builder = ChildrenToString(new StringBuilder(), Children);
        
            return builder.ToString();
        }
    
        /**
         * Generate a list of property children from a parse tree node
         * 
         * @param tree
         * @param prefixMap
         * 
         * @return the resulting property list
         */
        static internal IList<Property>
        CreateChildren(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        )
        {
            IList<ITree> treeChildren = tree.Children;        
            IList<Property> children = new List<Property>(treeChildren.Count);
        
            foreach (CommonTree treeChild in treeChildren) {
            
                Property property;
            
                switch (treeChild.Token.Type)
                {
                case OslcSelectParser.WILDCARD:
                    property = (Property) new WildcardImpl();
                    break;
                case OslcSelectParser.PREFIXED_NAME:
                    property = (Property)new PropertyImpl((CommonTree)treeChild.GetChild(0),
                                                          PropertyType.IDENTIFIER, prefixMap, false);
                    break;
                default:
                case OslcSelectParser.NESTED_PROPERTIES:
                    property = (Property)new NestedPropertyImpl(treeChild, prefixMap);
                    break;
                }
            
                children.Add(property);
            }
        
            // XXX - children = Collections.unmodifiableList(children);
        
            return children;
        }
    
        /**
         * Generate string representation of a children property list
         * 
         * @param builder
         * @param children
         * 
         * @return the builder representation of the property list
         */
        static internal StringBuilder
        ChildrenToString(
            StringBuilder builder,
            IList<Property> children
        )
        {
            bool first = true;
         
             foreach (Property property in children) {
             
                 if (first) {
                     first = false;
                 } else {
                     builder.Append(',');
                 }
             
                 builder.Append(property.ToString());
             }
        
             return builder;
        }
    
        private readonly CommonTree tree;
        protected readonly IDictionary<string, string> prefixMap;
        private IList<Property> children = null;
    }
}
