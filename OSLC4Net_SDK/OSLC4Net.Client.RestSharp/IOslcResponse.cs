using System;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.RestSharp;

/// <summary>
///     Interface for responses for OSLC resource requests.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IOslcResponse<T> where T : IResource
{
    Uri Uri { get; }

    OslcResponseStatus Status { get; }
}

public enum OslcResponseStatus
{
    Success,
    SuccessNoResource,
    AuthnFault,
    ClientNonAuthnFault,
    ServerFault
}

/// <summary>
///     Interface for a successful OSLC resource request containing a resource in the response.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IOslcResourceResponse<T> : IOslcResponse<T> where T : IResource
{
    T Resource { get; }
}


/// <summary>
///     Interface for a successful OSLC resource request containing a resource in the response.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IOslcAuthNeededResponse<T> : IOslcResponse<T> where T : IResource
{
    
}
