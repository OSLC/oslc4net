using System;
using System.Threading.Tasks;
using OSLC4Net.Core.Model;
using RestSharp;
using RestSharp.Authenticators;

namespace OSLC4Net.Client.RestSharp;

/// <summary>
///     RestSharp based OSLC client
/// </summary>
// TODO: consider a base class for all RestSharp clients but without auth
public abstract class OslcClientSharpBase : IOslcClient
{
    private readonly RestClient _client;

    public OslcClientSharpBase()
    {
        _client = InitClient();
    }

    public async Task<IOslcResponse<T>> GetResourceAsync<T>(Uri resourceUri)
        where T : IResource, new()
    {
        var request = PrepareGetRequest<T>(resourceUri);

        return WrapOslcResponse<T>(resourceUri, await _client.GetAsync(request));
    }

    protected virtual RestClient InitClient()
    {
        return new RestClient(InitClientOptions());
    }

    protected virtual RestClientOptions? InitClientOptions()
    {
        return null;
    }

    private OslcResponseSharp<T> WrapOslcResponse<T>(Uri resourceUri, RestResponse response)
        where T : IResource, new()
    {
        return new OslcResponseSharp<T>(resourceUri, response);
    }

    private static RestRequest PrepareGetRequest<T>(Uri resource) where T : IResource
    {
        return new RestRequest(resource);
    }
}

// no auth
public abstract class OslcClientSharpPublic : OslcClientSharpBase
{
}

public abstract class OslcClientSharpBasic : OslcClientSharpBase
{
    private readonly string _password;
    private readonly string _username;

    public OslcClientSharpBasic(string username, string password)
    {
        _username = username;
        _password = password;
    }

    /// <inheritdoc />
    protected override RestClientOptions? InitClientOptions()
    {
        return new RestClientOptions
        {
            Authenticator = new HttpBasicAuthenticator(_username, _password)
        };
    }
}


// TODO: implement an ASP.NET Core server to test the dance
public abstract class OslcClientSharpOauth10a : OslcClientSharpBase
{
    private readonly string _accessToken;
    private readonly string _accessTokenSecret;
    private readonly string _key;
    private readonly string _secret;

    public OslcClientSharpOauth10a(string key, string secret, string accessToken,
        string accessTokenSecret)
    {
        _key = key;
        _secret = secret;
        _accessToken = accessToken;
        _accessTokenSecret = accessTokenSecret;
    }

    /// <inheritdoc />
    protected override RestClientOptions? InitClientOptions()
    {
        return new RestClientOptions
        {
            Authenticator = OAuth1Authenticator.ForProtectedResource(_key, _secret,
                _accessToken, _accessTokenSecret)
        };
    }
}

internal class OslcResponseSharp<T> : IOslcResponse<T> where T : IResource, new()
{
    // TODO: implement IRestSerializer based on our MediaFormatter
    // https://restsharp.dev/docs/advanced/serialization#custom
    private readonly Lazy<T> _resourceDeserialized = new(() => new T());

    public OslcResponseSharp(Uri uri, RestResponse restSharpResponse)
    {
        Uri = uri;
        RestSharpResponse = restSharpResponse;
    }

    private RestResponse RestSharpResponse { get; }

    public Uri Uri { get; }
    public T Resource => _resourceDeserialized.Value;
}
