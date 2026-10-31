/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Core.Tests;

public sealed class ValueTypeMappingTests
{
    [Test]
    public async Task AnyResourceReferencePropertiesPreserveDeclaredShapeConstraints()
    {
        ResourceShape shape = ResourceShapeFactory.CreateResourceShape(
            "https://example.test", "shapes", "reference-probe", typeof(ReferenceProbe));

        foreach (string name in new[] { "reference", "contributors" })
        {
            Property property = shape.GetProperties().Single(property => property.GetName() == name);
            await Assert.That(property.GetValueType())
                .IsEqualTo(new Uri(ValueTypeExtension.ToString(ValueType.AnyResource)));
            await Assert.That(property.GetRepresentation())
                .IsEqualTo(new Uri(RepresentationExtension.ToString(Representation.Reference)));
            await Assert.That(property.GetOccurs()).IsEqualTo(new Uri(OccursExtension.ToString(
                name == "reference" ? Occurs.ZeroOrOne : Occurs.ZeroOrMany)));
        }
    }

    [OslcNamespace("https://example.test/types#")]
    [OslcResourceShape(title = "Reference Probe", describes = new[] { "https://example.test/types#ReferenceProbe" })]
    private sealed class ReferenceProbe
    {
        [OslcName("reference")]
        [OslcPropertyDefinition("https://example.test/types#reference")]
        [OslcValueType(ValueType.AnyResource)]
        [OslcRepresentation(Representation.Reference)]
        [OslcOccurs(Occurs.ZeroOrOne)]
        public Uri? Reference { get; set; }

        [OslcName("contributors")]
        [OslcPropertyDefinition("https://example.test/types#contributors")]
        [OslcValueType(ValueType.AnyResource)]
        [OslcRepresentation(Representation.Reference)]
        [OslcOccurs(Occurs.ZeroOrMany)]
        public HashSet<Uri> Contributors { get; set; } = [];
    }

    [Test]
    public async Task UnknownValueTypeIsTheDefaultEnumValue()
    {
        await Assert.That((int)ValueType.Unknown).IsEqualTo(0);
        await Assert.That(default(ValueType)).IsEqualTo(ValueType.Unknown);
    }

    [Test]
    public async Task ValueTypeUrisRoundTripForOslcAndRdfCompatibleDatatypes()
    {
        foreach (
            ValueType valueType in Enum.GetValues<ValueType>()
                .Where(static valueType => valueType != ValueType.Unknown)
        )
        {
            string uri = ValueTypeExtension.ToString(valueType);

            await Assert.That(uri).IsNotEmpty();
            await Assert.That(ValueTypeExtension.FromString(uri)).IsEqualTo(valueType);
        }
    }

    [Test]
    public async Task ResourceShapeFactoryTreatsBinaryArraysAsScalarLiterals()
    {
        ResourceShape shape = ResourceShapeFactory.CreateResourceShape(
            "https://example.test",
            "shapes",
            "datatype-probe",
            typeof(DatatypeProbe)
        );

        Property binary = shape
            .GetProperties()
            .Single(static property => property.GetName() == "binary");
        Property integer = shape
            .GetProperties()
            .Single(static property => property.GetName() == "integer");

        await Assert
            .That(binary.GetOccurs())
            .IsEqualTo(new Uri(OccursExtension.ToString(Occurs.ZeroOrOne)));
        await Assert
            .That(binary.GetValueType())
            .IsEqualTo(new Uri(ValueTypeExtension.ToString(ValueType.Base64Binary)));
        await Assert
            .That(integer.GetValueType())
            .IsEqualTo(new Uri(ValueTypeExtension.ToString(ValueType.Integer)));
    }

    [OslcNamespace("https://example.test/types#")]
    [OslcResourceShape(
        title = "Datatype Probe",
        describes = new[] { "https://example.test/types#DatatypeProbe" }
    )]
    private sealed class DatatypeProbe
    {
        [OslcPropertyDefinition("https://example.test/types#binary")]
        [OslcValueType(ValueType.Base64Binary)]
        public byte[]? Binary { get; set; }

        [OslcPropertyDefinition("https://example.test/types#integer")]
        [OslcValueType(ValueType.Integer)]
        public long? Integer { get; set; }
    }
}
