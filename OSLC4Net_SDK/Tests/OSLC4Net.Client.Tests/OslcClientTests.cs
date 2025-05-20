using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using Xunit;
// Still needed for ILogger
// NSubstitute.ExceptionExtensions is not explicitly needed if Assert.ThrowsAsync is used directly

namespace OSLC4Net.Client.Tests
{
    public class OslcClientRdfTypeTests
    {
        private const string TestResourceBaseUri = "http://example.com/resources/";
        private readonly FakeHttpMessageHandler _fakeHttpMessageHandler; // Changed
        private readonly OslcClient _oslcClient;
        private readonly ILogger<OslcClient> _mockLogger;

        // --- Mock Resource Classes (remain unchanged) ---
        [OslcName("MockResourceWithShape")]
        [OslcNamespace("http://example.com/ns#")]
        [OslcResourceShape(describes = new string[] { "http://example.com/ns#ExpectedType" })]
        public class MockResourceWithShape : AbstractResource
        {
            public MockResourceWithShape() : base() { }
            public MockResourceWithShape(Uri about) : base(about) { }
        }

        [OslcName("MockResourceWithoutShape")]
        [OslcNamespace("http://example.com/ns#")]
        public class MockResourceWithoutShape : AbstractResource
        {
            public MockResourceWithoutShape() : base() { }
            public MockResourceWithoutShape(Uri about) : base(about) { }
        }

        [OslcName("MockResourceWithMultipleAllowedTypes")]
        [OslcNamespace("http://example.com/ns#")]
        [OslcResourceShape(describes = new string[] { "http://example.com/ns#Type1", "http://example.com/ns#Type2" })]
        public class MockResourceWithMultipleAllowedTypes : AbstractResource
        {
            public MockResourceWithMultipleAllowedTypes() : base() { }
            public MockResourceWithMultipleAllowedTypes(Uri about) : base(about) { }
        }

        [OslcName("MockResourceWithEmptyDescribes")]
        [OslcNamespace("http://example.com/ns#")]
        [OslcResourceShape(describes = new string[] { })]
        public class MockResourceWithEmptyDescribes : AbstractResource
        {
            public MockResourceWithEmptyDescribes() : base() { }
            public MockResourceWithEmptyDescribes(Uri about) : base(about) { }
        }

        [OslcName("MockResourceWithNullDescribes")]
        [OslcNamespace("http://example.com/ns#")]
        [OslcResourceShape(describes = null)]
        public class MockResourceWithNullDescribes : AbstractResource
        {
            public MockResourceWithNullDescribes() : base() { }
            public MockResourceWithNullDescribes(Uri about) : base(about) { }
        }

        [OslcName("MockBaseResource")]
        [OslcNamespace("http://example.com/ns#")]
        [OslcResourceShape(describes = new string[] { "http://example.com/ns#BaseType" })]
        public class MockBaseResource : AbstractResource
        {
            public MockBaseResource() : base() { }
            public MockBaseResource(Uri about) : base(about) { }
        }

        [OslcName("MockDerivedResourceWithOwnShape")]
        [OslcNamespace("http://example.com/ns#")]
        [OslcResourceShape(describes = new string[] { "http://example.com/ns#DerivedType" })]
        public class MockDerivedResourceWithOwnShape : MockBaseResource
        {
            public MockDerivedResourceWithOwnShape() : base() { }
            public MockDerivedResourceWithOwnShape(Uri about) : base(about) { }
        }

        [OslcName("MockDerivedResourceNoShape")]
        [OslcNamespace("http://example.com/ns#")]
        public class MockDerivedResourceNoShape : MockBaseResource
        {
            public MockDerivedResourceNoShape() : base() { }
            public MockDerivedResourceNoShape(Uri about) : base(about) { }
        }

