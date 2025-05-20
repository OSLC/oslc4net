using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions; // For ThrowsAsync
using OSLC4Net.Client.Exceptions;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// VDS.RDF is not strictly needed here anymore as we rely on OslcClient's internal parsing.
// We are constructing RDF strings directly.

namespace OSLC4Net.Client.Tests
{
    [TestClass]
    public class OslcClientRdfTypeTests
    {
        private const string TestResourceBaseUri = "http://example.com/resources/";
        private HttpMessageHandler _mockHttpMessageHandler;
        private OslcClient _oslcClient;
        private ILogger<OslcClient> _mockLogger;

        // --- Mock Resource Classes ---
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

        // For hierarchy tests
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


        [TestInitialize]
        public void Setup()
        {
            _mockHttpMessageHandler = Substitute.For<HttpMessageHandler>();
            _mockLogger = Substitute.For<ILogger<OslcClient>>();
            
            var internalHttpClient = new HttpClient(_mockHttpMessageHandler);
            
            _oslcClient = new OslcClient(_mockLogger);
            // Use reflection to set the otherwise immutable _client field
            typeof(OslcClient).GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance)
                              .SetValue(_oslcClient, internalHttpClient);
        }

        private void SetupMockHttpResponse(string requestUri, string rdfContent, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "application/rdf+xml")
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(rdfContent, Encoding.UTF8, contentType)
            };

            // NSubstitute setup for HttpMessageHandler.SendAsync (protected method)
            // This requires NSubstitute.ProtectedExtensions if SendAsync were protected, but it's public on HttpMessageHandler.
             _mockHttpMessageHandler
                .GetType().GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public) // Ensure we get the right one
                .Invoke(_mockHttpMessageHandler, new object[] { Arg.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == requestUri), Arg.Any<CancellationToken>() })
                .Returns(Task.FromResult(response));

            // A more common NSubstitute setup if SendAsync was directly mockable (e.g. on an interface or public virtual)
            // _mockHttpMessageHandler.SendAsync(Arg.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == requestUri), Arg.Any<CancellationToken>())
            //    .Returns(Task.FromResult(response));
            // For HttpMessageHandler, the above reflection or a more specific NSubstitute setup for protected members is usually needed.
            // However, SendAsync is public. The issue might be how Arg.Is and Arg.Any are used with non-virtual methods.
            // The simplest way that often works with NSubstitute for HttpMessageHandler:
            typeof(HttpMessageHandler).GetMethod("SendAsync", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Invoke(_mockHttpMessageHandler, new object[] { Arg.Is<HttpRequestMessage>(m => m.RequestUri.AbsoluteUri == requestUri), Arg.Any<CancellationToken>() })
                .Returns(Task.FromResult(response));


            // Fallback to a more manual setup if the above still has issues with NSubstitute specifics:
            // We need to ensure the mock handler's SendAsync method is properly configured.
            // NSubstitute doesn't directly mock non-virtual methods of concrete classes like HttpClientHandler.
            // The _mockHttpMessageHandler is a substitute instance of HttpMessageHandler (abstract class).
            // Its SendAsync method IS abstract, so NSubstitute CAN override it.

             _mockHttpMessageHandler.SendAsync(
                Arg.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == requestUri),
                Arg.Any<CancellationToken>()
            ).Returns(Task.FromResult(response));
        }

        private string GetSingleRdf(string resourceUri, string rdfType, string title = "Test Resource")
        {
            return GetSingleRdf(resourceUri, new string[] { rdfType }, title);
        }
        
        private string GetSingleRdf(string resourceUri, string[] rdfTypes, string title = "Test Resource")
        {
            var typeTriples = new StringBuilder();
            foreach (var type in rdfTypes)
            {
                if(!string.IsNullOrEmpty(type))
                    typeTriples.AppendLine($"    <rdf:type rdf:resource=\"{type}\"/>");
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
                foreach (var type in res.rdfTypes)
                {
                     if(!string.IsNullOrEmpty(type))
                        typeTriples.AppendLine($"    <rdf:type rdf:resource=\"{type}\"/>");
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
                Arg.Is<object>(o => o.ToString().Contains(substring)),
                null,
                Arg.Any<Func<object, Exception, string>>());
        }

        // --- Test Methods ---

        [TestMethod]
        public async Task GetResourceAsync_Single_TypeMatchSuccess_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "res1";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#ExpectedType");
            SetupMockHttpResponse(resourceUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.IsNotNull(oslcResponse.Resources);
            Assert.AreEqual(1, oslcResponse.Resources.Count);
            Assert.IsInstanceOfType(oslcResponse.Resources[0], typeof(MockResourceWithShape));
            Assert.AreEqual(new Uri(resourceUri), oslcResponse.Resources[0].GetAbout());
        }

        [TestMethod]
        public async Task GetResourceAsync_Single_TypeMismatch_ThrowsOslcRdfTypeMismatchException()
        {
            var resourceUri = TestResourceBaseUri + "resMismatch";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#UnexpectedType");
            SetupMockHttpResponse(resourceUri, rdfContent);

            var exception = await Assert.ThrowsExceptionAsync<OslcRdfTypeMismatchException>(() => 
                _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri)
            );
            
            Assert.AreEqual(resourceUri, exception.ResourceUri);
            Assert.IsTrue(exception.ExpectedTypes.Contains("http://example.com/ns#ExpectedType"));
            Assert.IsTrue(exception.ActualTypes.Contains("http://example.com/ns#UnexpectedType"));
        }

        [TestMethod]
        public async Task GetResourceAsync_Single_NoRdfTypeInResource_ThrowsOslcRdfTypeMismatchException()
        {
            var resourceUri = TestResourceBaseUri + "resNoType";
            var rdfContent = GetSingleRdf(resourceUri, new string[] { null }, title: "Resource Without Type"); // Pass null or empty string for type
            SetupMockHttpResponse(resourceUri, rdfContent);
             
            var exception = await Assert.ThrowsExceptionAsync<OslcRdfTypeMismatchException>(() => 
                _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri)
            );

            Assert.AreEqual(resourceUri, exception.ResourceUri);
            Assert.IsTrue(exception.ExpectedTypes.Contains("http://example.com/ns#ExpectedType"));
            Assert.IsTrue(exception.ActualTypes.ToLowerInvariant().Contains("none"));
        }
        
        [TestMethod]
        public async Task GetResourceAsync_Single_TypeMatchSuccess_MultipleAllowedTypes_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resMultiAllowed";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#Type2");
            SetupMockHttpResponse(resourceUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithMultipleAllowedTypes>(resourceUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.AreEqual(1, oslcResponse.Resources.Count);
            Assert.IsInstanceOfType(oslcResponse.Resources[0], typeof(MockResourceWithMultipleAllowedTypes));
        }

        [TestMethod]
        public async Task GetResourceAsync_Single_TypeMatchSuccess_ResourceHasMultipleTypes_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resMultiActual";
            var rdfContent = GetSingleRdf(resourceUri, new string[] { "http://example.com/ns#OtherType", "http://example.com/ns#ExpectedType" });
            SetupMockHttpResponse(resourceUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(resourceUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.AreEqual(1, oslcResponse.Resources.Count);
            Assert.IsInstanceOfType(oslcResponse.Resources[0], typeof(MockResourceWithShape));
        }

        [TestMethod]
        public async Task GetResourceAsync_Single_NoOslcResourceShape_SkipsTypeCheck_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resNoShape";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#AnyType");
            SetupMockHttpResponse(resourceUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithoutShape>(resourceUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.AreEqual(1, oslcResponse.Resources.Count);
            Assert.IsInstanceOfType(oslcResponse.Resources[0], typeof(MockResourceWithoutShape));
        }

        [TestMethod]
        public async Task GetResourceAsync_Single_EmptyDescribesInShape_SkipsTypeCheck_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resEmptyDescribes";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#AnyType");
            SetupMockHttpResponse(resourceUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithEmptyDescribes>(resourceUri);
            
            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.AreEqual(1, oslcResponse.Resources.Count);
        }
        
        [TestMethod]
        public async Task GetResourceAsync_Single_NullDescribesInShape_SkipsTypeCheck_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resNullDescribes";
            var rdfContent = GetSingleRdf(resourceUri, "http://example.com/ns#AnyType");
            SetupMockHttpResponse(resourceUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithNullDescribes>(resourceUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.AreEqual(1, oslcResponse.Resources.Count);
        }

        [TestMethod]
        public async Task GetResourceAsync_TypeMatch_InheritedShapes_ReturnsResource()
        {
            var resourceUri = TestResourceBaseUri + "resInherited";
            // Test 1: Matches DerivedType
            var rdfContent1 = GetSingleRdf(resourceUri, "http://example.com/ns#DerivedType");
            SetupMockHttpResponse(resourceUri, rdfContent1);
            var oslcResponse1 = await _oslcClient.GetResourceAsync<MockDerivedResourceWithOwnShape>(resourceUri);
            Assert.IsNotNull(oslcResponse1?.Resources);
            Assert.AreEqual(1, oslcResponse1.Resources.Count, "Should match derived type");

            // Test 2: Matches BaseType
            var rdfContent2 = GetSingleRdf(resourceUri, "http://example.com/ns#BaseType");
            SetupMockHttpResponse(resourceUri, rdfContent2); // Re-setup for next call
            var oslcResponse2 = await _oslcClient.GetResourceAsync<MockDerivedResourceWithOwnShape>(resourceUri);
            Assert.IsNotNull(oslcResponse2?.Resources);
            Assert.AreEqual(1, oslcResponse2.Resources.Count, "Should match base type via hierarchy");
            
            // Test 3: MockDerivedNoShapeResource matches BaseType
            var rdfContent3 = GetSingleRdf(resourceUri, "http://example.com/ns#BaseType");
            SetupMockHttpResponse(resourceUri, rdfContent3);
            var oslcResponse3 = await _oslcClient.GetResourceAsync<MockDerivedResourceNoShape>(resourceUri);
            Assert.IsNotNull(oslcResponse3?.Resources);
            Assert.AreEqual(1, oslcResponse3.Resources.Count, "Derived with no shape should match base type");
        }

        [TestMethod]
        public async Task GetResourceAsync_Collection_FiltersInvalid_ReturnsValid()
        {
            var queryUri = TestResourceBaseUri + "query";
            var rdfContent = GetCollectionRdf(
                (TestResourceBaseUri + "valid1", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 1"),
                (TestResourceBaseUri + "invalid1", new[] { "http://example.com/ns#UnexpectedType" }, "Invalid Resource 1"),
                (TestResourceBaseUri + "valid2", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 2")
            );
            SetupMockHttpResponse(queryUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(queryUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.IsNotNull(oslcResponse.Resources);
            Assert.AreEqual(2, oslcResponse.Resources.Count);
            Assert.IsTrue(oslcResponse.Resources.All(r => r.GetTypes().Any(t => t.ToString() == "http://example.com/ns#ExpectedType")));
            AssertLoggedWarningContains("was discarded"); // Check if logging of discarded resource happened
        }

        [TestMethod]
        public async Task GetResourceAsync_Collection_AllInvalid_ReturnsEmptyAndLogs()
        {
            var queryUri = TestResourceBaseUri + "queryAllInvalid";
            var rdfContent = GetCollectionRdf(
                (TestResourceBaseUri + "invalid1", new[] { "http://example.com/ns#UnexpectedType1" }, "Invalid Resource 1"),
                (TestResourceBaseUri + "invalid2", new[] { "http://example.com/ns#UnexpectedType2" }, "Invalid Resource 2")
            );
            SetupMockHttpResponse(queryUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(queryUri);

            Assert.IsNotNull(oslcResponse);
            Assert.IsTrue(oslcResponse.IsSuccess);
            Assert.IsNotNull(oslcResponse.Resources);
            Assert.AreEqual(0, oslcResponse.Resources.Count);
            // Expect 2 warnings, one for each discarded resource
             _mockLogger.Received(2).Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString().Contains("was discarded")),
                null,
                Arg.Any<Func<object, Exception, string>>());
        }
        
        [TestMethod]
        public async Task GetResourceAsync_Collection_ResourceHasNoRdfType_IsFiltered()
        {
            var queryUri = TestResourceBaseUri + "queryFilterNoType";
             var rdfContent = GetCollectionRdf(
                (TestResourceBaseUri + "valid1", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 1"),
                (TestResourceBaseUri + "noType1", new string[] { null }, "Resource Without Type"), // No rdf:type
                (TestResourceBaseUri + "valid2", new[] { "http://example.com/ns#ExpectedType" }, "Valid Resource 2")
            );
            SetupMockHttpResponse(queryUri, rdfContent);

            var oslcResponse = await _oslcClient.GetResourceAsync<MockResourceWithShape>(queryUri);
            
            Assert.IsNotNull(oslcResponse?.Resources);
            Assert.AreEqual(2, oslcResponse.Resources.Count, "Only valid resources with correct type should be returned.");
            AssertLoggedWarningContains("was discarded");
            AssertLoggedWarningContains(TestResourceBaseUri + "noType1");
        }
    }
}
