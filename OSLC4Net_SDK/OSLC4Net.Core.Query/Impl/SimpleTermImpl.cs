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
    /// Implementation of SimpleTerm interface
    /// </summary>
    internal abstract class SimpleTermImpl : SimpleTerm
    {
        protected
        SimpleTermImpl(
            CommonTree tree,
            TermType type,
            IDictionary<string, string> prefixMap
        )
        {
            this.tree = tree;
            this.type = type;
            this.prefixMap = prefixMap;
        }

        public TermType Type
        {
            get
            {
                return type;
            }
        }

        public PName Property
        {
            get
            {
                if (property == null)
                {
                    string rawPName = tree.GetChild(0).Text;

                    property = new PName();
            
                    int colon = rawPName.IndexOf(':');
            
                    if (colon < 0) {
                        property.local = rawPName;
                    } else { 
                        if (colon > 0) {
                            property.prefix = rawPName.Substring(0, colon);
                            property.ns = prefixMap[property.prefix];
                        }
                        property.local = rawPName.Substring(colon + 1);
                    }
                }

                return property;
            }
        }

        protected readonly CommonTree tree;
        protected readonly IDictionary<string, string> prefixMap;
        private readonly TermType type;
        private PName property = null;
    }
}
