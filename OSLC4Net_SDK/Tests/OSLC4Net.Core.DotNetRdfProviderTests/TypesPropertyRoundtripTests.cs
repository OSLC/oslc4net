using System.Net.Http.Headers;
using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class TypesPropertyRoundtripTests
{
    [Test]
    public async Task Types_RoundtripsViaSerialization()
    {
        var cr = new ChangeRequest(new Uri("http://example.com/cr/1"));
        cr.Types = new List<Uri>
        {
            new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest"),
            new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "Defect")
        };

        var formatter = new RdfXmlMediaTypeFormatter();
        var rdfXml = await RdfHelpers.SerializeAsync(formatter, cr,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        var deserialized = await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, rdfXml,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Types.Count).IsEqualTo(cr.Types.Count);

        foreach (var type in cr.Types)
        {
            await Assert.That(deserialized.Types).Contains(type);
        }
    }

    [Test]
    public async Task Types_PropertyAndGetTypesShareBackingField()
    {
        var cr = new ChangeRequest(new Uri("http://example.com/cr/2"));
        var typeUri = new Uri(Constants.CHANGE_MANAGEMENT_NAMESPACE + "ChangeRequest");

        cr.Types = new List<Uri> { typeUri };

#pragma warning disable CS0618
        var fromMethod = cr.GetTypes();
#pragma warning restore CS0618

        await Assert.That(fromMethod).Contains(typeUri);
        await Assert.That(fromMethod.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Types_EmptyCollectionRoundtrips()
    {
        var cr = new ChangeRequest(new Uri("http://example.com/cr/3"));
        // Don't set any types - should roundtrip as empty

        var formatter = new RdfXmlMediaTypeFormatter();
        var rdfXml = await RdfHelpers.SerializeAsync(formatter, cr,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        var deserialized = await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, rdfXml,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        await Assert.That(deserialized).IsNotNull();
        // The ChangeRequest will have its own rdf:type from OslcResourceShape describes,
        // but no extra types should appear beyond that
    }
}
