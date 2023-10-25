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

namespace OSLC4Net.Core.Query;

public enum TermType
{
    COMPARISON,
    IN_TERM,
    NESTED,
    TOP_LEVEL
}

/// <summary>
/// Simple term from oslc.where clause
/// </summary>
public interface SimpleTerm
{
    TermType Type { get; }

    /**
     * @return type of simple term.  When {@link CompoundTerm}
     * return <code>null</code>.
     */
    PName Property { get; }
}
