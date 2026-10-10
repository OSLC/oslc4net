/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
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

using System.Net.Http.Headers;

namespace OSLC4Net.Core.Model;

/// <summary>
///     Constant strings and static MediaTypeHeaderValue representing OSLC media types
/// </summary>
/// <seealso cref="System.Net.Http.Headers.MediaTypeHeaderValue" />
public class OslcMediaType
{
    public const string APPLICATION_RDF_XML = "application/rdf+xml";
    public const string APPLICATION_JSON_LD = "application/ld+json";
    public const string TEXT_TURTLE = "text/turtle";
    public const string APPLICATION_NTRIPLES = "application/n-triples";

    public const string X_OSLC_COMPACT_XML = "x-oslc-compact+xml";
    public const string APPLICATION_X_OSLC_COMPACT_XML = $"application/{X_OSLC_COMPACT_XML}";

    public const string
        X_OSLC_COMPACT_JSON =
            "x-oslc-compact+json"; // TODO - Compact media type never defined in the OSLC spec for JSON

    public const string APPLICATION_X_OSLC_COMPACT_JSON = "application" + "/" + X_OSLC_COMPACT_JSON;
    private static readonly MediaTypeHeaderValue _applicationRdfXmlType = new(APPLICATION_RDF_XML);
    private static readonly MediaTypeHeaderValue _applicationJsonLdType = new(APPLICATION_JSON_LD);
    private static readonly MediaTypeHeaderValue _textTurtleType = new(TEXT_TURTLE);

    private static readonly MediaTypeHeaderValue _applicationXOslcCompactXmlType =
        new(APPLICATION_X_OSLC_COMPACT_XML);

    private static readonly MediaTypeHeaderValue _applicationXOslcCompactJsonType =
        new(APPLICATION_X_OSLC_COMPACT_JSON);

    public static MediaTypeHeaderValue ApplicationRdfXmlType => _applicationRdfXmlType;
    public static MediaTypeHeaderValue ApplicationJsonLdType => _applicationJsonLdType;
    public static MediaTypeHeaderValue TextTurtleType => _textTurtleType;
    public static MediaTypeHeaderValue ApplicationXOslcCompactXmlType => _applicationXOslcCompactXmlType;
    public static MediaTypeHeaderValue ApplicationXOslcCompactJsonType => _applicationXOslcCompactJsonType;

    [Obsolete] public const string APPLICATION_JSON = "application/json";

    [Obsolete] public const string APPLICATION_XML = "application/xml";

    [Obsolete] public const string TEXT_XML = "text/xml";

    [Obsolete]
    private static readonly MediaTypeHeaderValue _applicationJsonType = new(APPLICATION_JSON);

    [Obsolete]
    private static readonly MediaTypeHeaderValue _applicationXmlType = new(APPLICATION_XML);

    [Obsolete] private static readonly MediaTypeHeaderValue _textXmlType = new(TEXT_XML);

    [Obsolete] public static MediaTypeHeaderValue ApplicationJsonType => _applicationJsonType;

    [Obsolete] public static MediaTypeHeaderValue ApplicationXmlType => _applicationXmlType;

    [Obsolete] public static MediaTypeHeaderValue TextXmlType => _textXmlType;

}
