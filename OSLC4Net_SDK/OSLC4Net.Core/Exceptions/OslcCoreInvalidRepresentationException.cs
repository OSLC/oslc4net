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

using System;
using System.Reflection;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Exceptions;

public class OslcCoreInvalidRepresentationException : OslcCoreApplicationException
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="method"></param>
    /// <param name="representation"></param>
    public OslcCoreInvalidRepresentationException(Type resourceType, MethodInfo method, Representation representation) :
        base(MESSAGE_KEY, new object[] {resourceType.Name, method.Name, RepresentationExtension.ToString(representation)})
    {
        this.method         = method;
        this.representation = representation;
        this.resourceType   = resourceType;
    }

	    public MethodInfo GetMethod() {
        return method;
    }

    public Representation GetRepresentation() {
		    return representation;
	    }

    public Type GetResourceType() {
        return resourceType;
    }

    private static readonly string MESSAGE_KEY = "InvalidRepresentationException";

    private MethodInfo      method;
	    private Representation  representation;
    private Type            resourceType;
}
