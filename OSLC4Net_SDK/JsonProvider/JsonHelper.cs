/*******************************************************************************
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Json;
using System.Linq;
using System.Reflection;
using log4net;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.JsonProvider;

public class JsonHelper
{
    private static readonly char[] JSON_PROPERTY_DELIMITER_ARRAY = new char[] { ':' };

    private const string JSON_PROPERTY_DELIMITER = ":";
    private const string JSON_PROPERTY_PREFIXES = "prefixes";
    private const string JSON_PROPERTY_SUFFIX_ABOUT = "about";
    private const string JSON_PROPERTY_SUFFIX_MEMBER = "member";
    private const string JSON_PROPERTY_SUFFIX_RESOURCE = "resource";
    private const string JSON_PROPERTY_SUFFIX_RESPONSE_INFO = "responseInfo";
    private const string JSON_PROPERTY_SUFFIX_RESULTS = "results";
    private const string JSON_PROPERTY_SUFFIX_TOTAL_COUNT = "totalCount";
    private const string JSON_PROPERTY_SUFFIX_NEXT_PAGE = "nextPage";
    private const string JSON_PROPERTY_SUFFIX_TYPE = "type";
    private const string JSON_PROPERTY_SUFFIX_FIRST = "first";
    private const string JSON_PROPERTY_SUFFIX_REST = "rest";
    private const string JSON_PROPERTY_SUFFIX_NIL = "nil";
    private const string JSON_PROPERTY_SUFFIX_LIST = "List";
    private const string JSON_PROPERTY_SUFFIX_ALT = "Alt";
    private const string JSON_PROPERTY_SUFFIX_BAG = "Bag";
    private const string JSON_PROPERTY_SUFFIX_SEQ = "Seq";

    private const string RDF_ABOUT_URI = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_ABOUT;
    private const string RDF_TYPE_URI = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_TYPE;
    private const string RDF_NIL_URI = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_NIL;
    private const string RDF_RESOURCE_URI = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_RESOURCE;

    private const string METHOD_NAME_START_GET = "Get";
    private const string METHOD_NAME_START_IS = "Is";
    private const string METHOD_NAME_START_SET = "Set";

    private const string TRUE = "true";
    private const string FALSE = "false";

    private const string UTC_DATE_TIME_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

    private static readonly int METHOD_NAME_START_GET_LENGTH = METHOD_NAME_START_GET.Length;
    private static readonly int METHOD_NAME_START_IS_LENGTH = METHOD_NAME_START_IS.Length;

    private static readonly ILog logger = LogManager.GetLogger(typeof(JsonHelper));

    private JsonHelper()
    {
    }

    public static JsonObject CreateJson(IEnumerable<object> objects)
    {
        return CreateJson(null, null, null, null, objects, null);
    }

    public static JsonObject CreateJson(string descriptionAbout,
                                        string responseInfoAbout,
                                        string nextPageAbout,
                                        long? totalCount,
                                        IEnumerable<object> objects,
                                        IDictionary<string, object> properties)
    {
        var resultJsonObject = new JsonObject();

        IDictionary<string, string> namespaceMappings = new Dictionary<string, string>();
        IDictionary<string, string> reverseNamespaceMappings = new Dictionary<string, string>();

        if (descriptionAbout != null)
        {
            var jsonArray = new JsonArray();

            foreach (var obj in objects)
            {
                Dictionary<object, JsonObject> visitedObjects = new DictionaryWithReplacement<object, JsonObject>();
                var jsonObject = HandleSingleResource(obj,
                                                             new JsonObject(),
                                                             namespaceMappings,
                                                             reverseNamespaceMappings,
                                                             properties,
                                                             visitedObjects);

                if (jsonObject != null)
                {
                    jsonArray.Add(jsonObject);
                }
            }

            // Ensure we have an rdf prefix
            var rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                     OslcConstants.RDF_NAMESPACE,
                                                     namespaceMappings,
                                                     reverseNamespaceMappings);

            // Ensure we have an rdfs prefix
            var rdfsPrefix = EnsureNamespacePrefix(OslcConstants.RDFS_NAMESPACE_PREFIX,
                                                      OslcConstants.RDFS_NAMESPACE,
                                                      namespaceMappings,
                                                      reverseNamespaceMappings);

            resultJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT,
                                 descriptionAbout);

            resultJsonObject.Add(rdfsPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_MEMBER,
                                 jsonArray);

            if (responseInfoAbout != null)
            {
                // Ensure we have an oslc prefix
                var oslcPrefix = EnsureNamespacePrefix(OslcConstants.OSLC_CORE_NAMESPACE_PREFIX,
                                                          OslcConstants.OSLC_CORE_NAMESPACE,
                                                          namespaceMappings,
                                                          reverseNamespaceMappings);

                var responseInfoJsonObject = new JsonObject();

                responseInfoJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT,
                                           responseInfoAbout);

                responseInfoJsonObject.Add(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TOTAL_COUNT,
                                           totalCount != null ? totalCount : objects.Count());

                if (nextPageAbout != null)
                {
                    responseInfoJsonObject.Add(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_NEXT_PAGE,
                                               nextPageAbout);
                }

                var responseInfoTypesJsonArray = new JsonArray();

                var responseInfoTypeJsonObject = new JsonObject();

                responseInfoTypeJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
                                               OslcConstants.TYPE_RESPONSE_INFO);

                responseInfoTypesJsonArray.Add(responseInfoTypeJsonObject);

                responseInfoJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TYPE,
                                           responseInfoTypesJsonArray);

                resultJsonObject.Add(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESPONSE_INFO,
                                     responseInfoJsonObject);
            }
        }
        else if (objects.Count() == 1)
        {
            Dictionary<object, JsonObject> visitedObjects = new DictionaryWithReplacement<object, JsonObject>();
            HandleSingleResource(objects.First(),
                                 resultJsonObject,
                                 namespaceMappings,
                                 reverseNamespaceMappings,
                                 properties,
                                 visitedObjects);
        }

        // Set the namespace prefixes
        var namespaces = new JsonObject();
        foreach (var key in namespaceMappings.Keys)
        {
            namespaces.Add(key,
                           namespaceMappings[key]);
        }

        if (namespaces.Count > 0)
        {
            resultJsonObject.Add(JSON_PROPERTY_PREFIXES,
                                 namespaces);
        }

        return resultJsonObject;
    }

    public static object FromJson(JsonValue json,
                                  Type beanType)
    {
        var beans = new List<object>();
        IDictionary<string, string> namespaceMappings = new Dictionary<string, string>();
        IDictionary<string, string> reverseNamespaceMappings = new Dictionary<string, string>();

        // First read the prefixes and set up maps so we can create full property definition values later
        object prefixes = json.ContainsKey(JSON_PROPERTY_PREFIXES) ? json[JSON_PROPERTY_PREFIXES] : null;

        if (prefixes is JsonObject prefixesJsonObject)
        {
            foreach (var prefix in prefixesJsonObject.Keys)
            {
                var ns = (string)prefixesJsonObject[prefix];

                namespaceMappings.Add(prefix,
                                      ns);

                reverseNamespaceMappings.Add(ns,
                                             prefix);
            }
        }

        // We have to know the reverse mapping for the rdf namespace
        if (!reverseNamespaceMappings.ContainsKey(OslcConstants.RDF_NAMESPACE))
        {
            throw new OslcCoreMissingNamespaceDeclarationException(OslcConstants.RDF_NAMESPACE);
        }

        var rdfPrefix = reverseNamespaceMappings[OslcConstants.RDF_NAMESPACE];

        IDictionary<Type, IDictionary<string, MethodInfo>> classPropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MethodInfo>>();

        JsonArray jsonArray = null;

        // Look for rdfs:member
        if (reverseNamespaceMappings.ContainsKey(OslcConstants.RDFS_NAMESPACE))
        {
            var rdfsPrefix = reverseNamespaceMappings[OslcConstants.RDFS_NAMESPACE];
            object members = json.ContainsKey(rdfsPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_MEMBER) ?
                json[rdfsPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_MEMBER] : null;

            if (members is JsonArray)
            {
                jsonArray = (JsonArray)members;
            }
        }

        if (jsonArray == null)
        {
            // Look for oslc:results.  Seen in ChangeManagement.
            var oslcPrefix = reverseNamespaceMappings[OslcConstants.OSLC_CORE_NAMESPACE];

            if (oslcPrefix != null)
            {
                object results = json.ContainsKey(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESULTS) ?
                    json[oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESULTS] : null;

                if (results is JsonArray)
                {
                    jsonArray = (JsonArray)results;
                }
            }
        }

        if (jsonArray != null)
        {
            foreach (object obj in jsonArray)
            {
                if (obj is JsonObject resourceJsonObject)
                {
                    var bean = Activator.CreateInstance(beanType);

                    FromJSON(rdfPrefix,
                             namespaceMappings,
                             classPropertyDefinitionsToSetMethods,
                             resourceJsonObject,
                             beanType,
                             bean);

                    beans.Add(bean);
                }
            }
        }
        else if (json is JsonObject)
        {
            var bean = Activator.CreateInstance(beanType);

            FromJSON(rdfPrefix,
                     namespaceMappings,
                     classPropertyDefinitionsToSetMethods,
                     json as JsonObject,
                     beanType,
                     bean);

            beans.Add(bean);
        }
        else
        {
            foreach (JsonObject jsonObject in json as JsonArray)
            {
                var bean = Activator.CreateInstance(beanType);

                FromJSON(rdfPrefix,
                         namespaceMappings,
                         classPropertyDefinitionsToSetMethods,
                         jsonObject,
                         beanType,
                         bean);

                beans.Add(bean);
            }
        }

        // To support primitive arrays, we have to use Array reflection to
        // set individual elements. We cannot use Collection.toArray.
        // Array.set will unwrap objects to their corresponding primitives.
        Type[] types = { beanType };
        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(types));
        var add = list.GetType().GetMethod("Add", types);

        foreach (var bean in beans)
        {
            add.Invoke(list, new object[] { bean });
        }

        return list;
    }

    private static void BuildAttributeResource(IDictionary<string, string> namespaceMappings,
                                               IDictionary<string, string> reverseNamespaceMappings,
                                               Type resourceType,
                                               MethodInfo method,
                                               OslcPropertyDefinition propertyDefinitionAttribute,
                                               JsonObject jsonObject,
                                               object value,
                                               IDictionary<string, object> nestedProperties,
                                               bool onlyNested)
    {
        var propertyDefinition = propertyDefinitionAttribute.value;

        string name;
        var nameAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcName>(method);

        if (nameAttribute != null)
        {
            name = nameAttribute.value;
        }
        else
        {
            name = GetDefaultPropertyName(method);
        }

        if (!propertyDefinition.EndsWith(name))
        {
            throw new OslcCoreInvalidPropertyDefinitionException(resourceType,
                                                                 method,
                                                                 propertyDefinitionAttribute);
        }

        bool isRdfContainer;

        var collectionType =
            InheritedMethodAttributeHelper.GetAttribute<OslcRdfCollectionType>(method);

        if (collectionType != null &&
                OslcConstants.RDF_NAMESPACE.Equals(collectionType.namespaceURI) &&
                    (JSON_PROPERTY_SUFFIX_LIST.Equals(collectionType.collectionType)
                     || JSON_PROPERTY_SUFFIX_ALT.Equals(collectionType.collectionType)
                     || JSON_PROPERTY_SUFFIX_BAG.Equals(collectionType.collectionType)
                     || JSON_PROPERTY_SUFFIX_SEQ.Equals(collectionType.collectionType)))
        {
            isRdfContainer = true;
        }
        else
        {
            isRdfContainer = false;
        }

        JsonValue localResourceValue;

        var returnType = method.ReturnType;

        if (returnType.IsArray)
        {
            var jsonArray = new JsonArray();

            // We cannot cast to object[] in case this is an array of primitives.  We will use Array reflection instead.
            // Strange case about primitive arrays:  they cannot be cast to object[], but retrieving their individual elements
            // does not return primitives, but the primitive object wrapping counterparts like Integer, Byte, Double, etc.
            var length =
                (int)value.GetType().GetProperty("Length").GetValue(value, null);
            var getValue = value.GetType().GetMethod("GetValue", new Type[] { typeof(int) });
            for (var index = 0;
                 index < length;
                 index++)
            {
                var obj = getValue.Invoke(value, new object[] { index });

                var localResource = HandleLocalResource(namespaceMappings,
                                                              reverseNamespaceMappings,
                                                              resourceType,
                                                              method,
                                                              obj,
                                                              nestedProperties,
                                                              onlyNested);
                if (localResource != null)
                {
                    jsonArray.Add(localResource);
                }
            }

            if (isRdfContainer)
            {
                localResourceValue = BuildContainer(namespaceMappings,
                                                    reverseNamespaceMappings,
                                                    collectionType, jsonArray);
            }
            else
            {
                if (jsonArray.Count > 0)
                {
                    localResourceValue = jsonArray;
                }
                else
                {
                    localResourceValue = null;
                }
            }
        }
        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), returnType))
        {
            var jsonArray = new JsonArray();

            IEnumerable<object> collection = new EnumerableWrapper(value);

            foreach (var obj in collection)
            {
                var localResource = HandleLocalResource(namespaceMappings,
                                                              reverseNamespaceMappings,
                                                              resourceType,
                                                              method,
                                                              obj,
                                                              nestedProperties,
                                                              onlyNested);
                if (localResource != null)
                {
                    jsonArray.Add(localResource);
                }
            }

            if (isRdfContainer)
            {
                localResourceValue = BuildContainer(namespaceMappings,
                                                    reverseNamespaceMappings,
                                                    collectionType, jsonArray);
            }
            else
            {
                if (jsonArray.Count > 0)
                {
                    localResourceValue = jsonArray;
                }
                else
                {
                    localResourceValue = null;
                }
            }
        }
        else
        {
            localResourceValue = HandleLocalResource(namespaceMappings,
                                                     reverseNamespaceMappings,
                                                     resourceType,
                                                     method,
                                                     value,
                                                     nestedProperties,
                                                     onlyNested);
        }

        if (localResourceValue != null)
        {
            var ns = propertyDefinition.Substring(0,
                                                     propertyDefinition.Length - name.Length);

            if (!reverseNamespaceMappings.ContainsKey(ns))
            {
                throw new OslcCoreMissingNamespaceDeclarationException(ns);
            }

            var prefix = reverseNamespaceMappings[ns];

            jsonObject.Add(prefix + JSON_PROPERTY_DELIMITER + name,
                           localResourceValue);
        }
    }

    private static JsonValue BuildContainer(IDictionary<string, string> namespaceMappings,
                                            IDictionary<string, string> reverseNamespaceMappings,
                                            OslcRdfCollectionType collectionType,
                                            JsonArray jsonArray)
    {
        // Ensure we have an rdf prefix
        var rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                 OslcConstants.RDF_NAMESPACE,
                                                 namespaceMappings,
                                                 reverseNamespaceMappings);

        if (JSON_PROPERTY_SUFFIX_LIST.Equals(collectionType.collectionType))
        {
            var listObject = new JsonObject();

            listObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
                           OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_NIL);

            for (var i = jsonArray.Count - 1; i >= 0; i--)
            {
                var o = jsonArray[i];

                var newListObject = new JsonObject();
                newListObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST, o);
                newListObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_REST, listObject);

                listObject = newListObject;
            }

            return listObject;
        }

        var container = new JsonObject();

        container.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + collectionType.collectionType,
                      jsonArray);

        return container;
    }

    private static void BuildResource(IDictionary<string, string> namespaceMappings,
                                      IDictionary<string, string> reverseNamespaceMappings,
                                      object obj,
                                      Type objectType,
                                      JsonObject jsonObject,
                                      IDictionary<string, object> properties,
                                      IDictionary<object, JsonObject> visitedObjects)
    {
        visitedObjects.Add(obj, jsonObject);
        BuildResourceAttributes(namespaceMappings,
                                reverseNamespaceMappings,
                                    obj,
                                    objectType,
                                    jsonObject,
                                    properties,
                                    visitedObjects);

        // For JSON, we have to save array of rdf:type

        // Ensure we have an rdf prefix
        var rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                 OslcConstants.RDF_NAMESPACE,
                                                 namespaceMappings,
                                                 reverseNamespaceMappings);

        if (rdfPrefix != null)
        {
            var rdfTypesJsonArray = new JsonArray();

            string qualifiedName;
            if (objectType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
            {
                qualifiedName = TypeFactory.GetQualifiedName(objectType);
                AddType(rdfPrefix,
                        rdfTypesJsonArray,
                        qualifiedName);
            }
            else
            {
                qualifiedName = null;
            }

            if (obj is IExtendedResource extendedResource)
            {
                foreach (var type in extendedResource.GetTypes())
                {
                    var typeString = type.ToString();
                    if (!typeString.Equals(qualifiedName))
                    {
                        AddType(rdfPrefix,
                                rdfTypesJsonArray,
                                typeString);
                    }
                }
            }

            // The sub-class of IExtendedResource could have added an explicit rdf:type property

            if (jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TYPE))
            {
                jsonObject.Remove(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TYPE);
            }

            jsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TYPE,
                           rdfTypesJsonArray);
        }
    }

    private static void AddType(string rdfPrefix,
                                JsonArray rdfTypesJsonArray,
                                string typeURI)
    {
        var rdfTypeJsonObject = new JsonObject();
        rdfTypeJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
                              typeURI);
        rdfTypesJsonArray.Add(rdfTypeJsonObject);
    }

    private static void BuildResourceAttributes(IDictionary<string, string> namespaceMappings,
                                                IDictionary<string, string> reverseNamespaceMappings,
                                                object obj,
                                                Type objectType,
                                                JsonObject jsonObject,
                                                IDictionary<string, object> properties,
                                                IDictionary<object, JsonObject> visitedObjects)
    {
        if (properties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
        {
            return;
        }

        foreach (var method in objectType.GetMethods())
        {
            if (method.GetParameters().Length == 0)
            {
                var methodName = method.Name;
                if (((methodName.StartsWith(METHOD_NAME_START_GET)) &&
                     (methodName.Length > METHOD_NAME_START_GET_LENGTH)) ||
                    ((methodName.StartsWith(METHOD_NAME_START_IS)) &&
                     (methodName.Length > METHOD_NAME_START_IS_LENGTH)))
                {
                    var oslcPropertyDefinitionAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);

                    if (oslcPropertyDefinitionAttribute != null)
                    {
                        var value = method.Invoke(obj, null);

                        if (value != null)
                        {
                            IDictionary<string, object> nestedProperties = null;
                            var onlyNested = false;

                            if (properties != null)
                            {
                                var map = (IDictionary<string, object>)properties[oslcPropertyDefinitionAttribute.value];

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

                            BuildAttributeResource(namespaceMappings,
                                                   reverseNamespaceMappings,
                                                   objectType,
                                                   method,
                                                   oslcPropertyDefinitionAttribute,
                                                   jsonObject,
                                                   value,
                                                   nestedProperties,
                                                   onlyNested);
                        }
                    }
                }
            }
        }

        if (obj is IExtendedResource extendedResource)
        {
            AddExtendedProperties(namespaceMappings,
                                      reverseNamespaceMappings,
                                      jsonObject,
                                      extendedResource,
                                      properties,
                                      visitedObjects);
        }
    }

    protected static void AddExtendedProperties(IDictionary<string, string> namespaceMappings,
                                                IDictionary<string, string> reverseNamespaceMappings,
                                                JsonObject jsonObject,
                                                IExtendedResource extendedResource,
                                            IDictionary<string, object> properties,
                                            IDictionary<object, JsonObject> visitedObjects)
    {
        var extendedProperties = extendedResource.GetExtendedProperties();

        foreach (var qname in extendedProperties.Keys)
        {
            var ns = qname.GetNamespaceURI();
            var localName = qname.GetLocalPart();
            IDictionary<string, object> nestedProperties = null;
            var onlyNested = false;

            if (properties != null)
            {
                var map = (IDictionary<string, object>)properties[ns + localName];

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

            var value = GetExtendedPropertyJsonValue(namespaceMappings,
                                                           reverseNamespaceMappings,
                                                           extendedProperties[qname],
                                                           nestedProperties,
                                                           onlyNested,
                                                           visitedObjects);

            if (value == null && !onlyNested)
            {
                logger.Warn("Could not add extended property " + qname + " for resource " + extendedResource.GetAbout());
            }
            else
            {
                var prefix = reverseNamespaceMappings[ns];

                if (prefix == null)
                {
                    prefix = qname.GetPrefix();

                    // Add the prefix to the JSON namespace mappings.
                    namespaceMappings.Add(prefix, ns);
                    reverseNamespaceMappings.Add(ns, prefix);
                }

                // Add the value to the JSON object.
                jsonObject.Add(prefix + JSON_PROPERTY_DELIMITER + localName, value);
            }
        }
    }

    private static JsonValue GetExtendedPropertyJsonValue(IDictionary<string, string> namespaceMappings,
                                                          IDictionary<string, string> reverseNamespaceMappings,
                                                          object obj,
                                                          IDictionary<string, object> nestedProperties,
                                                          bool onlyNested,
                                                          IDictionary<object, JsonObject> visitedObjects)
    {
        var resourceType = obj.GetType();

        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), resourceType))
        {
            var jsonArray = new JsonArray();
            var c = (ICollection<object>)obj;

            foreach (var next in c)
            {
                var nextJson = GetExtendedPropertyJsonValue(namespaceMappings,
                                                                  reverseNamespaceMappings,
                                                                  next,
                                                                  nestedProperties,
                                                                  onlyNested,
                                                                  visitedObjects);
                if (nextJson != null)
                {
                    jsonArray.Add(nextJson);
                }
            }

            return jsonArray;
        }
        else if ((obj is string) ||
                 (obj is byte) ||
                 (obj is double) ||
                 (obj is decimal) ||
                 (obj is short) ||
                 (obj is int) ||
                 (obj is long))
        {
            if (onlyNested)
            {
                return null;
            }

            return obj.ToString();
        }
        else if (obj is bool)
        {
            if (onlyNested)
            {
                return null;
            }

            return ((bool)obj) ? TRUE : FALSE;
        }
        else if (obj is DateTime)
        {
            if (onlyNested)
            {
                return null;
            }

            return ((DateTime)obj).ToUniversalTime().ToString(UTC_DATE_TIME_FORMAT);
        }
        else if (obj is Uri)
        {
            if (onlyNested)
            {
                return null;
            }

            return HandleResourceReference(namespaceMappings,
                                           reverseNamespaceMappings,
                                           resourceType,
                                           null,
                                           (Uri)obj);
        }
        else if (obj is IResource && !visitedObjects.ContainsKey(obj))
        {
            return HandleSingleResource(obj,
                                        new JsonObject(),
                                        namespaceMappings,
                                        reverseNamespaceMappings,
                                        nestedProperties,
                                        visitedObjects);
        }
        else if (visitedObjects.ContainsKey(obj))
        {
            var returnObject = visitedObjects[obj];
            if (returnObject.Count == 0)
            {
                return returnObject;
            }
        }

        return null;
    }

    private static string GetDefaultPropertyName(MethodInfo method)
    {
        var methodName = method.Name;
        var startingIndex = methodName.StartsWith(METHOD_NAME_START_GET) ? METHOD_NAME_START_GET_LENGTH : METHOD_NAME_START_IS_LENGTH;
        var endingIndex = startingIndex + 1;

        // We want the name to start with a lower-case letter
        var lowercasedFirstCharacter = methodName.Substring(startingIndex,
                                                               1).ToLower(CultureInfo.GetCultureInfo("en"));

        if (methodName.Length == endingIndex)
        {
            return lowercasedFirstCharacter;
        }

        return lowercasedFirstCharacter +
               methodName.Substring(endingIndex);
    }

    private static JsonValue HandleLocalResource(IDictionary<string, string> namespaceMappings,
                                                 IDictionary<string, string> reverseNamespaceMappings,
                                                 Type resourceType,
                                                 MethodInfo method,
                                                 object obj,
                                                 IDictionary<string, object> nestedProperties,
                                                 bool onlyNested)
    {
        if ((obj is string) ||
            (obj is byte) ||
            (obj is double) ||
            (obj is decimal) ||
            (obj is short) ||
            (obj is int) ||
            (obj is long))
        {
            if (onlyNested)
            {
                return null;
            }

            return obj.ToString();
        }
        else if (obj is bool)
        {
            if (onlyNested)
            {
                return null;
            }

            return ((bool)obj) ? TRUE : FALSE;
        }
        else if (obj is DateTime)
        {
            if (onlyNested)
            {
                return null;
            }

            return ((DateTime)obj).ToUniversalTime().ToString(UTC_DATE_TIME_FORMAT);
        }
        else if (obj is Uri)
        {
            if (onlyNested)
            {
                return null;
            }

            return HandleResourceReference(namespaceMappings,
                                           reverseNamespaceMappings,
                                           resourceType,
                                           method,
                                           (Uri)obj);
        }
        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), obj.GetType()))
        {
            return HandleReifiedResource(namespaceMappings,
                                         reverseNamespaceMappings,
                                         obj.GetType(),
                                         method,
                                         obj,
                                         nestedProperties);
        }

        IDictionary<object, JsonObject> visitedObjects = new Dictionary<object, JsonObject>();
        return HandleSingleResource(obj,
                                    new JsonObject(),
                                    namespaceMappings,
                                    reverseNamespaceMappings,
                                    nestedProperties,
                                    visitedObjects);
    }

    private static JsonValue HandleReifiedResource(IDictionary<string, string> namespaceMappings,
                                                   IDictionary<string, string> reverseNamespaceMappings,
                                                   Type resourceType,
                                                   MethodInfo method,
                                                   object reifiedResource,
                                                   IDictionary<string, object> properties)
    {
        var value = reifiedResource.GetType().GetMethod("GetValue", Type.EmptyTypes).Invoke(reifiedResource, null);
        if (value == null)
        {
            return null;
        }

        if (!(value is Uri))
        {
            // The OSLC JSON serialization doesn't support reification on anything except
            // resources by reference (typically links with labels). Throw an exception
            // if the value isn't a Uri.
            // See http://open-services.net/bin/view/Main/OslcCoreSpecAppendixLinks
            throw new OslcCoreInvalidPropertyTypeException(resourceType,
                                                           method,
                                                           method.ReturnType);
        }

        // Add the resource reference value.
        var jsonObject = HandleResourceReference(namespaceMappings,
                                                        reverseNamespaceMappings,
                                                        resourceType,
                                                        method,
                                                        (Uri)value);

        // Add any reified statements.
        IDictionary<object, JsonObject> visitedObjects = new Dictionary<object, JsonObject>();
        BuildResourceAttributes(namespaceMappings,
                                reverseNamespaceMappings,
                                reifiedResource,
                                resourceType,
                                jsonObject,
                                properties,
                                visitedObjects);

        return jsonObject;
    }

    protected static JsonObject HandleResourceReference(IDictionary<string, string> namespaceMappings,
                                                        IDictionary<string, string> reverseNamespaceMappings,
                                                        Type resourceType,
                                                        MethodInfo method,
                                                        Uri uri)
    {
        if (!uri.IsAbsoluteUri)
        {
            throw new OslcCoreRelativeURIException(resourceType,
                                                   (method == null) ? "<none>" : method.Name,
                                                   uri);
        }

        // Special nested JsonObject for Uri
        var jsonObject = new JsonObject();

        // Ensure we have an rdf prefix
        var rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                 OslcConstants.RDF_NAMESPACE,
                                                 namespaceMappings,
                                                 reverseNamespaceMappings);

        jsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
                       uri.ToString());

        return jsonObject;
    }

    private static JsonObject HandleSingleResource(object obj,
                                                   JsonObject jsonObject,
                                                   IDictionary<string, string> namespaceMappings,
                                                   IDictionary<string, string> reverseNamespaceMappings,
                                                   IDictionary<string, object> properties,
                                                   IDictionary<object, JsonObject> visitedObjects)
    {
        var objectType = obj.GetType();

        // Collect the namespace prefix -> namespace mappings
        RecursivelyCollectNamespaceMappings(namespaceMappings,
                                            reverseNamespaceMappings,
                                            objectType);

        if (obj is IResource)
        {
            var aboutURI = ((IResource)obj).GetAbout();

            AddAboutURI(jsonObject,
                            namespaceMappings,
                            reverseNamespaceMappings,
                            objectType,
                            aboutURI);
        }

        BuildResource(namespaceMappings,
                      reverseNamespaceMappings,
                      obj,
                      objectType,
                      jsonObject,
                      properties,
                      visitedObjects);

        return jsonObject;
    }

    protected static void AddAboutURI(JsonObject jsonObject,
                                      IDictionary<string, string> namespaceMappings,
                                      IDictionary<string, string> reverseNamespaceMappings,
                                      Type objectType,
                                      Uri aboutURI)
    {
        if (aboutURI != null)
        {
            if (!aboutURI.IsAbsoluteUri)
            {
                throw new OslcCoreRelativeURIException(objectType,
                                                       "getAbout",
                                                       aboutURI);
            }

            // Ensure we have an rdf prefix
            var rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                     OslcConstants.RDF_NAMESPACE,
                                                     namespaceMappings,
                                                     reverseNamespaceMappings);

            jsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT,
                           aboutURI.ToString());
        }
    }

    private static string EnsureNamespacePrefix(string prefix,
                                                string ns,
                                                IDictionary<string, string> namespaceMappings,
                                                IDictionary<string, string> reverseNamespaceMappings)
    {
        var existingPrefix = reverseNamespaceMappings[ns];

        if (existingPrefix != null)
        {
            return existingPrefix;
        }

        var existingNamespace = namespaceMappings[prefix];

        if (existingNamespace == null)
        {
            namespaceMappings.Add(prefix,
                                  ns);

            reverseNamespaceMappings.Add(ns,
                                         prefix);

            return prefix;
        }

        // There is already a namespace for this prefix.  We need to generate a new unique prefix.
        var index = 1;

        while (true)
        {
            var newPrefix = prefix +
                            index;

            if (!namespaceMappings.ContainsKey(newPrefix))
            {
                namespaceMappings.Add(newPrefix,
                                      ns);

                reverseNamespaceMappings.Add(ns,
                                             newPrefix);

                return newPrefix;
            }

            index++;
        }
    }

    private static void RecursivelyCollectNamespaceMappings(IDictionary<string, string> namespaceMappings,
                                                            IDictionary<string, string> reverseNamespaceMappings,
                                                            Type objectType)
    {
        var oslcSchemaAttribute = (OslcSchema[])objectType.Assembly.GetCustomAttributes(typeof(OslcSchema), false);

        if (oslcSchemaAttribute.Length > 0)
        {
            var oslcNamespaceDefinitionAnnotations =
                (OslcNamespaceDefinition[])oslcSchemaAttribute[0].namespaceType.GetMethod("GetNamespaces", Type.EmptyTypes).Invoke(null, null);

            foreach (var oslcNamespaceDefinitionAnnotation in oslcNamespaceDefinitionAnnotations)
            {
                var prefix = oslcNamespaceDefinitionAnnotation.prefix;

                if (namespaceMappings.ContainsKey(prefix))
                {
                    continue;
                }

                var namespaceURI = oslcNamespaceDefinitionAnnotation.namespaceURI;

                namespaceMappings.Add(prefix,
                                      namespaceURI);

                reverseNamespaceMappings.Add(namespaceURI,
                                             prefix);
            }
        }

        var superType = objectType.BaseType;
        if (superType != null)
        {
            RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                reverseNamespaceMappings,
                                                superType);
        }

        var interfaces = objectType.GetInterfaces();
        if (interfaces != null)
        {
            foreach (var interfac in interfaces)
            {
                RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                    reverseNamespaceMappings,
                                                    interfac);
            }
        }
    }

    private static void FromJSON(string rdfPrefix,
                                 IDictionary<string, string> jsonNamespaceMappings,
                                 IDictionary<Type, IDictionary<string, MethodInfo>> classPropertyDefinitionsToSetMethods,
                                 JsonObject jsonObject,
                                 Type beanType,
                                 object bean)
    {
        IDictionary<string, MethodInfo> setMethodMap;

        if (!classPropertyDefinitionsToSetMethods.ContainsKey(beanType))
        {
            setMethodMap = CreatePropertyDefinitionToSetMethods(beanType);

            classPropertyDefinitionsToSetMethods.Add(beanType,
                                                     setMethodMap);
        }
        else
        {
            setMethodMap = classPropertyDefinitionsToSetMethods[beanType];
        }

        var isIReifiedResource = false;

        if (bean is IResource)
        {
            var aboutURIObject = jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT) ?
                (string)jsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT] : null;

            if (aboutURIObject != null)
            {
                var aboutURI = new Uri(aboutURIObject);

                if (!aboutURI.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(beanType,
                                                           "setAbout",
                                                           aboutURI);
                }

                ((IResource)bean).SetAbout(aboutURI);
            }
        }
        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), beanType))
        {
            isIReifiedResource = true;

            var resourceReference = (string)jsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE];

            beanType.GetMethod("SetValue", new Type[] { typeof(Uri) }).Invoke(bean, new object[] { new Uri(resourceReference) });
        }

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

        foreach (var prefixedName in jsonObject.Keys)
        {
            object jsonValue = jsonObject[prefixedName];
            var split = prefixedName.Split(JSON_PROPERTY_DELIMITER_ARRAY);

            if (split.Length != 2)
            {
                if (!JSON_PROPERTY_PREFIXES.Equals(prefixedName))
                {
                    logger.Warn("Ignored JSON property '" + prefixedName + "'.");
                }
            }
            else
            {
                var namespacePrefix = split[0];
                var name = split[1];
                var ns = jsonNamespaceMappings[namespacePrefix];

                if (ns == null)
                {
                    throw new OslcCoreMissingNamespacePrefixException(namespacePrefix);
                }

                var propertyDefinition = ns + name;
                MethodInfo setMethod;

                if (!setMethodMap.ContainsKey(propertyDefinition))
                {
                    if (RDF_ABOUT_URI.Equals(propertyDefinition) ||
                        (isIReifiedResource && RDF_RESOURCE_URI.Equals(propertyDefinition)))
                    {
                        // Ignore missing property definitions for rdf:about, rdf:types and
                        // rdf:resource for IReifiedResources.
                    }
                    else if (RDF_TYPE_URI.Equals(propertyDefinition))
                    {
                        if (extendedResource != null)
                        {
                            FillInRdfType(rdfPrefix, jsonObject, extendedResource);
                        }
                        // Otherwise ignore missing propertyDefinition for rdf:type.
                    }
                    else
                    {
                        if (extendedProperties == null)
                        {
                            logger.Info("Set method not found for object type:  " +
                                    beanType.Name +
                                    ", propertyDefinition:  " +
                                    propertyDefinition);
                        }
                        else
                        {
                            var value = FromExtendedJSONValue(jsonValue,
                                                                 rdfPrefix,
                                                                 jsonNamespaceMappings,
                                                                 beanType);
                            var qName = new QName(ns,
                                                          name,
                                                          namespacePrefix);

                            extendedProperties.Add(qName, value);
                        }
                    }
                }
                else
                {
                    setMethod = setMethodMap[propertyDefinition];

                    var setMethodParameterType = setMethod.GetParameters()[0].ParameterType;
                    var setMethodComponentParameterType = setMethodParameterType;

                    if (setMethodComponentParameterType.IsArray)
                    {
                        setMethodComponentParameterType = setMethodComponentParameterType.GetElementType();
                    }
                    else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), setMethodComponentParameterType))
                    {
                        setMethodComponentParameterType = setMethodComponentParameterType.GetGenericArguments()[0];
                    }

                    var parameter = FromJSONValue(rdfPrefix,
                                                     jsonNamespaceMappings,
                                                     classPropertyDefinitionsToSetMethods,
                                                     beanType,
                                                     setMethod,
                                                     setMethodParameterType,
                                                     setMethodComponentParameterType,
                                                     jsonValue);

                    if (parameter != null)
                    {
                        setMethod.Invoke(bean,
                                         new object[] { parameter });
                    }
                }
            }
        }
    }

    /*
     * Infer the appropriate bean value from the JSON value. We can't rely on
     * the setter parameter type since this is an extended value that has no
     * setter in the bean.
     */
    private static object FromExtendedJSONValue(object jsonValue,
                                                string rdfPrefix,
                                                IDictionary<string, string> jsonNamespaceMappings,
                                                Type beanType)
    {
        if (jsonValue is JsonArray jsonArray)
        {
            IList<object> collection = new List<object>();

            foreach (var value in jsonArray)
            {
                collection.Add(FromExtendedJSONValue(value, rdfPrefix, jsonNamespaceMappings, beanType));
            }

            return collection;
        }
        else if (jsonValue is JsonObject o)
        {

            // Is it a resource reference?
            object resourceURIValue = o.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
            o[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null;

            if (resourceURIValue != null)
            {
                var uri = new Uri((string)resourceURIValue);

                if (!uri.IsAbsoluteUri)
                {
                    throw new OslcCoreRelativeURIException(beanType,
                                                           "<none>",
                                                           uri);
                }

                return new Uri((string)resourceURIValue);
            }

            // Handle an inline resource.
            AbstractResource any = new AnyResource();
            FromJSON(rdfPrefix,
                     jsonNamespaceMappings,
                     new Dictionary<Type, IDictionary<string, MethodInfo>>(),
                     o,
                     typeof(AnyResource),
                     any);

            return any;
        }
        else if (jsonValue is string jsonString)
        {
            // Check if it's in the OSLC date format.
            try
            {
                return DateTime.Parse(jsonString);
            }
            catch (FormatException)
            {
                // It's not a date. Treat it as a string.
                return jsonString;
            }
        }

        return jsonValue;
    }

    protected static void FillInRdfType(string rdfPrefix,
                                        JsonObject jsonObject,
                                        IExtendedResource resource)
    {
        var typeProperty = rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TYPE;

        if (jsonObject.ContainsKey(typeProperty))
        {
            var types = (JsonArray)jsonObject[typeProperty];

            foreach (var typeObj in types)
            {
                resource.AddType(new Uri((string)typeObj[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE]));
            }
        }
    }

    private static bool IsRdfListNode(string rdfPrefix,
                                      Type beanType,
                                      MethodInfo setMethod,
                                      object jsonValue)
    {
        if (!(jsonValue is JsonObject))
        {
            return false;
        }

        var jsonObject = jsonValue as JsonObject;

        var isListNode =
                jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST)
                && jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_REST);

        if (isListNode)
        {
            return true;
        }

        var isNilResource = RDF_NIL_URI.Equals(
                jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                jsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null);

        if (!isNilResource)
        {
            return false;
        }

        var setMethodName = setMethod.Name;

        if (setMethodName.StartsWith(METHOD_NAME_START_SET))
        {
            var getMethodName = METHOD_NAME_START_GET + setMethodName.Substring(METHOD_NAME_START_GET_LENGTH);
            var getMethod = beanType.GetMethod(getMethodName, Type.EmptyTypes);

            if (getMethod == null)
            {
                var isMethodName = METHOD_NAME_START_IS + setMethodName.Substring(METHOD_NAME_START_GET_LENGTH);

                getMethod = beanType.GetMethod(isMethodName, Type.EmptyTypes);

                if (getMethod == null)
                {
                    return false;
                }
            }

            var collectionType =
                InheritedMethodAttributeHelper.GetAttribute<OslcRdfCollectionType>(getMethod);

            if (collectionType != null &&
                    OslcConstants.RDF_NAMESPACE.Equals(collectionType.namespaceURI) &&
                    "List".Equals(collectionType.collectionType))
            {
                return true;
            }
        }

        return false;
    }

    private static object FromJSONValue(string rdfPrefix,
                                        IDictionary<string, string> jsonNamespaceMappings,
                                        IDictionary<Type, IDictionary<string, MethodInfo>> classPropertyDefinitionsToSetMethods,
                                        Type beanType,
                                        MethodInfo setMethod,
                                        Type setMethodParameterType,
                                        Type setMethodComponentParameterType,
                                        object jsonValue)
    {
        var isRdfContainerNode = IsRdfListNode(rdfPrefix, beanType, setMethod, jsonValue);
        JsonArray container = null;

        if (!isRdfContainerNode && jsonValue is JsonObject)
        {
            var parent = (JsonObject)jsonValue;

            try
            {
                container = parent.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ALT) ?
                    (JsonArray)parent[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ALT] : null;

                if (container == null)
                {
                    container = parent.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_BAG) ?
                        (JsonArray)parent[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_BAG] : null;
                }

                if (container == null)
                {
                    container = parent.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_SEQ) ?
                        (JsonArray)parent[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_SEQ] : null;
                }
            }
            catch (InvalidCastException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }

            isRdfContainerNode = container != null;
        }

        if (!isRdfContainerNode && jsonValue is JsonObject)
        {
            var nestedJsonObject = (JsonObject)jsonValue;

            if (!InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), setMethodComponentParameterType))
            {
                // If this is the special case for an rdf:resource?
                var uriObject = nestedJsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                    (string)nestedJsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null;

                if (uriObject != null)
                {
                    var uri = new Uri(uriObject);

                    if (!uri.IsAbsoluteUri)
                    {
                        throw new OslcCoreRelativeURIException(beanType,
                                setMethod.Name,
                                uri);
                    }

                    return uri;
                }
            }

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

            FromJSON(rdfPrefix,
                     jsonNamespaceMappings,
                     classPropertyDefinitionsToSetMethods,
                     nestedJsonObject,
                     setMethodComponentParameterType,
                     nestedBean);

            return nestedBean;
        }
        else if (jsonValue is JsonArray || isRdfContainerNode)
        {
            ICollection<JsonValue> jsonArray;

            if (isRdfContainerNode && container == null)
            {
                jsonArray = new List<JsonValue>();

                var listNode = (JsonObject)jsonValue;

                while (listNode != null
                        && !RDF_NIL_URI.Equals(
                                 listNode.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                                    listNode[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null))
                {
                    var o = listNode.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST) ?
                        listNode[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST] : null;

                    jsonArray.Add(o);

                    listNode = listNode.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_REST) ?
                        (JsonObject)listNode[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_REST] : null;
                }
            }
            else if (isRdfContainerNode)
            {
                ICollection<JsonValue> array = container;

                jsonArray = array;
            }
            else
            {
                ICollection<JsonValue> array = (JsonArray)jsonValue;

                jsonArray = array;
            }

            IList<object> tempList = new List<object>();

            foreach (var jsonArrayEntryObject in jsonArray)
            {
                var parameterArrayObject = FromJSONValue(rdfPrefix,
                                                            jsonNamespaceMappings,
                                                            classPropertyDefinitionsToSetMethods,
                                                            beanType,
                                                            setMethod,
                                                            setMethodComponentParameterType,
                                                            setMethodComponentParameterType,
                                                            jsonArrayEntryObject);

                tempList.Add(parameterArrayObject);
            }

            if (setMethodParameterType.IsArray)
            {
                // To support primitive arrays, we have to use Array reflection to set individual elements.  We cannot use Collection.toArray.
                // Array.set will unwrap objects to their corresponding primitives.
                var array = Array.CreateInstance(setMethodComponentParameterType,
                                                   jsonArray.Count());
                var index = 0;

                foreach (var parameterArrayObject in tempList)
                {
                    array.SetValue(parameterArrayObject, index);

                    index++;
                }

                return array;
            }

            // This has to be a Collection

            var collection = Activator.CreateInstance(setMethodComponentParameterType);
            var values = (ICollection<object>)jsonValue;

            if (values.Count > 0)
            {
                var add = collection.GetType().GetMethod("Add", new Type[] { values.First().GetType() });

                foreach (var value in values)
                {
                    add.Invoke(collection, new object[] { value });
                }
            }

            return collection;
        }
        else
        {
            // TODO: JsonPrimitive<JsonValue can be a ready-made boolean
            var jsonPrimitive = jsonValue as JsonPrimitive;
            if (jsonPrimitive == null)
            {
                logger.Warn($"JSON value is not a primitive: '{jsonValue}'");
                throw new ArgumentException();
            }

            if (typeof(string) == setMethodComponentParameterType)
            {
                return (string)jsonPrimitive;
            }
            else if (typeof(bool) == setMethodComponentParameterType || typeof(bool?) == setMethodComponentParameterType)
            {
                if (jsonPrimitive.JsonType == JsonType.Boolean)
                {
                    return (bool)jsonPrimitive;
                }
                else if (jsonPrimitive.JsonType == JsonType.String)
                {
                    var boolString = (string)jsonPrimitive;
                    // TODO: revisit the decision not to use Boolean.TryParse()
                    // Cannot use Boolean.parseBoolean since it supports case-insensitive TRUE.
                    if (bool.TrueString.ToUpper().Equals(boolString.ToUpper()))
                    {
                        return true;
                    }
                    else if (bool.FalseString.ToUpper().Equals(boolString.ToUpper()))
                    {
                        return false;
                    }
                }
                else
                {
                    throw new ArgumentException($"'{jsonPrimitive}' has wrong format for Boolean.");
                }
            }
            else if (typeof(byte) == setMethodComponentParameterType || typeof(byte?) == setMethodComponentParameterType)
            {
                return byte.Parse(jsonPrimitive);
            }
            else if (typeof(short) == setMethodComponentParameterType || typeof(short?) == setMethodComponentParameterType)
            {
                return short.Parse(jsonPrimitive);
            }
            else if (typeof(int) == setMethodComponentParameterType || typeof(int?) == setMethodComponentParameterType)
            {
                if (jsonPrimitive.JsonType == JsonType.Number)
                {
                    return (int)jsonPrimitive;
                }
                else
                {
                    return int.Parse((string)jsonPrimitive);
                }
            }
            else if (typeof(long) == setMethodComponentParameterType || typeof(long?) == setMethodComponentParameterType)
            {
                if (jsonPrimitive.JsonType == JsonType.Number)
                {
                    return (long)jsonPrimitive;
                }
                else
                {
                    return long.Parse((string)jsonPrimitive);
                }
            }
            else if (typeof(float) == setMethodComponentParameterType || typeof(float?) == setMethodComponentParameterType)
            {
                if (jsonPrimitive.JsonType == JsonType.Number)
                {
                    return (float)jsonPrimitive;
                }
                else
                {
                    return float.Parse((string)jsonPrimitive);
                }
            }
            else if (typeof(decimal) == setMethodComponentParameterType || typeof(decimal?) == setMethodComponentParameterType)
            {
                if (jsonPrimitive.JsonType == JsonType.Number)
                {
                    return (decimal)jsonPrimitive;
                }
                else
                {
                    return decimal.Parse((string)jsonPrimitive);
                }
            }
            else if (typeof(double) == setMethodComponentParameterType || typeof(double?) == setMethodComponentParameterType)
            {
                if (jsonPrimitive.JsonType == JsonType.Number)
                {
                    return (double)jsonPrimitive;
                }
                else
                {
                    return double.Parse((string)jsonPrimitive);
                }
            }
            else if (typeof(DateTime) == setMethodComponentParameterType || typeof(DateTime?) == setMethodComponentParameterType)
            {
                if (!DateTime.TryParse(jsonPrimitive, out var parsedDate))
                {
                    logger.Warn($"Cannot parse '{jsonPrimitive}' as a DateTime");
                }
                return parsedDate;
            }
        }

        return null;
    }

    private static IDictionary<string, MethodInfo> CreatePropertyDefinitionToSetMethods(Type beanType)
    {
        IDictionary<string, MethodInfo> result = new Dictionary<string, MethodInfo>();
        var methods = beanType.GetMethods();

        foreach (var method in methods)
        {
            if (method.GetParameters().Length == 0)
            {
                var getMethodName = method.Name;
                if (((getMethodName.StartsWith(METHOD_NAME_START_GET)) &&
                     (getMethodName.Length > METHOD_NAME_START_GET_LENGTH)) ||
                    ((getMethodName.StartsWith(METHOD_NAME_START_IS)) &&
                     (getMethodName.Length > METHOD_NAME_START_IS_LENGTH)))
                {
                    var oslcPropertyDefinitionAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);

                    if (oslcPropertyDefinitionAttribute != null)
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

                        var getMethodReturnType = method.ReturnType;

                        var setMethod = beanType.GetMethod(setMethodName,
                                                                  new Type[] { getMethodReturnType });

                        if (setMethod == null)
                        {
                            throw new OslcCoreMissingSetMethodException(beanType,
                                                                        method);
                        }

                        result.Add(oslcPropertyDefinitionAttribute.value,
                                    setMethod);
                    }
                }
            }
        }

        return result;
    }

    private class DictionaryWithReplacement<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
        public DictionaryWithReplacement()
            : base()
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
