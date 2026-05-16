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

public static class TypeExtension
{
    public static string ToString(Type type)
    {
        var attributes = (Description[])type.GetType().GetField(type.ToString())!.GetCustomAttributes(typeof(Description), false);

        return attributes.Length > 0 ? attributes[0].value : string.Empty;
    }

    public static Type FromString(string value)
    {
        foreach (Type type in Enum.GetValues(typeof(Type)))
        {
            var description = ToString(type);

            if (description.Equals(value))
            {
                return type;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(value), value,
            "The string must correspond to one of the Type enum values");
    }
}
