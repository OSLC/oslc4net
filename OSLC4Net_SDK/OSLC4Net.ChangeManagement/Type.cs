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

public enum Type
{
    [Description("Defect")]
    Defect,
    [Description("Task")]
    Task,
    [Description("Story")]
    Story,

    // REVISIT: remove underscore in the next major version (@berezovskyi 2025-02)
    [Description("Bug Report")]
    Bug_Report,
    [Description("Feature Request")]
    Feature_Request
}

public static class TypeExtension
{
    public static string ToString(Type type)
    {
        var attributes = (Description[])type.GetType().GetField(type.ToString()).GetCustomAttributes(typeof(Description), false);

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

[AttributeUsage(AttributeTargets.Field)
]
internal class Description(string value) : Attribute
{
    /**
     *  Description of element; used in enumerations
     */
    public readonly string value = value;
}
