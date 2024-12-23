using System;
using System.Diagnostics;
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
    protected RestClient Client { get; set; } = null!;

    public async Task<IOslcResponse<T>> GetResourceAsync<T>(Uri resourceUri)
        where T : IResource, new()
    {
        var request = PrepareGetRequest<T>(resourceUri);

        return WrapOslcResponse<T>(resourceUri, await Client.GetAsync(request));
    }

    private static IOslcResponse<T> WrapOslcResponse<T>(Uri resourceUri, RestResponse response)
        where T : IResource, new()
    {
        if (RestSharpResponseFactory.ResponseStatusFrom(response) == OslcResponseStatus.Success)
        {
            return new OslcResourceResponseSharp<T>(resourceUri, response);
        }

        throw new NotImplementedException();
    }

    private static RestRequest PrepareGetRequest<T>(Uri resource) where T : IResource
    {
        return new RestRequest(resource);
    }
}

// no auth
public abstract class OslcClientSharpPublic : OslcClientSharpBase
{
    /// <inheritdoc />
    protected OslcClientSharpPublic(RestClient? client = null)
    {
        Client = client ?? new RestClient();
    }
}

public abstract class OslcClientSharpBasic : OslcClientSharpBase
{
    public OslcClientSharpBasic(string username, string password, RestClientOptions? options = null)
    {
        var restClientOptions = options ?? new RestClientOptions();
        restClientOptions.Authenticator = new HttpBasicAuthenticator(username, password);

        Client = new RestClient(restClientOptions);
    }
}

// TODO: implement an ASP.NET Core server to test the dance
public abstract class OslcClientSharpOauth10a : OslcClientSharpBase
{
    public OslcClientSharpOauth10a(string key, string secret, string accessToken,
        string accessTokenSecret, RestClientOptions? options = null)
    {
        var restClientOptions = options ?? new RestClientOptions
        {
            Authenticator = OAuth1Authenticator.ForProtectedResource(key, secret,
                accessToken, accessTokenSecret)
        };

        Client = new RestClient(restClientOptions);
    }
}

public interface IResponseSharp
{
    public RestResponse RestSharpResponse { get; }
}

internal class ResponseSharpMixin<T> : IResponseSharp where T : IResource, new()
{
    // TODO: implement IRestSerializer based on our MediaFormatter
    // https://restsharp.dev/docs/advanced/serialization#custom
    private readonly Lazy<T> _resourceDeserialized = new(() => new T());

    public OslcResponseStatus Status =>
        RestSharpResponseFactory.ResponseStatusFrom(RestSharpResponse);

    /// <inheritdoc />
    public RestResponse RestSharpResponse { get; }

    public T DeserializeResource()
    {
        Debug.Assert(_resourceDeserialized.Value != null, "_resourceDeserialized.Value != null");
        return _resourceDeserialized.Value;
    }

    public static ResponseSharpMixin<TT> From<TT>(RestResponse restSharpResponse)
        where TT : IResource, new()
    {
        throw new NotImplementedException();
    }
}

public class OslcResourceResponseSharp<T> : IResponseSharp, IOslcResourceResponse<T>
    where T : IResource, new()
{
    public OslcResourceResponseSharp(Uri uri, RestResponse restSharpResponse)
    {
        Uri = uri;
        Mixin = ResponseSharpMixin<T>.From<T>(restSharpResponse);
    }

    private ResponseSharpMixin<T> Mixin { get; }

    public Uri Uri { get; }

    // TODO
    /// <inheritdoc />
    public OslcResponseStatus Status => Mixin.Status;

    public T Resource => Mixin.DeserializeResource();

    public RestResponse RestSharpResponse => Mixin.RestSharpResponse;
}

public class RestSharpResponseFactory
{
    public static OslcResponseStatus ResponseStatusFrom(RestResponse response)
    {
        return (int)response.StatusCode switch
        {
            > 0 and < 200 => OslcResponseStatus.SuccessNoResource,
            200 => OslcResponseStatus.Success,
            // TODO: introduce "technical"/intermediate statuses
            > 200 and < 400 => OslcResponseStatus.SuccessNoResource,
            400 or (>= 402 and < 500) => OslcResponseStatus.ClientNonAuthnFault,
            // 403 is an authz fault, need to think what to do with it. In theory, nothing,
            // because a non-logged in used should get 401 and a logged in has nothing
            // to do other than to ask for access.
            // But maybe worth introducing OslcResponseStatus.AuthzFault
            401 => OslcResponseStatus.AuthnFault,
            >= 500 and <= 599 => OslcResponseStatus.ServerFault,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
