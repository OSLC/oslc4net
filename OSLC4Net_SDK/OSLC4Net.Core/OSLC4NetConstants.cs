/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
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

namespace OSLC4Net.Core;

/// <summary>
///     Constants specific to OSLC4Net
/// </summary>
/// <seealso cref="Oslc4Net.Core.OslcConstants" />
public static class OSLC4NetConstants
{
    /// <summary>
    ///     Needed because MediaTypeFormatter does not expose request URI
    /// </summary>
    public const string INNER_URI_HEADER = "$X-OSLC4Net-GraphUriBase";

    private static readonly IDictionary<string, object> _oslc4NetPropertySingleton =
        new Dictionary<string, object>(0, StringComparer.Ordinal);

    public static IDictionary<string, object> Oslc4NetPropertySingleton => _oslc4NetPropertySingleton;
}
