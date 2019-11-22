/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
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
    #region

    using System;

    #endregion

    /// <summary>
    /// Class representing namespace-qualified names
    /// </summary>
    public class QName
    {
        private string _localPart;

        private string _namespaceUri;

        private string _prefix;

        /// <summary>
        /// Constructor with local part only
        /// </summary>
        /// <param name="localPart"></param>
        public QName(string localPart)
        {
            _localPart = localPart;
        }

        /// <summary>
        /// Constructior with namespace and local part
        /// </summary>
        /// <param name="namespaceURI"></param>
        /// <param name="localPart"></param>
        public QName(string namespaceURI, string localPart)
            : this(namespaceURI, localPart, null)
        {
        }

        /// <summary>
        /// Constructor with namespace, local part and prefix/alias
        /// </summary>
        /// <param name="namespaceURI"></param>
        /// <param name="localPart"></param>
        /// <param name="prefix"></param>
        public QName(string namespaceURI, string localPart, string prefix)
        {
            if (namespaceURI == null || localPart == null)
            {
                throw new ArgumentException();
            }

            _namespaceUri = namespaceURI;
            _localPart = localPart;
            _prefix = prefix;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is QName))
            {
                return false;
            }

            return _namespaceUri.Equals(((QName)obj)._namespaceUri) && _localPart.Equals(((QName)obj)._localPart);
        }

        public override int GetHashCode()
        {
            return _namespaceUri.GetHashCode() * 31 + _localPart.GetHashCode();
        }

        public string GetLocalPart()
        {
            return _localPart;
        }

        public string GetNamespaceURI()
        {
            return _namespaceUri;
        }

        public string GetPrefix()
        {
            return _prefix;
        }

        public override string ToString()
        {
            if (_namespaceUri == null)
            {
                return _localPart;
            }

            return '{' + _namespaceUri + '}' + _localPart;
        }
    }
}