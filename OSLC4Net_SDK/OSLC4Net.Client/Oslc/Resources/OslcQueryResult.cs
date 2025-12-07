/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 * Copyright (c) 2023 Andrii Berezovskyi and OSLC4Net contributors.
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

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;

namespace OSLC4Net.Client.Oslc.Resources;

/// <summary>
///     The results of an OSLC query. If the query was paged, subsequent pages can be retrieved using
///     the Iterator interface.
///     This class is not currently thread safe.
/// </summary>
public class OslcQueryResult : IEnumerator<OslcQueryResult>
{
    private static readonly Uri _rdfsMemberUri = new("http://www.w3.org/2000/01/rdf-schema#member");

    private static readonly Uri _responsePredicateUri = new(OslcConstants.OSLC_CORE_NAMESPACE +
                                                            "ResponseInfo");

    private static readonly Uri _totalCountPredicateUri = new Uri("http://open-services.net/ns/core#totalCount");

    private readonly int _pageNumber;
    private readonly OslcQuery _query;

    private readonly HttpResponseMessage _response;

    private IUriNode? _infoResource;
    private bool _nextPageChecked;

    private string? _nextPageUrl = "";

    private IGraph? _rdfGraph;

    private bool _rdfInitialized;

    private IUriNode? _rdfType;
    private IUriNode? _resultsMemberContainer;

    private long? _totalCount;
    private readonly DotNetRdfHelper _rdfHelper;

    public OslcQueryResult(OslcQuery query, HttpResponseMessage response,
        DotNetRdfHelper? rdfHelper = null)
    {
        _query = query;
        _response = response;
        _rdfHelper = rdfHelper ?? Activator.CreateInstance<DotNetRdfHelper>();

        _pageNumber = 1;
    }

    private OslcQueryResult(OslcQueryResult prev, DotNetRdfHelper rdfHelper)
    {
        _rdfHelper = rdfHelper;
        _query = new OslcQuery(prev);
        // FIXME: we should split the data from logic - ctor should not be making calls; one of the methods should return a record with the data.
        _response = _query.GetResponseRawAsync().Result;

        _pageNumber = prev._pageNumber + 1;
    }

    public long? TotalCount
    {
        get { return _totalCount ??= GetTotalCount(); }
    }

    /// <summary>
    /// </summary>
    /// <returns>whether there is another page of results after this</returns>
    public bool MoveNext()
    {
        return !string.IsNullOrWhiteSpace(GetNextPageUrl());
    }

    /// <summary>
    ///     Gets the NEXT page of query results. Name <c>Current</c> for consistency with
    ///     <see cref="IEnumerator.Current" /> due to implementation-specific reasons.
    /// </summary>
    public OslcQueryResult Current
    {
        get
        {
            if (!MoveNext())
            {
                throw new InvalidOperationException();
            }

            return new OslcQueryResult(this, _rdfHelper);
        }
    }

    [Obsolete("May be removed in future versions.", false)]
    // TODO: do not expose implementation details to users (@berezovskyi 2024-10)
    object IEnumerator.Current => Current;

    /// <summary>
    ///     No-op disposal method
    /// </summary>
    // TODO: remove with IEnumerator (@berezovskyi 2024-10)
    [Obsolete("May be removed in future versions.", false)]
    void IDisposable.Dispose()
    {
    }

    // TODO: remove with IEnumerator (@berezovskyi 2024-10)
    [Obsolete("May be removed in future versions.", false)]
    public void Reset()
    {
        throw new InvalidOperationException();
    }

    private long? GetTotalCount()
    {
        InitializeRdf();
        if (_rdfGraph is null || _infoResource?.Uri is null)
        {
            return null;
        }

        var predicateNode = _rdfGraph.CreateUriNode(_totalCountPredicateUri);
        var totalCountObject = _rdfGraph
            .GetTriplesWithSubjectPredicate(_infoResource, predicateNode)
            .SingleOrDefault()?.Object as IValuedNode;

        if (totalCountObject is null)
        {
            return null;
        }

        var countString = totalCountObject.AsString();
        return long.TryParse(countString, out var totalCount) ? totalCount : null;
    }

