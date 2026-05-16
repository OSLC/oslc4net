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

namespace OSLC4Net.Client.Oslc.Jazz;

/// <summary>
/// Constants for Jazz rootservices entries
/// </summary>
public static class JazzRootServicesConstants
{
    //rootservices catalog properties.  See starting at:
    //https://jazz.net/wiki/bin/view/Main/RootServicesSpec#Change_Management_Service_Provid

    public const string CM_ROOTSERVICES_CATALOG_PROP = "cmServiceProviders";
    public const string QM_ROOTSERVICES_CATALOG_PROP = "qmServiceProviders";
    public const string RM_ROOTSERVICES_CATALOG_PROP = "rmServiceProviders";
    public const string AM_ROOTSERVICES_CATALOG_PROP = "amServiceProviders";
    public const string AUTO_ROOTSERVICES_CATALOG_PROP = "autoServiceProviders";

    //OAuth entries
    public const string OAUTH_REQUEST_TOKEN_URL = "oauthRequestTokenUrl";
    public const string OAUTH_USER_AUTH_URL = "oauthUserAuthorizationUrl";
    public const string OAUTH_ACCESS_TOKEN_URL = "oauthAccessTokenUrl";
    public const string OAUTH_REALM_NAME = "oauthRealmName";
}
