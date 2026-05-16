/*******************************************************************************
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 *******************************************************************************/

namespace OSLC4Net.Core.Query.Impl;

internal static class OperatorExtension
{
    public static string
    ToString(Operator op)
    {
        switch (op)
        {
            case Operator.EQUALS:
                return "=";
            case Operator.NOT_EQUALS:
                return "!=";
            case Operator.LESS_THAN:
                return "<";
            case Operator.GREATER_THAN:
                return ">";
            case Operator.LESS_EQUALS:
                return "<=";
            default:
            case Operator.GREATER_EQUALS:
                return ">=";
        }
    }
}
