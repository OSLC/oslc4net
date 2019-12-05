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
    /// Implementation of TypedValue interface
    /// </summary>
    internal class TypedValueImpl : ValueImpl, ITypedValue
    {
        public
        TypedValueImpl(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        ) : base(tree, ValueType.TYPED_STRING)
        {
            this.prefixMap = prefixMap;
        }

        public string Value
        {
            get
            {
                if (value == null)
                {
                    value = tree.GetChild(0).Text;
                    value = value.Substring(1, value.Length - 2);
                }

                return value;
            }
        }

        public PName PrefixedName
        {
            get
            {
                if (prefixedName == null)
                {
                    string rawPName = tree.GetChild(1).Text;

                    prefixedName = new PName();
            
                    int colon = rawPName.IndexOf(':');
            
                    if (colon < 0) {
                        prefixedName.local = rawPName;
                    } else { 
                        if (colon > 0) {
                            prefixedName.prefix = rawPName.Substring(0, colon);
                            prefixedName.ns = prefixMap[prefixedName.prefix];
                        }
                        prefixedName.local = rawPName.Substring(colon + 1);
                    }
                }

                return prefixedName;
            }
        }

        public override string ToString()
        {
            return '"' + Value.ToString() + "\"^^" + PrefixedName.ToString();
        }

        private readonly IDictionary<string, string> prefixMap;
        private string value = null;
        private PName prefixedName = null;
    }
}
