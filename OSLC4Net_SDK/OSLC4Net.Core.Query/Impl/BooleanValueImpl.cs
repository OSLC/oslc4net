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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query.Impl
{
    /// <summary>
    /// Implementation of BooleanValue interface
    /// </summary>
    internal class BooleanValueImpl : ValueImpl, IBooleanValue
    {
        public
        BooleanValueImpl(CommonTree tree) : base(tree, ValueType.BOOLEAN)
        {
        }

        public bool Value
        {
            get
            {
                if (value == null)
                {
                    value = bool.Parse(tree.Text);
                }

                return value.Value;
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        private bool? value = null;
    }
}
