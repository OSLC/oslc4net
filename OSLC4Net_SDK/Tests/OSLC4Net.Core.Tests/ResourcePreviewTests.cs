using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using TUnit;
using TUnit.Assertions;

namespace OSLC4Net.Core.Tests;

public class ResourcePreviewTests
{
    [Test]
    public async Task Compact_PropertyAndObsoleteGetterSetterEquivalency()
    {
        var compact = new Compact();
        var iconUri = new Uri("http://example.com/icon.png");
        var smallPreview = new Preview();
        var largePreview = new Preview();

        // 1. Test standard properties
        compact.Title = "Test Title";
        compact.ShortTitle = "Short";
        compact.Icon = iconUri;
        compact.SmallPreview = smallPreview;
        compact.LargePreview = largePreview;

        await Assert.That(compact.Title).IsEqualTo("Test Title");
        await Assert.That(compact.ShortTitle).IsEqualTo("Short");
        await Assert.That(compact.Icon).IsEqualTo(iconUri);
        await Assert.That(compact.SmallPreview).IsSameReferenceAs(smallPreview);
        await Assert.That(compact.LargePreview).IsSameReferenceAs(largePreview);

        // Test obsolete getters
#pragma warning disable CS0618 // Type or member is obsolete
        await Assert.That(compact.GetTitle()).IsEqualTo("Test Title");
        await Assert.That(compact.GetShortTitle()).IsEqualTo("Short");
        await Assert.That(compact.GetIcon()).IsEqualTo(iconUri);
        await Assert.That(compact.GetSmallPreview()).IsSameReferenceAs(smallPreview);
        await Assert.That(compact.GetLargePreview()).IsSameReferenceAs(largePreview);

        // Test obsolete setters
        var newIconUri = new Uri("http://example.com/newicon.png");
        var newSmall = new Preview();
        var newLarge = new Preview();

        compact.SetTitle("New Title");
        compact.SetShortTitle("New Short");
        compact.SetIcon(newIconUri);
        compact.SetSmallPreview(newSmall);
        compact.SetLargePreview(newLarge);
#pragma warning restore CS0618

        await Assert.That(compact.Title).IsEqualTo("New Title");
        await Assert.That(compact.ShortTitle).IsEqualTo("New Short");
        await Assert.That(compact.Icon).IsEqualTo(newIconUri);
        await Assert.That(compact.SmallPreview).IsSameReferenceAs(newSmall);
        await Assert.That(compact.LargePreview).IsSameReferenceAs(newLarge);
    }

    [Test]
    public async Task Compact_NewOSLCCore30Properties()
    {
        var compact = new Compact();

        compact.IconTitle = "My Icon Title";
        compact.IconAltLabel = "My Icon Alt";
        compact.IconSrcSet = "icon1.png 1x, icon2.png 2x";

        await Assert.That(compact.IconTitle).IsEqualTo("My Icon Title");
        await Assert.That(compact.IconAltLabel).IsEqualTo("My Icon Alt");
        await Assert.That(compact.IconSrcSet).IsEqualTo("icon1.png 1x, icon2.png 2x");
    }

    [Test]
    public async Task Preview_PropertyAndObsoleteGetterSetterEquivalency()
    {
        var preview = new Preview();
        var docUri = new Uri("http://example.com/doc.html");

        preview.Document = docUri;
        preview.HintWidth = "100px";
        preview.HintHeight = "200px";
        preview.InitialHeight = "150px";

        await Assert.That(preview.Document).IsEqualTo(docUri);
        await Assert.That(preview.HintWidth).IsEqualTo("100px");
        await Assert.That(preview.HintHeight).IsEqualTo("200px");
        await Assert.That(preview.InitialHeight).IsEqualTo("150px");

#pragma warning disable CS0618 // Type or member is obsolete
        await Assert.That(preview.GetDocument()).IsEqualTo(docUri);
        await Assert.That(preview.GetHintWidth()).IsEqualTo("100px");
        await Assert.That(preview.GetHintHeight()).IsEqualTo("200px");
        await Assert.That(preview.GetInitialHeight()).IsEqualTo("150px");

        var newDoc = new Uri("http://example.com/new.html");
        preview.SetDocument(newDoc);
        preview.SetHintWidth("300px");
        preview.SetHintHeight("400px");
        preview.SetInitialHeight("350px");
#pragma warning restore CS0618

        await Assert.That(preview.Document).IsEqualTo(newDoc);
        await Assert.That(preview.HintWidth).IsEqualTo("300px");
        await Assert.That(preview.HintHeight).IsEqualTo("400px");
        await Assert.That(preview.InitialHeight).IsEqualTo("350px");
    }

    [Test]
    public async Task Compact_ResourceShapeGeneration()
    {
        var shape = ResourceShapeFactory.CreateResourceShape(
            "http://example.com",
            "shapes",
            "compact",
            typeof(Compact));

        await Assert.That(shape).IsNotNull();
        var properties = shape.GetProperties();
        await Assert.That(properties).IsNotEmpty();

        var propertyNames = properties.Select(p => p.GetName()).ToList();

        // Verify OSLC 2.0 properties
        await Assert.That(propertyNames).Contains("title");
        await Assert.That(propertyNames).Contains("shortTitle");
        await Assert.That(propertyNames).Contains("icon");
        await Assert.That(propertyNames).Contains("smallPreview");
        await Assert.That(propertyNames).Contains("largePreview");

        // Verify OSLC 3.0 properties are present in the shape metadata
        await Assert.That(propertyNames).Contains("iconTitle");
        await Assert.That(propertyNames).Contains("iconAltLabel");
        await Assert.That(propertyNames).Contains("iconSrcSet");
    }
}
