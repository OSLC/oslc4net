using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OSLC4Net.Client.Tests // Ensure this namespace matches your test project
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
        }

        // Default constructor for cases where the handler is set later, or a default behavior is desired.
        public FakeHttpMessageHandler() 
        {
            // Default behavior: return a simple NotFound response if no specific handler is set via property/method.
            _handlerFunc = (request, cancellationToken) => 
                Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound) 
                { 
                    RequestMessage = request 
                });
        }

        // Optional: A way to set or change the handler logic after construction
        public void SetHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
             _handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
        }
        
        // This is the .NET Core version of SendAsync.
        // For .NET Framework, SendAsync is not abstract but Send is. However, HttpClient uses SendAsync.
        // Let's assume .NET Core/.NET 5+ structure for HttpMessageHandler.
        // If this project targets older .NET Framework where SendAsync is not abstract,
        // this might need adjustment (e.g. overriding Send).
        // However, HttpClient.SendAsync calls HttpMessageHandler.SendAsync.
        // In netstandard2.0 (which this project targets), SendAsync is:
        // protected internal abstract System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken);
        // So overriding it is correct.
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
        
        // Older .NET Framework (like net4x) might need this if SendAsync isn't directly overridable as abstract
        // protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        // {
        //    // This is a synchronous wrapper. HttpClient typically calls SendAsync.
        //    // If _handlerFunc is async, this could block.
        //    return _handlerFunc(request, cancellationToken).GetAwaiter().GetResult();
        // }
    }
}
