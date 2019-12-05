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
    /// Implementation of Property interface
    /// </summary>
    internal class PropertyImpl : Property
    {
        public
        PropertyImpl(
            CommonTree tree,
            PropertyType type,
            IDictionary<string, string> prefixMap,
            bool isWildcard
        )
        {
            this.tree = tree;
            this.type = type;
            this.prefixMap = prefixMap;
            this.isWildcard = isWildcard;
        }

        public PropertyType Type
        {
            get { return type; }
        }

        public bool IsWildcard
        {
            get { return isWildcard; }
        }

        public PName Identifier
        {
            get {

                if (identifier != null) {
                    return identifier;
                }

                if (isWildcard) {
                    throw new InvalidOperationException("wildcard has no identifier");
                }

                string rawIdentifier = tree.Text;
        
                identifier = new PName();
        
                int colon = rawIdentifier.IndexOf(':');
        
                if (colon < 0) {
                    identifier.local = rawIdentifier;
                } else { 
                    if (colon > 0) {
                        identifier.prefix = rawIdentifier.Substring(0, colon);
                        identifier.ns = prefixMap[identifier.prefix];
                    }
                    identifier.local = rawIdentifier.Substring(colon + 1);
                }

                return identifier;
            }
        }

        public override string ToString()
        {
            return Identifier.ToString();
        }
    
        private readonly CommonTree tree;
        private readonly PropertyType type;
        protected readonly IDictionary<string, string> prefixMap;
        private readonly bool isWildcard;
        private PName identifier = null;
    }
}
