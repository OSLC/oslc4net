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

    private readonly ILogger<DotNetRdfHelper> _logger = logger;
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

    public static IGraph CreateDotNetRdfGraph(IEnumerable<object> objects) =>
        CreateDotNetRdfGraph(null, null, null, null, objects, null);

    public static IGraph CreateDotNetRdfGraph(string descriptionAbout,
        string responseInfoAbout,
        string nextPageAbout,
        long? totalCount,
        IEnumerable<object> objects,
        IDictionary<string, object> properties)
    {
        IGraph graph = new Graph();
        var namespaceMappings = graph.NamespaceMap;

        IUriNode? descriptionResource = null;

        if (descriptionAbout != null)
        {
            descriptionResource = graph.CreateUriNode(new Uri(descriptionAbout));

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

            var countValue = totalCount ?? objects.Count();
            graph.Assert(new Triple(responseInfoResource,
                graph.CreateUriNode(
                    new Uri(OslcConstants.OSLC_CORE_NAMESPACE + PROPERTY_TOTAL_COUNT)),
                graph.CreateLiteralNode(countValue.ToString(CultureInfo.InvariantCulture))));

            if (nextPageAbout != null)
            {
                graph.Assert(new Triple(responseInfoResource,
                    graph.CreateUriNode(new Uri(OslcConstants.OSLC_CORE_NAMESPACE +
                                                PROPERTY_NEXT_PAGE)),
                    graph.CreateUriNode(new Uri(nextPageAbout))));
            }
        }

        foreach (var obj in objects)
        {
            HandleSingleResource(descriptionResource, obj, graph, namespaceMappings, properties);
        }

        if (descriptionAbout != null)
        {
            EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX, OslcConstants.RDF_NAMESPACE, namespaceMappings);
            EnsureNamespacePrefix(OslcConstants.RDFS_NAMESPACE_PREFIX, OslcConstants.RDFS_NAMESPACE, namespaceMappings);

            if (responseInfoAbout != null)
            {
                EnsureNamespacePrefix(OslcConstants.OSLC_CORE_NAMESPACE_PREFIX, OslcConstants.OSLC_CORE_NAMESPACE, namespaceMappings);
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
        var objType = obj.GetType();
        RecursivelyCollectNamespaceMappings(namespaceMappings, objType);

        Uri? aboutURI = (obj as IResource)?.GetAbout();
        INode mainResource;

        if (aboutURI != null)
        {
            if (!aboutURI.IsAbsoluteUri)
            {
                throw new OslcCoreRelativeURIException(objType, "GetAbout", aboutURI);
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

        BuildResource(obj, objType, graph, mainResource, properties);

        if (descriptionResource != null)
        {
            graph.Assert(new Triple(descriptionResource,
                graph.CreateUriNode(new Uri(OslcConstants.RDFS_NAMESPACE + "member")),
                mainResource));
        }
    }

    public object FromDotNetRdfNode(IUriNode resource, IGraph? graph, Type beanType)
    {
        var newInstance = Activator.CreateInstance(beanType);
        var typePropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MemberInfo>>();
        var visitedResources = new Dictionary<string, object>();
        FromDotNetRdfNode(typePropertyDefinitionsToSetMethods, beanType, newInstance, resource, graph, visitedResources);
        return newInstance;
    }

    public object FromDotNetRdfGraph(IGraph graph, Type beanType)
    {
        var results = Activator.CreateInstance(typeof(List<>).MakeGenericType(beanType));
        if (beanType.GetCustomAttributes(typeof(OslcResourceShape), true).Length > 0)
        {
            var qualifiedName = TypeFactory.GetQualifiedName(beanType);
            var triples = graph.GetTriplesWithPredicateObject(
                graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                graph.CreateUriNode(new Uri(qualifiedName))).ToList();

            if (triples.Any())
            {
                var addMethod = results.GetType().GetMethod("Add", [beanType]);
                var typePropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MemberInfo>>();

                foreach (var triple in triples)
                {
                    var resourceSubject = triple.Subject;
                    var newInstance = Activator.CreateInstance(beanType);
                    var visitedResources = new Dictionary<string, object>();
                    FromDotNetRdfNode(typePropertyDefinitionsToSetMethods, beanType, newInstance, resourceSubject, graph, visitedResources);
                    addMethod?.Invoke(results, [newInstance]);
                }
            }
        }
        return results;
    }

    private void FromDotNetRdfNode(
        IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
        Type beanType, object bean, INode resource, IGraph graph, IDictionary<string, object> visitedResources)
    {
        var (setMethodMap, extendedResource, extendedProperties) =
            InitializeBeanForRdfParsing(typePropertyDefinitionsToSetMethods, beanType, bean, resource, visitedResources);

        var propertyDefinitionsToArrayValues = new Dictionary<string, List<object>>();
        var singleValueMethodsUsed = new HashSet<MemberInfo>();

        foreach (var triple in graph.GetTriplesWithSubject(resource))
        {
            ProcessTripleForBean(typePropertyDefinitionsToSetMethods, beanType, bean, resource, graph, visitedResources,
                setMethodMap, triple, propertyDefinitionsToArrayValues, singleValueMethodsUsed,
                extendedResource, extendedProperties);
        }
        AssignCollectionPropertiesToBean(bean, setMethodMap, propertyDefinitionsToArrayValues);
    }

    private (IDictionary<string, MemberInfo> setMethodMap, IExtendedResource? extendedResource, IDictionary<QName, object>? extendedProperties)
        InitializeBeanForRdfParsing(
            IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
            Type beanType, object bean, INode resource, IDictionary<string, object> visitedResources)
    {
        IDictionary<string, MemberInfo> setMethodMap;
        if (!typePropertyDefinitionsToSetMethods.TryGetValue(beanType, out var existingMap))
        {
            existingMap = CreatePropertyDefinitionToSetMethods(beanType);
            typePropertyDefinitionsToSetMethods.Add(beanType, existingMap);
        }
        setMethodMap = existingMap;

        var resourceName = GetVisitedResourceName(resource);
        if(resourceName != null) visitedResources[resourceName] = bean;

        if (bean is IResource iResource && resource is IUriNode uriNode && uriNode.Uri != null)
        {
            if (!uriNode.Uri.IsAbsoluteUri)
            {
                throw new OslcCoreRelativeURIException(beanType, nameof(IResource.SetAbout), uriNode.Uri);
            }
            iResource.SetAbout(uriNode.Uri);
        }

        IExtendedResource? extendedResource = null;
        IDictionary<QName, object>? extendedProperties = null;
        if (bean is IExtendedResource er)
        {
            extendedResource = er;
            extendedProperties = new Dictionary<QName, object>();
            er.SetExtendedProperties(extendedProperties);
        }
        return (setMethodMap, extendedResource, extendedProperties);
    }

    private void ProcessTripleForBean(
        IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
        Type beanType, object bean, INode resourceContext, IGraph graph, IDictionary<string, object> visitedResources,
        IDictionary<string, MemberInfo> setMethodMap, Triple triple,
        IDictionary<string, List<object>> propertyDefinitionsToArrayValues, HashSet<MemberInfo> singleValueMethodsUsed,
        IExtendedResource? extendedResource, IDictionary<QName, object>? extendedProperties)
    {
        var predicate = (IUriNode)triple.Predicate;
        var rdfObjectNode = triple.Object;
        var predicateUri = predicate.Uri.ToString();

        if (!setMethodMap.TryGetValue(predicateUri, out var backingMember))
        {
            HandleUnmappedPredicate(beanType, predicate, rdfObjectNode, graph, visitedResources, extendedResource, extendedProperties, _logger);
            return;
        }

        var (baseParameterType, isMultiple) = GetBackingMemberPrimitiveType(backingMember);
        var actualRdfNodes = ResolveRdfObjectNodes(rdfObjectNode, graph, isMultiple, visitedResources);
        var componentParameterType = baseParameterType;
        Type? reifiedType = null;

        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), componentParameterType))
        {
            reifiedType = componentParameterType;
            while (componentParameterType.BaseType != null && componentParameterType.BaseType != typeof(object) &&
                   (!componentParameterType.IsGenericType || componentParameterType.GetGenericTypeDefinition() != typeof(IReifiedResource<>)))
            {
                componentParameterType = componentParameterType.BaseType;
            }
            if (componentParameterType.IsGenericType) componentParameterType = componentParameterType.GetGenericArguments()[0];
        }

        foreach (var currentRdfNode in actualRdfNodes)
        {
            var parameterValue = ConvertRdfNodeToObject(currentRdfNode, componentParameterType, beanType, backingMember.Name, graph,
                typePropertyDefinitionsToSetMethods, visitedResources, this);
            if (parameterValue == null) continue;

            if (reifiedType != null)
            {
                parameterValue = HandleReifiedProperty(typePropertyDefinitionsToSetMethods, reifiedType, parameterValue, componentParameterType,
                    triple, graph, visitedResources, this);
            }

            if (isMultiple)
            {
                if (!propertyDefinitionsToArrayValues.TryGetValue(predicateUri, out var values))
                {
                    values = [];
                    propertyDefinitionsToArrayValues[predicateUri] = values;
                }
                values.Add(parameterValue);
            }
            else
            {
                if (singleValueMethodsUsed.Contains(backingMember)) throw new OslcCoreMisusedOccursException(beanType, backingMember);
                SetValue(bean, backingMember, parameterValue);
                singleValueMethodsUsed.Add(backingMember);
            }
        }
    }

    private static void HandleUnmappedPredicate(Type beanType, IUriNode predicate, INode rdfObjectNode, IGraph graph,
        IDictionary<string, object> visitedResources, IExtendedResource? extendedResource,
        IDictionary<QName, object>? extendedProperties, ILogger logger)
    {
        var predicateUri = predicate.Uri.ToString();
        if (RdfSpecsHelper.RdfType.Equals(predicateUri))
        {
            if (extendedResource != null && rdfObjectNode is IUriNode typeUriNode) extendedResource.AddType(typeUriNode.Uri);
            return;
        }
        if (extendedProperties == null)
        {
            logger.LogWarning($"Set method not found for object type: {beanType.Name}, uri: {predicateUri}");
            return;
        }

        string? prefix;
        string localPart;
        string ns;
        if (graph.NamespaceMap.ReduceToQName(predicateUri, out var qname))
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
            prefix = graph.NamespaceMap.GetPrefix(new Uri(ns)) ?? GeneratePrefix(graph, ns);
        }
        var key = new QName(ns, localPart, prefix);
        var value = new DotNetRdfHelper(logger).HandleExtendedPropertyValue(beanType, rdfObjectNode, graph, visitedResources);

        if (!extendedProperties.TryGetValue(key, out var existingValue)) extendedProperties[key] = value;
        else if (existingValue is List<object> list) list.Add(value);
        else extendedProperties[key] = new List<object> { existingValue, value };
    }

    private static IList<INode> ResolveRdfObjectNodes(INode rdfObjectNode, IGraph graph, bool isMultiple, IDictionary<string, object> visitedResources)
    {
        var nil = new Uri(OslcConstants.RDF_NAMESPACE + RDF_NIL);
        if (isMultiple && rdfObjectNode is IUriNode uriNode)
        {
            bool isRdfList = uriNode.IsListRoot(graph) || nil.Equals(uriNode.Uri) ||
                             (graph.GetTriplesWithSubjectPredicate(uriNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst))).Any() &&
                              graph.GetTriplesWithSubjectPredicate(uriNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest))).Any());

            if (isRdfList)
            {
                var listNodes = new List<INode>();
                var currentListNode = uriNode;
                while (currentListNode != null && !nil.Equals(currentListNode.Uri))
                {
                    var resourceName = GetVisitedResourceName(currentListNode);
                    if(resourceName != null) visitedResources[resourceName] = new object();

                    var firstTriple = graph.GetTriplesWithSubjectPredicate(currentListNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst))).FirstOrDefault();
                    if (firstTriple?.Object != null) listNodes.Add(firstTriple.Object);

                    var restTriple = graph.GetTriplesWithSubjectPredicate(currentListNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest))).FirstOrDefault();
                    currentListNode = restTriple?.Object as IUriNode;
                }
                var topListName = GetVisitedResourceName(uriNode);
                if(topListName != null) visitedResources[topListName] = listNodes;
                return listNodes;
            }
            if (IsRdfCollectionResource(graph, uriNode))
            {
                var collectionNodes = new List<INode>();
                foreach (var liTriple in graph.GetTriplesWithSubjectPredicate(uriNode, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + "li"))))
                {
                    var liResourceName = GetVisitedResourceName(liTriple.Object);
                    if(liResourceName != null && liTriple.Object is IUriNode) visitedResources[liResourceName] = new object();
                    collectionNodes.Add(liTriple.Object);
                }
                var collectionName = GetVisitedResourceName(uriNode);
                if(collectionName != null) visitedResources[collectionName] = collectionNodes;
                return collectionNodes;
            }
        }
        return new ReadOnlyCollection<INode>([rdfObjectNode]);
    }

    private object? ConvertRdfNodeToObject(INode rdfNode, Type targetType, Type beanType, string memberName, IGraph graph,
        IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
        IDictionary<string, object> visitedResources, DotNetRdfHelper helperInstance)
    {
        if (rdfNode is ILiteralNode literalNode) return ConvertLiteralNode(literalNode, targetType);
        if (rdfNode is IUriNode uriNode)
        {
            if (targetType == typeof(Uri))
            {
                if (uriNode.Uri == null || !uriNode.Uri.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(beanType, nameof(memberName), uriNode.Uri);
                }
                return uriNode.Uri;
            }
            return ConvertUriOrBlankNodeToNestedBean(uriNode, targetType, graph, typePropertyDefinitionsToSetMethods, visitedResources, helperInstance);
        }
        if (rdfNode is IBlankNode blankNode)
        {
            return ConvertUriOrBlankNodeToNestedBean(blankNode, targetType, graph, typePropertyDefinitionsToSetMethods, visitedResources, helperInstance);
        }
        return null;
    }

    private object? ConvertLiteralNode(ILiteralNode literalNode, Type targetType)
    {
        string stringValue = literalNode.Value;
        if (targetType == typeof(string)) return stringValue;
        if (targetType == typeof(bool) || targetType == typeof(bool?))
        {
            if ("true".Equals(stringValue, StringComparison.Ordinal) || "1".Equals(stringValue)) return true;
            if ("false".Equals(stringValue, StringComparison.Ordinal) || "0".Equals(stringValue)) return false;
            throw new ArgumentException($"'{stringValue}' has wrong format for Boolean.", nameof(stringValue));
        }
        if (targetType == typeof(byte) || targetType == typeof(byte?)) return byte.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(short) || targetType == typeof(short?)) return short.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(int) || targetType == typeof(int?)) return int.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(long) || targetType == typeof(long?)) return long.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(BigInteger) || targetType == typeof(BigInteger?)) return BigInteger.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(float) || targetType == typeof(float?)) return float.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(decimal) || targetType == typeof(decimal?)) return decimal.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(double) || targetType == typeof(double?)) return double.Parse(stringValue, CultureInfo.InvariantCulture);
        if (targetType == typeof(DateTime) || targetType == typeof(DateTime?)) return DateTime.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        return null;
    }

    private object ConvertUriOrBlankNodeToNestedBean(INode rdfNode, Type targetType, IGraph graph,
        IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
        IDictionary<string, object> visitedResources, DotNetRdfHelper helperInstance)
    {
        object nestedBean = Activator.CreateInstance(MapToActivatableType(targetType));
        helperInstance.FromDotNetRdfNode(typePropertyDefinitionsToSetMethods, targetType, nestedBean, rdfNode, graph, visitedResources);
        return nestedBean;
    }

    private object HandleReifiedProperty(IDictionary<Type, IDictionary<string, MemberInfo>> typePropertyDefinitionsToSetMethods,
        Type reifiedType, object mainParameterValue, Type actualValueType,
        Triple originalTriple, IGraph graph, IDictionary<string, object> visitedResources, DotNetRdfHelper helperInstance)
    {
        var reifiedResourceInstance = Activator.CreateInstance(reifiedType);
        var setValueMethod = reifiedType.GetMethods().FirstOrDefault(m => m.Name == "SetValue" && m.GetParameters().Length == 1 &&
                                                                    m.GetParameters()[0].ParameterType.IsAssignableFrom(actualValueType));
        setValueMethod?.Invoke(reifiedResourceInstance, [mainParameterValue]);
        foreach (var reifiedTripleComponent in GetReifiedTriples(originalTriple, graph))
        {
             helperInstance.FromDotNetRdfNode(typePropertyDefinitionsToSetMethods, reifiedType, reifiedResourceInstance, reifiedTripleComponent.Subject, graph, visitedResources);
        }
        return reifiedResourceInstance;
    }

    private static void AssignCollectionPropertiesToBean(object bean, IDictionary<string, MemberInfo> setMethodMap,
        IDictionary<string, List<object>> propertyDefinitionsToArrayValues)
    {
        foreach (var (predicateUri, values) in propertyDefinitionsToArrayValues)
        {
            if (!setMethodMap.TryGetValue(predicateUri, out var settableMember)) continue;
            var parameterType = GetBackingMemberType(settableMember);
            if (parameterType.IsArray)
            {
                var elementType = parameterType.GetElementType()!;
                var array = Array.CreateInstance(elementType, values.Count);
                for (int i = 0; i < values.Count; i++) array.SetValue(values[i], i);
                SetValue(bean, settableMember, array);
            }
            else
            {
                var collection = Activator.CreateInstance(MapToActivatableType(parameterType));
                var addMethod = collection.GetType().GetMethod("Add", [values.FirstOrDefault()?.GetType() ?? typeof(object)]);
                if (addMethod != null) foreach (var value in values) addMethod.Invoke(collection, [value]);
                SetValue(bean, settableMember, collection);
            }
        }
    }

    private static void SetValue(object bean, MemberInfo backingMember, object parameter)
    {
        if (backingMember is MethodInfo methodInfo) methodInfo.Invoke(bean, [parameter]);
        else if (backingMember is PropertyInfo propertyInfo) propertyInfo.SetValue(bean, parameter);
        else throw new NotSupportedException($"Unsupported backing member type: {backingMember.GetType().Name}");
    }

    private static (Type componentType, bool isMultiple) GetBackingMemberPrimitiveType(MemberInfo backingMember) =>
        ExtractTypeInformation(GetBackingMemberType(backingMember));

    private static Type GetBackingMemberType(MemberInfo backingMember) => backingMember switch
    {
        MethodInfo methodInfo => methodInfo.GetParameters()[0].ParameterType,
        PropertyInfo propertyInfo => propertyInfo.PropertyType,
        _ => throw new ArgumentException("Backing member must be a method or property.", nameof(backingMember))
    };

    private static (Type componentType, bool isMultiple) ExtractTypeInformation(Type type)
    {
        bool isMultiple = type.IsArray;
        Type componentType = isMultiple ? type.GetElementType()! : type;

        if (!isMultiple && InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IEnumerable<>), type))
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                componentType = genericArguments[0];
                isMultiple = true;
            }
        }
        return (componentType, isMultiple);
    }

    private static Type MapToActivatableType(Type type)
    {
        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ISet<>), type))
            return typeof(HashSet<>).MakeGenericType(type.GetGenericArguments());
        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IEnumerable<>), type))
            return typeof(List<>).MakeGenericType(type.GetGenericArguments());
        return type;
    }

    private static bool IsRdfCollectionResource(IGraph graph, INode obj) =>
        obj is IUriNode resource &&
        (graph.GetTriplesWithSubjectPredicate(resource, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_ALT))).Any() ||
         graph.GetTriplesWithSubjectPredicate(resource, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_BAG))).Any() ||
         graph.GetTriplesWithSubjectPredicate(resource, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_SEQ))).Any());

    private static IEnumerable<Triple> GetReifiedTriples(Triple source, IGraph graph)
    {
        var rdfSubjectNode = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject));
        var reifiedSubjects = graph.GetTriplesWithPredicateObject(rdfSubjectNode, source.Subject).ToList();
        if (!reifiedSubjects.Any()) return Enumerable.Empty<Triple>();

        var rdfPredicateNode = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfPredicate));
        var rdfObjectNode = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject));
        var rdfTypeNode = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        var rdfStatementNode = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfStatement));

        foreach (var candidate in reifiedSubjects)
        {
            var reificationTriples = new List<Triple>(4);
            Triple? resultTriple = null;
            foreach (var nodeChild in graph.GetTriplesWithSubject(candidate.Subject))
            {
                if ((nodeChild.Predicate.Equals(rdfSubjectNode) && nodeChild.Object.Equals(source.Subject)) ||
                    (nodeChild.Predicate.Equals(rdfPredicateNode) && nodeChild.Object.Equals(source.Predicate)) ||
                    (nodeChild.Predicate.Equals(rdfObjectNode) && nodeChild.Object.Equals(source.Object)) ||
                    (nodeChild.Predicate.Equals(rdfTypeNode) && nodeChild.Object.Equals(rdfStatementNode)))
                {
                    reificationTriples.Add(nodeChild);
                }
                else resultTriple = nodeChild;
            }
            if (reificationTriples.Count == 4 && resultTriple != null)
            {
                graph.Retract(reificationTriples);
                return [resultTriple];
            }
        }
        return Enumerable.Empty<Triple>();
    }

    private static string GeneratePrefix(IGraph graph, string ns)
    {
        var map = graph.NamespaceMap;
        var i = 0;
        string candidatePrefix;
        do candidatePrefix = $"{GENERATED_PREFIX_START}{i++}";
        while (map.HasNamespace(candidatePrefix));
        map.AddNamespace(candidatePrefix, new Uri(ns));
        return candidatePrefix;
    }

    private object HandleExtendedPropertyValue(Type beanType, INode obj, IGraph graph, IDictionary<string, object> visitedResources)
    {
        if (obj is ILiteralNode literalNode)
        {
            return literalNode switch {
                BooleanNode bn => bn.AsBoolean(),
                ByteNode byteNode => byte.Parse(byteNode.Value, CultureInfo.InvariantCulture),
                DateTimeNode dtn => dtn.AsDateTime(),
                DecimalNode dn => dn.AsDecimal(),
                DoubleNode dbln => dbln.AsDouble(),
                FloatNode fn => fn.AsFloat(),
                LongNode ln => ln.AsInteger(),
                SignedByteNode sbn => (sbyte)sbn.AsInteger(),
                StringNode sn => sn.Value,
                UnsignedLongNode uln => uln.AsInteger(),
                _ => literalNode.Value
            };
        }

        var visitedResourceKey = GetVisitedResourceName(obj);
        bool isInlineResource = obj is IBlankNode || (obj is IUriNode uriForInlineCheck && graph.GetTriplesWithSubject(uriForInlineCheck).Any());

        if (isInlineResource && (visitedResourceKey == null || !visitedResources.ContainsKey(visitedResourceKey)))
        {
            AbstractResource any = new AnyResource();
            var typePropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MemberInfo>>();
            this.FromDotNetRdfNode(typePropertyDefinitionsToSetMethods, typeof(AnyResource), any, obj, graph, visitedResources);
            return any;
        }

        if (visitedResourceKey != null && visitedResources.TryGetValue(visitedResourceKey, out var visitedBean))
        {
            if (visitedBean.GetType() == typeof(object) && obj is IUriNode visitedUriNode)
            {
                 if (visitedUriNode.Uri?.IsAbsoluteUri == false)
                 {
                     throw new OslcCoreRelativeURIException(beanType, "<extended_property_uri>", visitedUriNode.Uri);
                 }
                 return visitedUriNode.Uri;
            }
            return visitedBean;
        }

        if (obj is IUriNode uriNode)
        {
            if (uriNode.Uri?.IsAbsoluteUri == false)
            {
                throw new OslcCoreRelativeURIException(beanType, "<extended_property_uri_ref>", uriNode.Uri);
            }
            return uriNode.Uri;
        }
        _logger.LogWarning($"Unhandled node type or state in HandleExtendedPropertyValue: {obj.GetType().Name}, Node: {obj}");
        return null;
    }

    private static IDictionary<string, MemberInfo> CreatePropertyDefinitionToSetMethods(Type beanType)
    {
        const string GetPrefix = "Get";
        const string IsPrefix = "Is";
        const string SetPrefix = "Set";
        var result = new Dictionary<string, MemberInfo>();
        foreach (var method in beanType.GetMethods())
        {
            if (method.GetParameters().Length != 0) continue;
            var methodName = method.Name;
            string propertyNameBase;
            if (methodName.StartsWith(GetPrefix) && methodName.Length > GetPrefix.Length) propertyNameBase = methodName.Substring(GetPrefix.Length);
            else if (methodName.StartsWith(IsPrefix) && methodName.Length > IsPrefix.Length) propertyNameBase = methodName.Substring(IsPrefix.Length);
            else continue;

            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);
            if (oslcPropertyDefinitionAnnotation == null) continue;

            var setMethodName = SetPrefix + propertyNameBase;
            var setMethod = beanType.GetMethod(setMethodName, [method.ReturnType]);
            if (setMethod != null) result.Add(oslcPropertyDefinitionAnnotation.value, setMethod);
            else throw new OslcCoreMissingSetMethodException(beanType, method);
        }
        foreach (var propertyInfo in beanType.GetProperties())
        {
            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(propertyInfo);
            if (oslcPropertyDefinitionAnnotation == null || !propertyInfo.CanWrite) continue;
            result.Add(oslcPropertyDefinitionAnnotation.value, propertyInfo);
        }
        return result;
    }

    private static void BuildResource(object obj, Type resourceType, IGraph graph, INode mainResource, IDictionary<string, object> oslcProperties)
    {
        if (oslcProperties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON) return;
        ProcessMethodsForResource(obj, resourceType, graph, mainResource, oslcProperties);
        ProcessPropertiesForResource(obj, resourceType, graph, mainResource, oslcProperties);
        if (obj is IExtendedResource extendedResource)
        {
            HandleExtendedProperties(resourceType, graph, mainResource, extendedResource, oslcProperties);
        }
    }

    private static void ProcessMethodsForResource(object obj, Type resourceType, IGraph graph, INode mainResource, IDictionary<string, object> oslcProperties)
    {
        foreach (var method in resourceType.GetMethods())
        {
            if (method.GetParameters().Length != 0) continue;
            var methodName = method.Name;
            if ((!methodName.StartsWith(METHOD_NAME_START_GET) || methodName.Length <= METHOD_NAME_START_GET_LENGTH) &&
                (!methodName.StartsWith(METHOD_NAME_START_IS) || methodName.Length <= METHOD_NAME_START_IS_LENGTH)) continue;
            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);
            if (oslcPropertyDefinitionAnnotation == null) continue;
            var value = method.Invoke(obj, null);
            if (value == null) continue;
            if (!TryGetNestedPropertiesConfig(oslcProperties, oslcPropertyDefinitionAnnotation.value, out var nestedProps, out var onlyNestedFlag)) continue;
            BuildAttributeResource(resourceType, method, oslcPropertyDefinitionAnnotation, graph, mainResource, value, nestedProps, onlyNestedFlag);
        }
    }

    private static void ProcessPropertiesForResource(object obj, Type resourceType, IGraph graph, INode mainResource, IDictionary<string, object> oslcProperties)
    {
        foreach (var property in resourceType.GetProperties())
        {
            var oslcPropertyDefinitionAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(property);
            if (oslcPropertyDefinitionAnnotation == null) continue;
            var value = property.GetValue(obj);
            if (value == null) continue;
            if (!TryGetNestedPropertiesConfig(oslcProperties, oslcPropertyDefinitionAnnotation.value, out var nestedProps, out var onlyNestedFlag)) continue;
            BuildAttributeResource(resourceType, property, oslcPropertyDefinitionAnnotation, graph, mainResource, value, nestedProps, onlyNestedFlag);
        }
    }

    private static bool TryGetNestedPropertiesConfig(IDictionary<string, object> oslcProperties, string propertyAnnotationValue,
        out IDictionary<string, object>? nestedProperties, out bool onlyNested)
    {
        nestedProperties = null;
        onlyNested = false;
        if (oslcProperties == null) return true;
        if (oslcProperties.TryGetValue(propertyAnnotationValue, out var mapObj) && mapObj is IDictionary<string, object> map)
        {
            nestedProperties = map;
            return true;
        }
        if (oslcProperties is SingletonWildcardProperties && !(oslcProperties is NestedWildcardProperties))
        {
            nestedProperties = OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
            return true;
        }
        if (oslcProperties is NestedWildcardProperties nestedWildcardProperties)
        {
            nestedProperties = nestedWildcardProperties.CommonNestedProperties();
            onlyNested = !(oslcProperties is SingletonWildcardProperties);
            return true;
        }
        return false;
    }

    private static void HandleExtendedProperties(Type resourceType, IGraph graph, INode mainResource,
        IExtendedResource extendedResource, IDictionary<string, object> properties)
    {
        foreach (var typeUri in extendedResource.GetTypes())
        {
            var propertyName = typeUri.ToString();
            bool shouldProcessType = properties == null || properties.ContainsKey(propertyName) ||
                                     properties is NestedWildcardProperties || properties is SingletonWildcardProperties;
            if (!shouldProcessType) continue;
            var typeResourceNode = graph.CreateUriNode(typeUri);
            var rdfTypeNode = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            if (!graph.GetTriplesWithSubjectPredicateObject(mainResource, rdfTypeNode, typeResourceNode).Any())
            {
                graph.Assert(new Triple(mainResource, rdfTypeNode, typeResourceNode));
            }
        }
        var extendedPropertiesMap = extendedResource.GetExtendedProperties();
        foreach (var qName in extendedPropertiesMap.Keys)
        {
            var propertyFullName = qName.GetNamespaceURI() + qName.GetLocalPart();
            if (!TryGetNestedPropertiesConfig(properties, propertyFullName, out var nestedProps, out var onlyNestedFlag)) continue;
            var propertyNode = graph.CreateUriNode(new Uri(propertyFullName));
            var value = extendedPropertiesMap[qName];
            if (value is ICollection<object> valueCollection) foreach (var item in valueCollection) HandleExtendedValue(resourceType, item, graph, mainResource, propertyNode, nestedProps, onlyNestedFlag);
            else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), value.GetType()))
            {
                IEnumerable<object> collection = new EnumerableWrapper(value);
                foreach (var item in collection) HandleExtendedValue(resourceType, item, graph, mainResource, propertyNode, nestedProps, onlyNestedFlag);
            }
            else HandleExtendedValue(resourceType, value, graph, mainResource, propertyNode, nestedProps, onlyNestedFlag);
        }
    }

    private static void HandleExtendedValue(Type objType, object value, IGraph graph, INode resource, IUriNode property,
        IDictionary<string, object>? nestedProperties, bool onlyNested)
    {
        if (value is AnyResource anyResource)
        {
            INode nestedResourceNode;
            var aboutUri = anyResource.GetAbout();
            if (aboutUri != null)
            {
                if (!aboutUri.IsAbsoluteUri) throw new OslcCoreRelativeURIException(typeof(AnyResource), nameof(AnyResource.GetAbout), aboutUri);
                nestedResourceNode = graph.CreateUriNode(aboutUri);
            }
            else nestedResourceNode = graph.CreateBlankNode();
            foreach (var type in anyResource.GetTypes())
            {
                var typePropertyName = type.ToString();
                if (nestedProperties == null || nestedProperties.ContainsKey(typePropertyName) ||
                    nestedProperties is SingletonWildcardProperties || nestedProperties is NestedWildcardProperties)
                {
                    graph.Assert(new Triple(nestedResourceNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)), graph.CreateUriNode(new Uri(typePropertyName))));
                }
            }
            HandleExtendedProperties(typeof(AnyResource), graph, nestedResourceNode, anyResource, nestedProperties);
            graph.Assert(new Triple(resource, property, nestedResourceNode));
        }
        else if (value.GetType().GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0 || value is Uri)
        {
            HandleLocalResource(objType, null, false, value, graph, resource, property, nestedProperties, onlyNested, null);
        }
        else
        {
            if (onlyNested) return;
            ILiteralNode? literal = CreateLiteralNodeFromObject(value, graph);
            if (literal != null) graph.Assert(new Triple(resource, property, literal));
            else throw new InvalidOperationException($"Unable to create RDF literal for .NET type: {value.GetType()}");
        }
    }

    private static ILiteralNode? CreateLiteralNodeFromObject(object value, IGraph graph) => value switch
    {
        string s => s.ToLiteral(graph),
        bool b => b.ToLiteral(graph),
        byte by => by.ToLiteral(graph),
        sbyte sb => sb.ToLiteral(graph),
        short sh => sh.ToLiteral(graph),
        ushort us => us.ToLiteral(graph),
        int i => i.ToLiteral(graph),
        uint ui => ui.ToLiteral(graph),
        long l => l.ToLiteral(graph),
        ulong ul => ul.ToLiteral(graph),
        BigInteger bi => bi.ToLiteral(graph),
        float f => f.ToLiteral(graph),
        decimal d => d.ToLiteral(graph),
        double dbl => dbl.ToLiteral(graph),
        DateTime dt => dt.ToUniversalTime().ToLiteral(graph),
        DateTimeOffset dto => dto.ToLiteral(graph),
        XElement xml => graph.CreateLiteralNode(xml.ToString(SaveOptions.DisableFormatting), new Uri(RdfSpecsHelper.RdfXmlLiteral)),
        _ => null
    };

    private static void BuildAttributeResource(Type resourceType, MemberInfo member, OslcPropertyDefinition propertyDefinitionAnnotation,
        IGraph graph, INode subjectResource, object propertyValue, IDictionary<string, object>? nestedProperties, bool onlyNested)
    {
        var propertyDefinitionUri = propertyDefinitionAnnotation.value;
        var nameAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcName>(member);
        string name = nameAnnotation?.value ?? GetDefaultPropertyName(member);
        if (!propertyDefinitionUri.EndsWith(name)) throw new OslcCoreInvalidPropertyDefinitionException(resourceType, member, propertyDefinitionAnnotation);

        var valueTypeAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcValueType>(member);
        var isXmlLiteral = valueTypeAnnotation?.value == ValueType.XMLLiteral;
        var attributePredicate = graph.CreateUriNode(new Uri(propertyDefinitionUri));
        Type actualValueType = member switch {
            MethodInfo methodInfo => methodInfo.ReturnType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            _ => throw new ArgumentOutOfRangeException(nameof(member), member, "Member must be MethodInfo or PropertyInfo.")
        };
        var collectionTypeAnnotation = InheritedMethodAttributeHelper.GetAttribute<OslcRdfCollectionType>(member);
        List<INode>? rdfNodeContainer = null;
        if (collectionTypeAnnotation != null && OslcConstants.RDF_NAMESPACE.Equals(collectionTypeAnnotation.namespaceURI) &&
            (RDF_LIST.Equals(collectionTypeAnnotation.collectionType) || RDF_ALT.Equals(collectionTypeAnnotation.collectionType) ||
             RDF_BAG.Equals(collectionTypeAnnotation.collectionType) || RDF_SEQ.Equals(collectionTypeAnnotation.collectionType)))
        {
            rdfNodeContainer = [];
        }

        if (propertyValue is Array arrayValue)
        {
            for (int i = 0; i < arrayValue.Length; i++)
            {
                object? arrayItem = arrayValue.GetValue(i);
                if (arrayItem != null) HandleLocalResource(resourceType, member, isXmlLiteral, arrayItem, graph, subjectResource, attributePredicate, nestedProperties, onlyNested, rdfNodeContainer);
            }
        }
        else if (propertyValue is System.Collections.ICollection && actualValueType.IsGenericType && actualValueType.GetGenericTypeDefinition() == typeof(ICollection<>))
        {
            foreach (var item in new EnumerableWrapper(propertyValue))
            {
                if (item != null) HandleLocalResource(resourceType, member, isXmlLiteral, item, graph, subjectResource, attributePredicate, nestedProperties, onlyNested, rdfNodeContainer);
            }
        }
        else HandleLocalResource(resourceType, member, isXmlLiteral, propertyValue, graph, subjectResource, attributePredicate, nestedProperties, onlyNested, rdfNodeContainer);

        if (rdfNodeContainer != null && collectionTypeAnnotation != null)
        {
            INode containerNode = CreateRdfContainer(collectionTypeAnnotation, rdfNodeContainer, graph);
            graph.Assert(new Triple(subjectResource, attributePredicate, containerNode));
        }
    }

    private static INode CreateRdfContainer(OslcRdfCollectionType collectionType, IList<INode> rdfNodeContainer, IGraph graph)
    {
        if (RDF_LIST.Equals(collectionType.collectionType))
        {
            INode root = graph.CreateBlankNode();
            var current = root;
            for (var i = 0; i < rdfNodeContainer.Count - 1; i++)
            {
                graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst)), rdfNodeContainer[i]));
                INode next = graph.CreateBlankNode();
                graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest)), next));
                current = next;
            }
            if (rdfNodeContainer.Any()) graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListFirst)), rdfNodeContainer[^1]));
            graph.Assert(new Triple(current, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfListRest)), graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + RDF_NIL))));
            return root;
        }
        INode container = graph.CreateBlankNode();
        string rdfCollectionType = collectionType.collectionType switch {
            RDF_ALT => OslcConstants.RDF_NAMESPACE + RDF_ALT,
            RDF_BAG => OslcConstants.RDF_NAMESPACE + RDF_BAG,
            _ => OslcConstants.RDF_NAMESPACE + RDF_SEQ // Default to Seq
        };
        graph.Assert(new Triple(container, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)), graph.CreateUriNode(new Uri(rdfCollectionType))));
        foreach (var node in rdfNodeContainer) graph.Assert(new Triple(container, graph.CreateUriNode(new Uri(OslcConstants.RDF_NAMESPACE + "li")), node));
        return container;
    }

    private static void HandleLocalResource(Type resourceType, MemberInfo? memberWithProperty, bool isXmlLiteral, object objToProcess,
        IGraph graph, INode subjectResource, IUriNode predicate, IDictionary<string, object>? nestedProperties, bool onlyNested, List<INode>? rdfNodeContainer)
    {
        var objectType = objToProcess.GetType();
        INode? rdfObjectNode;
        var isReifiedProperty = InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), objectType);
        var valueToConvert = isReifiedProperty ? objectType.GetMethod("GetValue", Type.EmptyTypes)?.Invoke(objToProcess, null) : objToProcess;
        if (valueToConvert == null) return;

        if (onlyNested && !(valueToConvert is Uri) && !valueToConvert.GetType().GetCustomAttributes(typeof(OslcResourceShape), false).Any()) return;

        rdfObjectNode = (!isXmlLiteral && !(valueToConvert is Uri) && !valueToConvert.GetType().GetCustomAttributes(typeof(OslcResourceShape), false).Any())
            ? CreateLiteralNodeFromObject(valueToConvert, graph) : null;

        if (rdfObjectNode == null) // If not a simple literal or specific handling needed
        {
            if (isXmlLiteral && valueToConvert is string stringValForXml) rdfObjectNode = graph.CreateLiteralNode(stringValForXml, new Uri(RdfSpecsHelper.RdfXmlLiteral));
            else if (valueToConvert is Uri uriValue)
            {
                if (!uriValue.IsAbsoluteUri) throw new OslcCoreRelativeURIException(resourceType, memberWithProperty?.Name ?? "<unknown_member>", uriValue);
                rdfObjectNode = graph.CreateUriNode(uriValue);
            }
            else if (valueToConvert.GetType().GetCustomAttributes(typeof(OslcResourceShape), false).Any())
            {
                var nestedResourceType = valueToConvert.GetType();
                var ns = TypeFactory.GetNamespace(nestedResourceType);
                var name = TypeFactory.GetName(nestedResourceType);
                Uri? aboutUri = (valueToConvert as IResource)?.GetAbout();
                INode newSubjectNode;
                if (aboutUri != null)
                {
                    if (!aboutUri.IsAbsoluteUri) throw new OslcCoreRelativeURIException(nestedResourceType, nameof(IResource.GetAbout), aboutUri);
                    newSubjectNode = graph.CreateUriNode(aboutUri);
                }
                else newSubjectNode = graph.CreateBlankNode();
                graph.Assert(new Triple(newSubjectNode, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)), graph.CreateUriNode(new Uri(ns + name))));
                BuildResource(valueToConvert, nestedResourceType, graph, newSubjectNode, nestedProperties);
                rdfObjectNode = newSubjectNode;
            }
            else
            {
                if (onlyNested) return;
                throw new InvalidOperationException($"Unable to create RDF node for .NET type: {valueToConvert.GetType()} in HandleLocalResource.");
            }
        }

        if (rdfObjectNode != null)
        {
            if (rdfNodeContainer != null)
            {
                if (isReifiedProperty) throw new OslcCoreInvalidPropertyDefinitionException(resourceType, memberWithProperty, null, "Reified resource not supported for RDF collection resources.");
                rdfNodeContainer.Add(rdfObjectNode);
            }
            else
            {
                var triple = new Triple(subjectResource, predicate, rdfObjectNode);
                if (isReifiedProperty && nestedProperties != OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
                {
                    AddReifiedStatements(graph, triple, objToProcess, nestedProperties);
                }
                graph.Assert(triple);
            }
        }
    }

    private static void AddReifiedStatements(IGraph graph, Triple triple, object reifiedResource, IDictionary<string, object> nestedProperties)
    {
        _ = reifiedResource.GetType(); // Ensure reifiedResource is used to satisfy compiler, actual type used via GetType()
        INode uriref = graph.CreateBlankNode();
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfSubject)), triple.Subject));
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfPredicate)), triple.Predicate));
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject)), triple.Object));
        graph.Assert(new Triple(uriref, graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)), graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfStatement))));
        BuildResource(reifiedResource, reifiedResource.GetType(), graph, uriref, nestedProperties);
    }

    private static string GetDefaultPropertyName(MemberInfo member)
    {
        var memberName = member.Name;
        var baseName = memberName.StartsWith(METHOD_NAME_START_GET)
                     ? memberName.Substring(METHOD_NAME_START_GET_LENGTH)
                     : memberName.Substring(METHOD_NAME_START_IS_LENGTH);
        if (string.IsNullOrEmpty(baseName)) return string.Empty;
        return char.ToLowerInvariant(baseName[0]) + baseName.Substring(1);
    }

    private static void RecursivelyCollectNamespaceMappings(INamespaceMapper namespaceMappings, Type resourceType)
    {
        if (resourceType.Assembly.GetCustomAttributes(typeof(OslcSchema), false) is OslcSchema[] oslcSchemaAttributes && oslcSchemaAttributes.Length > 0)
        {
            if (oslcSchemaAttributes[0].namespaceType.GetMethod("GetNamespaces", Type.EmptyTypes)?.Invoke(null, null) is OslcNamespaceDefinition[] oslcNamespaceDefinitionAnnotations)
            {
                foreach (var oslcNamespaceDefinitionAnnotation in oslcNamespaceDefinitionAnnotations)
                {
                    namespaceMappings.AddNamespace(oslcNamespaceDefinitionAnnotation.prefix, new Uri(oslcNamespaceDefinitionAnnotation.namespaceURI));
                }
            }
        }
        if (resourceType.BaseType != null) RecursivelyCollectNamespaceMappings(namespaceMappings, resourceType.BaseType);
        foreach (var interfac in resourceType.GetInterfaces()) RecursivelyCollectNamespaceMappings(namespaceMappings, interfac);
    }

    private static void EnsureNamespacePrefix(string prefix, string ns, INamespaceMapper namespaceMappings)
    {
        var uri = new Uri(ns);
        if (namespaceMappings.GetPrefix(uri) == null)
        {
            if (!namespaceMappings.HasNamespace(prefix)) namespaceMappings.AddNamespace(prefix, uri);
            else
            {
                var index = 1;
                while (true)
                {
                    var newPrefix = $"{prefix}{index}";
                    if (!namespaceMappings.HasNamespace(newPrefix))
                    {
                        namespaceMappings.AddNamespace(newPrefix, uri);
                        return;
                    }
                    index++;
                }
            }
        }
    }

    private static string? GetVisitedResourceName(INode resource) => resource switch
    {
        IUriNode uriNode => uriNode.Uri?.ToString(),
        IBlankNode blankNode => blankNode.InternalID,
        _ => null
    };
}
