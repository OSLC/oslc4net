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
 *
 *    Steve Pitschke - initial API and implementation
 *******************************************************************************/

namespace OSLC4Net.Core;

/// <summary>
///     Marker interface applied to a Map(String, Object> to
///     indicate that when selecting properties for output all immediate,
///     resource properties of the resource should be output with entries
///     in the {@link NestedWildcardProperties#commonNestedProperties()
/// </summary>
public interface NestedWildcardProperties
{
    /**
     * @return map of all member properties of nested resources to be
     * output
     */
    IDictionary<string, object> CommonNestedProperties();
}
