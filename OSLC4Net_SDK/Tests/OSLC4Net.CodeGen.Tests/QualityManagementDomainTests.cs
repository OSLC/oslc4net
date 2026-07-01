// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using OSLC4Net.Domains.QualityManagement;

namespace OSLC4Net.CodeGen.Tests;

public sealed class QualityManagementDomainTests
{
    [Test]
    public async Task VocabularyConstantsAreGenerated()
    {
        await Assert.That(QM.NS).IsEqualTo("http://open-services.net/ns/qm#");
        await Assert.That(QM.Prefix).IsEqualTo("oslc_qm");
        await Assert.That(QM.TestPlan).IsEqualTo("http://open-services.net/ns/qm#TestPlan");
        await Assert.That(QM.P.UsesTestCase).IsEqualTo("http://open-services.net/ns/qm#usesTestCase");
        await Assert.That(QM.Q.UsesTestCase).IsEqualTo(new QName(QM.NS, "usesTestCase", QM.Prefix));
    }

    [Test]
    public async Task TestPlanShapeMetadataIsGenerated()
    {
        Type testPlanType = typeof(TestPlan);

        OslcNamespace? namespaceAttribute = Attribute.GetCustomAttribute(testPlanType, typeof(OslcNamespace)) as OslcNamespace;
        OslcResourceShape? shapeAttribute = Attribute.GetCustomAttribute(testPlanType, typeof(OslcResourceShape)) as OslcResourceShape;

        await Assert.That(namespaceAttribute?.value).IsEqualTo(QM.NS);
        await Assert.That(shapeAttribute?.describes).IsEquivalentTo([QM.TestPlan]);
        await Assert.That(shapeAttribute?.title).IsEqualTo("QM Test Plan");
    }

    [Test]
    public async Task ShapePropertiesAreGenerated()
    {
        OslcPropertyDefinition? propertyDefinition = Attribute.GetCustomAttribute(
            typeof(TestPlan).GetProperty(nameof(TestPlan.UsesTestCase))!,
            typeof(OslcPropertyDefinition)) as OslcPropertyDefinition;
        OslcRange? range = Attribute.GetCustomAttribute(
            typeof(TestPlan).GetProperty(nameof(TestPlan.UsesTestCase))!,
            typeof(OslcRange)) as OslcRange;

        TestPlan testPlan = new(new Uri("https://example.test/test-plan/1"));
        testPlan.UsesTestCase.Add(new Uri("https://example.test/test-case/1"));

        await Assert.That(propertyDefinition?.value).IsEqualTo(QM.P.UsesTestCase);
        await Assert.That(range?.value).IsEquivalentTo([QM.TestCase]);
        await Assert.That(testPlan.UsesTestCase).Contains(new Uri("https://example.test/test-case/1"));
    }

    [Test]
    public async Task GeneratedShapeWorksWithResourceShapeFactory()
    {
        ResourceShape shape = ResourceShapeFactory.CreateResourceShape(
            "https://example.test",
            "resourceShapes",
            "testPlan",
            typeof(TestPlan));

        Property[] usesTestCaseProperties = shape.GetProperties()
            .Where(property => property.GetPropertyDefinition() == new Uri(QM.P.UsesTestCase))
            .ToArray();

        await Assert.That(shape.GetDescribes()).IsEquivalentTo([new Uri(QM.TestPlan)]);
        await Assert.That(usesTestCaseProperties).IsNotEmpty();
        await Assert.That(usesTestCaseProperties.SelectMany(property => property.GetRange()))
            .Contains(new Uri(QM.TestCase));
        await Assert.That(usesTestCaseProperties.Select(property => property.GetValueType()))
            .Contains(new Uri("http://open-services.net/ns/core#Resource"));
    }
}
