using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Core.Tests;

public class EnumExtensionTests
{
    [Test]
    public async Task TestValueTypeFromURI()
    {
        // ValueType.Boolean -> http://www.w3.org/2001/XMLSchema#boolean
        var uriString = OslcConstants.XML_NAMESPACE + "boolean";
        var uri = new URI(uriString);
        var valueType = ValueTypeExtension.FromURI(uri);

        await Assert.That(valueType).IsEqualTo(ValueType.Boolean);
    }

    [Test]
    public async Task TestOccursFromURI()
    {
        // Occurs.ExactlyOne -> http://open-services.net/ns/core#Exactly-one
        var uriString = OslcConstants.OSLC_CORE_NAMESPACE + "Exactly-one";
        var uri = new URI(uriString);
        var occurs = OccursExtension.FromURI(uri);

        await Assert.That(occurs).IsEqualTo(Occurs.ExactlyOne);
    }

    [Test]
    public async Task TestRepresentationFromURI()
    {
        // Representation.Reference -> http://open-services.net/ns/core#Reference
        var uriString = OslcConstants.OSLC_CORE_NAMESPACE + "Reference";
        var uri = new URI(uriString);
        var representation = RepresentationExtension.FromURI(uri);

        await Assert.That(representation).IsEqualTo(Representation.Reference);
    }
}
