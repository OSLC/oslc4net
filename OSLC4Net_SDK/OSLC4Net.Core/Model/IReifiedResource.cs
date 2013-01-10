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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSLC4Net.Core.Model
{
    /**
     * This interface helps model RDF reified statements in plain old Java objects.
     * OSLC commonly uses reification to describe metadata on links, such as labels.
     * The {@link #getValue()} and {@link #setValue(Object)} methods allow you to
     * set the actual object of the triple. All other properties in implementing
     * classes are statements about the statement. These additional properties
     * should have {@link OslcName} and {@link OslcPropertyDefinition} annotations.
     * See {@link Link} for an example.
     * <p>
     * Note: The parameterized type T must be an {@link URI} to serialize to JSON due
     * to current limitations in the OSLC JSON format.
     * 
     * @see AbstractReifiedResource
     * @see <a href="http://www.w3.org/TR/rdf-mt/#Reif">RDF Semantics: Reification</a>
     * @see <a href="http://www.w3.org/TR/rdf-primer/#reification">RDF Primer: Reification</a>
     */
    public interface IReifiedResource<T>
    {
        /**
         * Gets the object of the reified statement.
         * 
         * @return the object of the reified statement
         */
        T GetValue();

        /**
         * Sets the object of the reified statement.
         * 
         * @param value the object of the reified statement
         */
        void SetValue(T value);
    }
}
