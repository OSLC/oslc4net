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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace OSLC4Net.Client.Oslc.Resources;

/// <summary>
/// The results of an OSLC query. If the query was paged, subsequent pages can be retrieved using the Iterator interface.
///
/// This class is not currently thread safe.
/// </summary>
public class OslcQueryResult : IEnumerator<OslcQueryResult>
{
    private readonly OslcQuery query;

    private readonly HttpResponseMessage response;

    private readonly int pageNumber;

    private IGraph rdfGraph;

    private IUriNode rdfType, infoResource;

    private string nextPageUrl = "";

    private bool rdfInitialized = false;

    public OslcQueryResult(OslcQuery query, HttpResponseMessage response)
    {
        this.query = query;
        this.response = response;

        this.pageNumber = 1;
    }

    private OslcQueryResult(OslcQueryResult prev)
    {
        this.query = new OslcQuery(prev);
        this.response = this.query.GetResponse();

        this.pageNumber = prev.pageNumber + 1;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void InitializeRdf()
    {
        if (!rdfInitialized)
        {
            rdfInitialized = true;
            rdfGraph = new Graph();
            var stream = response.Content.ReadAsStreamAsync().Result;
            IRdfReader parser = new RdfXmlParser();
            var streamReader = new StreamReader(stream);

            using (streamReader)
            {
                parser.Load(rdfGraph, streamReader);

                //Find a resource with rdf:type of oslc:ResourceInfo
                rdfType = rdfGraph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
                var responseInfo = rdfGraph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + "ResponseInfo"));
                var triples = rdfGraph.GetTriplesWithPredicateObject(rdfType, responseInfo);

                //There should only be one - take the first
                infoResource = triples.Count() == 0 ? null : (triples.First().Subject as IUriNode);
            }
        }
    }

    internal string GetNextPageUrl()
    {

        InitializeRdf();

        if ((nextPageUrl == null || nextPageUrl.Length == 0) && infoResource != null)
        {
            var predicate = rdfGraph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + "nextPage"));
            var triples = rdfGraph.GetTriplesWithSubjectPredicate(infoResource, predicate);
            if (triples.Count() == 1 && triples.First().Object is IUriNode)
            {
                nextPageUrl = (triples.First().Object as IUriNode).Uri.OriginalString;
            }
            else
            {
                nextPageUrl = "";
            }
        }

        return nextPageUrl;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns>whether there is another page of results after this</returns>
    public bool MoveNext()
    {
        if (GetNextPageUrl().Length == 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///
    /// </summary>
    public OslcQueryResult Current
    {
        get
        {
            if (!MoveNext())
            {
                throw new InvalidOperationException();
            }
            return new OslcQueryResult(this);
        }
    }

    object IEnumerator.Current
    {
        get { return Current; }
    }

    void IDisposable.Dispose() { }

    public void Reset()
    {
        throw new InvalidOperationException();
    }

    public OslcQuery GetQuery()
    {
        return query;
    }

    /// <summary>
    /// Get the raw client response to a query.
    ///
    /// NOTE:  Using this method and consuming the response will make other methods
    /// which examine the response unavailable (Examples:  GetMemberUrls(), Current() and MoveNext()).
    /// When this method is invoked, the consumer is responsible for OSLC page processing
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage GetRawResponse()
    {
        return response;
    }

    /// <summary>
    /// Return the subject URLs of the query response.  The URLs are the location of all artifacts
    /// which satisfy the query conditions.
    ///
    /// NOTE:  Using this method consumes the query response and makes other methods
    /// which examine the response unavailable (Example: GetRawResponse().
    /// </summary>
    /// <returns></returns>
    public string[] GetMembersUrls()
    {

        InitializeRdf();

        IList<string> membersUrls = new List<string>();
        var membersResource = rdfGraph.CreateUriNode(new Uri(query.GetCapabilityUrl()));
        var triples = rdfGraph.GetTriplesWithSubject(membersResource);

        foreach (var triple in triples)
        {
            try
            {
                membersUrls.Add((triple.Object as IUriNode).Uri.ToString());
            }
            catch (Exception)
            {
                // TODO: make exception more specific
                Console.Error.WriteLine("Member was not a resource");
            }
        }

        return membersUrls.ToArray();
    }

    /// <summary>
    /// Return the enumeration of queried results from this page
    /// </summary>
    /// <returns>member triples from current page</returns>
    public IEnumerable<T> GetMembers<T>()
    {
        InitializeRdf();

        var membersResource = rdfGraph.CreateUriNode(new Uri(query.GetCapabilityUrl()));
        var triples = rdfGraph.GetTriplesWithSubject(membersResource);
        IEnumerable<T> result = new TripleEnumerableWrapper<T>(triples, rdfGraph);

        return result;
    }

    private class TripleEnumerableWrapper<T> : IEnumerable<T>
    {
        public TripleEnumerableWrapper(IEnumerable<Triple> triples, IGraph graph)
        {
            this.triples = triples;
            this.graph = graph;
        }

        IEnumerator
        IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new TripleEnumeratorWrapper(triples.GetEnumerator(), graph);
        }

        private class TripleEnumeratorWrapper : IEnumerator<T>
        {
            public TripleEnumeratorWrapper(IEnumerator<Triple> triples, IGraph graph)
            {
                this.triples = triples;
                this.graph = graph;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public T Current
            {
                get
                {
                    var member = triples.Current;

                    return (T)DotNetRdfHelper.FromDotNetRdfNode((IUriNode)member.Object, graph, typeof(T));
                }
            }

            public void Dispose()
            {
                triples.Dispose();
            }

            public bool MoveNext()
            {
                return triples.MoveNext();
            }

            public void Reset()
            {
                triples.Reset();
            }

            private readonly IEnumerator<Triple> triples;
            private readonly IGraph graph;
        }

        readonly IEnumerable<Triple> triples;
        private readonly IGraph graph;
    }
}
