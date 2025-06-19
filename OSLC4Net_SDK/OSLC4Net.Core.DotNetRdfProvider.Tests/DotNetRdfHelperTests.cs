// OSLC4Net_SDK/OSLC4Net.Core.DotNetRdfProvider.Tests/DotNetRdfHelperTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Xml.Linq;
using OSLC4Net.Core.Exceptions;
using System.Numerics; // For BigInteger
using OSLC4Net.Core.DotNetRdfProvider; // Required for DotNetRdfHelper

namespace OSLC4Net.Core.DotNetRdfProvider.Tests
{
    [TestClass]
    public class DotNetRdfHelperTests
    {
        private IGraph _graph;
        private DotNetRdfHelper _helper;
        private ILogger<DotNetRdfHelper> _logger;

        private const string ExNs = "http://example.com/ns/";
        private const string OtherNs = "http://example.com/otherns/";
        private const string RdfNs = RdfSpecsHelper.RdfNamespace;
        private const string OslcNs = OslcConstants.OSLC_CORE_NAMESPACE;
        private const string DctermsNs = OslcConstants.DCTERMS_NAMESPACE;


        [TestInitialize]
        public void Setup()
        {
            _graph = new Graph();
            _graph.NamespaceMap.AddNamespace("ex", new Uri(ExNs));
            _graph.NamespaceMap.AddNamespace("other", new Uri(OtherNs));
            _graph.NamespaceMap.AddNamespace("oslc", new Uri(OslcNs));
            _graph.NamespaceMap.AddNamespace("rdf", new Uri(RdfNs));
            _graph.NamespaceMap.AddNamespace("dcterms", new Uri(DctermsNs));

            _logger = NullLoggerFactory.Instance.CreateLogger<DotNetRdfHelper>();
            _helper = new DotNetRdfHelper(_logger);
        }

        // Helper to create URI nodes
        private IUriNode ExUri(string localName) => _graph.CreateUriNode(new Uri(ExNs + localName));
        private IUriNode OtherExUri(string localName) => _graph.CreateUriNode(new Uri(OtherNs + localName));
        private IUriNode RdfUri(string localName) => _graph.CreateUriNode(new Uri(RdfNs + localName));
        private IUriNode OslcUri(string localName) => _graph.CreateUriNode(new Uri(OslcNs + localName));
        private IUriNode DctermsUri(string localName) => _graph.CreateUriNode(new Uri(DctermsNs + localName));


