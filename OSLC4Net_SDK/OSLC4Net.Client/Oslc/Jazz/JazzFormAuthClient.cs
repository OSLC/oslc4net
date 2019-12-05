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
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using OSLC4Net.Client.Exceptions;

namespace OSLC4Net.Client.Oslc.Jazz
{
    public class JazzFormAuthClient : OslcClient
    {
	    private string url;
	    private string authUrl;
	    private string project;
	    private string user;
	    private string password;
	
	    private const string JAZZ_AUTH_MESSAGE_HEADER = "X-com-ibm-team-repository-web-auth-msg";
	    private const string JAZZ_AUTH_FAILED = "authfailed";

	    public JazzFormAuthClient() :
            base()
	    {
	    }
	
        /// <summary>
        /// Create a new Jazz Form Auth client for the given URL, user and password
        /// </summary>
        /// <param name="url">the URL of the Jazz server, including the web app context</param>
        /// <param name="user"></param>
        /// <param name="password"></param>
	    public JazzFormAuthClient(string url, string user, string password) :
            this()
	    {
		    this.url = url;
		    authUrl = url;  //default to base URL
		    this.user = user;
		    this.password = password;		
	    }
	
        /// <summary>
        /// Create a new Jazz Form Auth client for the given URL, user and password
        /// </summary>
        /// <param name="url">the URL of the Jazz server, including the web app context</param>
        /// <param name="authUrl">the base URL to use for authentication.  This is normally the 
	    /// application base URL for RQM and RTC and is the JTS application URL for fronting
	    /// applications like RRC and DM.</param>
        /// <param name="user"></param>
        /// <param name="password"></param>
	    public JazzFormAuthClient(string url, string authUrl, string user, string password) :
            this(url, user, password)
	    {
		    this.authUrl = authUrl;		
	    }
	
	    public string GetUrl() {
		    return url;
	    }
	    public void SetUrl(string url) {
		    this.url = url;
	    }
	
	    public string GetAuthUrl() {
		    return authUrl;
	    }
	
	    public void SetAuthUrl(string authUrl) {
		    this.authUrl = authUrl;
	    }

	    public string GetProject() {
		    return project;
	    }
	    public void SetProject(string project) {
		    this.project = project;
	    }
	    public string GetUser() {
		    return user;
	    }
	    public void SetUser(string user) {
		    this.user = user;
	    }
	    public string GetPassword() {
		    return password;
	    }
	    public void SetPassword(string password) {
		    this.password = password;
	    }

        /// <summary>
        /// Execute the sequence of HTTP requests to perform a form login to a Jazz server
        /// </summary>
        /// <returns>The HTTP status code of the final request to verify login is successful</returns>
	    public HttpStatusCode FormLogin()
        {
            HttpStatusCode statusCode = HttpStatusCode.Unused;
            string location = null;

            HttpResponseMessage resp;
		    try 
		    {
			
                resp = client.GetAsync(authUrl + "/authenticated/identity").Result;
			    statusCode = resp.StatusCode;

                if (statusCode == HttpStatusCode.Found)
                {
                    location = resp.Headers.Location.AbsoluteUri;
                    resp.ConsumeContent();
                    statusCode = FollowRedirects(statusCode, location);
                }
			
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
			    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("OSLC-Core-Version", "2.0");

                string securityCheckUrl = "j_username=" + user + "&j_password=" + password;
                StringContent content = new StringContent(securityCheckUrl, Encoding.UTF8);

                MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                mediaTypeValue.CharSet = "utf-8";

                content.Headers.ContentType = mediaTypeValue;

                resp = client.PostAsync(authUrl + "/j_security_check", content).Result;
		        statusCode = resp.StatusCode;

                string jazzAuthMessage = null;
                IEnumerable<string> values = new List<string>();

		        if (resp.Headers.TryGetValues(JAZZ_AUTH_MESSAGE_HEADER, out values)) {
		    	    jazzAuthMessage = values.Last();
		        }
		    
		        if (jazzAuthMessage != null && string.Compare(jazzAuthMessage, JAZZ_AUTH_FAILED, true) == 0)
		        {
                    resp.ConsumeContent();
		    	    throw new JazzAuthFailedException(user, url);
		        }
                else if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Found)
		        {
                    resp.ConsumeContent();
		    	    throw new JazzAuthErrorException(statusCode, url);
		        }
		        else //success
		        {
		    	    location = resp.Headers.Location.AbsoluteUri;
                    resp.ConsumeContent();
		    	    statusCode = FollowRedirects(statusCode, location);
		    	
		        }
		    } catch (JazzAuthFailedException jfe) {
			    throw jfe;
	        } catch (JazzAuthErrorException jee) {
	    	    throw jee;
	        } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
		    return statusCode;
	    }

        private HttpStatusCode FollowRedirects(HttpStatusCode statusCode, string location)
	    {

            while ((statusCode == HttpStatusCode.Found) && (location != null))
		    {
			    try {
                    HttpResponseMessage newResp = client.GetAsync(location).Result;
				    statusCode = newResp.StatusCode;
				    location = (newResp.Headers.Location != null) ? newResp.Headers.Location.AbsoluteUri : null;
                    newResp.ConsumeContent();
			    } catch (Exception e) {
				    Console.WriteLine(e.StackTrace);
			    }

		    }
		    return statusCode;
	    }

	    private HttpResponseMessage GetArtifactFeed(string feedUrl)
	    {
		    HttpResponseMessage resp = null;

		    try {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(OSLCConstants.ATOM));

			    resp = client.GetAsync(feedUrl).Result;

			    HttpStatusCode statusCode = resp.StatusCode;

                if (statusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("Status code from feed retrieval: " + statusCode);
                }
			
		    } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }

		    return resp;
	    }
    }
}
