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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSLC4Net.Core.Model
{

    public abstract class AbstractReifiedResource<T> : IReifiedResource<T>
    {
	    private T value;
	
	    protected AbstractReifiedResource()
	    {
	    }
	
	    protected AbstractReifiedResource(T value)
	    {
		    this.value = value;
	    }
	
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    public T GetValue()
	    {
		    return value;
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
	    public void SetValue(T value)
	    {
		    this.value = value;
	    }
    }
}
