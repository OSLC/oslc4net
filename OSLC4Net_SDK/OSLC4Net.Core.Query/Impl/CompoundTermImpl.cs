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
    /// Implementation of CompoundTerm interface
    /// </summary>
    internal class CompoundTermImpl : SimpleTermImpl, CompoundTerm
    {
        public
        CompoundTermImpl(
            CommonTree tree,
            bool isTopLevel,
            IDictionary<string, string> prefixMap
        ) : base(isTopLevel ? null : tree,
                 isTopLevel ? TermType.TOP_LEVEL : TermType.NESTED,
                 prefixMap)
        {
            this.tree = tree;
            this.isTopLevel = isTopLevel;
        }

        public IList<SimpleTerm> Children
        {
            get
            {
                if (children == null)
                {
                    IList<ITree> treeChildren =
                        isTopLevel ?
                            tree.Children :
                            ((CommonTree)tree.GetChild(1)).Children;

                    children =
                        new List<SimpleTerm>(
                                treeChildren.Count - (isTopLevel ? 0 : 1));
        
                    foreach (CommonTree child in treeChildren) {
            
                        SimpleTerm simpleTerm;
            
                        switch(child.Token.Type) {
                        case OslcWhereParser.SIMPLE_TERM:
                            simpleTerm = new ComparisonTermImpl(child, prefixMap);
                            break;
                        case OslcWhereParser.IN_TERM:
                            simpleTerm = new InTermImpl(child, prefixMap);
                            break;
                        case OslcWhereParser.COMPOUND_TERM:
                            simpleTerm = new CompoundTermImpl(child, false, prefixMap);
                            break;
                        default:
                            throw new InvalidOperationException("unimplemented type of simple term: " + child.Token.Text);
                        }
            
                        children.Add(simpleTerm);
                    }
                }

                return children;
            }
        }

        public override string ToString()
        {
             StringBuilder builder = new StringBuilder();
        
            if (! isTopLevel) {
                builder.Append(Property.ToString());
                builder.Append('{');
            }
        
            bool first = true;
        
            foreach (SimpleTerm term in Children) {
            
                if (first) {
                    first = false;
                } else {
                    builder.Append(" and ");
                }
            
                builder.Append(term.ToString());
            }
        
            if (! isTopLevel) {
                builder.Append('}');
            }
        
            return builder.ToString();
       }
    
        private readonly CommonTree tree;
        private readonly bool isTopLevel;
        private IList<SimpleTerm> children = null;
    }
}
