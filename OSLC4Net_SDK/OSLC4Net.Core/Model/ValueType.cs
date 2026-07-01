/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

namespace OSLC4Net.Core.Model;

/// <summary>
///     OSLC ValueType attribute
/// </summary>
/// <remarks>see https://docs.oasis-open-projects.org/oslc-op/core/v3.0/os/core-vocab.html</remarks>
public enum ValueType
{
    [URI("")]
    Unknown = 0,

    [URI(OslcConstants.XML_NAMESPACE + "anyURI")]
    AnyUri,

    [URI(OslcConstants.XML_NAMESPACE + "base64Binary")]
    Base64Binary,

    [URI(OslcConstants.XML_NAMESPACE + "boolean")]
    Boolean,

    [URI(OslcConstants.XML_NAMESPACE + "byte")]
    Byte,

    [URI(OslcConstants.XML_NAMESPACE + "date")]
    Date,

    [URI(OslcConstants.XML_NAMESPACE + "dateTime")]
    DateTime,

    [URI(OslcConstants.XML_NAMESPACE + "dateTimeStamp")]
    DateTimeStamp,

    [URI(OslcConstants.XML_NAMESPACE + "dayTimeDuration")]
    DayTimeDuration,

    [URI(OslcConstants.XML_NAMESPACE + "decimal")]
    Decimal,

    [URI(OslcConstants.XML_NAMESPACE + "double")]
    Double,

    [URI(OslcConstants.XML_NAMESPACE + "duration")]
    Duration,

    [URI(OslcConstants.XML_NAMESPACE + "float")]
    Float,

    [URI(OslcConstants.XML_NAMESPACE + "gDay")]
    GDay,

    [URI(OslcConstants.XML_NAMESPACE + "gMonth")]
    GMonth,

    [URI(OslcConstants.XML_NAMESPACE + "gMonthDay")]
    GMonthDay,

    [URI(OslcConstants.XML_NAMESPACE + "gYear")]
    GYear,

    [URI(OslcConstants.XML_NAMESPACE + "gYearMonth")]
    GYearMonth,

    [URI(OslcConstants.XML_NAMESPACE + "hexBinary")]
    HexBinary,

    [URI(OslcConstants.XML_NAMESPACE + "int")]
    Int,

    [URI(OslcConstants.XML_NAMESPACE + "integer")]
    Integer,

    [URI(OslcConstants.XML_NAMESPACE + "language")]
    Language,

    [URI(OslcConstants.XML_NAMESPACE + "long")]
    Long,

    [URI(OslcConstants.XML_NAMESPACE + "Name")]
    Name,

    [URI(OslcConstants.XML_NAMESPACE + "NCName")]
    NCName,

    [URI(OslcConstants.XML_NAMESPACE + "negativeInteger")]
    NegativeInteger,

    [URI(OslcConstants.XML_NAMESPACE + "NMTOKEN")]
    Nmtoken,

    [URI(OslcConstants.XML_NAMESPACE + "nonNegativeInteger")]
    NonNegativeInteger,

    [URI(OslcConstants.XML_NAMESPACE + "nonPositiveInteger")]
    NonPositiveInteger,

    [URI(OslcConstants.XML_NAMESPACE + "normalizedString")]
    NormalizedString,

    [URI(OslcConstants.XML_NAMESPACE + "positiveInteger")]
    PositiveInteger,

    [URI(OslcConstants.XML_NAMESPACE + "short")]
    Short,

    [URI(OslcConstants.XML_NAMESPACE + "string")]
    String,

    [URI(OslcConstants.XML_NAMESPACE + "time")]
    Time,

    [URI(OslcConstants.XML_NAMESPACE + "token")]
    Token,

    [URI(OslcConstants.XML_NAMESPACE + "unsignedByte")]
    UnsignedByte,

    [URI(OslcConstants.XML_NAMESPACE + "unsignedInt")]
    UnsignedInt,

    [URI(OslcConstants.XML_NAMESPACE + "unsignedLong")]
    UnsignedLong,

    [URI(OslcConstants.XML_NAMESPACE + "unsignedShort")]
    UnsignedShort,

    [URI(OslcConstants.XML_NAMESPACE + "yearMonthDuration")]
    YearMonthDuration,

    [URI(OslcConstants.RDF_NAMESPACE + "dirLangString")]
    DirLangString,

    [URI(OslcConstants.RDF_NAMESPACE + "HTML")]
    Html,

    [URI(OslcConstants.RDF_NAMESPACE + "JSON")]
    Json,

    [URI(OslcConstants.RDF_NAMESPACE + "langString")]
    LangString,

    [URI(OslcConstants.RDF_NAMESPACE + "XMLLiteral")]
    XMLLiteral,

    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "AnyResource")]
    AnyResource,

    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "Resource")]
    Resource,

    [URI(OslcConstants.OSLC_CORE_NAMESPACE + "LocalResource")]
    LocalResource,
}
