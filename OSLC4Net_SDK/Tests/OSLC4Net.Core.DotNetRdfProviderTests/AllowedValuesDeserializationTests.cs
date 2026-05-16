using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class AllowedValuesDeserializationTests
{
    private const string SamplePayload = """
<rdf:RDF
  xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
  xmlns:oslc="http://open-services.net/ns/core#" >
  <rdf:Description rdf:about="https://jazz.net/sandbox01-ccm/oslc/context/_moCl8H3PEfC-rJM7pd93jg/shapes/workitems/com.ibm.team.workitem.workItemType.programEpic/property/category/allowedValues">
    <rdf:type rdf:resource="http://open-services.net/ns/core#AllowedValues"/>
    <oslc:allowedValue rdf:resource="https://jazz.net/sandbox01-ccm/resource/itemOid/com.ibm.team.workitem.Category/_Kt2WgH3QEfC-rJM7pd93jg"/>
    <oslc:allowedValue rdf:resource="https://jazz.net/sandbox01-ccm/resource/itemOid/com.ibm.team.workitem.Category/_Kv92YH3QEfC-rJM7pd93jg"/>
    <oslc:allowedValue rdf:resource="https://jazz.net/sandbox01-ccm/resource/itemOid/com.ibm.team.workitem.Category/_KuYiAH3QEfC-rJM7pd93jg"/>
    <oslc:allowedValue rdf:resource="https://jazz.net/sandbox01-ccm/resource/itemOid/com.ibm.team.workitem.Category/_JJu4YH3QEfC-rJM7pd93jg"/>
  </rdf:Description>
</rdf:RDF>
""";

    [Test]
    public async Task AllowedValues_resource_deserializes_uri_members()
    {
        // Arrange
        var formatter = new RdfXmlMediaTypeFormatter();
        var mediaType = OslcMediaType.APPLICATION_RDF_XML_TYPE;

        // Act
        var allowedValues = await RdfHelpers.DeserializeAsync<AllowedValuesResource<Uri>>(formatter, SamplePayload, mediaType);

        // Assert
        await Assert.That(allowedValues).IsNotNull();
        await Assert.That(allowedValues!.AllowedValues).HasCount().EqualTo(4);

        await Assert.That(allowedValues.AllowedValues)
            .Contains(new Uri("https://jazz.net/sandbox01-ccm/resource/itemOid/com.ibm.team.workitem.Category/_Kt2WgH3QEfC-rJM7pd93jg"));
    }
}
