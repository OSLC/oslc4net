/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 * Copyright (c) 2025 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *******************************************************************************/

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     OSLC4Net Core exception
/// </summary>
public class OslcCoreApplicationException : Exception
{
    public OslcCoreApplicationException(string message) :
        base(message)
    {
    }

    public OslcCoreApplicationException(string message, Exception innerException) :
        base(message, innerException)
    {
    }
}
