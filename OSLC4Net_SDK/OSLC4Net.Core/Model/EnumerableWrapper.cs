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

namespace OSLC4Net.Core.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public class EnumerableWrapper : IEnumerable<object>
    {
        private object opaqueObj;

        public EnumerableWrapper(object opaqueObj)
        {
            this.opaqueObj = opaqueObj;
        }

        public IEnumerator<object> GetEnumerator()
        {
            var method = opaqueObj.GetType().GetMethod("GetEnumerator", Type.EmptyTypes);

            method = method.MakeGenericMethod();

            return new EnumeratorWrapper(method.Invoke(opaqueObj, null));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class EnumeratorWrapper : IEnumerator<object>
        {
            private PropertyInfo currentInfo;

            private MethodInfo moveNext;

            private object opaqueEnumerator;

            public EnumeratorWrapper(object opaqueEnumerator)
            {
                this.opaqueEnumerator = opaqueEnumerator;
                currentInfo = opaqueEnumerator.GetType().GetProperty("Current");
                moveNext = opaqueEnumerator.GetType().GetMethod("MoveNext", Type.EmptyTypes);
            }

            public object Current => currentInfo.GetValue(opaqueEnumerator, null);

            public void Dispose()
            {
                opaqueEnumerator.GetType().GetMethod("Dispose", Type.EmptyTypes)
                    .Invoke(opaqueEnumerator, Type.EmptyTypes);
            }

            public bool MoveNext()
            {
                return (bool)moveNext.Invoke(opaqueEnumerator, Type.EmptyTypes);
            }

            public void Reset()
            {
                opaqueEnumerator.GetType().GetMethod("Reset", Type.EmptyTypes)
                    .Invoke(opaqueEnumerator, Type.EmptyTypes);
            }
        }
    }
}