using System.Net;
using OSLC4Net.Core.Model;
using VDS.RDF;

namespace OSLC4Net.Client;

public sealed class OslcResponse<T> where T : IResource
{
    // TODO: make init (@berezovskyi 2024-10)
    public Graph? Graph { get; private set; } = default;
    public List<T>? Resources { get; private set; } = default;
    public Error? ErrorResource { get; private set; } = null;

    public HttpResponseMessage? ResponseMessage { get; private set; }

    // REVISIT: we should build in the flexibility to detect 200 OK HTML reponses as actually 401s etc (@berezovskyi 2024-10)
    public HttpStatusCode? StatusCode { get; private set; }

    public static OslcResponse<T> WithSuccess(List<T>? resources, Graph? g,
        HttpResponseMessage? responseMessage = null)
    {
        return new OslcResponse<T>
        {
            Resources = resources,
            Graph = g,
            ResponseMessage = responseMessage,
            StatusCode = responseMessage?.StatusCode
        };
    }

    // REVISIT: should also consider cases where exceptions are raised before an HttpResponseMessage, e.g. for NXDOMAIN (@berezovskyi 2024-10)
    public static OslcResponse<T> WithError(HttpResponseMessage? responseMessage = null, Error? errorResource = null,
        Graph? g = null)
    {
        return new OslcResponse<T>
        {
            ResponseMessage = responseMessage,
            StatusCode = responseMessage?.StatusCode,
            Graph = g,
            ErrorResource = errorResource
        };
    }
}