using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

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

        // Act
        var rdfXml = await SerializeAsync(formatter, allowedValues, mediaType);
        
        // Verify serialization
        await Verify(rdfXml, "xml");

        // Act - Deserialize
        var deserialized = await DeserializeAsync<AllowedValues>(formatter, rdfXml, mediaType);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized.GetAllowedValues()).HasCount().EqualTo(3);
        await Assert.That(deserialized.GetAllowedValues()).Contains("http://example.com/values/high");
        await Assert.That(deserialized.GetAllowedValues()).Contains("http://example.com/values/medium");
        await Assert.That(deserialized.GetAllowedValues()).Contains("http://example.com/values/low");
    }

    private static async Task<string> SerializeAsync<T>(MediaTypeFormatter formatter, T value, MediaTypeHeaderValue mediaType)
    {
        var stream = new MemoryStream();
        var content = new StringContent(string.Empty);
        content.Headers.ContentType = mediaType;

        await formatter.WriteToStreamAsync(typeof(T), value, stream, content, null);

        stream.Position = 0;
        return await new StreamReader(stream).ReadToEndAsync();
    }

    private static async Task<T> DeserializeAsync<T>(MediaTypeFormatter formatter, string str, MediaTypeHeaderValue mediaType)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(str);
        await writer.FlushAsync();
        stream.Position = 0;

        var content = new StreamContent(stream);
        content.Headers.ContentType = mediaType;

        return (T)await formatter.ReadFromStreamAsync(typeof(T), stream, content, null);
    }
}

