using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class AbstractResourceSerializationTests
{
    [OslcNamespace("http://example.com/ns#")]
    [OslcResourceShape(describes = ["http://example.com/ns#ConcreteResource"])]
    private class ConcreteResource : AbstractResource
    {
        public ConcreteResource() : base() { }
        public ConcreteResource(Uri about) : base(about) { }
    }

    [Test]
    public async Task TestTypesProperty_RoundTrip_Serialization()
    {
        var resource = new ConcreteResource(new Uri("http://example.com/resource"));
        var typeUri = new Uri("http://example.com/type");
        resource.Types.Add(typeUri);

        var formatter = new RdfXmlMediaTypeFormatter();

        // Serialize
        var rdfXml = await RdfHelpers.SerializeAsync(formatter, resource, OslcMediaType.APPLICATION_RDF_XML_TYPE);

        // Deserialize
        var deserializedResource = await RdfHelpers.DeserializeAsync<ConcreteResource>(formatter, rdfXml, OslcMediaType.APPLICATION_RDF_XML_TYPE);

        await Assert.That(deserializedResource).IsNotNull();
        await Assert.That(deserializedResource.Types).Contains(typeUri);
        // Verify that the type defined in OslcResourceShape is also present
        await Assert.That(deserializedResource.Types).Contains(new Uri("http://example.com/ns#ConcreteResource"));
        await Assert.That(deserializedResource.GetAbout()).IsEqualTo(resource.GetAbout());
    }
}
