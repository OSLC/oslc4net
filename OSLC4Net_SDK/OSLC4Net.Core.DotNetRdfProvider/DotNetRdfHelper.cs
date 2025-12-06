/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
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

using System.Collections.ObjectModel;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Core.DotNetRdfProvider;

/// <summary>
///     A class to assist with serialization and de-serialization of RDF/XML from/to .NET objects
/// </summary>
public class DotNetRdfHelper(ILogger<DotNetRdfHelper> logger)
{
    public DotNetRdfHelper() : this(NullLoggerFactory.Instance.CreateLogger<DotNetRdfHelper>())
    {
    }

    private readonly ILogger<DotNetRdfHelper> _logger =
        logger;
    private const string PROPERTY_TOTAL_COUNT = "totalCount";
    private const string PROPERTY_NEXT_PAGE = "nextPage";

    private const string METHOD_NAME_START_GET = "Get";
    private const string METHOD_NAME_START_IS = "Is";
    private const string METHOD_NAME_START_SET = "Set";

    private const string GENERATED_PREFIX_START = "j.";

    private const string RDF_ALT = "Alt";
    private const string RDF_BAG = "Bag";
    private const string RDF_SEQ = "Seq";

    private const string RDF_LIST = "List";

    private const string RDF_NIL = "nil";

    private static readonly int METHOD_NAME_START_GET_LENGTH = METHOD_NAME_START_GET.Length;
    private static readonly int METHOD_NAME_START_IS_LENGTH = METHOD_NAME_START_IS.Length;