        public OslcClientRdfTypeTests()
        {
            _mockLogger = Substitute.For<ILogger<OslcClient>>();
            _fakeHttpMessageHandler = new FakeHttpMessageHandler(); // Initialize with default

            var httpClientWithFakeHandler = new HttpClient(_fakeHttpMessageHandler);

            _oslcClient = new OslcClient(_mockLogger);

            var clientField = typeof(OslcClient).GetField("_client",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (clientField == null) throw new InvalidOperationException("Could not find private field '_client' in OslcClient for test setup.");
            clientField.SetValue(_oslcClient, httpClientWithFakeHandler);
        }

        // Removed SetupMockHttpResponse helper method

        private string GetSingleRdf(string resourceUri, string rdfType, string title = "Test Resource")
        {
            return GetSingleRdf(resourceUri, new string[] { rdfType }, title);
        }

        private string GetSingleRdf(string resourceUri, string[] rdfTypes, string title = "Test Resource")
        {
            var typeTriples = new StringBuilder();
            if (rdfTypes != null)
            {
                foreach (var type in rdfTypes)
                {
                    if(!string.IsNullOrEmpty(type))
                        typeTriples.AppendLine($"    <rdf:type rdf:resource=\"{type}\"/>");
                }
            }

            return $@"<rdf:RDF
    xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
    xmlns:dcterms=""http://purl.org/dc/terms/"">
  <rdf:Description rdf:about=""{resourceUri}"">
{typeTriples}
    <dcterms:title>{title}</dcterms:title>
  </rdf:Description>
</rdf:RDF>";
        }

        private string GetCollectionRdf(params (string uri, string[] rdfTypes, string title)[] resources)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<rdf:RDF
    xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
    xmlns:dcterms=""http://purl.org/dc/terms/"">");

            foreach (var res in resources)
            {
                var typeTriples = new StringBuilder();
                if (res.rdfTypes != null)
                {
                    foreach (var type in res.rdfTypes)
                    {
                         if(!string.IsNullOrEmpty(type))
                            typeTriples.AppendLine($"    <rdf:type rdf:resource=\"{type}\"/>");
                    }
                }
                sb.AppendLine($@"  <rdf:Description rdf:about=""{res.uri}"">
{typeTriples}
    <dcterms:title>{res.title}</dcterms:title>
  </rdf:Description>");
            }
            sb.AppendLine("</rdf:RDF>");
            return sb.ToString();
        }

        private void AssertLoggedWarningContains(string substring)
        {
            _mockLogger.Received().Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains(substring)),
                null,
                Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public async Task GetResourceAsync_Single_TypeMatchSuccess_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "res1";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#ExpectedType");
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal(resourceUri, request.RequestUri?.ToString());
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                };
                return Task.FromResult(response);
            });

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Single(oslcResponse.Resources);
            Assert.IsAssignableFrom<MockResourceWithShape>(oslcResponse.Resources[0]);
            Assert.Equal(new Uri(resourceUri), oslcResponse.Resources[0].GetAbout());
        }

        [Fact]
        public async Task GetResourceAsync_Single_TypeMismatch_ThrowsOslcRdfTypeMismatchException()
        {
            var resourceUri = TestResourceBaseUri + "resMismatch";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#UnexpectedType");
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var exception = await Assert.ThrowsAsync<OslcRdfTypeMismatchException>(() =>
                _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri, expectedMediaType)
            );

            Assert.Equal(resourceUri, exception.ResourceUri);
            Assert.Contains("http://example.com/ns#ExpectedType", exception.ExpectedTypes);
            Assert.Contains("http://example.com/ns#UnexpectedType", exception.ActualTypes);
        }

        [Fact]
        public async Task GetResourceAsync_Single_NoRdfTypeInResource_ThrowsOslcRdfTypeMismatchException()
        {
            var resourceUri = TestResourceBaseUri + "resNoType";
            var rdfContent =
                GetSingleRdf(resourceUri, [], "Resource Without Type"); // Pass null for types
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var exception = await Assert.ThrowsAsync<OslcRdfTypeMismatchException>(() =>
                _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri, expectedMediaType)
            );

            Assert.Equal(resourceUri, exception.ResourceUri);
            Assert.Contains("http://example.com/ns#ExpectedType", exception.ExpectedTypes);
            Assert.Contains("none", exception.ActualTypes, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetResourceAsync_Single_TypeMatchSuccess_MultipleAllowedTypes_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resMultiAllowed";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#Type2");
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithMultipleAllowedTypes>(resourceUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Single(oslcResponse.Resources);
            Assert.IsAssignableFrom<MockResourceWithMultipleAllowedTypes>(oslcResponse.Resources[0]);
        }

        [Fact]
        public async Task GetResourceAsync_Single_TypeMatchSuccess_ResourceHasMultipleTypes_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resMultiActual";
            var rdfContent = GetSingleRdf(resourceUri, new string[] { "http://example.com/ns#OtherType", "http://example.com/ns#ExpectedType" });
             var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Single(oslcResponse.Resources);
            Assert.IsAssignableFrom<MockResourceWithShape>(oslcResponse.Resources[0]);
        }

        [Fact]
        public async Task GetResourceAsync_Single_NoOslcResourceShape_SkipsTypeCheck_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resNoShape";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#AnyType");
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithoutShape>(resourceUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Single(oslcResponse.Resources);
            Assert.IsAssignableFrom<MockResourceWithoutShape>(oslcResponse.Resources[0]);
        }

        [Fact]
        public async Task GetResourceAsync_Single_EmptyDescribesInShape_SkipsTypeCheck_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resEmptyDescribes";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#AnyType");
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithEmptyDescribes>(resourceUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Single(oslcResponse.Resources);
        }

        [Fact]
        public async Task GetResourceAsync_Single_NullDescribesInShape_SkipsTypeCheck_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resNullDescribes";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#AnyType");
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithNullDescribes>(resourceUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Single(oslcResponse.Resources);
        }

        [Fact]
        public async Task GetResourceAsync_TypeMatch_InheritedShapes_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resInherited";
            var expectedMediaType = "application/rdf+xml";

            // Test 1: Matches DerivedType
            var rdfContent1 = GetSingleRdf(resourceUri, "http://example.com/ns#DerivedType");
            _fakeHttpMessageHandler.SetHandler((req, ct) => Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(rdfContent1, Encoding.UTF8, expectedMediaType), RequestMessage = req }));
            var oslcResponse1 = await _oslcClient.GetResourceAsync<MockDerivedResourceWithOwnShape>(resourceUri, expectedMediaType);
            Assert.NotNull(oslcResponse1?.Resources);
            Assert.Single(oslcResponse1.Resources);

            // Test 2: Matches BaseType
            var rdfContent2 = GetSingleRdf(resourceUri, "http://example.com/ns#BaseType");
            _fakeHttpMessageHandler.SetHandler((req, ct) => Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(rdfContent2, Encoding.UTF8, expectedMediaType), RequestMessage = req }));
            var oslcResponse2 = await _oslcClient.GetResourceAsync<MockDerivedResourceWithOwnShape>(resourceUri, expectedMediaType);
            Assert.NotNull(oslcResponse2?.Resources);
            Assert.Single(oslcResponse2.Resources);

            // Test 3: MockDerivedNoShapeResource matches BaseType
            var rdfContent3 = GetSingleRdf(resourceUri, "http://example.com/ns#BaseType");
            _fakeHttpMessageHandler.SetHandler((req, ct) => Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(rdfContent3, Encoding.UTF8, expectedMediaType), RequestMessage = req }));
            var oslcResponse3 = await _oslcClient.GetResourceAsync<MockDerivedResourceNoShape>(resourceUri, expectedMediaType);
            Assert.NotNull(oslcResponse3?.Resources);
            Assert.Single(oslcResponse3.Resources);
        }

        [Fact]
        public async Task GetResourceAsync_Collection_FiltersInvalid_ReturnsValid()
        {
            var queryUri = TestResourceBaseUri + "query";
            var rdfContent = GetCollectionRdf(
                (TestResourceBaseUri + "valid1", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 1"),
                (TestResourceBaseUri + "invalid1", new[] { "http://example.com/ns#UnexpectedType" }, "Invalid Resource 1"),
                (TestResourceBaseUri + "valid2", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 2")
            );
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(queryUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Equal(2, oslcResponse.Resources.Count);
            Assert.All(oslcResponse.Resources, r => Assert.Contains("http://example.com/ns#ExpectedType", r.GetTypes().Select(t => t.ToString())));
            AssertLoggedWarningContains("was discarded");
        }

        [Fact]
        public async Task GetResourceAsync_Collection_AllInvalid_ReturnsEmptyAndLogs()
        {
            var queryUri = TestResourceBaseUri + "queryAllInvalid";
            var rdfContent = GetCollectionRdf(
                (TestResourceBaseUri + "invalid1", new[] { "http://example.com/ns#UnexpectedType1" }, "Invalid Resource 1"),
                (TestResourceBaseUri + "invalid2", new[] { "http://example.com/ns#UnexpectedType2" }, "Invalid Resource 2")
            );
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(queryUri, expectedMediaType);

            Assert.NotNull(oslcResponse);
            Assert.True(oslcResponse.IsSuccess);
            Assert.NotNull(oslcResponse.Resources);
            Assert.Empty(oslcResponse.Resources);
             _mockLogger.Received(2).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("was discarded")),
                null,
                Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public async Task GetResourceAsync_Collection_ResourceHasNoRdfType_IsFiltered()
        {
            var queryUri = TestResourceBaseUri + "queryFilterNoType";
             var rdfContent = GetCollectionRdf(
                (TestResourceBaseUri + "valid1", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 1"),
                (TestResourceBaseUri + "noType1", null,
                    "Resource Without Type"), // Pass null for types
                (TestResourceBaseUri + "valid2", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 2")
            );
            var expectedMediaType = "application/rdf+xml";

            _fakeHttpMessageHandler.SetHandler((request, cancellationToken) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rdfContent, Encoding.UTF8, expectedMediaType),
                    RequestMessage = request
                })
            );

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(queryUri, expectedMediaType);

            Assert.NotNull(oslcResponse?.Resources);
            Assert.Equal(2, oslcResponse.Resources.Count);
            AssertLoggedWarningContains("was discarded");
            AssertLoggedWarningContains(TestResourceBaseUri + "noType1");
        }
    }
}
