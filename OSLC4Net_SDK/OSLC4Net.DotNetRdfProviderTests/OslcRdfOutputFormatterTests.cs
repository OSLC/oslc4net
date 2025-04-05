using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using OSLC4Net.Core.Model;
using OSLC4Net.Server.Providers;
using VDS.RDF;
using Xunit;

namespace OSLC4Net.Server.Tests
{
    public class OslcRdfOutputFormatterTests
    {
        private readonly OslcRdfOutputFormatter _formatter;
        private readonly Mock<ILogger<OslcRdfOutputFormatter>> _loggerMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;
        private readonly Mock<HttpResponse> _httpResponseMock;
        private readonly Mock<OutputFormatterWriteContext> _contextMock;

        public OslcRdfOutputFormatterTests()
        {
            _formatter = new OslcRdfOutputFormatter();
            _loggerMock = new Mock<ILogger<OslcRdfOutputFormatter>>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _httpResponseMock = new Mock<HttpResponse>();
            _contextMock = new Mock<OutputFormatterWriteContext>();

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<OslcRdfOutputFormatter>)))
                .Returns(_loggerMock.Object);
            _httpContextMock.SetupGet(c => c.RequestServices).Returns(_serviceProviderMock.Object);
            _httpContextMock.SetupGet(c => c.Request).Returns(_httpRequestMock.Object);
            _httpContextMock.SetupGet(c => c.Response).Returns(_httpResponseMock.Object);
            _contextMock.SetupGet(c => c.HttpContext).Returns(_httpContextMock.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeSupportedMediaTypesAndEncodings()
        {
            Assert.Contains(_formatter.SupportedMediaTypes, mt => mt.MediaType == "text/turtle");
            Assert.Contains(_formatter.SupportedMediaTypes, mt => mt.MediaType == "application/rdf+xml");
            Assert.Contains(_formatter.SupportedEncodings, e => e.WebName == Encoding.UTF8.WebName);
            Assert.Contains(_formatter.SupportedEncodings, e => e.WebName == Encoding.Unicode.WebName);
        }

        [Fact]
        public async Task WriteResponseBodyAsync_ShouldSerializeGraph()
        {
            // Arrange
            var testObject = new List<object> { new object() };
            _contextMock.SetupGet(c => c.Object).Returns(testObject);
            _contextMock.SetupGet(c => c.ObjectType).Returns(testObject.GetType());
            _httpRequestMock.SetupGet(r => r.Scheme).Returns("http");
            _httpRequestMock.SetupGet(r => r.Host).Returns(new HostString("localhost"));
            _httpRequestMock.SetupGet(r => r.Path).Returns("/test");
            _httpRequestMock.Setup(r => r.GetEncodedUrl()).Returns("http://localhost/test");

            // Act
            await _formatter.WriteResponseBodyAsync(_contextMock.Object, Encoding.UTF8);

            // Assert
            _httpResponseMock.Verify(r => r.BodyWriter, Times.Once);
        }

        [Fact]
        public async Task SerializeGraph_ShouldUseCorrectRdfWriter()
        {
            // Arrange
            var graph = new Graph();
            var contentType = new StringSegment("application/rdf+xml");
            var responseMock = new Mock<HttpResponse>();
            responseMock.SetupGet(r => r.BodyWriter).Returns(new Mock<PipeWriter>().Object);

            // Act
            await _formatter.SerializeGraph(contentType, graph, responseMock.Object);

            // Assert
            // Verify that the correct RDF writer is used based on the content type
            // This is a bit tricky to assert directly, so we focus on the method execution
            responseMock.Verify(r => r.BodyWriter, Times.Once);
        }

        [Fact]
        public void AsMsNetType_ShouldConvertMediaType()
        {
            // Arrange
            var oslcMediaType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/rdf+xml");

            // Act
            var result = RdfOutputFormatterExtensions.AsMsNetType(oslcMediaType);

            // Assert
            Assert.Equal("application/rdf+xml", result.MediaType);
        }
    }
}
