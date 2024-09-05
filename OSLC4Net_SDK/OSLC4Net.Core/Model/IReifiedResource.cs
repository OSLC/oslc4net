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
/// This interface helps model RDF reified statements in plain old Java objects.
/// OSLC commonly uses reification to describe metadata on links, such as labels.
/// The  #getValue() and#setValue(Object) methods allow you to
/// set the actual object of the triple. All other properties in implementing
/// classes are statements about the statement. These additional properties
/// should have OslcName and OslcPropertyDefinition annotations.
/// See Link for an example.
///
/// Note: The parameterized type T must be an URI to serialize to JSON due
/// to current limitations in the OSLC JSON format.
///
/// @see AbstractReifiedResource
/// @see <a href="http://www.w3.org/TR/rdf-mt/#Reif">RDF Semantics: Reification</a>
/// @see <a href="http://www.w3.org/TR/rdf-primer/#reification">RDF Primer: Reification</a>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReifiedResource<T>
{
    /// <summary>
    /// Gets the object of the reified statement.
    /// </summary>
    /// <returns></returns>
    T GetValue();

    /// <summary>
    /// Sets the object of the reified statement.
    /// </summary>
    /// <param name="value"></param>
    void SetValue(T value);
}
