/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;

namespace OSLC4Net.Server.Providers;

public class OslcRequest
{
    private const string ConfigurationContextHeader = "Configuration-Context";
    private const string OslcCoreVersionHeader = "OSLC-Core-Version";
    private const string PreferHeader = "Prefer";
    private const string SlugHeader = "Slug";

    private readonly Dictionary<Type, object> _resourcesByType = new();
    private readonly DotNetRdfHelper _rdfHelper;

    public OslcRequest(
        IGraph graph,
        IReadOnlyDictionary<string, string[]>? headers = null,
        DotNetRdfHelper? rdfHelper = null)
    {
        Graph = graph;
        Headers = new Dictionary<string, string[]>(
            headers ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
            StringComparer.OrdinalIgnoreCase);
        _rdfHelper = rdfHelper ?? Activator.CreateInstance<DotNetRdfHelper>();
    }

    public IGraph Graph { get; }

    /// <summary>
    ///     Request headers captured while the RDF request body was parsed.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Headers { get; }

    /// <summary>
    ///     Lazily materializes all known OSLC resources in the request body.
    /// </summary>
    public IReadOnlyList<IResource> Resources => ResourcesOfType<IResource>();

    /// <summary>
    ///     Gets the <c>OSLC-Core-Version</c> request header value.
    /// </summary>
    /// <remarks>
    ///     OSLC clients and servers use this header to indicate the OSLC version they expect or support.
    ///     See
    ///     <see href="https://docs.oasis-open-projects.org/oslc-op/core/v3.0/oslc-core.html#versionCompatibility_header">
    ///     OSLC Core 3.0, section 4.2.2</see>.
    /// </remarks>
    public string? OslcCoreVersion => HeaderValue(OslcCoreVersionHeader);

    /// <summary>
    ///     Gets the <c>Configuration-Context</c> request header value as an absolute URI, when present and valid.
    /// </summary>
    /// <remarks>
    ///     OSLC Configuration Management uses this header to pass the configuration context for versioned resources.
    ///     See
    ///     <see href="https://docs.oasis-open-projects.org/oslc-op/config/v1.0/config-resources.html#configcontext">
    ///     OSLC Configuration Management 1.0, section 4</see>.
    /// </remarks>
    public Uri? ConfigurationContext => Uri.TryCreate(HeaderValue(ConfigurationContextHeader), UriKind.Absolute, out Uri? uri)
        ? uri
        : null;

    /// <summary>
    ///     Gets the <c>Prefer</c> request header values.
    /// </summary>
    /// <remarks>
    ///     LDP defines preferences for requesting compact container representations through the HTTP
    ///     <c>Prefer</c> header. See
    ///     <see href="https://www.w3.org/TR/ldp/#prefer-parameters">LDP 1.0, section 7.2</see>.
    /// </remarks>
    public IReadOnlyList<string> Prefer => HeaderValues(PreferHeader);

    /// <summary>
    ///     Gets the <c>Slug</c> request header value.
    /// </summary>
    /// <remarks>
    ///     LDP servers may use this header as a client hint for choosing the URI of a resource created
    ///     through <c>POST</c>. See
    ///     <see href="https://www.w3.org/TR/ldp/#ldpc-post-slug">LDP 1.0, section 5.2.3.10</see>.
    /// </remarks>
    public string? Slug => HeaderValue(SlugHeader);

    public IReadOnlyList<T> ResourcesOfType<T>() where T : IResource
    {
        Type requestedType = typeof(T);
        if (_resourcesByType.TryGetValue(requestedType, out object? resources))
        {
            return (IReadOnlyList<T>)resources;
        }

        IReadOnlyList<T> typedResources = OslcResourceMaterializer.Materialize<T>(Graph, _rdfHelper);
        _resourcesByType.Add(requestedType, typedResources);
        return typedResources;
    }

    public T? ResourceOfType<T>() where T : IResource
    {
        IReadOnlyList<T> resources = ResourcesOfType<T>();
        return resources.Count > 0 ? resources[0] : default;
    }

    public string? HeaderValue(string name)
    {
        return Headers.TryGetValue(name, out string[]? values) && values.Length > 0
            ? values[0]
            : null;
    }

    public IReadOnlyList<string> HeaderValues(string name)
    {
        return Headers.TryGetValue(name, out string[]? values)
            ? values
            : [];
    }
}

public sealed class OslcRequest<T> : OslcRequest where T : IResource
{
    public OslcRequest(
        IGraph graph,
        IReadOnlyDictionary<string, string[]>? headers = null,
        DotNetRdfHelper? rdfHelper = null)
        : base(graph, headers, rdfHelper)
    {
    }

    public new IReadOnlyList<T> Resources => ResourcesOfType<T>();

    public T? Resource => Resources.Count > 0 ? Resources[0] : default;
}
