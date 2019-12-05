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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
using log4net;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;

namespace OSLC4Net.Core.DotNetRdfProvider
{
    /// <summary>
    /// A class to assist with serialization and de-serialization of RDF/XML from/to .NET objects
    /// </summary>
    public static class DotNetRdfHelper
    {
        private const string PROPERTY_TOTAL_COUNT = "totalCount";
        private const string PROPERTY_NEXT_PAGE = "nextPage";

        private const string METHOD_NAME_START_GET = "Get";
        private const string METHOD_NAME_START_IS = "Is";
        private const string METHOD_NAME_START_SET = "Set";

        private static int METHOD_NAME_START_GET_LENGTH = METHOD_NAME_START_GET.Length;
        private static int METHOD_NAME_START_IS_LENGTH = METHOD_NAME_START_IS.Length;

        private const string GENERATED_PREFIX_START = "j.";

        private const string RDF_ALT = "Alt";
        private const string RDF_BAG = "Bag";
        private const string RDF_SEQ = "Seq";

        private const string RDF_LIST = "List";

        private const string RDF_NIL = "nil";

        private static ILog logger = LogManager.GetLogger(typeof(DotNetRdfHelper));

        /// <summary>
        /// Create an RDF graph from a collection of .NET objects
        /// </summary>
        /// <param name="objects">A collection of .NET objects</param>
        /// <returns>The RDF Graph representing the objects</returns>
        public static IGraph CreateDotNetRdfGraph(IEnumerable<object> objects)
        {
            return CreateDotNetRdfGraph(null,
                                        null,
                                        null,
                                        null,
                                        objects,
                                        null);
        }

        /// <summary>
        /// Create an RDF Graph from a collection of objects
        /// </summary>
        /// <param name="descriptionAbout">URL for entire collection</param>
        /// <param name="responseInfoAbout">URL for current page of collection</param>
        /// <param name="nextPageAbout">optional URL for next page in collection</param>
        /// <param name="totalCount">optional total count of member across all pages; if null will use count of passed in members</param>
        /// <param name="objects">members from this page</param>
        /// <param name="properties">filtering list of properties for each member</param>
        /// <returns>RDF graph of collection</returns>
        public static IGraph CreateDotNetRdfGraph(string descriptionAbout,
                                                  string responseInfoAbout,
                                                  string nextPageAbout,
                                                  long? totalCount,
                                                  IEnumerable<object> objects,
                                                  IDictionary<string, object> properties)
        {
            IGraph graph = new Graph();
            INamespaceMapper namespaceMappings = graph.NamespaceMap;

            IUriNode descriptionResource = null;

            if (descriptionAbout != null)
            {
                descriptionResource = graph.CreateUriNode(new Uri(descriptionAbout));

                //The responseInfo can have the same subject URI as the overall collection, especially when paging
                //or query parameters are not included in the request to the QueryCapability.   If a responseInfoAbout 
                //URI is not provided, or if it is the same as the descriptionAbout URI, the descriptionResource should be
                //used for RespionseInfo predicates such as totalCount.

                IUriNode responseInfoResource = null;

                if ((responseInfoAbout != null) && (!responseInfoAbout.Equals(descriptionAbout)))
                {
                    responseInfoResource = graph.CreateUriNode(new Uri(responseInfoAbout));
                }
                else
                {
                    responseInfoResource = descriptionResource;
                }

                graph.Assert(new Triple(responseInfoResource, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                        graph.CreateUriNode(new Uri(OslcConstants.TYPE_RESPONSE_INFO))));

                graph.Assert(new Triple(responseInfoResource, graph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + PROPERTY_TOTAL_COUNT)),
                                        new StringNode(graph, (totalCount == null ? objects.Count() : totalCount).ToString())));

