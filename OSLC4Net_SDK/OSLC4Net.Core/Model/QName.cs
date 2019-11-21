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
            this._localPart = localPart;
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

            this._namespaceUri = namespaceURI;
            this._localPart = localPart;
            this._prefix = prefix;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is QName))
            {
                return false;
            }

            return this._namespaceUri.Equals(((QName)obj)._namespaceUri) && this._localPart.Equals(((QName)obj)._localPart);
        }

        public override int GetHashCode()
        {
            return this._namespaceUri.GetHashCode() * 31 + this._localPart.GetHashCode();
        }

        public string GetLocalPart()
        {
            return this._localPart;
        }

        public string GetNamespaceURI()
        {
            return this._namespaceUri;
        }

        public string GetPrefix()
        {
            return this._prefix;
        }

        public override string ToString()
        {
            if (this._namespaceUri == null)
            {
                return this._localPart;
            }

            return '{' + this._namespaceUri + '}' + this._localPart;
        }
    }
}