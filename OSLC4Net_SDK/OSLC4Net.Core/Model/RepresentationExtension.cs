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

public static class RepresentationExtension
{
    public static string ToString(Representation representation)
    {
        var attributes = (URI[])representation.GetType().GetField(representation.ToString())!
            .GetCustomAttributes(typeof(URI), false);

        return attributes.Length > 0 ? attributes[0].uri : string.Empty;
    }

    public static Representation FromString(string value)
    {
        foreach (Representation representation in Enum.GetValues(typeof(Representation)))
        {
            var uri = ToString(representation);

            if (uri.Equals(value))
            {
                return representation;
            }
        }

        throw new ArgumentException();
    }

    public static Representation FromURI(URI uri)
    {
        return FromString(uri.uri);
    }
}
