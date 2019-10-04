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
using System.ComponentModel;
using System.Linq;
using System.Text;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model
{
    /// <summary>
    /// Interface to represent an OSLC resource.
    /// </summary>
    public interface IResource
    {
        Uri GetAbout();

        void SetAbout(Uri about);
    }

    public static class AddAllExtension
    {
        public static void AddAll<T>(
            this ICollection<T> target,
            IEnumerable<T> source
        )
        {
            foreach (var item in source)
            {
                target.Add(item);
            }
        }
    }
}