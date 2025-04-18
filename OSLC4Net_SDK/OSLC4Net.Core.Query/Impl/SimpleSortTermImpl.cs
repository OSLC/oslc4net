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

using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query.Impl;

class SimpleSortTermImpl : SortTermImpl, SimpleSortTerm
{
    public SimpleSortTermImpl(
        CommonTree tree,
        IDictionary<string, string> prefixMap
    ) : base(SortTermType.SIMPLE, tree, prefixMap)
    {
    }

    public bool
    Ascending
    {
        get
        {
            if (ascending == null)
            {
                ascending = tree.GetChild(1).Text.Equals("+");
            }

            return ascending == true ? true : false;
        }
    }

    public override string
    ToString()
    {
        return Ascending + Identifier.ToString();
    }

    private bool? ascending = null;
}
