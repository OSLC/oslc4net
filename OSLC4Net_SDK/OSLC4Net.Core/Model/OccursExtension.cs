/*******************************************************************************
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 *******************************************************************************/

namespace OSLC4Net.Core.Model;

public static class OccursExtension
{
    public static string ToString(Occurs occurs)
    {
        var attributes = (URI[])occurs.GetType().GetField(occurs.ToString())!
            .GetCustomAttributes(typeof(URI), false);

        return attributes.Length > 0 ? attributes[0].uri : string.Empty;
    }

    public static Occurs FromString(string value)
    {
        foreach (Occurs occurs in Enum.GetValues(typeof(Occurs)))
        {
            var uri = ToString(occurs);

            if (uri.Equals(value))
            {
                return occurs;
            }
        }

        throw new ArgumentException();
    }

    public static Occurs FromURI(URI uri)
    {
        return FromString(uri.uri);
    }
}
