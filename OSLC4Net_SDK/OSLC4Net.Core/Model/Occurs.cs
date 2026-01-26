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

/// <summary>
///     OSLC Occurs attribute
/// </summary>
/// <remarks>see https://docs.oasis-open-projects.org/oslc-op/core/v3.0/os/core-vocab.html</remarks>
public enum Occurs
{
    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "Exactly-one")]
    ExactlyOne,

    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "Zero-or-one")]
    ZeroOrOne,

    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "Zero-or-many")]
    ZeroOrMany,

    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "One-or-many")]
    OneOrMany,
    [URI("")] Unknown
}
