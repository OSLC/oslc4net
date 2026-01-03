using OSLC4Net.Core.Model;
using TUnit.Assertions;
using TUnit.Core;

namespace OSLC4Net.Core.Tests;

public class SortedUriSetTests
{
    [Test]
    public async Task TestSortedUriSetSortingThroughCreationFactory()
    {
        // Arrange
        var factory = new CreationFactory();
        var uri1 = new Uri("http://example.com/b");
        var uri2 = new Uri("http://example.com/a");
        var uri3 = new Uri("http://example.com/c");

        // Act
        factory.AddResourceShape(uri1);
        factory.AddResourceShape(uri2);
        factory.AddResourceShape(uri3);

        var shapes = factory.GetResourceShapes();

        // Assert
        await Assert.That(shapes).HasCount(3);
        await Assert.That(shapes[0].AbsoluteUri).IsEqualTo("http://example.com/a");
        await Assert.That(shapes[1].AbsoluteUri).IsEqualTo("http://example.com/b");
        await Assert.That(shapes[2].AbsoluteUri).IsEqualTo("http://example.com/c");
    }
}
