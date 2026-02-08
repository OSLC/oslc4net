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
    public async Task TestTypesProperty()
    {
        var resource = new ConcreteResource();
        var typeUri = new Uri("http://example.com/type");

        // Test AddType updates Types
        resource.AddType(typeUri);
        await Assert.That(resource.Types).Contains(typeUri);
        await Assert.That(resource.Types).HasCount().EqualTo(1);

        // Test GetTypes reflects Types
        await Assert.That(resource.GetTypes()).IsEqualTo(resource.Types);

        // Test setting Types
        var newTypes = new List<Uri> { new Uri("http://example.com/type2") };
        resource.Types = newTypes;
        await Assert.That(resource.GetTypes()).IsEqualTo(newTypes);
        await Assert.That(resource.Types).Contains(newTypes[0]);
        await Assert.That(resource.Types).DoesNotContain(typeUri);

        // Test SetTypes updates Types
        var setTypes = new List<Uri> { new Uri("http://example.com/type3") };
        resource.SetTypes(setTypes);
        await Assert.That(resource.Types).IsEqualTo(setTypes);
    }
}
