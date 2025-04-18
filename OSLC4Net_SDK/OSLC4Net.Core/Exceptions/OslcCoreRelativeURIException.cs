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

namespace OSLC4Net.Core.Exceptions;

/// <summary>
///     Exception thrown when relative URIs are encountered in an RDF model
/// </summary>
public class OslcCoreRelativeURIException : OslcCoreApplicationException
{
    private static readonly string MESSAGE_KEY = "RelativeURIException";

    private readonly string methodName;
    private readonly Uri relativeURI;
    private readonly Type resourceType;

    public OslcCoreRelativeURIException(Type resourceType, string methodName, Uri relativeURI) :
        base(MESSAGE_KEY, new object[] { resourceType.Name, methodName, relativeURI.ToString() })
    {
        this.methodName = methodName;
        this.relativeURI = relativeURI;
        this.resourceType = resourceType;
    }
}
