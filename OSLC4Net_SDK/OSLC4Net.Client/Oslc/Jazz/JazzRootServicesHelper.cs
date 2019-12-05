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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

using log4net;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;
using OSLC4Net.Client.Exceptions;

namespace OSLC4Net.Client.Oslc.Jazz
{
    /// <summary>
    /// Helper class to assist in retrieval of attributes from the IBM Rational
    /// Jazz rootservices document
    /// 
    /// This class is not currently thread safe.
    /// </summary>
    public class JazzRootServicesHelper
    {
	    private string baseUrl;
	    private string rootServicesUrl;
	    private string catalogDomain;
	    private string catalogNamespace;
	    private string catalogProperty;
	    private string catalogUrl;
	    private ICollection<object[]> catalogs = new List<object[]>();

        //OAuth URLs
        string authorizationRealm;
        string requestTokenUrl;
        string authorizationTokenUrl;
        string accessTokenUrl;

	    private const string JFS_NAMESPACE = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/";
	    private const string JD_NAMESPACE = "http://jazz.net/xmlns/prod/jazz/discovery/1.0/";
	
        private static ILog logger = LogManager.GetLogger(typeof(JazzRootServicesHelper));
	
        /// <summary>
        /// Initialize Jazz rootservices-related URLs such as the catalog location and OAuth URLs
        /// 
        /// rootservices is unprotected and access does not require authentication
        /// </summary>
        /// <param name="url">base URL of the Jazz server, no including /rootservices.  Example:  https://example.com:9443/ccm</param>
        /// <param name="catalogDomain">Namespace of the OSLC domain to find the catalog for.  Example:  OSLCConstants.OSLC_CM</param>
	    public JazzRootServicesHelper (string url, string catalogDomain)
        {
		    baseUrl = url;
		    rootServicesUrl = baseUrl + "/rootservices";
		    this.catalogDomain = catalogDomain;
		
		    if (string.Compare(this.catalogDomain, OSLCConstants.OSLC_CM, true) == 0 ||
                string.Compare(this.catalogDomain, OSLCConstants.OSLC_CM_V2, true) == 0) {
			
			    catalogNamespace = OSLCConstants.OSLC_CM;
			    catalogProperty  = JazzRootServicesConstants.CM_ROOTSERVICES_CATALOG_PROP;
			
		    } else if (string.Compare(this.catalogDomain, OSLCConstants.OSLC_QM, true) == 0 ||
                       string.Compare(this.catalogDomain, OSLCConstants.OSLC_QM_V2, true) == 0) {
			
			    catalogNamespace = OSLCConstants.OSLC_QM;
			    catalogProperty =  JazzRootServicesConstants.QM_ROOTSERVICES_CATALOG_PROP;
			
		    } else if (string.Compare(this.catalogDomain, OSLCConstants.OSLC_RM, true) == 0 ||
                       string.Compare(this.catalogDomain, OSLCConstants.OSLC_RM_V2, true) == 0) {
			
			    catalogNamespace = OSLCConstants.OSLC_RM;
			    catalogProperty =  JazzRootServicesConstants.RM_ROOTSERVICES_CATALOG_PROP;
			
		    } else if (string.Compare(this.catalogDomain, OSLCConstants.OSLC_AM_V2, true) == 0) {
			
			    catalogNamespace = OSLCConstants.OSLC_AM_V2;
			    catalogProperty =  JazzRootServicesConstants.AM_ROOTSERVICES_CATALOG_PROP;
			
		    } 
		    else if (string.Compare(this.catalogDomain, OSLCConstants.OSLC_AUTO, true) == 0) {
			
			    catalogNamespace = OSLCConstants.OSLC_AUTO;
			    catalogProperty =  JazzRootServicesConstants.AUTO_ROOTSERVICES_CATALOG_PROP;
		
		    }
		    else {
			    logger.Fatal("Jazz rootservices only supports CM, RM, QM, and Automation catalogs");
		    }
				
		    ProcessRootServices();
	    }
	
