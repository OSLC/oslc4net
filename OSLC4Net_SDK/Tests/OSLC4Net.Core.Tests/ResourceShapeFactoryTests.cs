using OSLC4Net.Core.Model;
using OSLC4Net.Domains.RequirementsManagement;
using Xunit;

namespace OSLC4Net.Core.Tests;

/// <summary>
/// 测试ResourceShapeFactory.CreateResourceShape方法
/// </summary>
public class ResourceShapeFactoryTests
{
    private const string BaseUri = "http://example.com";
    private const string ResourceShapesPath = "resourceShapes";
    private const string ResourceShapePath = "requirement";

    [Fact]
    public void CreateResourceShape_WithRequirementType_ShouldReturnValidResourceShape()
    {
        // Arrange
        var resourceType = typeof(Requirement);

        // Act
        var resourceShape = ResourceShapeFactory.CreateResourceShape(
            BaseUri,
            ResourceShapesPath,
            ResourceShapePath,
            resourceType);

        // Assert
        Assert.NotNull(resourceShape);
        Assert.NotNull(resourceShape.GetAbout());
        Assert.Equal($"{BaseUri}/{ResourceShapesPath}/{ResourceShapePath}", resourceShape.GetAbout().ToString());
    }

    [Fact]
    public void CreateResourceShape_WithRequirementType_ShouldHaveCorrectTitle()
    {
        // Arrange
        var resourceType = typeof(Requirement);

        // Act
        var resourceShape = ResourceShapeFactory.CreateResourceShape(
            BaseUri,
            ResourceShapesPath,
            ResourceShapePath,
            resourceType);

        // Assert
        Assert.NotNull(resourceShape.GetTitle());
        Assert.Equal("Requirement Resource Shape", resourceShape.GetTitle());
    }

    [Fact]
    public void CreateResourceShape_WithRequirementType_ShouldHaveDescribes()
    {
        // Arrange
        var resourceType = typeof(Requirement);

        // Act
        var resourceShape = ResourceShapeFactory.CreateResourceShape(
            BaseUri,
            ResourceShapesPath,
            ResourceShapePath,
            resourceType);

        // Assert
        var describes = resourceShape.GetDescribes();
        Assert.NotNull(describes);
        Assert.NotEmpty(describes);
        Assert.Contains(describes, uri => uri.ToString().Equals(Constants.Domains.RM.Requirement, StringComparison.Ordinal));
    }

    [Fact]
    public void CreateResourceShape_WithRequirementType_ShouldHaveProperties()
    {
        // Arrange
        var resourceType = typeof(Requirement);

        // Act
        var resourceShape = ResourceShapeFactory.CreateResourceShape(
            BaseUri,
            ResourceShapesPath,
            ResourceShapePath,
            resourceType);

        // Assert
        var properties = resourceShape.GetProperties();
        Assert.NotNull(properties);
        Assert.NotEmpty(properties);

        // ResourceShapeFactory只处理以Get或Is开头的方法，Requirement类只有从AbstractResourceRecord继承的GetTypes()方法
        var propertyNames = properties.Select(p => p.GetName()).ToList();
        Assert.Contains("type", propertyNames);
    }

    [Fact]
    public void CreateResourceShape_WithRequirementType_ShouldHaveTypeProperty()
    {
        // Arrange
        var resourceType = typeof(Requirement);

        // Act
        var resourceShape = ResourceShapeFactory.CreateResourceShape(
            BaseUri,
            ResourceShapesPath,
            ResourceShapePath,
            resourceType);

        // Assert
        var properties = resourceShape.GetProperties();
        var typeProperty = properties.FirstOrDefault(p => p.GetName() == "type");

        Assert.NotNull(typeProperty);
        Assert.Equal("type", typeProperty.GetName());
        Assert.NotNull(typeProperty.GetValueType());
        Assert.NotNull(typeProperty.GetOccurs());
        Assert.Equal("http://www.w3.org/1999/02/22-rdf-syntax-ns#type", typeProperty.GetPropertyDefinition().ToString());
    }

    [Fact]
    public void CreateResourceShape_WithRequirementType_ShouldOnlyHaveGetterMethods()
    {
        // Arrange
        var resourceType = typeof(Requirement);

        // Act
        var resourceShape = ResourceShapeFactory.CreateResourceShape(
            BaseUri,
            ResourceShapesPath,
            ResourceShapePath,
            resourceType);

        // Assert
        var properties = resourceShape.GetProperties();

        Assert.Single(properties);
        Assert.Equal("type", properties[0].GetName());
    }

}
