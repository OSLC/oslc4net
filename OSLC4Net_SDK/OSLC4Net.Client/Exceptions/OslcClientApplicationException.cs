/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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

namespace OSLC4Net.Client.Exceptions;

/// <summary>
///  Base class for all application exceptions.
/// </summary>
public class OslcClientApplicationException : Exception
{
    public OslcClientApplicationException(string messageKey, object[] args) :
        base(MessageExtractor.GetMessage(messageKey, args))
    {
    }

    public OslcClientApplicationException(string messageKey, object[] args, Exception e) :
        base(MessageExtractor.GetMessage(messageKey, args), e)
    {
    }
}