/*******************************************************************************
 * Copyright (c) 2025 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *******************************************************************************/

using OSLC4Net.Client.Oslc;

namespace OSLC4Net.Client;

/// <summary>
/// Request parameters for OSLC client operations. These can be pre-set in the library,
/// configured in the OslcClient constructor, or overridden on a per-request basis.
/// </summary>
/// <remarks>
/// This class allows users to customize:
/// <list type="bullet">
///   <item>Accept header for content negotiation (which media types to request)</item>
///   <item>Content-Type header for POST/PUT operations</item>
///   <item>Custom headers for specific requests</item>
///   <item>OSLC Core version header</item>
/// </list>
/// </remarks>
public sealed class OslcRequestParams
{
    /// <summary>
    /// Default Accept header value for OSLC requests.
    /// Prefers Turtle, then RDF/XML, then N-Triples, then N3.
    /// </summary>
    public const string DefaultAcceptHeader =
        "text/turtle;q=1.0, application/rdf+xml;q=0.9, application/n-triples;q=0.8, text/n3;q=0.7";

    /// <summary>
    /// Default Content-Type for POST/PUT operations.
    /// </summary>
    public const string DefaultContentType = OSLCConstants.CT_RDF;

    /// <summary>
    /// Default OSLC Core version.
    /// </summary>
    public const string DefaultOslcCoreVersion = "2.0";

    /// <summary>
    /// Accept header for content negotiation.
    /// </summary>
    public string? AcceptHeader { get; init; }

    /// <summary>
    /// Content-Type header for POST/PUT operations.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// OSLC Core version header value.
    /// </summary>
    public string? OslcCoreVersion { get; init; }

    /// <summary>
    /// Additional custom headers to include in requests.
    /// </summary>
    public IDictionary<string, string>? CustomHeaders { get; init; }

    /// <summary>
    /// Creates default request parameters.
    /// </summary>
    public static OslcRequestParams Default => new()
    {
        AcceptHeader = DefaultAcceptHeader,
        ContentType = DefaultContentType,
        OslcCoreVersion = DefaultOslcCoreVersion
    };

    /// <summary>
    /// Creates request parameters for RDF/XML only requests.
    /// Useful for root services discovery or legacy servers.
    /// </summary>
    public static OslcRequestParams RdfXmlOnly => new()
    {
        AcceptHeader = OSLCConstants.CT_RDF,
        ContentType = OSLCConstants.CT_RDF,
        OslcCoreVersion = DefaultOslcCoreVersion
    };

    /// <summary>
    /// Creates request parameters for Turtle only requests.
    /// </summary>
    public static OslcRequestParams TurtleOnly => new()
    {
        AcceptHeader = "text/turtle",
        ContentType = "text/turtle",
        OslcCoreVersion = DefaultOslcCoreVersion
    };

    /// <summary>
    /// Merges these parameters with overrides. Override values take precedence.
    /// </summary>
    /// <param name="overrides">Override parameters (can be null).</param>
    /// <returns>A new OslcRequestParams with merged values.</returns>
    public OslcRequestParams Merge(OslcRequestParams? overrides)
    {
        if (overrides is null)
        {
            return this;
        }

        var mergedHeaders = new Dictionary<string, string>();

        if (CustomHeaders is not null)
        {
            foreach (var kvp in CustomHeaders)
            {
                mergedHeaders[kvp.Key] = kvp.Value;
            }
        }

        if (overrides.CustomHeaders is not null)
        {
            foreach (var kvp in overrides.CustomHeaders)
            {
                mergedHeaders[kvp.Key] = kvp.Value;
            }
        }

        return new OslcRequestParams
        {
            AcceptHeader = overrides.AcceptHeader ?? AcceptHeader,
            ContentType = overrides.ContentType ?? ContentType,
            OslcCoreVersion = overrides.OslcCoreVersion ?? OslcCoreVersion,
            CustomHeaders = mergedHeaders.Count > 0 ? mergedHeaders : null
        };
    }

    /// <summary>
    /// Creates a builder for fluent construction of request parameters.
    /// </summary>
    public static OslcRequestParamsBuilder Builder() => new();
}

/// <summary>
/// Builder for constructing OslcRequestParams in a fluent manner.
/// </summary>
public sealed class OslcRequestParamsBuilder
{
    private string? _acceptHeader;
    private string? _contentType;
    private string? _oslcCoreVersion;
    private Dictionary<string, string>? _customHeaders;

    /// <summary>
    /// Sets the Accept header.
    /// </summary>
    public OslcRequestParamsBuilder WithAccept(string accept)
    {
        _acceptHeader = accept;
        return this;
    }

    /// <summary>
    /// Sets the Content-Type header.
    /// </summary>
    public OslcRequestParamsBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    /// <summary>
    /// Sets the OSLC Core version.
    /// </summary>
    public OslcRequestParamsBuilder WithOslcCoreVersion(string version)
    {
        _oslcCoreVersion = version;
        return this;
    }

    /// <summary>
    /// Adds a custom header.
    /// </summary>
    public OslcRequestParamsBuilder WithHeader(string name, string value)
    {
        _customHeaders ??= [];
        _customHeaders[name] = value;
        return this;
    }

    /// <summary>
    /// Builds the OslcRequestParams.
    /// </summary>
    public OslcRequestParams Build()
    {
        return new OslcRequestParams
        {
            AcceptHeader = _acceptHeader,
            ContentType = _contentType,
            OslcCoreVersion = _oslcCoreVersion,
            CustomHeaders = _customHeaders
        };
    }
}
