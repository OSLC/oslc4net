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

/// <summary>
/// Implementation of Value interface
/// </summary>
internal class ValueImpl : Value
{
    protected
    ValueImpl(
        CommonTree tree,
        ValueType type
    )
    {
        this.tree = tree;
        this.type = type;
    }

    public ValueType Type
    {
        get
        {
            return type;
        }
    }

    protected readonly CommonTree tree;
    private readonly ValueType type;
}
