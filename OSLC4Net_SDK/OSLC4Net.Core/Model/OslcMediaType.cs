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

namespace OSLC4Net.Core.Model
{
    #region

    using System.Net.Http.Headers;

    #endregion

    /// <summary>
    /// Constant strings and static MediaTypeHeaderValue representing OSLC media types
    /// </summary>
    /// <seealso cref="MediaTypeHeaderValue"/>
    public class OslcMediaType
    {
        public const string APPLICATION = "application";

        public const string TEXT = "text";

        public const string RDF_XML = "rdf+xml";

        public const string APPLICATION_RDF_XML = APPLICATION + "/" + RDF_XML;

        public static readonly MediaTypeHeaderValue APPLICATION_RDF_XML_TYPE =
            new MediaTypeHeaderValue(APPLICATION_RDF_XML);

        public const string APPLICATION_JSON = APPLICATION + "/" + "json";

        public static readonly MediaTypeHeaderValue APPLICATION_JSON_TYPE = new MediaTypeHeaderValue(APPLICATION_JSON);

        public const string APPLICATION_XML = APPLICATION + "/" + "xml";

        public static readonly MediaTypeHeaderValue APPLICATION_XML_TYPE = new MediaTypeHeaderValue(APPLICATION_XML);

        public const string TEXT_XML = TEXT + "/" + "xml";

        public static readonly MediaTypeHeaderValue TEXT_XML_TYPE = new MediaTypeHeaderValue(TEXT_XML);

        public const string TURTLE = "turtle";

        public const string TEXT_TURTLE = TEXT + "/" + TURTLE;

        public static readonly MediaTypeHeaderValue TEXT_TURTLE_TYPE = new MediaTypeHeaderValue(TEXT_TURTLE);

        public const string X_OSLC_COMPACT_XML = "x-oslc-compact+xml";

        public const string APPLICATION_X_OSLC_COMPACT_XML = APPLICATION + "/" + X_OSLC_COMPACT_XML;

        public static readonly MediaTypeHeaderValue APPLICATION_X_OSLC_COMPACT_XML_TYPE =
            new MediaTypeHeaderValue(APPLICATION_X_OSLC_COMPACT_XML);

        public const string
            X_OSLC_COMPACT_JSON =
                "x-oslc-compact+json"; // TODO - Compact media type never defined in the OSLC spec for JSON

        public const string APPLICATION_X_OSLC_COMPACT_JSON = APPLICATION + "/" + X_OSLC_COMPACT_JSON;

        public static readonly MediaTypeHeaderValue APPLICATION_X_OSLC_COMPACT_JSON_TYPE =
            new MediaTypeHeaderValue(APPLICATION_X_OSLC_COMPACT_JSON);
    }
}