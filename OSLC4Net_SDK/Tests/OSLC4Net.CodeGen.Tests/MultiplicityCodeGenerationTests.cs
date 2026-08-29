/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using System.Reflection;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.CodeGen.Tests;

public sealed class MultiplicityCodeGenerationTests
{
    [Test]
    public async Task GeneratedPropertiesUseExpectedClrTypesForAllOccursValues()
    {
        await AssertProperty(
            nameof(MultiplicityProbe.RequiredString),
            typeof(string),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalString),
            typeof(string),
            NullabilityState.Nullable
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ZeroManyString),
            typeof(HashSet<string>),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OneManyString),
            typeof(HashSet<string>),
            NullabilityState.NotNull
        );

        await AssertProperty(
            nameof(MultiplicityProbe.RequiredResource),
            typeof(Uri),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalResource),
            typeof(Uri),
            NullabilityState.Nullable
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ZeroManyResource),
            typeof(HashSet<Uri>),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OneManyResource),
            typeof(HashSet<Uri>),
            NullabilityState.NotNull
        );

        await AssertProperty(
            nameof(MultiplicityProbe.OptionalAnyResource),
            typeof(Uri),
            NullabilityState.Nullable,
            ValueType.AnyResource
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalLocalResource),
            typeof(Uri),
            NullabilityState.Nullable,
            ValueType.LocalResource
        );

        await AssertProperty(
            nameof(MultiplicityProbe.RequiredBoolean),
            typeof(bool),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalBoolean),
            typeof(bool?),
            NullabilityState.Nullable
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ZeroManyBoolean),
            typeof(HashSet<bool>),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OneManyBoolean),
            typeof(HashSet<bool>),
            NullabilityState.NotNull
        );

        await AssertProperty(
            nameof(MultiplicityProbe.RequiredDateTime),
            typeof(DateTimeOffset),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalDateTime),
            typeof(DateTimeOffset?),
            NullabilityState.Nullable
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ZeroManyDateTime),
            typeof(HashSet<DateTimeOffset>),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OneManyDateTime),
            typeof(HashSet<DateTimeOffset>),
            NullabilityState.NotNull
        );

        // Known deviation: RDF/XSD integer is unbounded, but generated domains use long for now.
        await AssertProperty(
            nameof(MultiplicityProbe.RequiredInteger),
            typeof(long),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalInteger),
            typeof(long?),
            NullabilityState.Nullable
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ZeroManyInteger),
            typeof(HashSet<long>),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OneManyInteger),
            typeof(HashSet<long>),
            NullabilityState.NotNull
        );

        await AssertProperty(
            nameof(MultiplicityProbe.RequiredDecimal),
            typeof(decimal),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OptionalDecimal),
            typeof(decimal?),
            NullabilityState.Nullable
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ZeroManyDecimal),
            typeof(HashSet<decimal>),
            NullabilityState.NotNull
        );
        await AssertProperty(
            nameof(MultiplicityProbe.OneManyDecimal),
            typeof(HashSet<decimal>),
            NullabilityState.NotNull
        );
    }

    [Test]
    public async Task GeneratedPropertiesUseExpectedClrTypesForRdfCompatibleDatatypes()
    {
        await AssertProperty(
            nameof(MultiplicityProbe.AnyUriValue),
            typeof(Uri),
            NullabilityState.Nullable,
            ValueType.AnyUri
        );
        await AssertProperty(
            nameof(MultiplicityProbe.Base64BinaryValue),
            typeof(byte[]),
            NullabilityState.Nullable,
            ValueType.Base64Binary
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ByteValue),
            typeof(sbyte?),
            NullabilityState.Nullable,
            ValueType.Byte
        );
        await AssertProperty(
            nameof(MultiplicityProbe.DateValue),
            typeof(DateOnly?),
            NullabilityState.Nullable,
            ValueType.Date
        );
        await AssertProperty(
            nameof(MultiplicityProbe.DateTimeStampValue),
            typeof(DateTimeOffset?),
            NullabilityState.Nullable,
            ValueType.DateTimeStamp
        );
        await AssertProperty(
            nameof(MultiplicityProbe.DayTimeDurationValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.DayTimeDuration
        );
        await AssertProperty(
            nameof(MultiplicityProbe.DoubleValue),
            typeof(double?),
            NullabilityState.Nullable,
            ValueType.Double
        );
        await AssertProperty(
            nameof(MultiplicityProbe.DurationValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Duration
        );
        await AssertProperty(
            nameof(MultiplicityProbe.FloatValue),
            typeof(float?),
            NullabilityState.Nullable,
            ValueType.Float
        );
        await AssertProperty(
            nameof(MultiplicityProbe.GDayValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.GDay
        );
        await AssertProperty(
            nameof(MultiplicityProbe.GMonthValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.GMonth
        );
        await AssertProperty(
            nameof(MultiplicityProbe.GMonthDayValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.GMonthDay
        );
        await AssertProperty(
            nameof(MultiplicityProbe.GYearValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.GYear
        );
        await AssertProperty(
            nameof(MultiplicityProbe.GYearMonthValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.GYearMonth
        );
        await AssertProperty(
            nameof(MultiplicityProbe.HexBinaryValue),
            typeof(byte[]),
            NullabilityState.Nullable,
            ValueType.HexBinary
        );
        await AssertProperty(
            nameof(MultiplicityProbe.IntValue),
            typeof(int?),
            NullabilityState.Nullable,
            ValueType.Int
        );
        await AssertProperty(
            nameof(MultiplicityProbe.LanguageValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Language
        );
        await AssertProperty(
            nameof(MultiplicityProbe.LongValue),
            typeof(long?),
            NullabilityState.Nullable,
            ValueType.Long
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NameValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Name
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NCNameValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.NCName
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NegativeIntegerValue),
            typeof(long?),
            NullabilityState.Nullable,
            ValueType.NegativeInteger
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NmtokenValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Nmtoken
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NonNegativeIntegerValue),
            typeof(long?),
            NullabilityState.Nullable,
            ValueType.NonNegativeInteger
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NonPositiveIntegerValue),
            typeof(long?),
            NullabilityState.Nullable,
            ValueType.NonPositiveInteger
        );
        await AssertProperty(
            nameof(MultiplicityProbe.NormalizedStringValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.NormalizedString
        );
        await AssertProperty(
            nameof(MultiplicityProbe.PositiveIntegerValue),
            typeof(long?),
            NullabilityState.Nullable,
            ValueType.PositiveInteger
        );
        await AssertProperty(
            nameof(MultiplicityProbe.ShortValue),
            typeof(short?),
            NullabilityState.Nullable,
            ValueType.Short
        );
        await AssertProperty(
            nameof(MultiplicityProbe.TimeValue),
            typeof(TimeOnly?),
            NullabilityState.Nullable,
            ValueType.Time
        );
        await AssertProperty(
            nameof(MultiplicityProbe.TokenValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Token
        );
        await AssertProperty(
            nameof(MultiplicityProbe.UnsignedByteValue),
            typeof(byte?),
            NullabilityState.Nullable,
            ValueType.UnsignedByte
        );
        await AssertProperty(
            nameof(MultiplicityProbe.UnsignedIntValue),
            typeof(uint?),
            NullabilityState.Nullable,
            ValueType.UnsignedInt
        );
        await AssertProperty(
            nameof(MultiplicityProbe.UnsignedLongValue),
            typeof(ulong?),
            NullabilityState.Nullable,
            ValueType.UnsignedLong
        );
        await AssertProperty(
            nameof(MultiplicityProbe.UnsignedShortValue),
            typeof(ushort?),
            NullabilityState.Nullable,
            ValueType.UnsignedShort
        );
        await AssertProperty(
            nameof(MultiplicityProbe.YearMonthDurationValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.YearMonthDuration
        );
        await AssertProperty(
            nameof(MultiplicityProbe.DirLangStringValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.DirLangString
        );
        await AssertProperty(
            nameof(MultiplicityProbe.HtmlValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Html
        );
        await AssertProperty(
            nameof(MultiplicityProbe.JsonValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.Json
        );
        await AssertProperty(
            nameof(MultiplicityProbe.LangStringValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.LangString
        );
        await AssertProperty(
            nameof(MultiplicityProbe.XmlLiteralValue),
            typeof(string),
            NullabilityState.Nullable,
            ValueType.XMLLiteral
        );
        await AssertProperty(
            nameof(MultiplicityProbe.AnyResourceValue),
            typeof(Uri),
            NullabilityState.Nullable,
            ValueType.AnyResource
        );
    }

    [Test]
    public async Task GeneratedUriSetsUseFragmentAwareComparerForAllManyOccursValues()
    {
        MultiplicityProbe probe = new();

        probe.ZeroManyResource.Add(new Uri("https://example.test/resource#one"));
        probe.ZeroManyResource.Add(new Uri("https://example.test/resource#two"));
        probe.OneManyResource.Add(new Uri("https://example.test/other#one"));
        probe.OneManyResource.Add(new Uri("https://example.test/other#two"));

        await Assert
            .That(probe.ZeroManyResource.Comparer)
            .IsSameReferenceAs(OslcUriEqualityComparer.Instance);
        await Assert
            .That(probe.OneManyResource.Comparer)
            .IsSameReferenceAs(OslcUriEqualityComparer.Instance);
        await Assert.That(probe.ZeroManyResource.Count).IsEqualTo(2);
        await Assert.That(probe.OneManyResource.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GeneratorResolvesConstantAttributeArguments()
    {
        OslcResourceShape? shapeAttribute =
            typeof(ConstantAttributeProbe).GetCustomAttribute<OslcResourceShape>();

        await Assert.That(MultiplicityVocabulary.NS).IsEqualTo(MultiplicityUris.Vocabulary);
        await Assert.That(shapeAttribute?.title).IsEqualTo(MultiplicityUris.ShapeTitle);
    }

    private static async Task AssertProperty(
        string propertyName,
        Type expectedType,
        NullabilityState expectedReadState,
        ValueType? expectedValueType = null
    )
    {
        PropertyInfo property = typeof(MultiplicityProbe).GetProperty(propertyName)!;
        NullabilityInfo nullability = new NullabilityInfoContext().Create(property);

        await Assert.That(property.PropertyType).IsEqualTo(expectedType);
        await Assert.That(nullability.ReadState).IsEqualTo(expectedReadState);

        if (expectedValueType is { } valueType)
        {
            OslcValueType? attribute = property.GetCustomAttribute<OslcValueType>();
            await Assert.That(attribute?.value).IsEqualTo(valueType);
        }
    }
}