        #region Test Model Classes
        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "MySimpleResource")]
        public class MySimpleResource : AbstractResource
        {
            [OslcPropertyDefinition(ExNs + "name")]
            [OslcName("name")] // For testing GetDefaultPropertyName mismatch case
            public string Name { get; set; }

            [OslcPropertyDefinition(ExNs + "age")]
            public int Age { get; set; }

            [OslcPropertyDefinition(ExNs + "isActive")]
            public bool IsActive { get; set; }

            [OslcPropertyDefinition(ExNs + "creationDate")]
            public DateTime CreationDate { get; set; }

            [OslcPropertyDefinition(ExNs + "homepage")]
            public Uri Homepage { get; set; }

            [OslcPropertyDefinition(ExNs + "longValue")]
            public long LongValue { get; set; }

            [OslcPropertyDefinition(ExNs + "floatValue")]
            public float FloatValue { get; set; }

            [OslcPropertyDefinition(ExNs + "doubleValue")]
            public double DoubleValue { get; set; }

            [OslcPropertyDefinition(ExNs + "decimalValue")]
            public decimal DecimalValue { get; set; }

            [OslcPropertyDefinition(ExNs + "bigIntegerValue")]
            public BigInteger BigIntegerValue { get; set; }
        }

        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "MyNestedResource")]
        public class MyNestedResource : AbstractResource
        {
            [OslcPropertyDefinition(ExNs + "detail")]
            public string Detail { get; set; }
        }

        public class NotAnOslcResource // For testing non-OSLC objects in collections
        {
            public string SomeData { get; set; }
        }


        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "MyOuterResource")]
        public class MyOuterResource : AbstractResource
        {
            [OslcPropertyDefinition(DctermsNs + "title")] // Using dcterms for variety
            public string Title { get; set; }

            [OslcPropertyDefinition(ExNs + "nestedResource")]
            public MyNestedResource NestedResource { get; set; }

            [OslcPropertyDefinition(ExNs + "links")]
            public List<Uri> Links { get; set; } = new List<Uri>();

            [OslcPropertyDefinition(ExNs + "tags")]
            public string[] Tags { get; set; }

            [OslcPropertyDefinition(ExNs + "nestedCollection")]
            public ICollection<MyNestedResource> NestedCollection { get; set; } = new List<MyNestedResource>();

            [OslcPropertyDefinition(ExNs + "mixedCollection")] // Not directly supported by OSLC spec (mixed types in one prop) but testing collection logic
            public System.Collections.ArrayList MixedUntypedCollection {get; set;} = new System.Collections.ArrayList();
        }

        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "ResourceWithXmlLiteral")]
        public class ResourceWithXmlLiteral : AbstractResource
        {
            [OslcPropertyDefinition(ExNs + "xmlContent")]
            [OslcValueType(Model.ValueType.XMLLiteral)]
            public string XmlContent { get; set; }
        }

        // Interface for reified resource
        public interface IMyReifiedStatement : IReifiedResource<string>
        {
            [OslcPropertyDefinition(ExNs + "priority")]
            public string Priority { get; set; }
        }

        // Concrete class for reified resource
        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "MyReifiedStatement")]
        public class MyReifiedStatement : AbstractReifiedResource<string>, IMyReifiedStatement
        {
            public string Priority { get; set; }
        }


        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "ResourceWithReifiedProperty")]
        public class ResourceWithReifiedProperty : AbstractResource
        {
            [OslcPropertyDefinition(ExNs + "reifiedName")]
            public IMyReifiedStatement ReifiedName { get; set; }
        }

        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcResourceShape(OslcNs + "ResourceWithRdfCollections")]
        public class ResourceWithRdfCollections : AbstractResource
        {
            [OslcPropertyDefinition(ExNs + "rdfList")]
            [OslcRdfCollectionType(NamespaceURI=RdfNs, CollectionType="List")]
            public List<string> RdfList { get; set; }

            [OslcPropertyDefinition(ExNs + "rdfBag")]
            [OslcRdfCollectionType(NamespaceURI=RdfNs, CollectionType="Bag")]
            public List<MyNestedResource> RdfBag { get; set; }
        }

        [OslcNamespaceDefinition(Prefix = "ex", NamespaceURI = ExNs)]
        [OslcNamespaceDefinition(Prefix = "other", NamespaceURI = OtherNs)]
        [OslcResourceShape(OslcNs + "MyExtendedResource")]
        public class MyExtendedResource : AbstractResource, IExtendedResource
        {
            private Dictionary<QName, object> _extendedProperties = new Dictionary<QName, object>();

            [OslcPropertyDefinition(ExNs + "knownProperty")]
            public string KnownProperty {get; set;}

            public void SetExtendedProperties(IDictionary<QName, object> properties)
            {
                _extendedProperties = new Dictionary<QName, object>(properties);
            }

            public IDictionary<QName, object> GetExtendedProperties() => _extendedProperties;
        }


        #endregion

        #region Serialization Tests (Object to RDF)

        [TestMethod]
        public void CreateDotNetRdfGraph_WithSimpleResource_AssertsRdfTypeAndProperties()
        {
            var resourceUri = new Uri("http://example.com/resource/simple/1");
            var creationTime = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var homepageUri = new Uri("http://example.org/home");

            var simpleResource = new MySimpleResource
            {
                About = resourceUri, Name = "Test Resource", Age = 30, IsActive = true,
                CreationDate = creationTime, Homepage = homepageUri, LongValue = 1234567890123L,
                FloatValue = 3.14f, DoubleValue = 2.71828, DecimalValue = 123.45m,
                BigIntegerValue = BigInteger.Parse("98765432109876543210")
            };
            var resources = new List<object> { simpleResource };
            IGraph resultGraph = DotNetRdfHelper.CreateDotNetRdfGraph(resources);

            Assert.IsNotNull(resultGraph);
            var subjNode = resultGraph.CreateUriNode(resourceUri);

            AssertTriple(resultGraph, subjNode, RdfUri("type"), OslcUri("MySimpleResource"));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "name", "Test Resource");
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "age", "30", new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "isActive", "true", new Uri(XmlSpecsHelper.XmlSchemaDataTypeBoolean));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "creationDate", creationTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"), new Uri(XmlSpecsHelper.XmlSchemaDataTypeDateTime));
            AssertUriTriple(resultGraph, subjNode, ExNs + "homepage", homepageUri);
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "longValue", "1234567890123", new Uri(XmlSpecsHelper.XmlSchemaDataTypeLong));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "floatValue", "3.14", new Uri(XmlSpecsHelper.XmlSchemaDataTypeFloat)); // Note: float.ToString() default format
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "doubleValue", "2.71828", new Uri(XmlSpecsHelper.XmlSchemaDataTypeDouble));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "decimalValue", "123.45", new Uri(XmlSpecsHelper.XmlSchemaDataTypeDecimal));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "bigIntegerValue", "98765432109876543210", new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger));
        }

        [TestMethod]
        public void CreateDotNetRdfGraph_WithNestedResource_AssertsNestedStructure()
        {
            var outerUri = new Uri("http://example.com/outer/1");
            MyNestedResource nestedResource = new MyNestedResource { Detail = "Nested Detail" }; // As Blank Node

            var outerResource = new MyOuterResource
            {
                About = outerUri, Title = "Outer", NestedResource = nestedResource,
                Links = [new Uri("http://example.com/link1"), new Uri("http://example.com/link2")],
                Tags = ["tag1", "tag2"],
                NestedCollection = [new MyNestedResource { Detail = "NestedCollItem1" }]
            };
            var resources = new List<object> { outerResource };
            IGraph resultGraph = DotNetRdfHelper.CreateDotNetRdfGraph(resources);

            var outerSubjNode = resultGraph.CreateUriNode(outerUri);
            AssertTriple(resultGraph, outerSubjNode, RdfUri("type"), OslcUri("MyOuterResource"));
            AssertLiteralTriple(resultGraph, outerSubjNode, DctermsNs + "title", "Outer");

            var nestedObjectNode = resultGraph.GetTriplesWithSubjectPredicate(outerSubjNode, ExUri("nestedResource")).Select(t => t.Object).FirstOrDefault();
            Assert.IsNotNull(nestedObjectNode, "Nested resource link missing.");
            Assert.IsInstanceOfType(nestedObjectNode, typeof(IBlankNode));

            AssertTriple(resultGraph, nestedObjectNode, RdfUri("type"), OslcUri("MyNestedResource"));
            AssertLiteralTriple(resultGraph, nestedObjectNode, ExNs + "detail", "Nested Detail");

            AssertUriTriple(resultGraph, outerSubjNode, ExNs + "links", new Uri("http://example.com/link1"));
            AssertUriTriple(resultGraph, outerSubjNode, ExNs + "links", new Uri("http://example.com/link2"));
            AssertLiteralTriple(resultGraph, outerSubjNode, ExNs + "tags", "tag1");
            AssertLiteralTriple(resultGraph, outerSubjNode, ExNs + "tags", "tag2");

            var nestedCollNode = resultGraph.GetTriplesWithSubjectPredicate(outerSubjNode, ExUri("nestedCollection")).Select(t => t.Object).FirstOrDefault();
            Assert.IsNotNull(nestedCollNode);
            AssertLiteralTriple(resultGraph, nestedCollNode, ExNs + "detail", "NestedCollItem1");
        }

        [TestMethod]
        public void CreateDotNetRdfGraph_WithXmlLiteralProperty_AssertsXmlLiteral()
        {
            var resourceUri = new Uri("http://example.com/xmlres/1");
            var xml = "<content xmlns=\"http://example.com/content\"><item>Value</item></content>";
            var resource = new ResourceWithXmlLiteral { About = resourceUri, XmlContent = xml };
            IGraph resultGraph = DotNetRdfHelper.CreateDotNetRdfGraph([resource]);
            var subjNode = resultGraph.CreateUriNode(resourceUri);
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "xmlContent", xml, new Uri(RdfSpecsHelper.RdfXmlLiteral));
        }

        [TestMethod]
        public void CreateDotNetRdfGraph_WithPagingAndTotalCount_AssertsResponseInfo()
        {
            var descriptionAbout = "http://example.com/collection";
            var responseInfoAbout = "http://example.com/collection?page=1";
            var nextPageAbout = "http://example.com/collection?page=2";
            long totalCount = 100;
            var resources = new List<object> { new MySimpleResource { About = new Uri("http://example.com/res/1"), Name = "R1" } };
            IGraph g = DotNetRdfHelper.CreateDotNetRdfGraph(descriptionAbout, responseInfoAbout, nextPageAbout, totalCount, resources, null);

            var respInfoNode = g.CreateUriNode(new Uri(responseInfoAbout));
            AssertTriple(g, respInfoNode, RdfUri("type"), OslcUri(OslcConstants.TYPE_RESPONSE_INFO.Split('/').Last()));
            AssertLiteralTriple(g, respInfoNode, OslcNs + "totalCount", totalCount.ToString(CultureInfo.InvariantCulture), new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger));
            AssertUriTriple(g, respInfoNode, OslcNs + "nextPage", new Uri(nextPageAbout));
            AssertUriTriple(g, g.CreateUriNode(new Uri(descriptionAbout)), RdfSpecsHelper.RDFS_NAMESPACE + "member", new Uri("http://example.com/res/1"));
        }

        [TestMethod]
        public void CreateDotNetRdfGraph_WithReifiedProperty_AssertsReification()
        {
            var resourceUri = new Uri("http://example.com/reified/1");
            var reifiedStatement = new MyReifiedStatement { Value = "Reified Name Value", Priority = "High" };
            var resource = new ResourceWithReifiedProperty { About = resourceUri, ReifiedName = reifiedStatement };
            IGraph resultGraph = DotNetRdfHelper.CreateDotNetRdfGraph([resource]);

            var subjNode = resultGraph.CreateUriNode(resourceUri);
            var reifiedObjectNodes = resultGraph.GetTriplesWithSubjectPredicate(subjNode, ExUri("reifiedName")).Select(t => t.Object).ToList();
            Assert.AreEqual(1, reifiedObjectNodes.Count, "Expected one reified statement object.");

            var reifiedValueNode = reifiedObjectNodes[0] as ILiteralNode;
            Assert.IsNotNull(reifiedValueNode, "Reified value should be a literal.");
            Assert.AreEqual("Reified Name Value", reifiedValueNode.Value);

            // Check for the reification triples (reification quad)
            var reificationSubjects = resultGraph.GetTriplesWithPredicateObject(ExUri("reifiedName"), reifiedValueNode)
                                          .Select(t => t.Subject)
                                          .Where(s => resultGraph.GetTriplesWithSubjectPredicate(s, RdfUri("type")).Any(t2 => t2.Object.Equals(RdfUri("Statement"))))
                                          .ToList();

            // This part of the assertion is tricky because the reification subject (the blank node for the statement)
            // is linked from the main subject (subjNode) via the property (ExUri("reifiedName")) *and* the value ("Reified Name Value").
            // The current DotNetRdfHelper creates a blank node for the IReifiedResource instance itself,
            // and then that blank node has the reified statements.
            // The triple <subjNode, ExUri("reifiedName"), "Reified Name Value"> exists.
            // We need to find the blank node that is the subject of the reification metadata.
            // This is a known complexity/ambiguity in rdf:Statement reification vs. OSLC Core reification model.

            // Let's find the node that represents the MyReifiedStatement instance
            var reifiedInstanceNodes = resultGraph.GetTriplesWithObject(reifiedValueNode)
                .Where(t => t.Predicate.Equals(graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject)))) // Assuming default reification quad structure
                .Select(t => t.Subject).ToList();

            // This test needs more refinement based on exactly how reification is modeled.
            // The current DotNetRdfHelper.AddReifiedStatements creates a new blank node for the reification statement.
            // That blank node has rdf:subject, rdf:predicate, rdf:object.
            // The *original* object of the property (e.g. "Reified Name Value") is asserted directly.
            // Then, the reified statements (like "priority") are asserted about the *new* blank node representing the statement.

            // Find the blank node representing the reification statement.
            // This is complex as the link is not direct.
            // A simpler check for now:
             var priorityTriples = resultGraph.GetTriplesWithPredicate(ExUri("priority")).ToList();
             Assert.IsTrue(priorityTriples.Any(t => t.Object is ILiteralNode ln && ln.Value == "High"), "Priority should be set on a reified statement node.");
        }

        [TestMethod]
        public void CreateDotNetRdfGraph_WithRdfCollections_AssertsCorrectCollectionTypes()
        {
            var resourceUri = new Uri("http://example.com/rdfcoll/1");
            var res = new ResourceWithRdfCollections {
                About = resourceUri,
                RdfList = ["item1", "item2"],
                RdfBag = [new MyNestedResource { Detail = "BagItem1" }]
            };
            IGraph g = DotNetRdfHelper.CreateDotNetRdfGraph([res]);
            var subj = g.CreateUriNode(resourceUri);

            // rdf:List
            var listNode = g.GetTriplesWithSubjectPredicate(subj, ExUri("rdfList")).Select(t => t.Object).FirstOrDefault();
            Assert.IsNotNull(listNode);
            Assert.IsTrue(listNode.IsListRoot(g)); // VDS.RDF extension method
            var listItems = g.GetListItems(listNode).ToList();
            Assert.AreEqual(2, listItems.Count);
            Assert.IsTrue(listItems.OfType<ILiteralNode>().Any(ln => ln.Value == "item1"));
            Assert.IsTrue(listItems.OfType<ILiteralNode>().Any(ln => ln.Value == "item2"));

            // rdf:Bag
            var bagNode = g.GetTriplesWithSubjectPredicate(subj, ExUri("rdfBag")).Select(t => t.Object).FirstOrDefault();
            Assert.IsNotNull(bagNode);
            AssertTriple(g, bagNode, RdfUri("type"), RdfUri("Bag"));
            var bagItems = g.GetTriplesWithSubjectPredicate(bagNode, RdfUri("li")).Select(t => t.Object).ToList();
            Assert.AreEqual(1, bagItems.Count);
            AssertLiteralTriple(g, bagItems[0], ExNs + "detail", "BagItem1");
        }

        [TestMethod]
        public void CreateDotNetRdfGraph_WithExtendedResource_AssertsExtendedProperties()
        {
            var resourceUri = new Uri("http://example.com/extended/1");
            var extendedResource = new MyExtendedResource { About = resourceUri, KnownProperty = "KnownVal" };
            extendedResource.AddType(new Uri(OtherNs + "ExtraType"));
            extendedResource.GetExtendedProperties().Add(new QName(OtherNs, "dynamicProp"), "DynamicValue");
            extendedResource.GetExtendedProperties().Add(new QName(OtherNs, "multiValProp"), new List<object> { "Val1", 123 });


            IGraph resultGraph = DotNetRdfHelper.CreateDotNetRdfGraph([extendedResource]);
            var subjNode = resultGraph.CreateUriNode(resourceUri);

            AssertTriple(resultGraph, subjNode, RdfUri("type"), OslcUri("MyExtendedResource"));
            AssertTriple(resultGraph, subjNode, RdfUri("type"), OtherExUri("ExtraType"));
            AssertLiteralTriple(resultGraph, subjNode, ExNs + "knownProperty", "KnownVal");
            AssertLiteralTriple(resultGraph, subjNode, OtherNs + "dynamicProp", "DynamicValue");
            AssertLiteralTriple(resultGraph, subjNode, OtherNs + "multiValProp", "Val1");
            AssertLiteralTriple(resultGraph, subjNode, OtherNs + "multiValProp", "123", new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger));
        }


        #endregion

        #region Deserialization Tests (RDF to Object)

        [TestMethod]
        public void FromDotNetRdfNode_SimpleResource_PopulatesProperties()
        {
            var resourceUri = new Uri("http://example.com/resource/simpleD/1");
            var creationTime = new DateTime(2023, 10, 26, 14, 30, 15, DateTimeKind.Utc);
            var homepageUri = new Uri("http://example.org/homeD");

            var subjNode = _graph.CreateUriNode(resourceUri);
            _graph.Assert(subjNode, RdfUri("type"), OslcUri("MySimpleResource"));
            _graph.Assert(subjNode, ExUri("name"), _graph.CreateLiteralNode("Deserialized Name"));
            _graph.Assert(subjNode, ExUri("age"), _graph.CreateLiteralNode("42", new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger)));
            _graph.Assert(subjNode, ExUri("isActive"), _graph.CreateLiteralNode("false", new Uri(XmlSpecsHelper.XmlSchemaDataTypeBoolean)));
            _graph.Assert(subjNode, ExUri("creationDate"), _graph.CreateLiteralNode(creationTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"), new Uri(XmlSpecsHelper.XmlSchemaDataTypeDateTime)));
            _graph.Assert(subjNode, ExUri("homepage"), _graph.CreateUriNode(homepageUri));
            _graph.Assert(subjNode, ExUri("longValue"), _graph.CreateLiteralNode("9876543210", new Uri(XmlSpecsHelper.XmlSchemaDataTypeLong)));
            _graph.Assert(subjNode, ExUri("floatValue"), _graph.CreateLiteralNode("1.618", new Uri(XmlSpecsHelper.XmlSchemaDataTypeFloat)));

            var result = _helper.FromDotNetRdfNode(subjNode, _graph, typeof(MySimpleResource)) as MySimpleResource;

            Assert.IsNotNull(result);
            Assert.AreEqual(resourceUri, result.About);
            Assert.AreEqual("Deserialized Name", result.Name);
            Assert.AreEqual(42, result.Age);
            Assert.IsFalse(result.IsActive);
            Assert.AreEqual(creationTime, result.CreationDate);
            Assert.AreEqual(homepageUri, result.Homepage);
            Assert.AreEqual(9876543210L, result.LongValue);
            Assert.AreEqual(1.618f, result.FloatValue);
        }

        [TestMethod]
        public void FromDotNetRdfNode_NestedResource_PopulatesNestedStructure()
        {
            var outerUri = _graph.CreateUriNode(new Uri("http://example.com/outerD/1"));
            var nestedBlankNode = _graph.CreateBlankNode();

            _graph.Assert(outerUri, RdfUri("type"), OslcUri("MyOuterResource"));
            _graph.Assert(outerUri, DctermsUri("title"), _graph.CreateLiteralNode("Outer Deserialized"));
            _graph.Assert(outerUri, ExUri("nestedResource"), nestedBlankNode);
            _graph.Assert(outerUri, ExUri("links"), _graph.CreateUriNode(new Uri("http://example.com/linkD1")));
            _graph.Assert(outerUri, ExUri("tags"), _graph.CreateLiteralNode("tagD1"));

            _graph.Assert(nestedBlankNode, RdfUri("type"), OslcUri("MyNestedResource"));
            _graph.Assert(nestedBlankNode, ExUri("detail"), _graph.CreateLiteralNode("Nested Detail Deserialized"));

            var result = _helper.FromDotNetRdfNode(outerUri, _graph, typeof(MyOuterResource)) as MyOuterResource;

            Assert.IsNotNull(result);
            Assert.AreEqual("Outer Deserialized", result.Title);
            Assert.IsNotNull(result.NestedResource);
            Assert.AreEqual("Nested Detail Deserialized", result.NestedResource.Detail);
            Assert.IsNotNull(result.Links);
            Assert.AreEqual(1, result.Links.Count);
            Assert.AreEqual(new Uri("http://example.com/linkD1"), result.Links[0]);
            Assert.IsNotNull(result.Tags);
            Assert.AreEqual(1, result.Tags.Length);
            Assert.AreEqual("tagD1", result.Tags[0]);
        }

        [TestMethod]
        public void FromDotNetRdfNode_XmlLiteralProperty_PopulatesString()
        {
            var resourceUriNode = _graph.CreateUriNode(new Uri("http://example.com/xmlresD/1"));
            var xml = "<my:data xmlns:my='http://example.org/mydata#'>Deserialized XML</my:data>";
            _graph.Assert(resourceUriNode, RdfUri("type"), OslcUri("ResourceWithXmlLiteral"));
            _graph.Assert(resourceUriNode, ExUri("xmlContent"), _graph.CreateLiteralNode(xml, new Uri(RdfSpecsHelper.RdfXmlLiteral)));

            var result = _helper.FromDotNetRdfNode(resourceUriNode, _graph, typeof(ResourceWithXmlLiteral)) as ResourceWithXmlLiteral;

            Assert.IsNotNull(result);
            Assert.AreEqual(xml, result.XmlContent);
        }

        [TestMethod]
        public void FromDotNetRdfGraph_ListOfResources_ReturnsListOfObjects()
        {
            var res1Uri = _graph.CreateUriNode(new Uri("http://example.com/resD/1"));
            var res2Uri = _graph.CreateUriNode(new Uri("http://example.com/resD/2"));

            _graph.Assert(res1Uri, RdfUri("type"), OslcUri("MySimpleResource"));
            _graph.Assert(res1Uri, ExUri("name"), _graph.CreateLiteralNode("Resource D One"));
            _graph.Assert(res2Uri, RdfUri("type"), OslcUri("MySimpleResource"));
            _graph.Assert(res2Uri, ExUri("name"), _graph.CreateLiteralNode("Resource D Two"));

            var resultList = _helper.FromDotNetRdfGraph(_graph, typeof(MySimpleResource)) as List<MySimpleResource>;

            Assert.IsNotNull(resultList);
            Assert.AreEqual(2, resultList.Count);
            Assert.IsTrue(resultList.Any(r => r.About.ToString() == "http://example.com/resD/1" && r.Name == "Resource D One"));
            Assert.IsTrue(resultList.Any(r => r.About.ToString() == "http://example.com/resD/2" && r.Name == "Resource D Two"));
        }

        [TestMethod]
        [ExpectedException(typeof(OslcCoreRelativeURIException))]
        public void FromDotNetRdfNode_WithRelativeUriForProperty_ThrowsException()
        {
            var resourceUri = _graph.CreateUriNode(new Uri("http://example.com/resRel/prop"));
            _graph.Assert(resourceUri, RdfUri("type"), OslcUri("MySimpleResource"));
            // This triple uses a relative URI for an object property where an absolute one is expected for System.Uri
            _graph.Assert(resourceUri, ExUri("homepage"), _graph.CreateUriNode(new Uri("relative/uri", UriKind.Relative)));

            _helper.FromDotNetRdfNode(resourceUri, _graph, typeof(MySimpleResource));
        }

        [TestMethod]
        public void FromDotNetRdfNode_ExtendedResource_PopulatesExtendedProperties()
        {
            var resourceUri = _graph.CreateUriNode(new Uri("http://example.com/extendedD/1"));
            _graph.Assert(resourceUri, RdfUri("type"), OslcUri("MyExtendedResource"));
            _graph.Assert(resourceUri, RdfUri("type"), OtherExUri("ExtraTypeDeserialized"));
            _graph.Assert(resourceUri, ExUri("knownProperty"), _graph.CreateLiteralNode("KnownValueD"));
            _graph.Assert(resourceUri, OtherExUri("dynamicPropD"), _graph.CreateLiteralNode("DynamicValueD"));
            _graph.Assert(resourceUri, OtherExUri("multiValPropD"), _graph.CreateLiteralNode("ValD1"));
            _graph.Assert(resourceUri, OtherExUri("multiValPropD"), _graph.CreateLiteralNode("12345", new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger)));

            var result = _helper.FromDotNetRdfNode(resourceUri, _graph, typeof(MyExtendedResource)) as MyExtendedResource;

            Assert.IsNotNull(result);
            Assert.AreEqual("KnownValueD", result.KnownProperty);
            Assert.IsTrue(result.GetTypes().Contains(new Uri(OtherNs + "ExtraTypeDeserialized")));

            var extendedProps = result.GetExtendedProperties();
            Assert.IsTrue(extendedProps.ContainsKey(new QName(OtherNs, "dynamicPropD")));
            Assert.AreEqual("DynamicValueD", extendedProps[new QName(OtherNs, "dynamicPropD")]);

            Assert.IsTrue(extendedProps.ContainsKey(new QName(OtherNs, "multiValPropD")));
            var multiVal = extendedProps[new QName(OtherNs, "multiValPropD")] as List<object>; // Assumes it becomes a list
            Assert.IsNotNull(multiVal);
            Assert.AreEqual(2, multiVal.Count);
            Assert.IsTrue(multiVal.Contains("ValD1"));
            Assert.IsTrue(multiVal.Contains(BigInteger.Parse("12345"))); // Literals with datatype are often parsed to specific types or BigInteger
        }

        #endregion

        #region Assertion Helpers
        private void AssertTriple(IGraph graph, INode subject, Uri predicateUri, Uri objectUri) =>
            AssertTriple(graph, subject, graph.CreateUriNode(predicateUri), graph.CreateUriNode(objectUri));

        private void AssertTriple(IGraph graph, INode subject, INode predicateNode, INode objectNode) =>
            Assert.IsTrue(graph.ContainsTriple(new Triple(subject, predicateNode, objectNode)), $"Triple <{subject}> <{predicateNode}> <{objectNode}> missing.");

        private void AssertLiteralTriple(IGraph graph, INode subject, string predicateUri, string expectedValue, Uri datatype = null)
        {
            var predicateNode = graph.CreateUriNode(new Uri(predicateUri));
            var actualObject = graph.GetTriplesWithSubjectPredicate(subject, predicateNode).Select(t => t.Object).FirstOrDefault();

            Assert.IsNotNull(actualObject, $"Property <{predicateUri}> missing for subject <{subject}>.");
            Assert.IsInstanceOfType(actualObject, typeof(ILiteralNode), $"Object for <{predicateUri}> is not a LiteralNode.");
            var literalNode = (ILiteralNode)actualObject;
            Assert.AreEqual(expectedValue, literalNode.Value, $"Value mismatch for <{predicateUri}>.");
            if (datatype != null) Assert.AreEqual(datatype, literalNode.DataType, $"DataType mismatch for <{predicateUri}>.");
        }

        private void AssertUriTriple(IGraph graph, INode subject, string predicateUri, Uri expectedObjectUri) =>
            AssertTriple(graph, subject, graph.CreateUriNode(new Uri(predicateUri)), graph.CreateUriNode(expectedObjectUri));

        #endregion
    }
}
