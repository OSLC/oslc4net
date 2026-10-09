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
        Tree = tree;
        this.type = type;
        PrefixMap = prefixMap;
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
                var rawPName = Tree.GetChild(0).Text;

                property = new PName();

                var colon = rawPName.IndexOf(':');

                if (colon < 0)
                {
                    property.LocalName = rawPName;
                }
                else
                {
                    if (colon > 0)
                    {
                        property.Prefix = rawPName.Substring(0, colon);
                        property.Namespace = PrefixMap[property.Prefix];
                    }
                    property.LocalName = rawPName.Substring(colon + 1);
                }
            }

            return property;
        }
    }

    protected CommonTree Tree { get; }
    protected IDictionary<string, string> PrefixMap { get; }
    private readonly TermType type;
    private PName property;
}
