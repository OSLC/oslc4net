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

using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Exceptions;

namespace OSLC4Net.Client.Oslc.Jazz;

public class JazzFormAuthClient : OslcClient
{
    private String url;
    private String authUrl;
    private String project;
    private String user;
    private String password;

    private const String JAZZ_AUTH_MESSAGE_HEADER = "X-com-ibm-team-repository-web-auth-msg";
    private const String JAZZ_AUTH_FAILED = "authfailed";

    private readonly ILogger _logger;

    public JazzFormAuthClient(ILogger<OslcClient> logger) :
        base(logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create a new Jazz Form Auth client for the given URL, user and password
    /// </summary>
    /// <param name="url">the URL of the Jazz server, including the web app context</param>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="logger"></param>
    public JazzFormAuthClient(String url, String user, String password, ILogger<OslcClient> logger) :
        this(logger)
    {
        this.url = url;
        this.authUrl = url;  //default to base URL
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
    /// <param name="logger"></param>
    public JazzFormAuthClient(String url, String authUrl, String user, String password, ILogger<OslcClient> logger) :
        this(url, user, password, logger)
    {
        this.authUrl = authUrl;
    }

    public String GetUrl()
    {
        return url;
    }
    public void SetUrl(String url)
    {
        this.url = url;
    }

    public String GetAuthUrl()
    {
        return authUrl;
    }

    public void SetAuthUrl(String authUrl)
    {
        this.authUrl = authUrl;
    }

    public String GetProject()
    {
        return project;
    }
    public void SetProject(String project)
    {
        this.project = project;
    }
    public String GetUser()
    {
        return user;
    }
    public void SetUser(String user)
    {
        this.user = user;
    }
    public String GetPassword()
    {
        return password;
    }
    public void SetPassword(String password)
    {
        this.password = password;
    }

    /// <summary>
    /// Execute the sequence of HTTP requests to perform a form login to a Jazz server
    /// </summary>
    /// <returns>The HTTP status code of the final request to verify login is successful</returns>
    public async Task<HttpStatusCode> FormLoginAsync()
    {
        HttpStatusCode statusCode = HttpStatusCode.Unused;
        HttpResponseMessage resp;
        try
        {

            resp = await GetHttpClient().GetAsync(authUrl + "/authenticated/identity").ConfigureAwait(false);
            statusCode = resp.StatusCode;

            string location;
            if (statusCode == HttpStatusCode.Found)
            {
                location = resp.Headers.Location.AbsoluteUri;
                resp.ConsumeContent();
                statusCode = await FollowRedirectsAsync(statusCode, location).ConfigureAwait(false);
            }

            GetHttpClient().DefaultRequestHeaders.Clear();
            GetHttpClient().DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            GetHttpClient().DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            GetHttpClient().DefaultRequestHeaders.Add("OSLC-Core-Version", "2.0");

            String securityCheckUrl = "j_username=" + this.user + "&j_password=" + this.password;
            StringContent content = new StringContent(securityCheckUrl, System.Text.Encoding.UTF8);

            MediaTypeHeaderValue mediaTypeValue = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            mediaTypeValue.CharSet = "utf-8";

            content.Headers.ContentType = mediaTypeValue;

            resp = await GetHttpClient().PostAsync(authUrl + "/j_security_check", content).ConfigureAwait(false);
            statusCode = resp.StatusCode;

            String jazzAuthMessage = null;
            IEnumerable<string> values = new List<string>();

            if (resp.Headers.TryGetValues(JAZZ_AUTH_MESSAGE_HEADER, out values))
            {
                jazzAuthMessage = values.Last();
            }

            if (jazzAuthMessage != null && String.Compare(jazzAuthMessage, JAZZ_AUTH_FAILED, true) == 0)
            {
                resp.ConsumeContent();
                throw new JazzAuthFailedException(this.user, this.url);
            }
            else if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Found)
            {
                resp.ConsumeContent();
                throw new JazzAuthErrorException(statusCode, this.url);
            }
            else //success
            {
                location = resp.Headers.Location.AbsoluteUri;
                resp.ConsumeContent();
                statusCode = await FollowRedirectsAsync(statusCode, location).ConfigureAwait(false);

            }
        }
        catch (JazzAuthFailedException jfe)
        {
            throw;
        }
        catch (JazzAuthErrorException jee)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during login");
        }
        return statusCode;
    }

    private async Task<HttpStatusCode> FollowRedirectsAsync(HttpStatusCode statusCode, String location)
    {

        while ((statusCode == HttpStatusCode.Found) && (location != null))
        {
            try
            {
                HttpResponseMessage newResp = await GetHttpClient().GetAsync(location).ConfigureAwait(false);
                statusCode = newResp.StatusCode;
                location = newResp.Headers.Location?.AbsoluteUri;
                newResp.ConsumeContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error following redirect");
            }

        }
        return statusCode;
    }
}
