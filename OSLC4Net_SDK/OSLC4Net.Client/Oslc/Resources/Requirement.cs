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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Client.Oslc.Resources;

/// <summary>
///     OSLC shape for <c>oslc_rm:Requirement</c>
/// </summary>
[OslcNamespace(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE)]
[OslcResourceShape(title = "Requirement Resource Shape", describes = new string[] { RmConstants.TYPE_REQUIREMENT })]
[Obsolete("See OSLC4Net.Domains.RequirementsManagement")]
public class Requirement : RequirementBase
{
    public Requirement()
    {
        AddType(new Uri(RmConstants.TYPE_REQUIREMENT));
    }

    public Requirement(Uri about) : base(about)
    {
        AddType(new Uri(RmConstants.TYPE_REQUIREMENT));
    }
}
