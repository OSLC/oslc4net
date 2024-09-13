using System;
using System.Threading.Tasks;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.RestSharp;

public interface IOslcClient
{
    Task<IOslcResponse<T>> GetResourceAsync<T>(Uri resource) where T : IResource, new();
}
