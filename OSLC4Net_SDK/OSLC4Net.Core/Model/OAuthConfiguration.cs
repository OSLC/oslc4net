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
    #region

    using System;

    using OSLC4Net.Core.Attribute;

    #endregion

    /// <summary>
    /// An OSLC OAuth configuration resource
    /// </summary>
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
    [OslcResourceShape(
        title = "OSLC OAuth Configuration Resource Shape",
        describes = new string[] { OslcConstants.TYPE_O_AUTH_CONFIGURATION })]
    public class OAuthConfiguration : AbstractResource
    {
        private Uri _authorizationUri;

        private Uri _oauthAccessTokenUri;

        private Uri _oauthRequestTokenUri;

        /// <summary>
        /// 
        /// </summary>
        public OAuthConfiguration()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oauthRequestTokenURI"></param>
        /// <param name="authorizationURI"></param>
        /// <param name="oauthAccessTokenURI"></param>
        public OAuthConfiguration(Uri oauthRequestTokenURI, Uri authorizationURI, Uri oauthAccessTokenURI)
            : this()
        {
            this._oauthRequestTokenUri = oauthRequestTokenURI;
            this._authorizationUri = authorizationURI;
            this._oauthAccessTokenUri = oauthAccessTokenURI;
        }

        [OslcDescription("Uri for obtaining OAuth authorization")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "authorizationURI")]
        [OslcReadOnly]
        [OslcTitle("Authorization Uri")]
        public Uri GetAuthorizationURI()
        {
            return this._authorizationUri;
        }

        [OslcDescription("Uri for obtaining OAuth access token")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "oauthAccessTokenURI")]
        [OslcReadOnly]
        [OslcTitle("Access Token Uri")]
        public Uri GetOauthAccessTokenURI()
        {
            return this._oauthAccessTokenUri;
        }

        [OslcDescription("Uri for obtaining OAuth request token")]
        [OslcOccurs(Occurs.ExactlyOne)]
        [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "oauthRequestTokenURI")]
        [OslcReadOnly]
        [OslcTitle("Request Token Uri")]
        public Uri GetOauthRequestTokenURI()
        {
            return this._oauthRequestTokenUri;
        }

        public void SetAuthorizationURI(Uri authorizationURI)
        {
            this._authorizationUri = authorizationURI;
        }

        public void SetOauthAccessTokenURI(Uri oauthAccessTokenURI)
        {
            this._oauthAccessTokenUri = oauthAccessTokenURI;
        }

        public void SetOauthRequestTokenURI(Uri oauthRequestTokenURI)
        {
            this._oauthRequestTokenUri = oauthRequestTokenURI;
        }
    }
}