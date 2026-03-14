using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

[OslcResourceShape(title = "Test Shape", describes = new[] { "http://example.com/TestResource" })]
[OslcNamespace("http://example.com/ns#")]
public class TestResourceWithTypesProperty : AbstractResource
{
    [OslcDescription("Test property")]
    [OslcName("name")]
    [OslcPropertyDefinition("http://example.com/ns#name")]
    [OslcTitle("Name")]
    public string Name { get; set; } = "";
}

public class ResourceShapePropertyAnnotationTests
{
    [Test]
    public async Task CreateResourceShape_DiscoversTypesPropertyAnnotation()
    {
        var shape = ResourceShapeFactory.CreateResourceShape(
            "http://example.com",
            OslcConstants.PATH_RESOURCE_SHAPES,
            "changeRequest",
            typeof(ChangeRequest));

        var properties = shape.GetProperties();
        var typeProperty = properties.FirstOrDefault(p =>
            p.GetPropertyDefinition().ToString() == OslcConstants.RDF_NAMESPACE + "type");

        await Assert.That(typeProperty).IsNotNull();
        await Assert.That(typeProperty!.GetName()).IsEqualTo("type");
        await Assert.That(typeProperty.GetTitle()).IsEqualTo("Types");
    }

    [Test]
    public async Task CreateResourceShape_NoDuplicateRdfTypeProperty()
    {
        var shape = ResourceShapeFactory.CreateResourceShape(
            "http://example.com",
            OslcConstants.PATH_RESOURCE_SHAPES,
            "changeRequest",
            typeof(ChangeRequest));

        var properties = shape.GetProperties();
        var typeProperties = properties.Where(p =>
            p.GetPropertyDefinition().ToString() == OslcConstants.RDF_NAMESPACE + "type").ToList();

        // Should be exactly one rdf:type property (from the Types property, not from GetTypes)
        await Assert.That(typeProperties.Count).IsEqualTo(1);
    }

    [Test]
    public async Task CreateResourceShape_MinimalResource_DiscoversTypesAndCustomProperty()
    {
        var shape = ResourceShapeFactory.CreateResourceShape(
            "http://example.com",
            OslcConstants.PATH_RESOURCE_SHAPES,
            "testResource",
            typeof(TestResourceWithTypesProperty));

        var properties = shape.GetProperties();

        var nameProperty = properties.FirstOrDefault(p =>
            p.GetPropertyDefinition().ToString() == "http://example.com/ns#name");
        await Assert.That(nameProperty).IsNotNull();

        var typeProperty = properties.FirstOrDefault(p =>
            p.GetPropertyDefinition().ToString() == OslcConstants.RDF_NAMESPACE + "type");
        await Assert.That(typeProperty).IsNotNull();
    }
}
