﻿/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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

namespace OSLC4Net.Core.Resources
{
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

    /// <summary>
    /// The results of an OSLC query. If the query was paged, subsequent pages can be retrieved using the Iterator interface.
    ///
    /// This class is not currently thread safe.
    /// </summary>
    public class OslcQueryResult : IEnumerator<OslcQueryResult>
    {
        private readonly OslcQuery _query;

        private readonly HttpResponseMessage _response;

        private readonly int _pageNumber;

        private IGraph _rdfGraph;

        private IUriNode _rdfType, _infoResource;

        private string _nextPageUrl = string.Empty;

        private bool _rdfInitialized = false;

        public OslcQueryResult(OslcQuery query, HttpResponseMessage response)
        {
            _query = query;
            _response = response;

            _pageNumber = 1;
        }

        private OslcQueryResult(OslcQueryResult prev)
        {
            _query = new OslcQuery(prev);
            _response = _query.GetResponse();

            _pageNumber = prev._pageNumber + 1;
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

        void IDisposable.Dispose()
        {
        }

        public void Reset()
        {
            throw new InvalidOperationException();
        }

        public OslcQuery GetQuery()
        {
            return _query;
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
            return _response;
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
            IUriNode membersResource = _rdfGraph.CreateUriNode(new Uri(_query.GetCapabilityUrl()));
            IEnumerable<Triple> triples = _rdfGraph.GetTriplesWithSubject(membersResource);

            foreach (Triple triple in triples)
            {
                try
                {
                    membersUrls.Add((triple.Object as IUriNode).Uri.ToString());
                }
                catch (Exception t)
                {
                    //FIXME
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

            IUriNode membersResource = _rdfGraph.CreateUriNode(new Uri(_query.GetCapabilityUrl()));
            IEnumerable<Triple> triples = _rdfGraph.GetTriplesWithSubject(membersResource);
            IEnumerable<T> result = new TripleEnumerableWrapper<T>(triples);

            return result;
        }

        internal string GetNextPageUrl()
        {
            InitializeRdf();

            if ((_nextPageUrl == null || _nextPageUrl.Length == 0) && _infoResource != null)
            {
                IUriNode predicate = _rdfGraph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + "nextPage"));
                IEnumerable<Triple> triples = _rdfGraph.GetTriplesWithSubjectPredicate(_infoResource, predicate);
                if (triples.Count() == 1 && triples.First().Object is IUriNode)
                {
                    _nextPageUrl = (triples.First().Object as IUriNode).Uri.OriginalString;
                }
                else
                {
                    _nextPageUrl = string.Empty;
                }
            }

            return _nextPageUrl;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InitializeRdf()
        {
            if (!_rdfInitialized)
            {
                _rdfInitialized = true;
                _rdfGraph = new Graph();
                Stream stream = _response.Content.ReadAsStreamAsync().Result;
                IRdfReader parser = new RdfXmlParser();
                StreamReader streamReader = new StreamReader(stream);

                using (streamReader)
                {
                    parser.Load(_rdfGraph, streamReader);

                    // Find a resource with rdf:type of oslc:ResourceInfo
                    _rdfType = _rdfGraph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
                    IUriNode responseInfo = _rdfGraph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + "ResponseInfo"));
                    IEnumerable<Triple> triples = _rdfGraph.GetTriplesWithPredicateObject(_rdfType, responseInfo);

                    // There should only be one - take the first
                    _infoResource = triples.Count() == 0 ? null : (triples.First().Subject as IUriNode);
                }
            }
        }

        private class TripleEnumerableWrapper<T> : IEnumerable<T>
        {
            private IEnumerable<Triple> _triples;

            public TripleEnumerableWrapper(IEnumerable<Triple> triples)
            {
                _triples = triples;
            }

            IEnumerator
            IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<T>
            GetEnumerator()
            {
                return new TripleEnumeratorWrapper<T>(_triples.GetEnumerator());
            }

            private class TripleEnumeratorWrapper<T> : IEnumerator<T>
            {
                private IEnumerator<Triple> _triples;

                public TripleEnumeratorWrapper(IEnumerator<Triple> triples)
                {
                    _triples = triples;
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
                        Triple member = _triples.Current;

                        return (T)DotNetRdfHelper.FromDotNetRdfNode((IUriNode)member.Object, typeof(T));
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
}