    // REVISIT: I don't think the query result shall be thread-safe (@berezovskyi 2024-10)
    [MethodImpl(MethodImplOptions.Synchronized)]
    private void InitializeRdf()
    {
        if (_rdfInitialized)
        {
            return;
        }

        _rdfInitialized = true;
        _rdfGraph = new Graph();
        var stream = _response.Content.ReadAsStreamAsync().Result;
        IRdfReader parser = new RdfXmlParser();
        var streamReader = new StreamReader(stream);

        using (streamReader)
        {
            parser.Load(_rdfGraph, streamReader);

            //Find a resource with rdf:type of oslc:ResourceInfo
            _rdfType = _rdfGraph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            var responseInfo =
                _rdfGraph.CreateUriNode(_responsePredicateUri);
            var triples = _rdfGraph.GetTriplesWithPredicateObject(_rdfType, responseInfo)
                ?.ToArray();

            _infoResource ??= GetSingleInfoResource(triples);
            _infoResource ??= GetInfoResourceExactQueryMatch(triples);
            _infoResource ??= GetInfoResourceSubjectStartsWithQueryUri(triples);
            _infoResource ??= GetInfoResourceExactCapabilityMatch(_rdfGraph);
            _infoResource ??= GetInfoResourceSubjectStartsWithCapabilityUri(triples);

            // Use the same resource for members container, with same fallback chain
            _resultsMemberContainer ??= GetInfoResourceExactCapabilityMatch(_rdfGraph);
            _resultsMemberContainer ??= GetSingleInfoResource(triples);
            _resultsMemberContainer ??= GetInfoResourceExactQueryMatch(triples);
            _resultsMemberContainer ??= GetInfoResourceSubjectStartsWithQueryUri(triples);
            _resultsMemberContainer ??= GetInfoResourceSubjectStartsWithCapabilityUri(triples);
            _resultsMemberContainer ??= _infoResource;
        }
    }

    private static IUriNode? GetSingleInfoResource(Triple[]? triples)
    {
        return triples?.Length == 1 ? triples.First().Subject as IUriNode : null;
    }

    private IUriNode? GetInfoResourceExactQueryMatch(Triple[]? triples)
    {
        return triples?.SingleOrDefault(spo =>
            spo.Subject is IUriNode node &&
            node.Uri == _query.QueryUri)?.Subject as IUriNode;
    }

    /// <summary>
    ///     OSLC Query spec only requires there to be a resource with the same URI as the capability
    ///     (base) URI and for it to have members. NOTE: not the same as the <c>oslc:InfoResource</c>.
    /// </summary>
    private IUriNode? GetInfoResourceExactCapabilityMatch(IGraph graph)
    {
        var triples = graph.GetTriplesWithSubjectPredicate(
            graph.CreateUriNode(new Uri(_query.CapabilityUrl)), graph.CreateUriNode("rdfs:member"));
        return triples?.FirstOrDefault(spo =>
            spo.Subject is IUriNode node &&
            node.Uri.Equals(new Uri(_query.CapabilityUrl)))?.Subject as IUriNode;
    }

    private IUriNode? GetInfoResourceSubjectStartsWithQueryUri(Triple[]? triples)
    {
        return triples?.SingleOrDefault(spo =>
            spo.Subject is IUriNode node &&
            node.Uri.ToString().StartsWith(_query.QueryUri.ToString(), StringComparison.Ordinal))?.Subject as IUriNode;
    }

    private IUriNode? GetInfoResourceSubjectStartsWithCapabilityUri(Triple[]? triples)
    {
        return triples?.SingleOrDefault(spo =>
            spo.Subject is IUriNode node &&
            node.Uri.ToString().StartsWith(_query.CapabilityUrl, StringComparison.Ordinal))?.Subject as IUriNode;
    }

