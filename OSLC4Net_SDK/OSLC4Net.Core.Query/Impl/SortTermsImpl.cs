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
    class SortTermsImpl : OrderByClause
    {
        public SortTermsImpl(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        )
        {
            this.tree = tree;
            this.prefixMap = prefixMap;
        }

        public IList<SortTerm> Children
        {
            get
            {
                if (children == null) {
            
                    IList<CommonTree> rawChildren = (IList<CommonTree>)tree.Children;
            
                    children = new List<SortTerm>(rawChildren.Count());
            
                    foreach (CommonTree child in rawChildren) {

                        object simpleTerm;
                
                        switch(child.Token.Type) {
                        default:
                            throw new InvalidOperationException("unimplemented type of sort term: " + child.Token.Text);
                        }
                
                        children.Add((SortTerm)simpleTerm);
                    }

                    // XXX - Can't figure out why this doesn't work
                    // children = children.AsReadOnly();
                }
        
                return children;
            }
        }

        private readonly CommonTree tree;
        private readonly IDictionary<string, string> prefixMap;
        private IList<SortTerm> children = null;
    }
}