    /// <summary>
    ///     Create an RDF graph from a collection of .NET objects
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
    ///     Create an RDF Graph from a collection of objects
    /// </summary>
    /// <param name="descriptionAbout">URL for entire collection</param>
    /// <param name="responseInfoAbout">URL for current page of collection</param>
    /// <param name="nextPageAbout">optional URL for next page in collection</param>
    /// <param name="totalCount">
    ///     optional total count of member across all pages; if null will use count of
    ///     passed in members
    /// </param>
    /// <param name="objects">members from this page</param>
    /// <param name="properties">filtering list of properties for each member</param>
    /// <returns>RDF graph of collection</returns>
    public static IGraph CreateDotNetRdfGraph(string? descriptionAbout,
        string? responseInfoAbout,
        string? nextPageAbout,
        long? totalCount,
        IEnumerable<object>? objects,
        IDictionary<string, object>? properties)
    {
        IGraph graph = new Graph();
        var namespaceMappings = graph.NamespaceMap;

        IUriNode? descriptionResource = null;

        if (descriptionAbout != null)
        {
            descriptionResource = graph.CreateUriNode(new Uri(descriptionAbout));

            //The responseInfo can have the same subject URI as the overall collection, especially when paging
            //or query parameters are not included in the request to the QueryCapability.   If a responseInfoAbout
            //URI is not provided, or if it is the same as the descriptionAbout URI, the descriptionResource should be
            //used for RespionseInfo predicates such as totalCount.

            IUriNode responseInfoResource;
            if (responseInfoAbout != null && !responseInfoAbout.Equals(descriptionAbout))
            {
                responseInfoResource = graph.CreateUriNode(new Uri(responseInfoAbout));
            }
            else
            {
                responseInfoResource = descriptionResource;
            }

            graph.Assert(new Triple(responseInfoResource,
                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                graph.CreateUriNode(new Uri(OslcConstants.TYPE_RESPONSE_INFO))));

            var countValue = totalCount ?? objects?.Count() ?? 0;
            graph.Assert(new Triple(responseInfoResource,
                graph.CreateUriNode(
                    new Uri(OslcConstants.OSLC_CORE_NAMESPACE + PROPERTY_TOTAL_COUNT)),
                graph.CreateLiteralNode(countValue.ToString())));

            if (nextPageAbout != null)
            {
                graph.Assert(new Triple(responseInfoResource,
                    graph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE +
                                                PROPERTY_NEXT_PAGE)),
                    graph.CreateUriNode(new Uri(nextPageAbout))));
            }
        }

        if (objects != null)
        {
            foreach (var obj in objects)
            {
                HandleSingleResource(descriptionResource,
                    obj,
                    graph,
                    namespaceMappings,
                    properties);
            }
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

    private static void HandleSingleResource(INode? descriptionResource,
        object obj,
        IGraph graph,
        INamespaceMapper namespaceMappings,
        IDictionary<string, object>? properties)
    {
        var objType = obj.GetType();

        // Collect the namespace prefix -> namespace mappings
        RecursivelyCollectNamespaceMappings(namespaceMappings,
            objType);

        Uri? aboutURI = null;
        if (obj is IResource resource)
        {
            aboutURI = resource.About;
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
            var ns = TypeFactory.GetNamespace(objType);
            var name = TypeFactory.GetName(objType);

            graph.Assert(new Triple(mainResource,
                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                graph.CreateUriNode(new Uri(ns + name))));
        }

        BuildResource(obj,
            objType,
            graph,
            mainResource,
            properties);

        if (descriptionResource != null)
        {
            graph.Assert(new Triple(descriptionResource,
                graph.CreateUriNode(new Uri(OslcConstants.RDFS_NAMESPACE + "member")),
                mainResource));
        }
    }

    /// <summary>
    ///     Create a .NET object from an RDF Node
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="beanType"></param>
    /// <returns></returns>
    public object FromDotNetRdfNode(IUriNode resource,
        IGraph? graph,
        Type beanType)
    {
        var newInstance = Activator.CreateInstance(beanType)!;
        var typePropertyDefinitionsToSetMethods =
            new Dictionary<Type, IDictionary<string, MemberInfo>>();
        // TODO: check that all .Add() calls were converted into [k] = v
        //IDictionary<string, object> visitedResources = new DictionaryWithReplacement<string, object>();
        var visitedResources = new Dictionary<string, object>(StringComparer.Ordinal);

        if (graph != null)
        {
            FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                beanType,
                newInstance,
                resource,
                graph,
                visitedResources);
        }

        return newInstance;
    }

    /// <summary>
    ///     Create a .NET object from an RDF graph
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="beanType"></param>
    /// <returns></returns>
    public object FromDotNetRdfGraph(IGraph graph,
        Type beanType)
    {
        Type[] types = { beanType };
        var results = Activator.CreateInstance(typeof(List<>).MakeGenericType(types))!;

        if (beanType.GetCustomAttributes(typeof(OslcResourceShape), true).Length > 0)
        {
            var qualifiedName = TypeFactory.GetQualifiedName(beanType);

            IEnumerable<Triple> triples = graph.GetTriplesWithPredicateObject(
                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                graph.CreateUriNode(new Uri(qualifiedName)));

            if (triples.Any())
            {
                triples = new List<Triple>(triples);

                var add = results.GetType().GetMethod("Add", types)!;
                var
                    typePropertyDefinitionsToSetMethods =
                        new Dictionary<Type, IDictionary<string, MemberInfo>>();

                foreach (var triple in triples)
                {
                    var resource = triple.Subject;
                    var newInstance = Activator.CreateInstance(beanType)!;
                    //IDictionary<string, object> visitedResources = new DictionaryWithReplacement<string, object>();
                    IDictionary<string, object> visitedResources = new Dictionary<string, object>(StringComparer.Ordinal);

                    FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                        beanType,
                        newInstance,
                        resource,
                        graph,
                        visitedResources);

                    add.Invoke(results, new[] { newInstance });
                }
            }
        }

        return results;
    }

    private void FromDotNetRdfNode(
        IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
        Type beanType,
        object bean,
        INode resource,
        IGraph graph,
        IDictionary<string, object> visitedResources)
    {
        IDictionary<string, MemberInfo> setMethodMap;

        if (typePropertyDefinitionsToSetMethods.TryGetValue(beanType, out var beanTypeValue))
        {
            setMethodMap = beanTypeValue;
        }
        else
        {
            setMethodMap = CreatePropertyDefinitionToSetMethods(beanType);

            typePropertyDefinitionsToSetMethods.Add(beanType,
                setMethodMap);
        }

        var visitedName = GetVisitedResourceName(resource);
        if (visitedName != null)
        {
            visitedResources[visitedName] = bean;
        }

        if (bean is IResource iResource)
        {
            var aboutUri = resource is IUriNode node ? node.Uri : null;
            if (aboutUri != null)
            {
                if (!aboutUri.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(beanType,
                        "setAbout",
                        aboutUri);
                }

                iResource.About = aboutUri;
            }
        }

        // Collect values for array properties. We do this since values for
        // arrays are not required to be contiguous.
        IDictionary<string, List<object>> propertyDefinitionsToArrayValues =
            new Dictionary<string, List<object>>(StringComparer.Ordinal);

        // Ensure a single-value property is not set more than once
        HashSet<MemberInfo> singleValueMethodsUsed = [];

        IEnumerable<Triple> triples = graph.GetTriplesWithSubject(resource);

        IExtendedResource? extendedResource;
        IDictionary<QName, object>? extendedProperties;
        if (bean is IExtendedResource extendedResourcePoco)
        {
            extendedResource = extendedResourcePoco;
            // TODO: check that all .Add() calls are replaced with []=
            extendedProperties = new Dictionary<QName, object>();
            extendedResource.SetExtendedProperties(extendedProperties);
        }
        else
        {
            extendedResource = null;
            extendedProperties = null;
        }

        foreach (var triple in triples)
        {
            var predicate = (IUriNode)triple.Predicate;
            var obj = triple.Object;
            var uri = predicate.Uri.ToString();

            if (!setMethodMap.TryGetValue(uri, out var backingMember))
            {
                if (RdfSpecsHelper.RdfType.Equals(uri))
                {
                    if (extendedResource != null)
                    {
                        var type = ((IUriNode)obj).Uri;
                        extendedResource.AddType(type);
                    }
                    // Otherwise ignore missing propertyDefinition for rdf:type.
                }
                else
                {
                    if (extendedProperties == null)
                    {
                        _logger.LogWarning("Set method not found for object type:  " +
                                           beanType.Name +
                                           ", uri:  " +
                                           uri);
                    }
                    else
                    {
                        var predicateUri = predicate.Uri.ToString();
                        string? prefix = null;
                        string localPart;
                        string ns;
                        string qname;

                        if (graph.NamespaceMap.ReduceToQName(predicateUri, out qname))
                        {
                            var colon = qname.IndexOf(':');
                            prefix = qname.Substring(0, colon);
                            localPart = qname.Substring(colon + 1);
                            ns = predicateUri.Substring(0, predicateUri.Length - localPart.Length);
                        }
                        else
                        {
                            var hash = predicateUri.LastIndexOf('#');
                            var slash = predicateUri.LastIndexOf('/');
                            var idx = hash > slash ? hash : slash;

                            localPart = predicateUri.Substring(idx + 1);
                            ns = predicateUri.Substring(0, idx + 1);
                            try
                            {
                                prefix = graph.NamespaceMap.GetPrefix(new Uri(ns));
                            }
                            catch
                            {
                            }
                            if (prefix == null)
                            {
                                prefix = GeneratePrefix(graph, ns);
                            }
                        }

                        var key = new QName(ns, localPart, prefix);
                        var value =
                            HandleExtendedPropertyValue(beanType, obj, graph, visitedResources);
                        if (!extendedProperties.TryGetValue(key, out var previous))
                        {
                            extendedProperties[key] = value;
                        }
                        else
                        {
                            // TODO: untangle this mess (Andrew 2023-09)
                            // I think the idea was to store the property object directly if the property has
                            // a cardinality of 1, when a new object is added, the backing object shall switch to
                            // an ICollection. ImmutableList<T> should work quite well here.
                            // Alternatively, bite the bullet an back every property with an ICollection.
                            IList<object> collection;
                            if (previous is IList<object> list)
                            {
                                collection = list;
                            }
                            else
                            {
                                collection = new List<object> { previous };
                                extendedProperties[key] = collection;
                            }

                            collection.Add(value);
                        }
                    }
                }
            }
            else
            {
                var (setMethodComponentParameterType, multiple) =
                    GetBackingMemberPrimitiveType(backingMember);

                var nil = new Uri(OslcConstants.RDF_NAMESPACE + RDF_NIL);
                IList<INode> objects;

                if (multiple && obj is IUriNode node && (
                        (graph.GetTriplesWithSubjectPredicate(node,
                             graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst))).Any()
                         && graph.GetTriplesWithSubjectPredicate(node,
                             graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest))).Any())
                        || nil.Equals(node?.Uri)
                        || node.IsListRoot(graph)))
                {
                    objects = new List<INode>();
                    var listNode = node;

                    while (listNode != null && !nil.Equals(listNode.Uri))
                    {
                        var listNodeName = GetVisitedResourceName(listNode);
                        if (listNodeName != null)
                        {
                            visitedResources[listNodeName] = new object();
                        }

                        var o = (IUriNode)graph.GetTriplesWithSubjectPredicate(listNode,
                                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst))).First()
                            .Object;
                        objects.Add(o);

                        listNode = (IUriNode)graph.GetTriplesWithSubjectPredicate(listNode,
                                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest))).First()
                            .Object;
                    }

                    var nodeName = GetVisitedResourceName(node);
                    if (nodeName != null)
                    {
                        visitedResources[nodeName] = objects;
                    }
                }
                else if (multiple && IsRdfCollectionResource(graph, obj))
                {
                    objects = new List<INode>();
                    IEnumerable<Triple> trips = graph.GetTriplesWithSubjectPredicate(obj,
                        graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + "li")));

                    foreach (var trip in trips)
                    {
                        var o = trip.Object;

                        if (o is IUriNode uriNode)
                        {
                            var uriNodeName = GetVisitedResourceName(uriNode);
                            if (uriNodeName != null)
                            {
                                visitedResources[uriNodeName] = new object();
                            }
                        }

                        objects.Add(o);
                    }

                    var objName = GetVisitedResourceName(obj as IUriNode);
                    if (objName != null)
                    {
                        visitedResources[objName] = objects;
                    }
                }
                else
                {
                    objects = new List<INode> { obj };

                    objects = new ReadOnlyCollection<INode>(objects);
                }

                Type? reifiedType = null;
                if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                        typeof(IReifiedResource<>), setMethodComponentParameterType))
                {
                    reifiedType = setMethodComponentParameterType;

                    while (!setMethodComponentParameterType.IsGenericType)
                    {
                        setMethodComponentParameterType = setMethodComponentParameterType.BaseType!;
                    }

                    setMethodComponentParameterType =
                        setMethodComponentParameterType.GetGenericArguments()[0];
                }

                foreach (var o in objects)
                {
                    object? parameter = null;
                    if (o is ILiteralNode)
                    {
                        var literal = o as ILiteralNode;
                        var stringValue = literal?.Value;

                        if (stringValue != null)
                        {
                            if (typeof(string) == setMethodComponentParameterType)
                            {
                                parameter = stringValue;
                            }
                            else if (typeof(bool) == setMethodComponentParameterType ||
                                     typeof(bool?) == setMethodComponentParameterType)
                            {
                                // XML supports both 'true' and '1' for a true Boolean.
                                // Cannot use Boolean.parseBoolean since it supports case-insensitive TRUE.
                                if ("true".Equals(stringValue) ||
                                    "1".Equals(stringValue))
                                {
                                    parameter = true;
                                }
                                // XML supports both 'false' and '0' for a false Boolean.
                                else if ("false".Equals(stringValue) ||
                                         "0".Equals(stringValue))
                                {
                                    parameter = false;
                                }
                                else
                                {
                                    throw new ArgumentException("'" + stringValue +
                                                                "' has wrong format for Boolean.");
                                }
                            }
                            else if (typeof(byte) == setMethodComponentParameterType ||
                                     typeof(byte?) == setMethodComponentParameterType)
                            {
                                parameter = byte.Parse(stringValue);
                            }
                            else if (typeof(short) == setMethodComponentParameterType ||
                                     typeof(short?) == setMethodComponentParameterType)
                            {
                                parameter = short.Parse(stringValue);
                            }
                            else if (typeof(int) == setMethodComponentParameterType ||
                                     typeof(int?) == setMethodComponentParameterType)
                            {
                                parameter = int.Parse(stringValue);
                            }
                            else if (typeof(long) == setMethodComponentParameterType ||
                                     typeof(long?) == setMethodComponentParameterType)
                            {
                                parameter = long.Parse(stringValue);
                            }
                            else if (typeof(BigInteger) == setMethodComponentParameterType)
                            {
                                parameter = BigInteger.Parse(stringValue);
                            }
                            else if (typeof(float) == setMethodComponentParameterType ||
                                     typeof(float?) == setMethodComponentParameterType)
                            {
                                parameter = float.Parse(stringValue);
                            }
                            else if (typeof(decimal) == setMethodComponentParameterType ||
                                     typeof(decimal?) == setMethodComponentParameterType)
                            {
                                parameter = decimal.Parse(stringValue);
                            }
                            else if (typeof(double) == setMethodComponentParameterType ||
                                     typeof(double?) == setMethodComponentParameterType)
                            {
                                parameter = double.Parse(stringValue);
                            }
                            else if (typeof(DateTime) == setMethodComponentParameterType ||
                                     typeof(DateTime?) == setMethodComponentParameterType)
                            {
                                parameter = DateTime.Parse(stringValue);
                            }
                        }
                    }
                    else if (o is IUriNode)
                    {
                        var nestedResource = o as IUriNode;

                        if (typeof(Uri) == setMethodComponentParameterType)
                        {
                            var nestedResourceUriString = nestedResource?.Uri.ToString();

                            if (nestedResourceUriString != null)
                            {
                                Guard.IsNotNull(nestedResource?.Uri?.IsAbsoluteUri);
                                var nestedResourceUri = nestedResource.Uri;

                                if (nestedResourceUri.IsAbsoluteUri == false)
                                {
                                    throw new OslcCoreRelativeURIException(beanType,
                                        backingMember.Name,
                                        nestedResourceUri);
                                }

                                parameter = nestedResourceUri;
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
                                nestedBean =
                                    Activator.CreateInstance(
                                        MapToActivatableType(setMethodComponentParameterType))!;
                            }

                            FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                setMethodComponentParameterType,
                                nestedBean,
                                nestedResource!,
                                graph,
                                visitedResources);

                            parameter = nestedBean;
                        }
                    }
                    else if (o is IBlankNode)
                    {
                        var nestedResource = o as IBlankNode;
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
                            nestedBean = Activator.CreateInstance(setMethodComponentParameterType)!;
                        }

                        FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                            setMethodComponentParameterType,
                            nestedBean,
                            nestedResource,
                            graph,
                            visitedResources);

                        parameter = nestedBean;
                    }

                    if (parameter != null)
                    {
                        if (reifiedType != null)
                        {
                            // This property supports reified statements. Create the
                            // new resource to hold the value and any metadata.
                            var reifiedResource = Activator.CreateInstance(reifiedType)!;

                            // Find a setter for the actual value.
                            foreach (var method in reifiedType.GetMethods())
                            {
                                if (!"SetValue".Equals(method.Name))
                                {
                                    continue;
                                }

                                var parameters = method.GetParameters();
                                if (parameters.Length == 1 && parameters[0].ParameterType
                                        .IsAssignableFrom(setMethodComponentParameterType))
                                {
                                    method.Invoke(reifiedResource, new[] { parameter });
                                    break;
                                }
                            }

                            // Fill in any reified statements.
                            var reifiedTriples = GetReifiedTriples(triple, graph);

                            foreach (var reifiedTriple in reifiedTriples)
                            {
                                FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                                    reifiedType,
                                    reifiedResource,
                                    reifiedTriple.Subject,
                                    graph,
                                    visitedResources);
                            }

                            parameter = reifiedResource;
                        }

                        if (multiple)
                        {
                            List<object> values;
                            if (propertyDefinitionsToArrayValues.TryGetValue(uri, out var value))
                            {
                                values = value;
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
                            if (singleValueMethodsUsed.Contains(backingMember))
                            {
                                throw new OslcCoreMisusedOccursException(beanType,
                                    backingMember);
                            }

                            SetValue(bean, backingMember, parameter);

                            singleValueMethodsUsed.Add(backingMember);
                        }
                    }
                }
            }
        }

        // Now, handle array and collection values since all are collected.
        foreach (var uri in propertyDefinitionsToArrayValues.Keys)
        {
            var values = propertyDefinitionsToArrayValues[uri];
            var settableMember = setMethodMap[uri];
            var parameterType = GetBackingMemberType(settableMember);

            if (parameterType.IsArray)
            {
                var setMethodComponentParameterType = parameterType.GetElementType()!;

                // To support primitive arrays, we have to use Array reflection to
                // set individual elements. We cannot use Collection.toArray.
                // Array.set will unwrap objects to their corresponding primitives.
                var array = Array.CreateInstance(setMethodComponentParameterType,
                    values.Count);

                var index = 0;
                foreach (var value in values)
                {
                    array.SetValue(value,
                        index++);
                }

                SetValue(bean, settableMember, array);
            }
            // Else - we are dealing with a collection or a subclass of collection
            else
            {
                var collection = Activator.CreateInstance(MapToActivatableType(parameterType))!;

                if (values.Count > 0)
                {
                    var add = collection.GetType().GetMethod("Add", [values[0].GetType()]);

                    foreach (var value in values)
                    {
                        add!.Invoke(collection, [value]);
                    }
                }

                SetValue(bean, settableMember, collection);
            }
        }
    }

    private static void SetValue(object bean, MemberInfo backingMember, object parameter)
    {
        if (backingMember is MethodInfo methodInfo)
        {
            methodInfo.Invoke(bean, [parameter]);
        }
        else if (backingMember is PropertyInfo propertyInfo)
        {
            propertyInfo.SetValue(bean, parameter);
        }
        else
        {
            throw new NotSupportedException("Unsupported backing member type.");
        }
    }

    /// <returns>T for non-collection type T and T for collection T[]</returns>
    private static (Type, bool) GetBackingMemberPrimitiveType(MemberInfo backingMember)
    {
        var type = GetBackingMemberType(backingMember);
        return ExtractTypeInformation(type);
    }

    private static Type GetBackingMemberType(MemberInfo backingMember)
    {
        return backingMember switch
        {
            MethodInfo methodInfo => methodInfo.GetParameters()[0]
                .ParameterType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            _ => throw new ArgumentException("Unsupported backing member type", nameof(backingMember))
        };
    }

    private static (Type, bool) ExtractTypeInformation(Type setMethodComponentParameterType)
    {
        var multiple = setMethodComponentParameterType.IsArray;
        if (multiple)
        {
            setMethodComponentParameterType =
                setMethodComponentParameterType.GetElementType()!;
        }
        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                     typeof(IEnumerable<>), setMethodComponentParameterType))
        {
            var genericArguments = setMethodComponentParameterType.GetGenericArguments();

            // REVISIT: multiple args and old-school enumerables - but not e.g. String (@berezovskyi 2025-05)
            if (genericArguments.Length == 1)
            {
                setMethodComponentParameterType = genericArguments[0];

                multiple = true;
            }
        }

        return (setMethodComponentParameterType, multiple);
    }

    private static Type MapToActivatableType(Type setMethodComponentParameterType)
    {
        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                typeof(ISet<>), setMethodComponentParameterType))
        {
            return typeof(HashSet<>).MakeGenericType(setMethodComponentParameterType
                .GetGenericArguments());
        }

        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                typeof(IEnumerable<>), setMethodComponentParameterType))
        {
            return typeof(List<>).MakeGenericType(setMethodComponentParameterType
                .GetGenericArguments());
        }

        return setMethodComponentParameterType;
    }

    private static bool IsRdfCollectionResource(IGraph graph, INode obj)
    {
        if (obj is IUriNode resource)
        {
            if (graph.GetTriplesWithSubjectPredicate(resource,
                    graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_ALT))).Any()
                || graph.GetTriplesWithSubjectPredicate(resource,
                    graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_BAG))).Any()
                || graph.GetTriplesWithSubjectPredicate(resource,
                    graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_SEQ))).Any())
            {
                return true;
            }
        }

        return false;
    }

    // TODO: check if the RDF 1.1 reification works correctly and cover with basic tests.
    private static IEnumerable<Triple> GetReifiedTriples(Triple source, IGraph graph)
    {
        var rdfSubject = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject));
        IEnumerable<Triple> reifiedSubjects =
            graph.GetTriplesWithPredicateObject(rdfSubject, source.Subject);

        if (!reifiedSubjects.Any())
        {
            return Array.Empty<Triple>();
        }

        var rdfPredicate = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfPredicate));
        var rdfObject = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject));
        var rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var rdfStatement = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfStatement));

        IList<Triple> reificationTriples = new List<Triple>(4);
        var result = new Triple?[1];

        foreach (var candidate in reifiedSubjects)
        {
            IEnumerable<Triple> blankNodes = graph.GetTriplesWithSubject(candidate.Subject);

            foreach (var nodeChild in blankNodes)
            {
                if ((nodeChild.Predicate.Equals(rdfSubject) &&
                     nodeChild.Object.Equals(source.Subject)) ||
                    (nodeChild.Predicate.Equals(rdfPredicate) &&
                     nodeChild.Object.Equals(source.Predicate)) ||
                    (nodeChild.Predicate.Equals(rdfObject) &&
                     nodeChild.Object.Equals(source.Object)) ||
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

                return new[] { result[0]! };
            }

            reificationTriples.Clear();
            result[0] = null;
        }

        return Array.Empty<Triple>();
    }

    /**
     * Generates a prefix for unrecognized namespaces when reading in unknown
     * properties and content.
     *
     * @param graph
     * the graph
     * @param ns
     * the unrecognized namespace Uri that needs a prefix
     * @return the generated prefix (e.g., 'j.0')
     */
    private static string GeneratePrefix(IGraph graph, string ns)
    {
        var map = graph.NamespaceMap;
        var i = 0;
        string candidatePrefix;
        do
        {
            candidatePrefix = GENERATED_PREFIX_START + i;
            ++i;
        } while (map.HasNamespace(candidatePrefix) == true);

        map.AddNamespace(candidatePrefix, new Uri(ns));
        return candidatePrefix;
    }

    private object HandleExtendedPropertyValue(Type beanType,
        INode obj,
        IGraph graph,
        IDictionary<string, object> visitedResources)
    {
        // TODO: use modern C#
        if (obj is ILiteralNode node)
        {
            if (node is BooleanNode booleanNode)
            {
                return booleanNode.AsBoolean();
            }

            if (node is ByteNode byteNode)
            {
                return byte.Parse(byteNode.Value);
            }

            if (node is DateTimeNode timeNode)
            {
                return timeNode.AsDateTime();
            }

            if (node is DecimalNode decimalNode)
            {
                return decimalNode.AsDecimal();
            }

            if (node is DoubleNode doubleNode)
            {
                return doubleNode.AsDouble();
            }

            if (node is FloatNode floatNode)
            {
                return floatNode.AsFloat();
            }

            if (node is DecimalNode node1)
            {
                return node1.AsDecimal();
            }

            if (node is LongNode longNode)
            {
                return longNode.AsInteger();
            }

            if (node is SignedByteNode signedByteNode)
            {
                return (byte)signedByteNode.AsInteger();
            }

            if (node is StringNode stringNode)
            {
                return stringNode.AsString();
            }

            if (node is UnsignedLongNode unsignedLongNode)
            {
                return unsignedLongNode.AsInteger();
            }

            return node.Value;
        }

        var nestedResource = obj as IUriNode;

        // REVISIT: Is this an inline resource? AND we have not visited it yet?
        var visitedName = GetVisitedResourceName(obj);
        if ((obj is IBlankNode || graph.GetTriplesWithSubject(nestedResource).Any()) &&
            visitedName != null && !visitedResources.ContainsKey(visitedName))
        {
            AbstractResource any = new AnyResource();
            var typePropertyDefinitionsToSetMethods =
                new Dictionary<Type, IDictionary<string, MemberInfo>>();
            FromDotNetRdfNode(typePropertyDefinitionsToSetMethods,
                typeof(AnyResource),
                any,
                obj,
                graph,
                visitedResources);

            return any;
        }

        if ((obj is IBlankNode || graph.GetTriplesWithSubject(nestedResource).Any()) && visitedName != null)
        {
            return visitedResources[visitedName];
        }

        // It's a resource reference.
        var nestedResourceUri = nestedResource?.Uri;
        if (nestedResourceUri?.IsAbsoluteUri == false)
        {
            throw new OslcCoreRelativeURIException(beanType, "<none>",
                nestedResourceUri);
        }

        return nestedResourceUri!;
    }

    private static IDictionary<string, MemberInfo> CreatePropertyDefinitionToSetMethods(
        Type beanType)
    {
        var result = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);

        var methods = beanType.GetMethods();

        foreach (var method in methods)
        {
            if (method.GetParameters().Length != 0)
            {
                continue;
            }

            var getMethodName = method.Name;

            if ((!getMethodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal) ||
                 getMethodName.Length <= METHOD_NAME_START_GET_LENGTH) &&
                (!getMethodName.StartsWith(METHOD_NAME_START_IS, StringComparison.Ordinal) ||
                 getMethodName.Length <= METHOD_NAME_START_IS_LENGTH))
            {
                continue;
            }

            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper
                .GetAttribute<OslcPropertyDefinition>(method);

            if (oslcPropertyDefinitionAnnotation == null)
            {
                continue;
            }

            // We need to find the set companion setMethod
            string setMethodName;
            if (getMethodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal))
            {
                setMethodName = string.Concat(METHOD_NAME_START_SET, getMethodName.AsSpan(METHOD_NAME_START_GET_LENGTH));
            }
            else
            {
                setMethodName = string.Concat(METHOD_NAME_START_SET, getMethodName.AsSpan(METHOD_NAME_START_IS_LENGTH));
            }

            var getMethodReturnType = method.ReturnType;

            var setMethod = beanType.GetMethod(setMethodName,
                new[] { getMethodReturnType });

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

        foreach (var propertyInfo in beanType.GetProperties())
        {
            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper
                .GetAttribute<OslcPropertyDefinition>(propertyInfo);

            if (oslcPropertyDefinitionAnnotation == null)
            {
                continue;
            }

            result.Add(oslcPropertyDefinitionAnnotation.value, propertyInfo);
        }

        return result;
    }

    private static void BuildResource(object obj,
        Type resourceType,
        IGraph graph,
        INode mainResource,
        IDictionary<string, object>? oslcProperties)
    {
        if (oslcProperties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
        {
            return;
        }

        foreach (var method in resourceType.GetMethods())
        {
            if (method.GetParameters().Length != 0)
            {
                continue;
            }

            var methodName = method.Name;

            if ((!methodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal) ||
                 methodName.Length <= METHOD_NAME_START_GET_LENGTH) &&
                (!methodName.StartsWith(METHOD_NAME_START_IS, StringComparison.Ordinal) ||
                 methodName.Length <= METHOD_NAME_START_IS_LENGTH))
            {
                continue;
            }

            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper
                .GetAttribute<OslcPropertyDefinition>(method);

            if (oslcPropertyDefinitionAnnotation == null)
            {
                continue;
            }

            var value = method.Invoke(obj, null);

            if (value == null)
            {
                continue;
            }

            IDictionary<string, object>? nestedProperties = null;
            var onlyNested = false;

            if (oslcProperties != null)
            {
                if (oslcProperties.TryGetValue(oslcPropertyDefinitionAnnotation.value, out var mapObj) && mapObj is IDictionary<string, object> map)
                {
                    nestedProperties = map;
                }
                else if (oslcProperties is SingletonWildcardProperties &&
                         !(oslcProperties is NestedWildcardProperties))
                {
                    nestedProperties =
                        OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                }
                else if (oslcProperties is NestedWildcardProperties properties)
                {
                    nestedProperties = properties
                        .CommonNestedProperties();
                    onlyNested = properties is not SingletonWildcardProperties;
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

        foreach (var property in resourceType.GetProperties())
        {
            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper
                .GetAttribute<OslcPropertyDefinition>(property);

            if (oslcPropertyDefinitionAnnotation == null)
            {
                continue;
            }

            var value = property.GetValue(obj);

            if (value == null)
            {
                continue;
            }

            IDictionary<string, object>? nestedProperties = null;
            var onlyNested = false;

            if (oslcProperties != null)
            {
                if (oslcProperties.TryGetValue(oslcPropertyDefinitionAnnotation.value, out var mapObj) && mapObj is IDictionary<string, object> map)
                {
                    nestedProperties = map;
                }
                else if (oslcProperties is SingletonWildcardProperties &&
                         !(oslcProperties is NestedWildcardProperties))
                {
                    nestedProperties =
                        OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                }
                else if (oslcProperties is NestedWildcardProperties properties)
                {
                    nestedProperties = properties
                        .CommonNestedProperties();
                    onlyNested = properties is not SingletonWildcardProperties;
                }
                else
                {
                    continue;
                }
            }

            BuildAttributeResource(resourceType,
                property,
                oslcPropertyDefinitionAnnotation,
                graph,
                mainResource,
                value,
                nestedProperties,
                onlyNested);
        }

        // Handle any extended properties.
        if (obj is IExtendedResource extendedResource)
        {
            HandleExtendedProperties(resourceType,
                graph,
                mainResource,
                extendedResource,
                oslcProperties);
        }
    }

    private static void HandleExtendedProperties(Type resourceType,
        IGraph graph,
        INode mainResource,
        IExtendedResource extendedResource,
        IDictionary<string, object>? properties)
    {
        foreach (var type in extendedResource.GetTypes())
        {
            var propertyName = type.ToString();

            if (properties != null &&
                !properties.ContainsKey(propertyName) &&
                !(properties is NestedWildcardProperties) &&
                !(properties is SingletonWildcardProperties))
            {
                continue;
            }

            var typeResource = graph.CreateUriNode(new Uri(propertyName));
            var rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            if (!graph.GetTriplesWithPredicateObject(rdfType, typeResource).Any())
            {
                graph.Assert(new Triple(mainResource, rdfType, typeResource));
            }
        }

        var extendedProperties = extendedResource.GetExtendedProperties();

        foreach (var qName in extendedProperties.Keys)
        {
            var propertyName = qName.NamespaceUri + qName.LocalPart;
            IDictionary<string, object>? nestedProperties = null;
            var onlyNested = false;

            if (properties != null)
            {
                if (properties.TryGetValue(propertyName, out var mapObj) && mapObj is IDictionary<string, object> map)
                {
                    nestedProperties = map;
                }
                else if (properties is SingletonWildcardProperties &&
                         !(properties is NestedWildcardProperties))
                {
                    nestedProperties = OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                }
                else if (properties is NestedWildcardProperties wildcardProperties)
                {
                    nestedProperties =
                        wildcardProperties.CommonNestedProperties();
                    onlyNested = wildcardProperties is not SingletonWildcardProperties;
                }
                else
                {
                    continue;
                }
            }

            var property = graph.CreateUriNode(new Uri(propertyName));
            var value = extendedProperties[qName];

            if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>),
                    value.GetType()))
            {
                IEnumerable<object> collection = new EnumerableWrapper(value);
                foreach (var next in collection)
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
        IDictionary<string, object>? nestedProperties,
        bool onlyNested)
    {
        if (value is AnyResource)
        {
            var any = (AbstractResource)value;
            INode nestedResource;
            var aboutURI = any.About;
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

            foreach (var type in any.GetTypes())
            {
                var propertyName = type.ToString();

                if (nestedProperties == null ||
                    nestedProperties.ContainsKey(propertyName) ||
                    nestedProperties is NestedWildcardProperties ||
                    nestedProperties is SingletonWildcardProperties)
                {
                    graph.Assert(new Triple(nestedResource,
                        graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                        graph.CreateUriNode(new Uri(propertyName))));
                }
            }

            HandleExtendedProperties(typeof(AnyResource), graph, nestedResource, any,
                nestedProperties);
            graph.Assert(new Triple(resource, property, nestedResource));
        }
        else if (value.GetType().GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0 ||
                 value is Uri)
        {
            //TODO:  Until we handle XMLLiteral for incoming unknown resources, need to assume it is not XMLLiteral
            var xmlliteral = false;
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
            var valueType = value.GetType();

            if (typeof(string) == valueType)
            {
                literal = ((string)value).ToLiteral(graph);
            }
            else if (typeof(bool) == valueType)
            {
                literal = ((bool)value).ToLiteral(graph);
            }
            else if (typeof(byte) == valueType)
            {
                literal = ((byte)value).ToLiteral(graph);
            }
            else if (typeof(short) == valueType)
            {
                literal = ((short)value).ToLiteral(graph);
            }
            else if (typeof(int) == valueType)
            {
                literal = ((int)value).ToLiteral(graph);
            }
            else if (typeof(long) == valueType)
            {
                literal = ((long)value).ToLiteral(graph);
            }
            else if (typeof(BigInteger) == valueType)
            {
                literal = ((long)(BigInteger)value).ToLiteral(graph);
            }
            else if (typeof(float) == valueType)
            {
                literal = ((float)value).ToLiteral(graph);
            }
            else if (typeof(decimal) == valueType)
            {
                literal = ((decimal)value).ToLiteral(graph);
            }
            else if (typeof(double) == valueType)
            {
                literal = ((double)value).ToLiteral(graph);
            }
            else if (typeof(DateTime) == valueType)
            {
                literal = ((DateTime)value).ToUniversalTime().ToLiteral(graph);
            }
            else if (typeof(XElement) == valueType)
            {
                literal = graph.CreateLiteralNode(((XElement)value).ToString(SaveOptions.None),
                    new Uri(RdfSpecsHelper.RdfXmlLiteral));
            }
            else
            {
                throw new InvalidOperationException("Unkown type: " + valueType);
            }

            graph.Assert(new Triple(resource, property, literal));
        }
    }

    private static void BuildAttributeResource(Type resourceType,
        MemberInfo method,
        OslcPropertyDefinition propertyDefinitionAnnotation,
        IGraph graph,
        INode resource,
        object value,
        IDictionary<string, object>? nestedProperties,
        bool onlyNested)
    {
        var propertyDefinition = propertyDefinitionAnnotation.value;

        var nameAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcName>(method);

        string name;
        if (nameAnnotation != null)
        {
            name = nameAnnotation.value;
        }
        else
        {
            name = GetDefaultPropertyName(method);
        }

        if (!propertyDefinition.EndsWith(name, StringComparison.Ordinal))
        {
            throw new OslcCoreInvalidPropertyDefinitionException(resourceType,
                method,
                propertyDefinitionAnnotation);
        }

        var valueTypeAnnotation =
            InheritedMethodAttributeHelper.GetAttribute<OslcValueType>(method);

        var xmlLiteral = valueTypeAnnotation is { value: ValueType.XMLLiteral };

        var attribute = graph.CreateUriNode(new Uri(propertyDefinition));

        var valueType = method switch
        {
            MethodInfo methodInfo => methodInfo.ReturnType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };

        var collectionType =
            InheritedMethodAttributeHelper.GetAttribute<OslcRdfCollectionType>(method);
        List<INode>? rdfNodeContainer;

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

        if (valueType.IsArray)
        {
            // We cannot cast to object[] in case this is an array of
            // primitives. We will use Array reflection instead.
            // Strange case about primitive arrays: they cannot be cast to
            // object[], but retrieving their individual elements
            // via Array.get does not return primitives, but the primitive
            // object wrapping counterparts like Integer, Byte, Double, etc.

            var length =
                (int)value.GetType().GetProperty("Length")!.GetValue(value, null)!;
            var getValue = value.GetType().GetMethod("GetValue", new[] { typeof(int) })!;

            for (var index = 0;
                 index < length;
                 index++)
            {
                var obj = getValue.Invoke(value, new object[] { index })!;

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
                var container = CreateRdfContainer(collectionType,
                    rdfNodeContainer,
                    graph);
                graph.Assert(new Triple(resource, attribute, container));
            }
        }
        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>),
                     valueType))
        {
            IEnumerable<object> collection = new EnumerableWrapper(value);

            foreach (var obj in collection)
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
                var container = CreateRdfContainer(collectionType,
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
            var current = root;

            for (var i = 0; i < rdfNodeContainer.Count - 1; i++)
            {
                var node = rdfNodeContainer[i];

                graph.Assert(new Triple(current,
                    graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst)), node));

                INode next = graph.CreateBlankNode();

                graph.Assert(new Triple(current,
                    graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest)), next));

                current = next;
            }

            if (rdfNodeContainer.Count > 0)
            {
                graph.Assert(new Triple(current,
                    graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst)),
                    rdfNodeContainer[rdfNodeContainer.Count - 1]));
            }

            graph.Assert(new Triple(current,
                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest)),
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

        foreach (var node in rdfNodeContainer)
        {
            graph.Assert(new Triple(container,
                graph.CreateUriNode(OslcConstants.RDF_NAMESPACE + "li"),
                node));
        }

        return container;
    }

    private static void HandleLocalResource(Type resourceType,
        MemberInfo? method,
        bool xmlLiteral,
        object obj,
        IGraph graph,
        INode resource,
        IUriNode attribute,
        IDictionary<string, object>? nestedProperties,
        bool onlyNested,
        List<INode>? rdfNodeContainer)
    {
        var objType = obj.GetType();

        INode? nestedNode = null;
        var isReifiedResource =
            InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>),
                obj.GetType());
        var value = !isReifiedResource
            ? obj
            : obj.GetType().GetMethod("GetValue", Type.EmptyTypes)?.Invoke(obj, null);

        if (value is string)
        {
            if (onlyNested)
            {
                return;
            }

            if (xmlLiteral)
            {
                nestedNode = graph.CreateLiteralNode(value.ToString(),
                    new Uri(RdfSpecsHelper.RdfXmlLiteral));
            }
            else
            {
                nestedNode = graph.CreateLiteralNode(value.ToString());
            }
        }
        else if (value is bool ||
                 value is byte ||
                 value is short ||
                 value is int ||
                 value is long ||
                 value is BigInteger ||
                 value is float ||
                 value is decimal ||
                 value is double)
        {
            if (onlyNested)
            {
                return;
            }

            _ = value.GetType();

            if (value is bool b)
            {
                nestedNode = b.ToLiteral(graph);
            }
            else if (value is byte b1)
            {
                nestedNode = b1.ToLiteral(graph);
            }
            else if (value is short s)
            {
                nestedNode = s.ToLiteral(graph);
            }
            else if (value is int i)
            {
                nestedNode = i.ToLiteral(graph);
            }
            else if (value is long l)
            {
                nestedNode = l.ToLiteral(graph);
            }
            else if (value is BigInteger bigint)
            {
                nestedNode = ((long)bigint).ToLiteral(graph);
            }
            else if (value is float f)
            {
                nestedNode = f.ToLiteral(graph);
            }
            else if (value is decimal value1)
            {
                nestedNode = value1.ToLiteral(graph);
            }
            else if (value is double d)
            {
                nestedNode = d.ToLiteral(graph);
            }
        }
        else if (value is Uri uri)
        {
            if (onlyNested)
            {
                return;
            }

            if (!uri.IsAbsoluteUri)
            {
                throw new OslcCoreRelativeURIException(resourceType,
                    method == null ? "<none>" : method.Name,
                    uri);
            }

            // URIs represent references to other resources identified by their IDs, so they need to be managed as such
            nestedNode = graph.CreateUriNode(uri);
        }
        else if (value is DateTime time)
        {
            if (onlyNested)
            {
                return;
            }

            nestedNode = time.ToUniversalTime().ToLiteral(graph);
        }
        else if (objType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
        {
            var ns = TypeFactory.GetNamespace(objType);
            var name = TypeFactory.GetName(objType);

            Uri? aboutUri = null;
            if (value is IResource iResource)
            {
                aboutUri = iResource.About;
            }

            INode nestedResource;
            if (aboutUri != null)
            {
                if (!aboutUri.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(objType,
                        "getAbout",
                        aboutUri);
                }

                nestedResource = graph.CreateUriNode(aboutUri);
            }
            else
            {
                nestedResource = graph.CreateBlankNode();
            }

            graph.Assert(new Triple(nestedResource,
                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
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
                        method!,
                        null);
                }

                // Instead of adding a nested node to model, add it to a list
                rdfNodeContainer.Add(nestedNode);
            }
            else
            {
                var triple = new Triple(resource, attribute, nestedNode);

                if (isReifiedResource &&
                    nestedProperties != OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
                {
                    AddReifiedStatements(graph, triple, obj, nestedProperties!);
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
        _ = reifiedResource.GetType();
        INode uriref = graph.CreateBlankNode();

        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject)),
            triple.Subject));
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfPredicate)),
            triple.Predicate));
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject)),
            triple.Object));
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
            graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfStatement))));

        BuildResource(reifiedResource,
            reifiedResource.GetType(),
            graph,
            uriref,
            nestedProperties);
    }

    private static string GetDefaultPropertyName(MemberInfo method)
    {
        var methodName = method.Name;
        var startingIndex = methodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal)
            ? METHOD_NAME_START_GET_LENGTH
            : METHOD_NAME_START_IS_LENGTH;
        var endingIndex = startingIndex + 1;

        // We want the name to start with a lower-case letter
        var lowercasedFirstCharacter = methodName.Substring(startingIndex,
            1).ToLower(CultureInfo.GetCultureInfo("en"));

        if (methodName.Length == endingIndex)
        {
            return lowercasedFirstCharacter;
        }

        return string.Concat(lowercasedFirstCharacter, methodName.AsSpan(endingIndex));
    }

    private static void RecursivelyCollectNamespaceMappings(INamespaceMapper namespaceMappings,
        Type resourceType)
    {
        var oslcSchemaAttribute =
            (OslcSchema[])resourceType.Assembly.GetCustomAttributes(typeof(OslcSchema), false);

        if (oslcSchemaAttribute.Length > 0)
        {
            var oslcNamespaceDefinitionAnnotations =
                (OslcNamespaceDefinition[])oslcSchemaAttribute[0].namespaceType
                    .GetMethod("GetNamespaces", Type.EmptyTypes)!.Invoke(null, null)!;

            foreach (var oslcNamespaceDefinitionAnnotation in oslcNamespaceDefinitionAnnotations)
            {
                var prefix = oslcNamespaceDefinitionAnnotation.prefix;
                var namespaceURI = oslcNamespaceDefinitionAnnotation.namespaceURI;

                namespaceMappings.AddNamespace(prefix,
                    new Uri(namespaceURI));
            }
        }

        var baseType = resourceType.BaseType;

        if (baseType != null)
        {
            RecursivelyCollectNamespaceMappings(namespaceMappings,
                baseType);
        }

        var interfaces = resourceType.GetInterfaces();

        if (interfaces != null)
        {
            foreach (var interfac in interfaces)
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
        var uri = new Uri(ns);

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
                var index = 1;

                while (true)
                {
                    var newPrefix = prefix + index;

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

    // REVISIT: consider throwing an exception instead of returning null (@berezovskyi 2025-04)
    private static string? GetVisitedResourceName(INode? resource)
    {
        string? visitedResourceName = null;
        if (resource is IUriNode)
        {
            visitedResourceName = (resource as IUriNode)?.Uri.ToString();
        }
        else if (resource is IBlankNode)
        {
            visitedResourceName = (resource as IBlankNode)?.InternalID;
        }

        return visitedResourceName;
    }
}
