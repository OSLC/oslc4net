using System;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.RestSharp;

public interface IOslcResponse<T> where T : IResource
{
    Uri Uri { get; }
    T Resource { get; }
}
