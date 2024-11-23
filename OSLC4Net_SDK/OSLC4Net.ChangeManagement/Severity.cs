/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
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

namespace OSLC4Net.ChangeManagement;

public enum Severity
{
    Unclassified,
    Minor,
    Normal,
    Major,
    Critical,
    Blocker
}

class SeverityExtension
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

        throw new ArgumentException();
    }
}
