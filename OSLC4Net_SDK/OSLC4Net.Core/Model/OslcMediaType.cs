/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
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
    public static readonly MediaTypeHeaderValue APPLICATION_RDF_XML_TYPE = new(APPLICATION_RDF_XML);
    public static readonly MediaTypeHeaderValue APPLICATION_JSON_LD_TYPE = new(APPLICATION_JSON_LD);
    public static readonly MediaTypeHeaderValue TEXT_TURTLE_TYPE = new(TEXT_TURTLE);

    public static readonly MediaTypeHeaderValue APPLICATION_X_OSLC_COMPACT_XML_TYPE =
        new(APPLICATION_X_OSLC_COMPACT_XML);

    public static readonly MediaTypeHeaderValue APPLICATION_X_OSLC_COMPACT_JSON_TYPE =
        new(APPLICATION_X_OSLC_COMPACT_JSON);

    [Obsolete] public const string APPLICATION_JSON = "application/json";

    [Obsolete] public const string APPLICATION_XML = "application/xml";

    [Obsolete] public const string TEXT_XML = "text/xml";

    [Obsolete]
    public static readonly MediaTypeHeaderValue APPLICATION_JSON_TYPE = new(APPLICATION_JSON);

    [Obsolete]
    public static readonly MediaTypeHeaderValue APPLICATION_XML_TYPE = new(APPLICATION_XML);

    [Obsolete] public static readonly MediaTypeHeaderValue TEXT_XML_TYPE = new(TEXT_XML);

}
