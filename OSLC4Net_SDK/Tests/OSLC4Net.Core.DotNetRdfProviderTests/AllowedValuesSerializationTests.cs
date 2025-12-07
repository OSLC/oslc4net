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
        var allowedValues = new AllowedValuesResource<string>();
        allowedValues.AddAllowedValue("http://example.com/values/high");
        allowedValues.AddAllowedValue("http://example.com/values/medium");
        allowedValues.AddAllowedValue("http://example.com/values/low");

        var formatter = new RdfXmlMediaTypeFormatter();
        var mediaType = OslcMediaType.APPLICATION_RDF_XML_TYPE;

        var rdfXml = await RdfHelpers.SerializeAsync(formatter, allowedValues, mediaType);
        var deserialized = await RdfHelpers.DeserializeAsync<AllowedValuesResource<string>>(formatter, rdfXml, mediaType);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized.AllowedValues).HasCount().EqualTo(allowedValues.AllowedValues.Count);

        foreach (var val in allowedValues.AllowedValues)
        {
            await Assert.That(deserialized.AllowedValues).Contains(val);
        }

        await Verify(deserialized);
    }
}