    internal string? GetNextPageUrl()
    {
        InitializeRdf();

        Debug.Assert(_rdfGraph != null);

        if (_nextPageChecked)
        {
            return _nextPageUrl;
        }

        _nextPageChecked = true;

        // If we don't have a valid info resource with a URI, we can't look for next page
        if (_infoResource?.Uri == null)
        {
            return null;
        }

        var predicate =
            _rdfGraph!.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + "nextPage"));
        var triples = _rdfGraph.GetTriplesWithSubjectPredicate(_infoResource, predicate);
        var triplesEnumerated = triples.ToList();
        if (triplesEnumerated.Count == 1 && triplesEnumerated.First().Object is IUriNode)
        {
            _nextPageUrl = (triplesEnumerated.First().Object as IUriNode)?.Uri.OriginalString;
        }

        return _nextPageUrl;
    }

    [Obsolete("Marked for removal", false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public OslcQuery GetQuery()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        return _query;
    }

    /// <summary>
    ///     Get the raw client response to a query.
    ///     NOTE:  Using this method and consuming the response will make other methods
    ///     which examine the response unavailable (Examples:  GetMemberUrls(), Current() and MoveNext()).
    ///     When this method is invoked, the consumer is responsible for OSLC page processing
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage GetRawResponse()
    {
        return _response;
    }

    /// <summary>
    ///     Return the subject URLs of the query response.  The URLs are the location of all artifacts
    ///     which satisfy the query conditions.
    ///     NOTE:  Using this method consumes the query response and makes other methods
    ///     which examine the response unavailable (Example: GetRawResponse().
    /// </summary>
    /// <returns></returns>
    public string[] GetMembersUrls()
    {
        InitializeRdf();
        Debug.Assert(_rdfGraph != null);

        IList<string> membersUrls = new List<string>();

        // If we couldn't find a valid members container with a URI, return empty list
        if (_resultsMemberContainer?.Uri == null)
        {
            return membersUrls.ToArray();
        }

        var triples = _rdfGraph!.GetTriplesWithSubject(_resultsMemberContainer);

        foreach (var triple in triples)
        {
            if (!triple.Predicate.Equals(
                    _rdfGraph.GetUriNode(_rdfsMemberUri)))
            {
                continue;
            }

            if (triple.Object is IUriNode uriNode)
            {
                membersUrls.Add(uriNode.Uri.ToString());
            }
            // REVISIT: log or throw
        }

        return membersUrls.ToArray();
    }

    /// <summary>
    ///     Return the enumeration of queried results from this page
    /// </summary>
    /// <returns>member triples from current page</returns>
    public IEnumerable<T> GetMembers<T>()
    {
        InitializeRdf();
        Debug.Assert(_rdfGraph != null);

        // If we couldn't find a valid members container with a URI, return empty enumerable
        if (_resultsMemberContainer?.Uri == null)
        {
            return Enumerable.Empty<T>();
        }

        var triples = _rdfGraph!.GetTriplesWithSubject(_resultsMemberContainer);

        // Filter to only rdfs:member predicates with URI objects (same logic as GetMembersUrls)
        var memberTriples = triples.Where(triple =>
            triple.Predicate.Equals(_rdfGraph.GetUriNode(_rdfsMemberUri)) &&
            triple.Object is IUriNode);

        IEnumerable<T> result = new TripleEnumerableWrapper<T>(memberTriples, _rdfGraph, _rdfHelper);

        return result;
    }

    private sealed class TripleEnumerableWrapper<T> : IEnumerable<T>
    {
        private readonly IGraph? _graph;
        private readonly DotNetRdfHelper _dotNetRdfHelper;

        private readonly IEnumerable<Triple> _triples;

        public TripleEnumerableWrapper(IEnumerable<Triple> triples, IGraph? graph,
            DotNetRdfHelper dotNetRdfHelper)
        {
            _triples = triples;
            _graph = graph;
            _dotNetRdfHelper = dotNetRdfHelper;
        }

        IEnumerator
            IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new TripleEnumeratorWrapper(_triples.GetEnumerator(), _graph, _dotNetRdfHelper);
        }

        private sealed class TripleEnumeratorWrapper : IEnumerator<T>
        {
            private readonly IGraph? _graph;

            private readonly IEnumerator<Triple> _triples;
            private readonly DotNetRdfHelper _dotNetRdfHelper;

            public TripleEnumeratorWrapper(IEnumerator<Triple> triples, IGraph? graph,
                DotNetRdfHelper dotNetRdfHelper)
            {
                _triples = triples;
                _graph = graph;
                _dotNetRdfHelper = dotNetRdfHelper;
            }

            object IEnumerator.Current => Current!;

            public T Current
            {
                get
                {
                    var member = _triples.Current ??
                                 throw new ArgumentNullException(nameof(_triples.Current));
                    return (T)_dotNetRdfHelper.FromDotNetRdfNode((IUriNode)member.Object, _graph,
                        typeof(T));
                }
            }

            public void Dispose()
            {
                _triples.Dispose();
            }

            public bool MoveNext()
            {
                return _triples.MoveNext();
            }

            public void Reset()
            {
                _triples.Reset();
            }
        }
    }
}
