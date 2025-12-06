using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class AllowedValuesSerializationTests
{
    [Test]
    public async Task TestAllowedValuesRoundTrip()
    {
        // Arrange
        var allowedValues = new AllowedValues();
        allowedValues.AddAllowedValue("http://example.com/values/high");
        allowedValues.AddAllowedValue("http://example.com/values/medium");
        allowedValues.AddAllowedValue("http://example.com/values/low");

        var formatter = new RdfXmlMediaTypeFormatter();
        var mediaType = OslcMediaType.APPLICATION_RDF_XML_TYPE;

        var rdfXml = await RdfHelpers.SerializeAsync(formatter, allowedValues, mediaType);
        var deserialized = await RdfHelpers.DeserializeAsync<AllowedValues>(formatter, rdfXml, mediaType);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized.GetAllowedValues()).HasCount().EqualTo(allowedValues.GetAllowedValues().Length);
        
        foreach (var val in allowedValues.GetAllowedValues())
        {
            await Assert.That(deserialized.GetAllowedValues()).Contains(val);
        }

        await Verify(deserialized);
    }
}

