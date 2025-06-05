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

using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     Exception thrown for a service provider registration failure
/// </summary>
public class OslcCoreRegistrationException(
    ServiceProvider serviceProvider,
    int statusCode,
    string responseMessage) : OslcCoreRequestException(
    statusCode, null, serviceProvider)
{
    public ServiceProvider ServiceProvider { get; } = serviceProvider;
    public new string ResponseMessage { get; } = responseMessage;
}
