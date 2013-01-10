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
using System.Xml.Linq;

namespace OSLC4Net.Core.Model
{
    public class QName
    {
        public QName(string localPart)
        {
            this.localPart = localPart;
        }

        public QName(
            string namespaceURI,
            string localPart
        )
        {
            this.namespaceURI = namespaceURI;
            this.localPart = localPart;
        }

        public QName(
            string namespaceURI,
            string localPart,
            string prefix
        ) 
        {
            this.namespaceURI = namespaceURI;
            this.localPart = localPart;
            this.prefix = prefix;
        }

        public string GetNamespaceURI()
        {
            return namespaceURI;
        }

        public string GetLocalPart()
        {
            return localPart;
        }

        public string GetPrefix()
        {
            return prefix;
        }

        public override string ToString()
        {
            if (namespaceURI == null)
            {
                return localPart;
            }

            return '{' + namespaceURI + '}' + localPart;
        }

        private string namespaceURI;
        private string localPart;
        private string prefix;
    }
}
