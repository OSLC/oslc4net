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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using OSLC4Net.Client.Exceptions;

namespace OSLC4Net.Client.Oslc.Jazz
{
    public class JazzOAuthClient : OslcClient
    {
        /// <summary>
        /// Initialize an OAuthClient with the required OAuth URLs
        /// </summary>
        /// <param name="requestTokenURL"></param>
        /// <param name="authorizationTokenURL"></param>
        /// <param name="accessTokenURL"></param>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>\
        /// <param name="authUrl"></param>
	    public JazzOAuthClient(string requestTokenURL,
                               string authorizationTokenURL,
                               string accessTokenURL,
                               string consumerKey,
                               string consumerSecret,
                               string user,
                               string passwd,
                               string authUrl) :
            base(null, OAuthHandler(requestTokenURL, authorizationTokenURL, accessTokenURL, consumerKey, consumerSecret,
                                    user, passwd, authUrl))
        {
	    }

        /// <summary>
        /// Initialize an OAuthClient with the required OAuth URLs
        /// </summary>
        /// <param name="requestTokenURL"></param>
        /// <param name="authorizationTokenURL"></param>
        /// <param name="accessTokenURL"></param>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="oauthRealmName"></param>
        /// <param name="authUrl"></param>
	    public JazzOAuthClient(string requestTokenURL,
                               string authorizationTokenURL,
                               string accessTokenURL,
                               string consumerKey,
                               string consumerSecret,
                               string oauthRealmName,
                               string user,
                               string passwd,
                               string authUrl) :
            base(null, OAuthHandler(requestTokenURL, authorizationTokenURL, accessTokenURL, consumerKey, consumerSecret,
                                    user, passwd, authUrl))
        {
	    }

        private class TokenManager : IConsumerTokenManager
        {
            public TokenManager(string consumerKey, string consumerSecret)
            {
                this.consumerKey = consumerKey;
                this.consumerSecret = consumerSecret;
            }

            public string ConsumerKey
            {
                get { return consumerKey; }
            }

            public string ConsumerSecret
            {
                get { return consumerSecret; }
            }

            public string GetTokenSecret(string token)
            {
                return tokensAndSecrets[token];
            }

            public void StoreNewRequestToken(UnauthorizedTokenRequest request,
                                             ITokenSecretContainingMessage response)
            {
                tokensAndSecrets[response.Token] = response.TokenSecret;
            }

            public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey,
                                                                 string requestToken,
                                                                 string accessToken,
                                                                 string accessTokenSecret)
            {
                tokensAndSecrets.Remove(requestToken);
                tokensAndSecrets[accessToken] = accessTokenSecret;
            }

            public TokenType GetTokenType(string token)
            {
                throw new NotImplementedException();
            }

            public string GetRequestToken()
            {
                return tokensAndSecrets.First().Key;
            }

            private readonly IDictionary<string, string> tokensAndSecrets =
                new Dictionary<string, string>();
            private readonly string consumerKey;
            private readonly string consumerSecret;
        }
	
        private static HttpMessageHandler OAuthHandler(string requestTokenURL,
                                                       string authorizationTokenURL,
                                                       string accessTokenURL,
                                                       string consumerKey,
                                                       string consumerSecret,
                                                       string user,
                                                       string passwd,
                                                       string authUrl)
        {
            ServiceProviderDescription serviceDescription = new ServiceProviderDescription();

            serviceDescription.AccessTokenEndpoint = new MessageReceivingEndpoint(new Uri(accessTokenURL), HttpDeliveryMethods.PostRequest);
            serviceDescription.ProtocolVersion = ProtocolVersion.V10a;
            serviceDescription.RequestTokenEndpoint = new MessageReceivingEndpoint(new Uri(requestTokenURL), HttpDeliveryMethods.PostRequest);
            serviceDescription.TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() };
            serviceDescription.UserAuthorizationEndpoint = new MessageReceivingEndpoint(new Uri(authorizationTokenURL), HttpDeliveryMethods.PostRequest);

            TokenManager tokenManager = new TokenManager(consumerKey, consumerSecret);
            WebConsumer consumer = new WebConsumer(serviceDescription, tokenManager);

            // callback is never called by CLM, but needed to do OAuth based forms login
            // XXX - Dns.GetHostName() alway seems to return simple, uppercased hostname
            string callback = "https://" + Dns.GetHostName() + '.' + IPGlobalProperties.GetIPGlobalProperties().DomainName +  ":9443/cb";

            callback = callback.ToLower();

            consumer.PrepareRequestUserAuthorization(new Uri(callback), null, null);
            OslcClient oslcClient = new OslcClient();
            HttpClient client = oslcClient.GetHttpClient();

            HttpStatusCode statusCode = HttpStatusCode.Unused;
            string location = null;
            HttpResponseMessage resp;

		    try 
		    {
                client.DefaultRequestHeaders.Clear();

                resp = client.GetAsync(authorizationTokenURL + "?oauth_token=" + tokenManager.GetRequestToken() +
                                                            "&oauth_callback=" + Uri.EscapeUriString(callback).Replace("#", "%23").Replace("/", "%2F").Replace(":", "%3A")).Result;
                statusCode = resp.StatusCode;

                if (statusCode == HttpStatusCode.Found)
                {
                    location = resp.Headers.Location.AbsoluteUri;
                    resp.ConsumeContent();
                    statusCode = FollowRedirects(client, statusCode, location);
                }

                string securityCheckUrl = "j_username=" + user + "&j_password=" + passwd;
                StringContent content = new StringContent(securityCheckUrl, System.Text.Encoding.UTF8);

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
                    throw new JazzAuthFailedException(user, authUrl);
		        }
                else if (statusCode != HttpStatusCode.OK && statusCode != HttpStatusCode.Found)
		        {
                    resp.ConsumeContent();
                    throw new JazzAuthErrorException(statusCode, authUrl);
		        }
		        else //success
		        {
		    	    Uri callbackUrl = resp.Headers.Location;

                    resp = client.GetAsync(callbackUrl.AbsoluteUri).Result;
                    callbackUrl = resp.Headers.Location;
                    resp = client.GetAsync(callbackUrl.AbsoluteUri).Result;
                    callbackUrl = resp.Headers.Location;

                    NameValueCollection qscoll = callbackUrl.ParseQueryString();

                    if (callbackUrl.OriginalString.StartsWith(callback + '?') && qscoll["oauth_verifier"] != null)
                    {
                        DesktopConsumer desktopConsumer = new DesktopConsumer(serviceDescription, tokenManager);
                        AuthorizedTokenResponse authorizedTokenResponse = desktopConsumer.ProcessUserAuthorization(tokenManager.GetRequestToken(), qscoll["oauth_verifier"]);
                        
                        return consumer.CreateAuthorizingHandler(authorizedTokenResponse.AccessToken, CreateSSLHandler());
                    }

                    throw new JazzAuthErrorException(statusCode, authUrl);
                }
		    } catch (JazzAuthFailedException jfe) {
			    throw jfe;
	        } catch (JazzAuthErrorException jee) {
	    	    throw jee;
	        } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }

            // return consumer.CreateAuthorizingHandler(accessToken);
            return null;
	    }

        private static HttpStatusCode FollowRedirects(HttpClient client, HttpStatusCode statusCode, string location)
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

        private const string JAZZ_AUTH_MESSAGE_HEADER = "X-com-ibm-team-repository-web-auth-msg";
        private const string JAZZ_AUTH_FAILED = "authfailed";
    }
}
