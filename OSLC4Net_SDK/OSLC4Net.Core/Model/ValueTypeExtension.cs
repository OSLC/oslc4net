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

namespace OSLC4Net.Core.Model;

public static class ValueTypeExtension
{
    public static string ToString(ValueType valueType)
    {
        var attributes = (URI[])valueType.GetType().GetField(valueType.ToString())!
            .GetCustomAttributes(typeof(URI), false);

        return attributes.Length > 0 ? attributes[0].uri : string.Empty;
    }

    public static ValueType FromString(string value)
    {
        foreach (ValueType valueType in Enum.GetValues(typeof(ValueType)))
        {
            var uri = ToString(valueType);

            if (uri.Equals(value))
            {
                return valueType;
            }
        }

        return ValueType.Unknown; // XXX - Java code does return null;
    }

    public static ValueType FromURI(URI uri)
    {
        return FromString(uri.ToString());
    }
}
