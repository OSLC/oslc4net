/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
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

using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query.Impl;

class SortTermImpl : SortTerm
{
    public SortTermImpl(
        SortTermType type,
        CommonTree tree,
        IDictionary<string, string> prefixMap
    )
    {
        this.type = type;
        Tree = tree;
        PrefixMap = prefixMap;
    }

    public SortTermType
    Type
    {
        get { return type; }
    }

    public PName
    Identifier
    {
        get
        {
            if (identifier == null)
            {

                var rawProperty = Tree.GetChild(0).Text;

                identifier = new PName();

                var colon = rawProperty.IndexOf(':');

                if (colon < 0)
                {
                    identifier.LocalName = rawProperty;
                }
                else
                {
                    if (colon > 0)
                    {
                        identifier.Prefix = rawProperty.Substring(0, colon);
                        identifier.Namespace = PrefixMap[identifier.Prefix];
                    }
                    identifier.LocalName = rawProperty.Substring(colon + 1);
                }
            }

            return identifier;
        }
    }

    private readonly SortTermType type;
    protected CommonTree Tree { get; }
    protected IDictionary<string, string> PrefixMap { get; }
    private PName identifier;
}
