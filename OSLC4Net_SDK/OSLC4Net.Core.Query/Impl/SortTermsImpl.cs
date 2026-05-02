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
using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query.Impl;

/// <summary>
/// Implementation of OrderByClause interface that parses and resolves sort terms from an oslc.orderBy query parameter.
///
/// It lazily evaluates the Antlr CommonTree representing the terms, generating a read-only list of
/// <see cref="SortTerm"/> implementations (either <see cref="SimpleSortTermImpl"/> or <see cref="ScopedSortTermImpl"/>).
/// For example, parsing "+oslc:name,-oslc:shortId,dcterms:creator{+foaf:name}" will yield three sort terms:
/// two simple terms and one scoped term.
/// </summary>
sealed class SortTermsImpl : OrderByClause
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
            if (children == null)
            {
                var treeChildren = tree.Children;
                var list = new List<SortTerm>(treeChildren.Count);

                foreach (CommonTree child in treeChildren)
                {
                    SortTerm term;

                    switch (child.Token.Type)
                    {
                        case OslcOrderByParser.SIMPLE_TERM:
                            term = new SimpleSortTermImpl(child, prefixMap);
                            break;
                        case OslcOrderByParser.SCOPED_TERM:
                            term = new ScopedSortTermImpl(child, prefixMap);
                            break;
                        default:
                            throw new InvalidOperationException("unimplemented type of sort term: " + child.Token.Text);
                    }

                    list.Add(term);
                }

                children = list.AsReadOnly();
            }

            return children;
        }
    }

    private readonly CommonTree tree;
    private readonly IDictionary<string, string> prefixMap;
    private IList<SortTerm> children;
}
