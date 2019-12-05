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
    /// Implementation of NestedProperty interface
    /// </summary>
    internal class NestedPropertyImpl : PropertyImpl, NestedProperty
    {
        public
        NestedPropertyImpl(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        ) : base((CommonTree)((CommonTree)tree.GetChild(0)).GetChild(0), PropertyType.NESTED_PROPERTY,
                  prefixMap, ((CommonTree)tree.GetChild(0)).Token.Type == OslcSelectParser.WILDCARD)
        {
            this.tree = tree;
            // children = PropertiesImpl.CreateChildren((CommonTree)tree.GetChild(1), prefixMap);
        }

        public IList<Property> Children
        {
            get
            {
                if (children == null)
                {
                    children = PropertiesImpl.CreateChildren((CommonTree)tree.GetChild(1), prefixMap);
                }

                return children;
            }
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
        
            buffer.Append(IsWildcard ?
                                "*" :
                                Identifier.ToString());
            buffer.Append('{');
        
            PropertiesImpl.ChildrenToString(buffer, Children);
        
            buffer.Append('}');
        
            return buffer.ToString();
        }
    
        private readonly CommonTree tree;
        private IList<Property> children = null;
    }
}
