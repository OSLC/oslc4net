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
    /// Implementation of InTerm interface
    /// </summary>
    internal class InTermImpl : SimpleTermImpl, InTerm
    {
        public
        InTermImpl(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        ) : base(tree, TermType.IN_TERM, prefixMap)
        {
        }

        public IList<IValue> Values
        {
            get
            {
                if (values == null)
                {
                    IList<ITree> treeValues =  ((CommonTree)tree.GetChild(1)).Children;

                    values = new List<IValue>(treeValues.Count - 1);
            
                    foreach (CommonTree treeValue in treeValues) {
                
                        IValue value =
                            ComparisonTermImpl.CreateValue(
                                    treeValue, "unspported literal value type",
                                    prefixMap);
                
                        values.Add(value);
                    }
                }

                return values;
            }
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
        
            buffer.Append(Property.ToString());
            buffer.Append(" in [");
        
            bool first = true;
        
            foreach (IValue value in Values) {
            
                if (first) {
                    first = false;
                } else {
                    buffer.Append(',');
                }
            
                buffer.Append(value.ToString());
            }
        
            buffer.Append(']');
        
            return buffer.ToString();
        }

        private List<IValue> values = null;
    }
}
