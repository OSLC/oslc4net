// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using OSLC4Net.Domains.RequirementsManagement;

namespace OSLC4Net.CodeGen.Tests;

public sealed class RequirementsManagementDomainTests
{
    [Test]
    public async Task VocabularyMatchesCompatibilityConstants()
    {
        await Assert.That(RM.NS).IsEqualTo(Constants.Domains.RM.NS);
        await Assert.That(RM.Prefix).IsEqualTo(Constants.Domains.RM.Prefix);
        await Assert.That(RM.Requirement).IsEqualTo(Constants.Domains.RM.Requirement);
        await Assert.That(RM.RequirementCollection).IsEqualTo(Constants.Domains.RM.RequirementCollection);
        await Assert.That(RM.P.ManagedBy).IsEqualTo(Constants.Domains.RM.P.ManagedBy);
        await Assert.That(RM.Q.ManagedBy).IsEqualTo(Constants.Domains.RM.Q.ManagedBy);
        await Assert.That(RM.P.ValidatedBy).IsEqualTo(Constants.Domains.RM.P.ValidatedBy);
        await Assert.That(RM.Q.ValidatedBy).IsEqualTo(Constants.Domains.RM.Q.ValidatedBy);
    }

    [Test]
    public async Task RequirementShapeHasExpectedMetadata()
    {
        OslcResourceShape? shapeAttribute = Attribute.GetCustomAttribute(
            typeof(Requirement),
            typeof(OslcResourceShape)) as OslcResourceShape;

        OslcNamespace? namespaceAttribute = Attribute.GetCustomAttribute(
            typeof(Requirement),
            typeof(OslcNamespace)) as OslcNamespace;

        await Assert.That(namespaceAttribute?.value).IsEqualTo(RM.NS);
        await Assert.That(shapeAttribute?.describes).IsEquivalentTo([RM.Requirement]);
        await Assert.That(shapeAttribute?.title).IsEqualTo("Requirement Resource Shape");
    }

    [Test]
    public async Task RequirementPropertiesMatchKnownShapeSurface()
    {
        string[] propertyDefinitions = typeof(Requirement)
            .GetProperties()
            .Select(property => Attribute.GetCustomAttribute(property, typeof(OslcPropertyDefinition)) as OslcPropertyDefinition)
            .Where(attribute => attribute is not null)
            .Select(attribute => attribute!.value)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        await Assert.That(propertyDefinitions).Contains("http://purl.org/dc/terms/title");
        await Assert.That(propertyDefinitions).Contains("http://open-services.net/ns/rm#validatedBy");
        await Assert.That(propertyDefinitions).Contains("http://open-services.net/ns/rm#satisfiedBy");
        await Assert.That(propertyDefinitions).Contains("http://open-services.net/ns/core#serviceProvider");
        await Assert.That(propertyDefinitions.Length).IsGreaterThanOrEqualTo(24);
    }

    [Test]
    public async Task RequirementCollectionPropertiesMatchKnownShapeSurface()
    {
        string[] propertyDefinitions = typeof(RequirementCollection)
            .GetProperties()
            .Select(property => Attribute.GetCustomAttribute(property, typeof(OslcPropertyDefinition)) as OslcPropertyDefinition)
            .Where(attribute => attribute is not null)
            .Select(attribute => attribute!.value)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        await Assert.That(propertyDefinitions).Contains("http://open-services.net/ns/rm#uses");
        await Assert.That(propertyDefinitions).Contains("http://open-services.net/ns/rm#managedBy");
        await Assert.That(propertyDefinitions).Contains("http://open-services.net/ns/rm#trackedBy");
        await Assert.That(propertyDefinitions).Contains("http://purl.org/dc/terms/description");
        await Assert.That(propertyDefinitions.Length).IsGreaterThanOrEqualTo(24);
    }

    [Test]
    public async Task GeneratedRequirementWorksWithResourceShapeFactory()
    {
        ResourceShape shape = ResourceShapeFactory.CreateResourceShape(
            "https://example.test",
            "resourceShapes",
            "requirement",
            typeof(Requirement));

        Property[] validatedByProperties = shape.GetProperties()
            .Where(property => property.GetPropertyDefinition() == new Uri(RM.P.ValidatedBy))
            .ToArray();

        await Assert.That(shape.GetDescribes()).IsEquivalentTo([new Uri(RM.Requirement)]);
        await Assert.That(validatedByProperties).IsNotEmpty();
        await Assert.That(validatedByProperties.Select(property => property.GetValueType()))
            .Contains(new Uri("http://open-services.net/ns/core#Resource"));
        await Assert.That(validatedByProperties.Select(property => property.GetOccurs()))
            .Contains(new Uri("http://open-services.net/ns/core#Zero-or-many"));
    }

    [Test]
    public async Task GeneratedUriSetsUseFragmentAwareComparer()
    {
        Requirement requirement = new();

        requirement.ValidatedBy.Add(new Uri("https://example.test/resource#one"));
        requirement.ValidatedBy.Add(new Uri("https://example.test/resource#two"));

        await Assert.That(requirement.ValidatedBy.Count).IsEqualTo(2);
    }

    [Test]
    public async Task RegularClassGenerationInheritsAbstractResource()
    {
        GeneratedRequirementClass resource = new(new Uri("https://example.test/requirement/1"));

        resource.ValidatedBy.Add(new Uri("https://example.test/test-case#one"));
        resource.ValidatedBy.Add(new Uri("https://example.test/test-case#two"));

        await Assert.That(resource is AbstractResource).IsTrue();
        await Assert.That(resource.GetAbout()).IsEqualTo(new Uri("https://example.test/requirement/1"));
        await Assert.That(resource.ValidatedBy.Count).IsEqualTo(2);
    }
}