                if (nextPageAbout != null)
                {
                    graph.Assert(new Triple(responseInfoResource, graph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE + PROPERTY_NEXT_PAGE)),
                                            graph.CreateUriNode(new Uri(nextPageAbout))));
                }

            }

            foreach (object obj in objects)
            {
                HandleSingleResource(descriptionResource,
                                     obj,
                                     graph,
                                     namespaceMappings,
                                     properties);
            }

            if (descriptionAbout != null)
            {
                // Ensure we have an rdf prefix
                EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                      OslcConstants.RDF_NAMESPACE,
                                      namespaceMappings);

                // Ensure we have an rdfs prefix
                EnsureNamespacePrefix(OslcConstants.RDFS_NAMESPACE_PREFIX,
                                      OslcConstants.RDFS_NAMESPACE,
                                      namespaceMappings);

                if (responseInfoAbout != null)
                {
                    // Ensure we have an oslc prefix
                    EnsureNamespacePrefix(OslcConstants.OSLC_CORE_NAMESPACE_PREFIX,
                                          OslcConstants.OSLC_CORE_NAMESPACE,
                                          namespaceMappings);
                }
            }


            return graph;
        }

        private static void HandleSingleResource(INode descriptionResource,
                                                 object obj,
                                                 IGraph graph,
                                                 INamespaceMapper namespaceMappings,
                                                 IDictionary<string, object> properties)
        {
            Type objType = obj.GetType();

            // Collect the namespace prefix -> namespace mappings
            RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                objType);

            Uri aboutURI = null;
            if (obj is IResource)
            {
                aboutURI = ((IResource)obj).GetAbout();
            }

            INode mainResource;

            if (aboutURI != null)
            {
                if (!aboutURI.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(objType,
                                                           "getAbout",
                                                           aboutURI);
                }

                mainResource = graph.CreateUriNode(aboutURI);
            }
            else
            {
                mainResource = graph.CreateBlankNode();
            }

            if (objType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
            {
                string ns = TypeFactory.GetNamespace(objType);
                string name = TypeFactory.GetName(objType);

                graph.Assert(new Triple(mainResource, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                        graph.CreateUriNode(new Uri(ns + name))));
            }

            BuildResource(obj,
                          objType,
                          graph,
                          mainResource,
                          properties);

            if (descriptionResource != null)
            {
                graph.Assert(new Triple(descriptionResource, graph.CreateUriNode(new Uri(OslcConstants.RDFS_NAMESPACE + "member")),
                                        mainResource));
            }
        }

        /// <summary>
        /// Create a .NET object from an RDF Node
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="beanType"></param>
        /// <returns></returns>
        public static object FromDotNetRdfNode(IUriNode resource,
                                               Type beanType)
        {
            object newInstance = Activator.CreateInstance(beanType);
            IDictionary<Type, IDictionary<string, MethodInfo>> typePropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MethodInfo>>();
            IDictionary<string, object> visitedResources = new DictionaryWithReplacement<string, object>();

            FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                              beanType,
                              newInstance,
                              resource,
                              visitedResources);

            return newInstance;
        }

        /// <summary>
        /// Create a .NET object from an RDF graph
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="beanType"></param>
        /// <returns></returns>
        public static object FromDotNetRdfGraph(IGraph graph,
                                                Type beanType)
        {
            Type[] types = { beanType };
            object results = Activator.CreateInstance(typeof(List<>).MakeGenericType(types));

            if (beanType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
            {
                string qualifiedName = TypeFactory.GetQualifiedName(beanType);

                IEnumerable<Triple> triples = graph.GetTriplesWithPredicateObject(graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                                                                  graph.CreateUriNode(new Uri(qualifiedName)));

                if (triples.Count() > 0)
                {
                    triples = new List<Triple>(triples);

                    MethodInfo add = results.GetType().GetMethod("Add", types);
                    IDictionary<Type, IDictionary<string, MethodInfo>> typePropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MethodInfo>>();

                    foreach (Triple triple in triples)
                    {
                        INode resource = (INode)triple.Subject;
                        object newInstance = Activator.CreateInstance(beanType);
                        IDictionary<string, object> visitedResources = new DictionaryWithReplacement<string, object>();

                        FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                          beanType,
                                          newInstance,
                                          resource,
                                          visitedResources);

                        add.Invoke(results, new object[] { newInstance });
                    }
                }
            }

            return results;
        }

        private static void FromDotNetRdfNode(IDictionary<Type, IDictionary<string, MethodInfo>> typePropertyDefinitionsToSetMethods,
                                              Type beanType,
                                              object bean,
                                              INode resource,
                                              IDictionary<string, object> visitedResources)
        {
            IGraph graph = resource.Graph;

            IDictionary<string, MethodInfo> setMethodMap;

            if (typePropertyDefinitionsToSetMethods.ContainsKey(beanType))
            {
                setMethodMap = typePropertyDefinitionsToSetMethods[beanType];
            }
            else
            {
                setMethodMap = CreatePropertyDefinitionToSetMethods(beanType);

                typePropertyDefinitionsToSetMethods.Add(beanType,
                                                        setMethodMap);
            }

            visitedResources.Add(GetVisitedResourceName(resource), bean);

            if (bean is IResource)
            {
                Uri aboutURI = resource is IUriNode ? ((IUriNode)resource).Uri : null;
                if (aboutURI != null)
                {
                    if (!aboutURI.IsAbsoluteUri)
                    {
                        throw new OslcCoreRelativeURIException(beanType,
                                                               "setAbout",
                                                               aboutURI);
                    }

                    ((IResource)bean).SetAbout(aboutURI);
                }
            }

            // Collect values for array properties. We do this since values for
            // arrays are not required to be contiguous.
            IDictionary<string, List<object>> propertyDefinitionsToArrayValues = new Dictionary<string, List<object>>();

            // Ensure a single-value property is not set more than once
            ISet<MethodInfo> singleValueMethodsUsed = new HashSet<MethodInfo>();

            IEnumerable<Triple> triples = graph.GetTriplesWithSubject(resource);

            IExtendedResource extendedResource;
            IDictionary<QName, object> extendedProperties;
            if (bean is IExtendedResource)
            {
                extendedResource = (IExtendedResource)bean;
                extendedProperties = new DictionaryWithReplacement<QName, object>();
                extendedResource.SetExtendedProperties(extendedProperties);
            }
            else
            {
                extendedResource = null;
                extendedProperties = null;
            }

            foreach (Triple triple in triples)
            {
                IUriNode predicate = (IUriNode)triple.Predicate;
                INode obj = triple.Object;
                string uri = predicate.Uri.ToString();

                if (!setMethodMap.ContainsKey(uri))
                {
                    if (RdfSpecsHelper.RdfType.Equals(uri))
                    {
                        if (extendedResource != null)
                        {
                            Uri type = ((IUriNode)obj).Uri;
                            extendedResource.AddType(type);
                        }
                        // Otherwise ignore missing propertyDefinition for rdf:type.
                    }
                    else
                    {
                        if (extendedProperties == null)
                        {
                            logger.Warn("Set method not found for object type:  " +
                                           beanType.Name +
                                           ", uri:  " +
                                           uri);
                        }
                        else
                        {
                            string predicateUri = predicate.Uri.ToString();
                            string prefix;
                            string localPart;
                            string ns;
                            string qname;

                            if (graph.NamespaceMap.ReduceToQName(predicateUri, out qname))
                            {
                                int colon = qname.IndexOf(':');
                                prefix = qname.Substring(0, colon);
                                localPart = qname.Substring(colon + 1);
                                ns = predicateUri.Substring(0, predicateUri.Length - localPart.Length);
                            }
                            else
                            {
                                int hash = predicateUri.LastIndexOf('#');
                                int slash = predicateUri.LastIndexOf('/');
                                int idx = hash > slash ? hash : slash;

                                localPart = predicateUri.Substring(idx + 1);
                                ns = predicateUri.Substring(0, idx + 1);
                                prefix = graph.NamespaceMap.GetPrefix(new Uri(ns));
                                if (prefix == null)
                                {
                                    prefix = GeneratePrefix(graph, ns);
                                }
                            }
                            QName key = new QName(ns, localPart, prefix);
                            object value = HandleExtendedPropertyValue(beanType, obj, visitedResources);
                            if (!extendedProperties.ContainsKey(key))
                            {
                                extendedProperties.Add(key, value);
                            }
                            else
                            {
                                object previous = extendedProperties[key];
                                IList<object> collection;
                                if (previous is IList<object>)
                                {
                                    collection = ((IList<object>)previous);
                                }
                                else
                                {
                                    collection = new List<object>();
                                    collection.Add(previous);
                                    extendedProperties.Add(key, collection);
                                }

                                collection.Add(value);
                            }
                        }
                    }
                }
                else
                {
                    MethodInfo setMethod = setMethodMap[uri];
                    Type setMethodComponentParameterType = setMethod.GetParameters()[0].ParameterType;

                    bool multiple = setMethodComponentParameterType.IsArray;
                    if (multiple)
                    {
                        setMethodComponentParameterType = setMethodComponentParameterType.GetElementType();
                    }
                    else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), setMethodComponentParameterType))
                    {
                        multiple = true;

                        setMethodComponentParameterType = setMethodComponentParameterType.GetGenericArguments()[0];
                    }

                    Uri nil = new Uri(OslcConstants.RDF_NAMESPACE + RDF_NIL);
                    IList<INode> objects;

                    if (multiple && obj is IUriNode && (
                           (graph.GetTriplesWithSubjectPredicate(obj, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst))).Count() > 0
                               && graph.GetTriplesWithSubjectPredicate(obj, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest))).Count() > 0)
                           || nil.Equals((obj as IUriNode).Uri)
                           || (((IUriNode)obj).IsListRoot(graph))))
                    {
                        objects = new List<INode>();
                        IUriNode listNode = obj as IUriNode;

                        while (listNode != null && !nil.Equals(listNode.Uri))
                        {
                            visitedResources.Add(GetVisitedResourceName(listNode), new object());

                            IUriNode o = (IUriNode)graph.GetTriplesWithSubjectPredicate(listNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst))).First().Object;
                            objects.Add(o);

                            listNode = (IUriNode)graph.GetTriplesWithSubjectPredicate(listNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest))).First().Object;
                        }

                        visitedResources.Add(GetVisitedResourceName(obj as IUriNode), objects);
                    }
                    else if (multiple && IsRdfCollectionResource(graph, obj))
                    {
                        objects = new List<INode>();
                        IEnumerable<Triple> trips = graph.GetTriplesWithSubjectPredicate(obj, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + "li")));

                        foreach (Triple trip in trips)
                        {
                            INode o = trip.Object;

                            if (o is IUriNode)
                            {
                                visitedResources.Add(GetVisitedResourceName(o as IUriNode), new object());
                            }

                            objects.Add(o);
                        }

                        visitedResources.Add(GetVisitedResourceName(obj as IUriNode), objects);
                    }
                    else
                    {
                        objects = new List<INode>();

                        objects.Add(obj);

                        objects = new ReadOnlyCollection<INode>(objects);
                    }

                    Type reifiedType = null;
                    if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), setMethodComponentParameterType))
                    {
                        reifiedType = setMethodComponentParameterType;

                        while (!setMethodComponentParameterType.IsGenericType)
                        {
                            setMethodComponentParameterType = setMethodComponentParameterType.BaseType;
                        }

                        setMethodComponentParameterType = setMethodComponentParameterType.GetGenericArguments()[0];
                    }

                    foreach (INode o in objects)
                    {
                        object parameter = null;
                        if (o is ILiteralNode)
                        {
                            ILiteralNode literal = o as ILiteralNode;
                            string stringValue = literal.Value;

                            if (typeof(string) == setMethodComponentParameterType)
                            {
                                parameter = stringValue;
                            }
                            else if ((typeof(bool) == setMethodComponentParameterType) ||
                                     (typeof(bool?) == setMethodComponentParameterType))
                            {
                                // XML supports both 'true' and '1' for a true Boolean.
                                // Cannot use Boolean.parseBoolean since it supports case-insensitive TRUE.
                                if (("true".Equals(stringValue)) ||
                                    ("1".Equals(stringValue)))
                                {
                                    parameter = true;
                                }
                                // XML supports both 'false' and '0' for a false Boolean.
                                else if (("false".Equals(stringValue)) ||
                                         ("0".Equals(stringValue)))
                                {
                                    parameter = false;
                                }
                                else
                                {
                                    throw new ArgumentException("'" + stringValue + "' has wrong format for Boolean.");
                                }
                            }
                            else if ((typeof(byte) == setMethodComponentParameterType) ||
                                     (typeof(byte?) == setMethodComponentParameterType))
                            {
                                parameter = byte.Parse(stringValue);
                            }
                            else if ((typeof(short) == setMethodComponentParameterType) ||
                                     (typeof(short?) == setMethodComponentParameterType))
                            {
                                parameter = short.Parse(stringValue);
                            }
                            else if ((typeof(int) == setMethodComponentParameterType) ||
                                     (typeof(int?) == setMethodComponentParameterType))
                            {
                                parameter = int.Parse(stringValue);
                            }
                            else if ((typeof(long) == setMethodComponentParameterType) ||
                                     (typeof(long?) == setMethodComponentParameterType))
                            {
                                parameter = long.Parse(stringValue);
                            }
                            else if (typeof(BigInteger) == setMethodComponentParameterType)
                            {
                                parameter = BigInteger.Parse(stringValue);
                            }
                            else if ((typeof(float) == setMethodComponentParameterType) ||
                                     (typeof(float?) == setMethodComponentParameterType))
                            {
                                parameter = float.Parse(stringValue);
                            }
                            else if ((typeof(decimal) == setMethodComponentParameterType) ||
                                     (typeof(decimal?) == setMethodComponentParameterType))
                            {
                                parameter = decimal.Parse(stringValue);
                            }
                            else if ((typeof(double) == setMethodComponentParameterType) ||
                                     (typeof(double?) == setMethodComponentParameterType))
                            {
                                parameter = double.Parse(stringValue);
                            }
                            else if ((typeof(DateTime) == setMethodComponentParameterType) ||
                                     (typeof(DateTime?) == setMethodComponentParameterType))
                            {
                                parameter = DateTime.Parse(stringValue);
                            }
                        }
                        else if (o is IUriNode)
                        {
                            IUriNode nestedResource = o as IUriNode;

                            if (typeof(Uri) == setMethodComponentParameterType)
                            {
                                string nestedResourceURIString = nestedResource.Uri.ToString();

                                if (nestedResourceURIString != null)
                                {
                                    Uri nestedResourceURI = nestedResource.Uri;

                                    if (!nestedResourceURI.IsAbsoluteUri)
                                    {
                                        throw new OslcCoreRelativeURIException(beanType,
                                                                               setMethod.Name,
                                                                               nestedResourceURI);
                                    }

                                    parameter = nestedResourceURI;
                                }
                            }
                            else
                            {
                                object nestedBean;

                                if (setMethodComponentParameterType == typeof(string))
                                {
                                    nestedBean = "";
                                }
                                else if (setMethodComponentParameterType == typeof(Uri))
                                {
                                    nestedBean = new Uri("http://localhost/");
                                }
                                else
                                {
                                    nestedBean = Activator.CreateInstance(setMethodComponentParameterType);
                                }

                                FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                                  setMethodComponentParameterType,
                                                  nestedBean,
                                                  nestedResource,
                                                  visitedResources);

                                parameter = nestedBean;
                            }
                        }
                        else if (o is IBlankNode)
                        {
                            IBlankNode nestedResource = o as IBlankNode;
                            object nestedBean;

                            if (setMethodComponentParameterType == typeof(string))
                            {
                                nestedBean = "";
                            }
                            else if (setMethodComponentParameterType == typeof(Uri))
                            {
                                nestedBean = new Uri("http://localhost/");
                            }
                            else
                            {
                                nestedBean = Activator.CreateInstance(setMethodComponentParameterType);
                            }

                            FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                              setMethodComponentParameterType,
                                              nestedBean,
                                              nestedResource,
                                              visitedResources);

                            parameter = nestedBean;
                        }

                        if (parameter != null)
                        {
                            if (reifiedType != null)
                            {
                                // This property supports reified statements. Create the
                                // new resource to hold the value and any metadata.
                                object reifiedResource = Activator.CreateInstance(reifiedType);

                                // Find a setter for the actual value.
                                foreach (MethodInfo method in reifiedType.GetMethods())
                                {
                                    if (!"SetValue".Equals(method.Name))
                                    {
                                        continue;
                                    }
                                    ParameterInfo[] parameters = method.GetParameters();
                                    if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(setMethodComponentParameterType))
                                    {
                                        method.Invoke(reifiedResource, new object[] { parameter });
                                        break;
                                    }
                                }

                                // Fill in any reified statements.
                                IEnumerable<Triple> reifiedTriples = GetReifiedTriples(triple);

                                foreach (Triple reifiedTriple in reifiedTriples)
                                {
                                    FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                                      reifiedType,
                                                      reifiedResource,
                                                      reifiedTriple.Subject,
                                                      visitedResources);
                                }

                                parameter = reifiedResource;
                            }

                            if (multiple)
                            {
                                List<object> values;
                                if (propertyDefinitionsToArrayValues.ContainsKey(uri))
                                {
                                    values = propertyDefinitionsToArrayValues[uri];
                                }
                                else
                                {
                                    values = new List<object>();

                                    propertyDefinitionsToArrayValues.Add(uri,
                                                                         values);
                                }

                                values.Add(parameter);
                            }
                            else
                            {
                                if (singleValueMethodsUsed.Contains(setMethod))
                                {
                                    throw new OslcCoreMisusedOccursException(beanType,
                                                                             setMethod);
                                }

                                setMethod.Invoke(bean,
                                                 new object[] { parameter });

                                singleValueMethodsUsed.Add(setMethod);
                            }
                        }
                    }
                }
            }

            // Now, handle array and collection values since all are collected.
            foreach (string uri in propertyDefinitionsToArrayValues.Keys)
            {
                List<object> values = propertyDefinitionsToArrayValues[uri];
                MethodInfo setMethod = setMethodMap[uri];
                Type parameterType = setMethod.GetParameters()[0].ParameterType;

                if (parameterType.IsArray)
                {
                    Type setMethodComponentParameterType = parameterType.GetElementType();

                    // To support primitive arrays, we have to use Array reflection to
                    // set individual elements. We cannot use Collection.toArray.
                    // Array.set will unwrap objects to their corresponding primitives.
                    Array array = Array.CreateInstance(setMethodComponentParameterType,
                                                       values.Count);

                    int index = 0;
                    foreach (object value in values)
                    {
                        array.SetValue(value,
                                       index++);
                    }

                    setMethod.Invoke(bean,
                                     new object[] { array });
                }
                // Else - we are dealing with a collection or a subclass of collection
                else
                {
                    object collection = Activator.CreateInstance(parameterType);

                    if (values.Count > 0)
                    {
                        MethodInfo add = collection.GetType().GetMethod("Add", new Type[] { values[0].GetType() });

                        foreach (object value in values)
                        {
                            add.Invoke(collection, new object[] { value });
                        }
                    }

                    setMethod.Invoke(bean,
                                     new object[] { collection });
                }
            }
        }

        private static bool IsRdfCollectionResource(IGraph graph, INode obj)
        {
            if (obj is IUriNode)
            {
                IUriNode resource = (IUriNode)obj;
                if (graph.GetTriplesWithSubjectPredicate(resource, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_ALT))).Count() > 0
                    || graph.GetTriplesWithSubjectPredicate(resource, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_BAG))).Count() > 0
                    || graph.GetTriplesWithSubjectPredicate(resource, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_SEQ))).Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // XXX - Not sure if this is how reification works.  Will have to test later.
        private static IEnumerable<Triple> GetReifiedTriples(Triple source)
        {
            IGraph graph = source.Graph;
            IUriNode rdfSubject = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject));
            IEnumerable<Triple> reifiedSubjects =
                graph.GetTriplesWithPredicateObject(rdfSubject, source.Subject);

            if (reifiedSubjects.Count() == 0)
            {
                return new Triple[0];
            }

            IUriNode rdfPredicate = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfPredicate));
            IUriNode rdfObject = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject));
            IUriNode rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            IUriNode rdfStatement = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfStatement));

            IList<Triple> reificationTriples = new List<Triple>(4);
            Triple[] result = new Triple[1];

            foreach (Triple candidate in reifiedSubjects)
            {
                IEnumerable<Triple> blankNodes = graph.GetTriplesWithSubject(candidate.Subject);

                foreach (Triple nodeChild in blankNodes)
                {
                    if ((nodeChild.Predicate.Equals(rdfSubject) && nodeChild.Object.Equals(source.Subject)) ||
                        (nodeChild.Predicate.Equals(rdfPredicate) && nodeChild.Object.Equals(source.Predicate)) ||
                        (nodeChild.Predicate.Equals(rdfObject) && nodeChild.Object.Equals(source.Object)) ||
                        (nodeChild.Predicate.Equals(rdfType) && nodeChild.Object.Equals(rdfStatement)))
                    {
                        reificationTriples.Add(nodeChild);
                    }
                    else
                    {
                        result[0] = nodeChild;
                    }
                }

                if (reificationTriples.Count == 4 && result[0] != null)
                {
                    graph.Retract(reificationTriples);

                    return result;
                }

                reificationTriples.Clear();
                result[0] = null;
            }

            return new Triple[0];
        }

        /**
         * Generates a prefix for unrecognized namespaces when reading in unknown
         * properties and content.
         * 
         * @param graph
         *            the graph
         * @param ns
         *            the unrecognized namespace Uri that needs a prefix
         * @return the generated prefix (e.g., 'j.0')
         */
        private static string GeneratePrefix(IGraph graph, string ns)
        {
            INamespaceMapper map = graph.NamespaceMap;
            int i = 0;
            string candidatePrefix;
            do
            {
                candidatePrefix = GENERATED_PREFIX_START + i;
                ++i;
            } while (map.GetNamespaceUri(candidatePrefix) == null);

            map.AddNamespace(candidatePrefix, new Uri(ns));
            return candidatePrefix;
        }

        private static object HandleExtendedPropertyValue(Type beanType,
                                                          INode obj,
                                                          IDictionary<string, object> visitedResources)
        {
            if (obj is ILiteralNode)
            {
                if (obj is BooleanNode)
                {
                    return ((BooleanNode)obj).AsBoolean();
                }
                else if (obj is ByteNode)
                {
                    return byte.Parse(((ByteNode)obj).Value);
                }
                else if (obj is DateTimeNode)
                {
                    return ((DateTimeNode)obj).AsDateTime();
                }
                else if (obj is DecimalNode)
                {
                    return ((DecimalNode)obj).AsDecimal();
                }
                else if (obj is DoubleNode)
                {
                    return ((DoubleNode)obj).AsDouble();
                }
                else if (obj is FloatNode)
                {
                    return ((FloatNode)obj).AsFloat();
                }
                else if (obj is DecimalNode)
                {
                    return ((DecimalNode)obj).AsDecimal();
                }
                else if (obj is LongNode)
                {
                    return ((LongNode)obj).AsInteger();
                }
                else if (obj is SignedByteNode)
                {
                    return (byte)((SignedByteNode)obj).AsInteger();
                }
                else if (obj is StringNode)
                {
                    return ((StringNode)obj).AsString();
                }
                else if (obj is UnsignedLongNode)
                {
                    return ((UnsignedLongNode)obj).AsInteger();
                }
                else
                {
                    return ((ILiteralNode)obj).Value;
                }
            }

            IUriNode nestedResource = obj as IUriNode;

            // Is this an inline resource? AND we have not visited it yet?
            if ((obj is IBlankNode || nestedResource.Graph.GetTriplesWithSubject(nestedResource).Count() > 0) &&
                (!visitedResources.ContainsKey(GetVisitedResourceName(obj))))
            {
                AbstractResource any = new AnyResource();
                IDictionary<Type, IDictionary<string, MethodInfo>> typePropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MethodInfo>>();
                FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                  typeof(AnyResource),
                                  any,
                                  obj,
                                  visitedResources);

                return any;
            }

            if (obj is IBlankNode || nestedResource.Graph.GetTriplesWithSubject(nestedResource).Count() > 0)
            {
                return visitedResources[GetVisitedResourceName(nestedResource)];
            }
            else
            {
                // It's a resource reference.
                Uri nestedResourceURI = nestedResource.Uri;
                if (!nestedResourceURI.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(beanType, "<none>",
                                                           nestedResourceURI);
                }

                return nestedResourceURI;
            }
        }

        private static IDictionary<string, MethodInfo> CreatePropertyDefinitionToSetMethods(Type beanType)
        {
            IDictionary<string, MethodInfo> result = new Dictionary<string, MethodInfo>();

            MethodInfo[] methods = beanType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 0)
                {
                    string getMethodName = method.Name;

                    if (((getMethodName.StartsWith(METHOD_NAME_START_GET)) &&
                         (getMethodName.Length > METHOD_NAME_START_GET_LENGTH)) ||
                        ((getMethodName.StartsWith(METHOD_NAME_START_IS)) &&
                         (getMethodName.Length > METHOD_NAME_START_IS_LENGTH)))
                    {
                        OslcPropertyDefinition oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);

                        if (oslcPropertyDefinitionAnnotation != null)
                        {
                            // We need to find the set companion setMethod
                            string setMethodName;
                            if (getMethodName.StartsWith(METHOD_NAME_START_GET))
                            {
                                setMethodName = METHOD_NAME_START_SET +
                                                getMethodName.Substring(METHOD_NAME_START_GET_LENGTH);
                            }
                            else
                            {
                                setMethodName = METHOD_NAME_START_SET +
                                                getMethodName.Substring(METHOD_NAME_START_IS_LENGTH);
                            }

                            Type getMethodReturnType = method.ReturnType;

                            MethodInfo setMethod = beanType.GetMethod(setMethodName,
                                                                      new Type[] { getMethodReturnType });

                            if (setMethod != null)
                            {
                                result.Add(oslcPropertyDefinitionAnnotation.value,
                                           setMethod);
                            }
                            else
                            {
                                throw new OslcCoreMissingSetMethodException(beanType,
                                                                            method);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static void BuildResource(object obj,
                                          Type resourceType,
                                          IGraph graph,
                                          INode mainResource,
                                          IDictionary<string, object> properties)
        {
            if (properties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
            {
                return;
            }

            foreach (MethodInfo method in resourceType.GetMethods())
            {
                if (method.GetParameters().Length == 0)
                {
                    string methodName = method.Name;

                    if (((methodName.StartsWith(METHOD_NAME_START_GET)) &&
                         (methodName.Length > METHOD_NAME_START_GET_LENGTH)) ||
                        ((methodName.StartsWith(METHOD_NAME_START_IS)) &&
                         (methodName.Length > METHOD_NAME_START_IS_LENGTH)))
                    {
                        OslcPropertyDefinition oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);

                        if (oslcPropertyDefinitionAnnotation != null)
                        {
                            object value = method.Invoke(obj, null);

                            if (value != null)
                            {
                                IDictionary<string, object> nestedProperties = null;
                                bool onlyNested = false;

                                if (properties != null)
                                {
                                    IDictionary<string, object> map = (IDictionary<string, object>)properties[oslcPropertyDefinitionAnnotation.value];

                                    if (map != null)
                                    {
                                        nestedProperties = map;
                                    }
                                    else if (properties is SingletonWildcardProperties &&
                                             !(properties is NestedWildcardProperties))
                                    {
                                        nestedProperties = OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                                    }
                                    else if (properties is NestedWildcardProperties)
                                    {
                                        nestedProperties = ((NestedWildcardProperties)properties).CommonNestedProperties();
                                        onlyNested = !(properties is SingletonWildcardProperties);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                BuildAttributeResource(resourceType,
                                                       method,
                                                       oslcPropertyDefinitionAnnotation,
                                                       graph,
                                                       mainResource,
                                                       value,
                                                       nestedProperties,
                                                       onlyNested);
                            }
                        }
                    }
                }
            }

            // Handle any extended properties.
            if (obj is IExtendedResource)
            {
                IExtendedResource extendedResource = (IExtendedResource)obj;
                HandleExtendedProperties(resourceType,
                                         graph,
                                         mainResource,
                                         extendedResource,
                                         properties);
            }
        }

        private static void HandleExtendedProperties(Type resourceType,
                                                     IGraph graph,
                                                     INode mainResource,
                                                     IExtendedResource extendedResource,
                                                     IDictionary<string, object> properties)
        {
            foreach (Uri type in extendedResource.GetTypes())
            {
                string propertyName = type.ToString();

                if (properties != null &&
                    properties[propertyName] == null &&
                    !(properties is NestedWildcardProperties) &&
                    !(properties is SingletonWildcardProperties))
                {
                    continue;
                }

                IUriNode typeResource = graph.CreateUriNode(new Uri(propertyName));
                IUriNode rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
                if (graph.GetTriplesWithPredicateObject(rdfType, typeResource).Count() == 0)
                {
                    graph.Assert(new Triple(mainResource, rdfType, typeResource));
                }
            }

            IDictionary<QName, object> extendedProperties = extendedResource.GetExtendedProperties();

            foreach (QName qName in extendedProperties.Keys)
            {
                string propertyName = qName.GetNamespaceURI() + qName.GetLocalPart();
                IDictionary<string, object> nestedProperties = null;
                bool onlyNested = false;

                if (properties != null)
                {
                    IDictionary<string, object> map = (IDictionary<string, object>)properties[propertyName];

                    if (map != null)
                    {
                        nestedProperties = map;
                    }
                    else if (properties is SingletonWildcardProperties &&
                             !(properties is NestedWildcardProperties))
                    {
                        nestedProperties = OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                    }
                    else if (properties is NestedWildcardProperties)
                    {
                        nestedProperties = ((NestedWildcardProperties)properties).CommonNestedProperties();
                        onlyNested = !(properties is SingletonWildcardProperties);
                    }
                    else
                    {
                        continue;
                    }
                }

                IUriNode property = graph.CreateUriNode(new Uri(propertyName));
                object value = extendedProperties[qName];

                if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), value.GetType()))
                {
                    IEnumerable<object> collection = new EnumerableWrapper(value);
                    foreach (object next in collection)
                    {
                        HandleExtendedValue(resourceType,
                                            next,
                                            graph,
                                            mainResource,
                                            property,
                                            nestedProperties,
                                            onlyNested);
                    }
                }
                else
                {
                    HandleExtendedValue(resourceType,
                                        value,
                                        graph,
                                        mainResource,
                                        property,
                                        nestedProperties,
                                        onlyNested);
                }
            }
        }

        private static void HandleExtendedValue(Type objType,
                                                object value,
                                                IGraph graph,
                                                INode resource,
                                                IUriNode property,
                                                IDictionary<string, object> nestedProperties,
                                                bool onlyNested)
        {
            if (value is AnyResource)
            {
                AbstractResource any = (AbstractResource)value;
                INode nestedResource;
                Uri aboutURI = any.GetAbout();
                if (aboutURI != null)
                {
                    if (!aboutURI.IsAbsoluteUri)
                    {
                        throw new OslcCoreRelativeURIException(typeof(AnyResource),
                            "getAbout",
                            aboutURI);
                    }

                    nestedResource = graph.CreateUriNode(aboutURI);
                }
                else
                {
                    nestedResource = graph.CreateBlankNode();
                }

                foreach (Uri type in any.GetTypes())
                {
                    string propertyName = type.ToString();

                    if (nestedProperties == null ||
                        nestedProperties[propertyName] != null ||
                        nestedProperties is NestedWildcardProperties ||
                        nestedProperties is SingletonWildcardProperties)
                    {
                        graph.Assert(new Triple(nestedResource, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                                graph.CreateUriNode(new Uri(propertyName))));
                    }
                }

                HandleExtendedProperties(typeof(AnyResource), graph, nestedResource, any, nestedProperties);
                graph.Assert(new Triple(resource, property, nestedResource));
            }
            else if (value.GetType().GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0 || value is Uri)
            {
                //TODO:  Until we handle XMLLiteral for incoming unknown resources, need to assume it is not XMLLiteral
                bool xmlliteral = false;
                HandleLocalResource(objType,
                                    null,
                                    xmlliteral,
                                    value,
                                    graph,
                                    resource,
                                    property,
                                    nestedProperties,
                                    onlyNested,
                                    null);
            }
            else
            {
                if (onlyNested)
                {
                    return;
                }

                ILiteralNode literal;
                Type valueType = value.GetType();

                if (typeof(string) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((string)value, graph);
                }
                else if (typeof(bool) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((bool)value, graph);
                }
                else if (typeof(byte) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((byte)value, graph);
                }
                else if (typeof(short) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((short)value, graph);
                }
                else if (typeof(int) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((int)value, graph);
                }
                else if (typeof(long) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((long)value, graph);
                }
                else if (typeof(BigInteger) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((long)(BigInteger)value, graph);
                }
                else if (typeof(float) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((float)value, graph);
                }
                else if (typeof(decimal) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((decimal)value, graph);
                }
                else if (typeof(double) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral((double)value, graph);
                }
                else if (typeof(DateTime) == valueType)
                {
                    literal = LiteralExtensions.ToLiteral(((DateTime)value).ToUniversalTime(), graph);
                }
                else if (typeof(XElement) == valueType)
                {
                    literal = graph.CreateLiteralNode(((XElement)value).ToString(SaveOptions.None), new Uri(RdfSpecsHelper.RdfXmlLiteral));
                }
                else
                {
                    throw new InvalidOperationException("Unkown type: " + valueType);
                }

                graph.Assert(new Triple(resource, property, literal));
            }
        }

        private static void BuildAttributeResource(Type resourceType,
                                                   MethodInfo method,
                                                   OslcPropertyDefinition propertyDefinitionAnnotation,
                                                   IGraph graph,
                                                   INode resource,
                                                   object value,
                                                   IDictionary<string, object> nestedProperties,
                                                   bool onlyNested)
        {
            string propertyDefinition = propertyDefinitionAnnotation.value;

            OslcName nameAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcName>(method);

            string name;
            if (nameAnnotation != null)
            {
                name = nameAnnotation.value;
            }
            else
            {
                name = GetDefaultPropertyName(method.Name);
            }

            if (!propertyDefinition.EndsWith(name))
            {
                throw new OslcCoreInvalidPropertyDefinitionException(resourceType,
                                                                     method,
                                                                     propertyDefinitionAnnotation);
            }

            OslcValueType valueTypeAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcValueType>(method);

            bool xmlLiteral = valueTypeAnnotation == null ? false : Model.ValueType.XMLLiteral.Equals(valueTypeAnnotation.value);

            IUriNode attribute = graph.CreateUriNode(new Uri(propertyDefinition));

            Type returnType = method.ReturnType;
            OslcRdfCollectionType collectionType =
                InheritedMethodAttributeHelper.GetAttribute<OslcRdfCollectionType>(method);
            List<INode> rdfNodeContainer;

            if (collectionType != null &&
                    OslcConstants.RDF_NAMESPACE.Equals(collectionType.namespaceURI) &&
                      (RDF_LIST.Equals(collectionType.collectionType)
                       || RDF_ALT.Equals(collectionType.collectionType)
                       || RDF_BAG.Equals(collectionType.collectionType)
                       || RDF_SEQ.Equals(collectionType.collectionType)))
            {
                rdfNodeContainer = new List<INode>();
            }
            else
            {
                rdfNodeContainer = null;
            }

            if (returnType.IsArray)
            {
                // We cannot cast to object[] in case this is an array of
                // primitives. We will use Array reflection instead.
                // Strange case about primitive arrays: they cannot be cast to
                // object[], but retrieving their individual elements
                // via Array.get does not return primitives, but the primitive
                // object wrapping counterparts like Integer, Byte, Double, etc.

                int length =
                    (int)value.GetType().GetProperty("Length").GetValue(value, null);
                MethodInfo getValue = value.GetType().GetMethod("GetValue", new Type[] { typeof(int) });

                for (int index = 0;
                     index < length;
                     index++)
                {
                    object obj = getValue.Invoke(value, new object[] { index });

                    HandleLocalResource(resourceType,
                                        method,
                                        xmlLiteral,
                                        obj,
                                        graph,
                                        resource,
                                        attribute,
                                        nestedProperties,
                                        onlyNested,
                                        rdfNodeContainer);
                }

                if (rdfNodeContainer != null)
                {
                    INode container = CreateRdfContainer(collectionType,
                                                         rdfNodeContainer,
                                                         graph);
                    graph.Assert(new Triple(resource, attribute, container));
                }
            }
            else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), returnType))
            {
                IEnumerable<object> collection = new EnumerableWrapper(value);

                foreach (object obj in collection)
                {
                    HandleLocalResource(resourceType,
                                        method,
                                        xmlLiteral,
                                        obj,
                                        graph,
                                        resource,
                                        attribute,
                                        nestedProperties,
                                        onlyNested,
                                        rdfNodeContainer);
                }

                if (rdfNodeContainer != null)
                {
                    INode container = CreateRdfContainer(collectionType,
                                                         rdfNodeContainer,
                                                         graph);
                    graph.Assert(new Triple(resource, attribute, container));
                }
            }
            else
            {
                HandleLocalResource(resourceType,
                                    method,
                                    xmlLiteral,
                                    value,
                                    graph,
                                    resource,
                                    attribute,
                                    nestedProperties,
                                    onlyNested,
                                    null);
            }
        }

        private static INode CreateRdfContainer(OslcRdfCollectionType collectionType,
                                                IList<INode> rdfNodeContainer,
                                                IGraph graph)
        {
            if (RDF_LIST.Equals(collectionType.collectionType))
            {
                INode root = graph.CreateBlankNode();
                INode current = root;

                for (int i = 0; i < rdfNodeContainer.Count() - 1; i++)
                {
                    INode node = rdfNodeContainer[i];

                    graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst)), node));

                    INode next = graph.CreateBlankNode();

                    graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest)), next));

                    current = next;
                }

                if (rdfNodeContainer.Count() > 0)
                {
                    graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst)),
                                            rdfNodeContainer[rdfNodeContainer.Count() - 1]));
                }

                graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest)),
                                        graph.CreateUriNode(OslcConstants.RDF_NAMESPACE + RDF_NIL)));

                return root;
            }

            INode container = graph.CreateBlankNode();

            if (RDF_ALT.Equals(collectionType.collectionType))
            {
                graph.Assert(new Triple(container, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                        graph.CreateUriNode(OslcConstants.RDF_NAMESPACE + RDF_ALT)));
            }
            else if (RDF_BAG.Equals(collectionType.collectionType))
            {
                graph.Assert(new Triple(container, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                        graph.CreateUriNode(OslcConstants.RDF_NAMESPACE + RDF_BAG)));
            }
            else
            {
                graph.Assert(new Triple(container, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                        graph.CreateUriNode(OslcConstants.RDF_NAMESPACE + RDF_SEQ)));
            }

            foreach (INode node in rdfNodeContainer)
            {
                graph.Assert(new Triple(container, graph.CreateUriNode(OslcConstants.RDF_NAMESPACE + "li"),
                                        node));
            }

            return container;
        }

        private static void HandleLocalResource(Type resourceType,
                                                MethodInfo method,
                                                bool xmlLiteral,
                                                object obj,
                                                IGraph graph,
                                                INode resource,
                                                IUriNode attribute,
                                                IDictionary<string, object> nestedProperties,
                                                bool onlyNested,
                                                List<INode> rdfNodeContainer)
        {
            Type objType = obj.GetType();

            INode nestedNode = null;
            bool isReifiedResource = InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), obj.GetType());
            object value = (!isReifiedResource) ? obj : obj.GetType().GetMethod("GetValue", Type.EmptyTypes).Invoke(obj, null);

            if (value is string)
            {
                if (onlyNested)
                {
                    return;
                }

                if (xmlLiteral)
                {
                    nestedNode = new StringNode(graph, value.ToString(), new Uri(RdfSpecsHelper.RdfXmlLiteral));
                }
                else
                {
                    nestedNode = new StringNode(graph, value.ToString());
                }
            }
            else if ((value is bool) ||
                     (value is byte) ||
                     (value is short) ||
                     (value is int) ||
                     (value is long) ||
                     (value is BigInteger) ||
                     (value is float) ||
                     (value is decimal) ||
                     (value is double))
            {
                if (onlyNested)
                {
                    return;
                }

                Type valueType = value.GetType();

                if (value is bool)
                {
                    nestedNode = LiteralExtensions.ToLiteral((bool)value, graph);
                }
                else if (value is byte)
                {
                    nestedNode = LiteralExtensions.ToLiteral((byte)value, graph);
                }
                else if (value is short)
                {
                    nestedNode = LiteralExtensions.ToLiteral((short)value, graph);
                }
                else if (value is int)
                {
                    nestedNode = LiteralExtensions.ToLiteral((int)value, graph);
                }
                else if (value is long)
                {
                    nestedNode = LiteralExtensions.ToLiteral((long)value, graph);
                }
                else if (value is BigInteger)
                {
                    nestedNode = LiteralExtensions.ToLiteral((long)(BigInteger)value, graph);
                }
                else if (value is float)
                {
                    nestedNode = LiteralExtensions.ToLiteral((float)value, graph);
                }
                else if (value is decimal)
                {
                    nestedNode = LiteralExtensions.ToLiteral((decimal)value, graph);
                }
                else if (value is double)
                {
                    nestedNode = LiteralExtensions.ToLiteral((double)value, graph);
                }
            }
            else if (value is Uri)
            {
                if (onlyNested)
                {
                    return;
                }

                Uri uri = (Uri)value;

                if (!uri.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(resourceType,
                                                           (method == null) ? "<none>" : method.Name,
                                                           uri);
                }

                // URIs represent references to other resources identified by their IDs, so they need to be managed as such
                nestedNode = graph.CreateUriNode(uri);
            }
            else if (value is DateTime)
            {
                if (onlyNested)
                {
                    return;
                }

                nestedNode = new DateTimeNode(graph, ((DateTime)value).ToUniversalTime());
            }
            else if (objType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
            {
                string ns = TypeFactory.GetNamespace(objType);
                string name = TypeFactory.GetName(objType);

                Uri aboutURI = null;
                if (value is IResource)
                {
                    aboutURI = ((IResource)value).GetAbout();
                }

                INode nestedResource;
                if (aboutURI != null)
                {
                    if (!aboutURI.IsAbsoluteUri)
                    {
                        throw new OslcCoreRelativeURIException(objType,
                                                               "getAbout",
                                                               aboutURI);
                    }

                    nestedResource = graph.CreateUriNode(aboutURI);
                }
                else
                {
                    nestedResource = graph.CreateBlankNode();
                }

                graph.Assert(new Triple(nestedResource, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                                        graph.CreateUriNode(new Uri(ns + name))));

                BuildResource(value,
                              objType,
                              graph,
                              nestedResource,
                              nestedProperties);

                nestedNode = nestedResource;
            }

            if (nestedNode != null)
            {
                if (rdfNodeContainer != null)
                {
                    if (isReifiedResource)
                    {
                        // Reified resource is not supported for rdf collection resources
                        throw new OslcCoreInvalidPropertyDefinitionException(resourceType,
                                                                             method,
                                                                             null);
                    }

                    // Instead of adding a nested node to model, add it to a list
                    rdfNodeContainer.Add(nestedNode);
                }
                else
                {
                    Triple triple = new Triple(resource, attribute, nestedNode);

                    if (isReifiedResource &&
                        nestedProperties != OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
                    {
                        AddReifiedStatements(graph, triple, obj, nestedProperties);
                    }

                    // Finally, add the triple to the graph.
                    graph.Assert(triple);
                }
            }
        }

        private static void AddReifiedStatements(IGraph graph,
                                                 Triple triple,
                                                 object reifiedResource,
                                                 IDictionary<string, object> nestedProperties)
        {
            Type reifiedResourceType = reifiedResource.GetType();
            INode uriref = graph.CreateBlankNode();

            graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject)), triple.Subject));
            graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfPredicate)), triple.Predicate));
            graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject)), triple.Object));
            graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)), graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfStatement))));

            BuildResource(reifiedResource,
                          reifiedResource.GetType(),
                          graph,
                          uriref,
                          nestedProperties);
        }

        private static string GetDefaultPropertyName(string methodName)
        {
            int startingIndex = methodName.StartsWith(METHOD_NAME_START_GET) ? METHOD_NAME_START_GET_LENGTH : METHOD_NAME_START_IS_LENGTH;
            int endingIndex = startingIndex + 1;

            // We want the name to start with a lower-case letter
            string lowercasedFirstCharacter = methodName.Substring(startingIndex,
                                                                   1).ToLower(CultureInfo.GetCultureInfo("en"));

            if (methodName.Length == endingIndex)
            {
                return lowercasedFirstCharacter;
            }

            return lowercasedFirstCharacter +
                   methodName.Substring(endingIndex);
        }

        private static void RecursivelyCollectNamespaceMappings(INamespaceMapper namespaceMappings,
                                                                Type resourceType)
        {
            OslcSchema[] oslcSchemaAttribute = (OslcSchema[])resourceType.Assembly.GetCustomAttributes(typeof(OslcSchema), false);

            if (oslcSchemaAttribute.Length > 0)
            {
                OslcNamespaceDefinition[] oslcNamespaceDefinitionAnnotations =
                    (OslcNamespaceDefinition[])oslcSchemaAttribute[0].namespaceType.GetMethod("GetNamespaces", Type.EmptyTypes).Invoke(null, null);

                foreach (OslcNamespaceDefinition oslcNamespaceDefinitionAnnotation in oslcNamespaceDefinitionAnnotations)
                {
                    string prefix = oslcNamespaceDefinitionAnnotation.prefix;
                    string namespaceURI = oslcNamespaceDefinitionAnnotation.namespaceURI;

                    namespaceMappings.AddNamespace(prefix,
                                                   new Uri(namespaceURI));
                }
            }

            Type baseType = resourceType.BaseType;

            if (baseType != null)
            {
                RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                    baseType);
            }

            Type[] interfaces = resourceType.GetInterfaces();

            if (interfaces != null)
            {
                foreach (Type interfac in interfaces)
                {
                    RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                        interfac);
                }
            }
        }

        private static void EnsureNamespacePrefix(string prefix,
                                                  string ns,
                                                  INamespaceMapper namespaceMappings)
        {
            Uri uri = new Uri(ns);

            if (namespaceMappings.GetPrefix(uri) == null)
            {
                if (!namespaceMappings.HasNamespace(prefix))
                {
                    namespaceMappings.AddNamespace(prefix,
                                                   uri);
                }
                else
                {
                    // There is already a namespace for this prefix.  We need to generate a new unique prefix.
                    int index = 1;

                    while (true)
                    {
                        string newPrefix = prefix + index;

                        if (!namespaceMappings.HasNamespace(newPrefix))
                        {
                            namespaceMappings.AddNamespace(newPrefix,
                                                           uri);

                            return;
                        }

                        index++;
                    }
                }
            }
        }

        private static string GetVisitedResourceName(INode resource)
        {
            string visitedResourceName = null;

            if (resource is IUriNode)
            {
                visitedResourceName = (resource as IUriNode).Uri.ToString();
            }
            else
            {
                visitedResourceName = (resource as IBlankNode).InternalID.ToString();
            }

            return visitedResourceName;
        }

        private class DictionaryWithReplacement<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
        {
            public DictionaryWithReplacement() : base()
            {
            }

            public void Add(TKey key, TValue value)
            {
                if (ContainsKey(key))
                {
                    Remove(key);
                }

                base.Add(key, value);
            }
        }
    }
}
