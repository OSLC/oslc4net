#nullable enable
using System.Net;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client;

public sealed class OslcResponse<T> where T : IResource
{
    // TODO: make init (@berezovskyi 2024-10)
    public T? Resource { get; private set; } = default(T);

    public HttpResponseMessage? ResponseMessage { get; private set; }

    // REVISIT: we should build in the flexibility to detect 200 OK HTML reponses as actually 401s etc (@berezovskyi 2024-10)
    public HttpStatusCode? StatusCode { get; private set; }

    public static OslcResponse<T> WithSuccess(T resource,
        HttpResponseMessage? responseMessage = null)
    {
        return new OslcResponse<T>
        {
            Resource = resource,
            ResponseMessage = responseMessage,
            StatusCode = responseMessage?.StatusCode
        };
    }

    // REVISIT: should also consider cases where exceptions are raised before an HttpResponseMessage, e.g. for NXDOMAIN (@berezovskyi 2024-10)
    public static OslcResponse<T> WithError(HttpResponseMessage? responseMessage = null)
    {
        return new OslcResponse<T>
        {
            ResponseMessage = responseMessage, StatusCode = responseMessage?.StatusCode
        };
    }
}
