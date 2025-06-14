/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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

namespace OSLC4Net.Client.Exceptions;

/// <summary>
///  Base class for all application exceptions.
/// </summary>
public class OslcClientApplicationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the OslcClientApplicationException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public OslcClientApplicationException(string message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OslcClientApplicationException class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public OslcClientApplicationException(string message, Exception innerException) :
        base(message, innerException)
    {
    }
}
