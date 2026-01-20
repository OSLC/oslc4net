/*******************************************************************************
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 *******************************************************************************/

namespace OSLC4Net.ChangeManagement;

internal static class SeverityExtension
{
    public static string ToString(Severity severity)
    {
        return severity.ToString();
    }

    public static Severity FromString(string value)
    {
        foreach (Severity severity in Enum.GetValues(typeof(Severity)))
        {
            var stringValue = ToString(severity);

            if (stringValue.Equals(value))
            {
                return severity;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(value), value,
            "The string must correspond to one of the Severity enum values");
    }
}
