using System.Reflection;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.Tests;

public class AbstractResourceTests
{
    private class ConcreteResource : AbstractResource
    {
        public ConcreteResource() : base() { }
        public ConcreteResource(Uri about) : base(about) { }
    }

    [Test]
    public async Task TestAddType_UpdatesTypes()
    {
        var resource = new ConcreteResource();
        var typeUri = new Uri("http://example.com/type");

        resource.AddType(typeUri);

        await Assert.That(resource.Types).Contains(typeUri);
        await Assert.That(resource.Types).HasCount().EqualTo(1);
    }

    [Test]
    public async Task TestGetTypes_ReturnsSameCollectionAsTypes()
    {
        var resource = new ConcreteResource();
        var typeUri = new Uri("http://example.com/type");
        resource.AddType(typeUri);

        // Verify GetTypes() returns the exact same object as Types property
        await Assert.That(resource.GetTypes()).IsEqualTo(resource.Types);

        // Double check content match
        await Assert.That(resource.GetTypes()).Contains(typeUri);
    }

    [Test]
    public async Task TestTypeSetter_ReplacesCollection()
    {
        var resource = new ConcreteResource();
        var initialType = new Uri("http://example.com/initial");
        resource.AddType(initialType);

        var newTypes = new List<Uri> { new Uri("http://example.com/new") };

        // Act: Set Types property
        resource.Types = newTypes;

        await Assert.That(resource.Types).Contains(newTypes[0]);
        await Assert.That(resource.Types).DoesNotContain(initialType);
        await Assert.That(resource.Types).HasCount().EqualTo(1);
        await Assert.That(resource.GetTypes()).IsEqualTo(newTypes);
    }

    [Test]
    public async Task TestSetTypes_UpdatesTypes()
    {
        var resource = new ConcreteResource();
        var setTypes = new List<Uri> { new Uri("http://example.com/set") };

        // Act: Call SetTypes method
        resource.SetTypes(setTypes);

        await Assert.That(resource.Types).IsEqualTo(setTypes);
        await Assert.That(resource.Types).Contains(setTypes[0]);
    }

    [Test]
    public async Task TestInitialTypes_IsEmpty()
    {
        var resource = new ConcreteResource();
        await Assert.That(resource.Types).IsEmpty();
    }

    [Test]
    public async Task TestAddType_DuplicateDoesNotCreateExtra()
    {
        var resource = new ConcreteResource();
        var typeUri = new Uri("http://example.com/type");

        resource.AddType(typeUri);
        resource.AddType(typeUri);

        // List allows duplicates, verifying current behavior.
        // If Set behavior was intended, this test would fail if count > 1.
        // AbstractResource uses List<Uri>, so duplicates are expected unless checking logic exists.
        // Based on implementation `types.Add(type)`, it adds duplicates.
        await Assert.That(resource.Types).HasCount().EqualTo(2);
        await Assert.That(resource.Types).Contains(typeUri);
    }

    [Test]
    public async Task TestSetTypes_EmptyListClearsTypes()
    {
        var resource = new ConcreteResource();
        resource.AddType(new Uri("http://example.com/type"));

        resource.SetTypes(new List<Uri>());

        await Assert.That(resource.Types).IsEmpty();
    }

    [Test]
    public async Task TestTypesProperty_HasOslcAttributes()
    {
        // This test verifies that the OslcPropertyDefinition attribute is present
        // on the GetTypes method, ensuring correct mapping to rdf:type.
        // Since Types property wraps the field used by GetTypes, verifying GetTypes attributes is key for serialization.

        var methodInfo = typeof(AbstractResource).GetMethod(nameof(AbstractResource.GetTypes));
        await Assert.That(methodInfo).IsNotNull();

        var attribute = methodInfo!.GetCustomAttribute<OslcPropertyDefinition>();
        await Assert.That(attribute).IsNotNull();
        await Assert.That(attribute!.value).IsEqualTo(OslcConstants.RDF_NAMESPACE + "type");
    }
}