        /// <summary>
        /// Get the OSLC Catalog URL
        /// </summary>
        /// <returns></returns>
	    public string GetCatalogUrl()
	    {
		    return catalogUrl;
	    }

        /// <summary>
        /// Create an OAuth client
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="secret"></param>
        /// <param name="user"></param>
        /// <param name="passwd"></param>
        /// <param name="authUrl"></param>
        /// <returns></returns>
        public JazzOAuthClient InitOAuthClient(string consumerKey, string secret, string user, string passwd, string authUrl)
        {
		    return new JazzOAuthClient (
								    requestTokenUrl,
								    authorizationTokenUrl,
								    accessTokenUrl,
								    consumerKey,
								    secret,
								    authorizationRealm,
                                    user,
                                    passwd,
                                    authUrl );		
	    }
	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
	    public JazzFormAuthClient InitFormClient(string userid, string password)
	    {
		    return new JazzFormAuthClient(baseUrl, userid, password);
	    }
	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <param name="authUrl">the base URL to use for authentication.  This is normally the 
	    /// application base URL for RQM and RTC and is the JTS application URL for fronting
	    /// applications like RRC and DM.</param>
        /// <returns></returns>
	    public JazzFormAuthClient InitFormClient(string userid, string password, string authUrl)
	    {
		    return new JazzFormAuthClient(baseUrl, authUrl, userid, password);
		
	    }
	
	    private void ProcessRootServices()
	    {
		    try {
			    OslcClient rootServicesClient = new OslcClient();
			    HttpResponseMessage response = rootServicesClient.GetResource(rootServicesUrl, OSLCConstants.CT_RDF);
			    Stream stream = response.Content.ReadAsStreamAsync().Result;
                IGraph rdfGraph = new Graph();
                IRdfReader parser = new RdfXmlParser();
                StreamReader streamReader = new StreamReader(stream);

                using (streamReader)
                {
                    parser.Load(rdfGraph, streamReader);
 
			        //get the catalog URL
			        catalogUrl = GetRootServicesProperty(rdfGraph, catalogNamespace, catalogProperty);
						
			        //get the OAuth URLs
			        requestTokenUrl = GetRootServicesProperty(rdfGraph, JFS_NAMESPACE, JazzRootServicesConstants.OAUTH_REQUEST_TOKEN_URL);
			        authorizationTokenUrl = GetRootServicesProperty(rdfGraph, JFS_NAMESPACE, JazzRootServicesConstants.OAUTH_USER_AUTH_URL);
			        accessTokenUrl = GetRootServicesProperty(rdfGraph, JFS_NAMESPACE, JazzRootServicesConstants.OAUTH_ACCESS_TOKEN_URL);
			        try { // Following field is optional, try to get it, if not found ignore exception because it will use the default
				        authorizationRealm = GetRootServicesProperty(rdfGraph, JFS_NAMESPACE, JazzRootServicesConstants.OAUTH_REALM_NAME);
			        } catch (ResourceNotFoundException e) {
				        // Ignore
			        }
                }
		    } catch (Exception e) {
			    throw new RootServicesException(baseUrl, e);
		    }
		
				
	    }
	
	    private string GetRootServicesProperty(IGraph rdfGraph, string ns, string predicate)
        {
            string returnVal = null;
				
		    IUriNode prop = rdfGraph.CreateUriNode(new Uri(ns + predicate));
		    IEnumerable<Triple> triples = rdfGraph.GetTriplesWithPredicate(prop);

		    if (triples.Count() == 1)
            {
                IUriNode obj = triples.First().Object as IUriNode;

                if (obj != null)
                {
			        returnVal = obj.Uri.ToString();
                }
            }

		    if (returnVal == null)
		    {
			    throw new ResourceNotFoundException(baseUrl, ns + predicate);
		    }

		    return returnVal;
	    }
    }
}
