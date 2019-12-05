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
using System.Text;

using OSLC4Net.Core;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;

using log4net;

namespace OSLC4Net.Core.JsonProvider
{
    public class JsonHelper
    {
        private static readonly char[] JSON_PROPERTY_DELIMITER_ARRAY = new char[] {':'};

        private const string JSON_PROPERTY_DELIMITER            = ":";
        private const string JSON_PROPERTY_PREFIXES             = "prefixes";
        private const string JSON_PROPERTY_SUFFIX_ABOUT         = "about";
        private const string JSON_PROPERTY_SUFFIX_MEMBER        = "member";
        private const string JSON_PROPERTY_SUFFIX_RESOURCE      = "resource";
        private const string JSON_PROPERTY_SUFFIX_RESPONSE_INFO = "responseInfo";
        private const string JSON_PROPERTY_SUFFIX_RESULTS       = "results";
        private const string JSON_PROPERTY_SUFFIX_TOTAL_COUNT   = "totalCount";
        private const string JSON_PROPERTY_SUFFIX_NEXT_PAGE     = "nextPage";
        private const string JSON_PROPERTY_SUFFIX_TYPE          = "type";
        private const string JSON_PROPERTY_SUFFIX_FIRST         = "first";
        private const string JSON_PROPERTY_SUFFIX_REST          = "rest";
        private const string JSON_PROPERTY_SUFFIX_NIL           = "nil";
        private const string JSON_PROPERTY_SUFFIX_LIST          = "List";
        private const string JSON_PROPERTY_SUFFIX_ALT           = "Alt";
        private const string JSON_PROPERTY_SUFFIX_BAG           = "Bag";
        private const string JSON_PROPERTY_SUFFIX_SEQ           = "Seq";

        private const string RDF_ABOUT_URI    = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_ABOUT;
        private const string RDF_TYPE_URI     = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_TYPE;
        private const string RDF_NIL_URI      = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_NIL;
        private const string RDF_RESOURCE_URI = OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_RESOURCE;
    
        private const string METHOD_NAME_START_GET = "Get";
        private const string METHOD_NAME_START_IS  = "Is";
        private const string METHOD_NAME_START_SET = "Set";

        private const string TRUE  = "true";
        private const string FALSE = "false";

        private const string UTC_DATE_TIME_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

        private static readonly int METHOD_NAME_START_GET_LENGTH = METHOD_NAME_START_GET.Length;
        private static readonly int METHOD_NAME_START_IS_LENGTH = METHOD_NAME_START_IS.Length;

        private static ILog logger = LogManager.GetLogger(typeof(JsonHelper));

        private JsonHelper()
        {
        }

        public static JsonObject CreateJson(IEnumerable<object> objects)
        {
            return CreateJson(null, null, null, null, objects, null);
        }

        public static JsonObject CreateJson(string                      descriptionAbout,
                                            string                      responseInfoAbout,
                                            string                      nextPageAbout,
                                            long?                      totalCount,
                                            IEnumerable<object>         objects,
                                            IDictionary<string, object> properties)
        {
            JsonObject resultJsonObject = new JsonObject();

            IDictionary<string, string> namespaceMappings        = new Dictionary<string, string>();
            IDictionary<string, string> reverseNamespaceMappings = new Dictionary<string, string>();

            if (descriptionAbout != null)
            {
                JsonArray jsonArray = new JsonArray();

                foreach (object obj in objects)
                {
            	    Dictionary<object,JsonObject> visitedObjects = new DictionaryWithReplacement<object,JsonObject>();
                    JsonObject jsonObject = HandleSingleResource(obj,
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
                string rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                         OslcConstants.RDF_NAMESPACE,
                                                         namespaceMappings,
                                                         reverseNamespaceMappings);

                // Ensure we have an rdfs prefix
                string rdfsPrefix = EnsureNamespacePrefix(OslcConstants.RDFS_NAMESPACE_PREFIX,
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
                    string oslcPrefix = EnsureNamespacePrefix(OslcConstants.OSLC_CORE_NAMESPACE_PREFIX,
                                                              OslcConstants.OSLC_CORE_NAMESPACE,
                                                              namespaceMappings,
                                                              reverseNamespaceMappings);

                    JsonObject responseInfoJsonObject = new JsonObject();

                    responseInfoJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT,
                                               responseInfoAbout);

                    responseInfoJsonObject.Add(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TOTAL_COUNT,
                                               totalCount != null ? totalCount : objects.Count());

                    if (nextPageAbout != null)
                    {
                        responseInfoJsonObject.Add(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_NEXT_PAGE,
                                                   nextPageAbout);
                    }
                
                    JsonArray responseInfoTypesJsonArray = new JsonArray();

                    JsonObject responseInfoTypeJsonObject = new JsonObject();

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
        	    Dictionary<object,JsonObject> visitedObjects = new DictionaryWithReplacement<object,JsonObject>();
                HandleSingleResource(objects.First(),
                                     resultJsonObject,
                                     namespaceMappings,
                                     reverseNamespaceMappings,
                                     properties,
                                     visitedObjects);
            }

            // Set the namespace prefixes
            JsonObject namespaces = new JsonObject();
            foreach (string key in namespaceMappings.Keys)
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

        public static object FromJson(JsonValue   json,
                                      Type        beanType)
        {
            List<object>                beans                    = new List<object>();
            IDictionary<string, string> namespaceMappings        = new Dictionary<string, string>();
            IDictionary<string, string> reverseNamespaceMappings = new Dictionary<string, string>();

            // First read the prefixes and set up maps so we can create full property definition values later
            object prefixes = json.ContainsKey(JSON_PROPERTY_PREFIXES) ? json[JSON_PROPERTY_PREFIXES] : null;

            if (prefixes is JsonObject)
            {
                JsonObject prefixesJsonObject = (JsonObject) prefixes;

                foreach (string prefix in prefixesJsonObject.Keys)
                {
                    string ns = (string)prefixesJsonObject[prefix];

                    namespaceMappings.Add(prefix,
                                          ns);

                    reverseNamespaceMappings.Add(ns,
                                                 prefix);
                }
            }

            // We have to know the reverse mapping for the rdf namespace
            if (! reverseNamespaceMappings.ContainsKey(OslcConstants.RDF_NAMESPACE))
            {
                throw new OslcCoreMissingNamespaceDeclarationException(OslcConstants.RDF_NAMESPACE);
            }

            string rdfPrefix = reverseNamespaceMappings[OslcConstants.RDF_NAMESPACE];

            IDictionary<Type, IDictionary<string, MethodInfo>> classPropertyDefinitionsToSetMethods = new Dictionary<Type, IDictionary<string, MethodInfo>>();

            JsonArray jsonArray = null;

            // Look for rdfs:member
            if (reverseNamespaceMappings.ContainsKey(OslcConstants.RDFS_NAMESPACE))
            {
                string rdfsPrefix = reverseNamespaceMappings[OslcConstants.RDFS_NAMESPACE];
                object members = json.ContainsKey(rdfsPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_MEMBER) ?
                    json[rdfsPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_MEMBER] : null;

                if (members is JsonArray)
                {
                    jsonArray = (JsonArray) members;
                }
            }

            if (jsonArray == null)
            {
                // Look for oslc:results.  Seen in ChangeManagement.
                string oslcPrefix = reverseNamespaceMappings[OslcConstants.OSLC_CORE_NAMESPACE];

                if (oslcPrefix != null)
                {
                    object results = json.ContainsKey(oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESULTS) ?
                        json[oslcPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESULTS] : null;

                    if (results is JsonArray)
                    {
                        jsonArray = (JsonArray) results;
                    }
                }
            }

            if (jsonArray != null)
            {
                foreach (object obj in jsonArray)
                {
                    if (obj is JsonObject)
                    {
                        JsonObject resourceJsonObject = (JsonObject) obj;

                        object bean = Activator.CreateInstance(beanType);

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
                object bean = Activator.CreateInstance(beanType);

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
                    object bean = Activator.CreateInstance(beanType);

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
            object list = Activator.CreateInstance(typeof(List<>).MakeGenericType(types));
            MethodInfo add = list.GetType().GetMethod("Add", types);

            foreach (object bean in beans)
            {
                add.Invoke(list, new object[] { bean });
            }

            return list;
        }

        private static void BuildAttributeResource(IDictionary<string, string>      namespaceMappings,
                                                   IDictionary<string, string>      reverseNamespaceMappings,
                                                   Type                             resourceType,
                                                   MethodInfo                       method,
                                                   OslcPropertyDefinition           propertyDefinitionAttribute,
                                                   JsonObject                       jsonObject,
                                                   object                           value,
                                                   IDictionary<string, object>      nestedProperties,
                                                   bool                             onlyNested)
        {
            string propertyDefinition = propertyDefinitionAttribute.value;

            string name;
            OslcName nameAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcName>(method);

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
        
            OslcRdfCollectionType collectionType =
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

            Type returnType = method.ReturnType;

            if (returnType.IsArray)
            {
                JsonArray jsonArray = new JsonArray();

                // We cannot cast to object[] in case this is an array of primitives.  We will use Array reflection instead.
                // Strange case about primitive arrays:  they cannot be cast to object[], but retrieving their individual elements
                // does not return primitives, but the primitive object wrapping counterparts like Integer, Byte, Double, etc.
                int length =
                    (int)value.GetType().GetProperty("Length").GetValue(value, null);
                MethodInfo getValue = value.GetType().GetMethod("GetValue", new Type[] { typeof(int) });
                for (int index = 0;
                     index < length;
                     index++)
                {
                    object obj = getValue.Invoke(value, new object[] { index });

                    JsonValue localResource = HandleLocalResource(namespaceMappings,
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
                JsonArray jsonArray = new JsonArray();

                IEnumerable<object> collection = new EnumerableWrapper(value);

                foreach (object obj in collection)
                {
                    JsonValue localResource = HandleLocalResource(namespaceMappings,
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
                string ns = propertyDefinition.Substring(0,
                                                         propertyDefinition.Length - name.Length);

                if (! reverseNamespaceMappings.ContainsKey(ns))
                {
                    throw new OslcCoreMissingNamespaceDeclarationException(ns);
                }

                string prefix = reverseNamespaceMappings[ns];

                jsonObject.Add(prefix + JSON_PROPERTY_DELIMITER + name,
                               localResourceValue);
            }
        }
    
        private static JsonValue BuildContainer(IDictionary<string, string>    namespaceMappings,
                                                IDictionary<string, string>    reverseNamespaceMappings,
                                                OslcRdfCollectionType          collectionType,
                                                JsonArray                      jsonArray)
        {
            // Ensure we have an rdf prefix
            string rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                     OslcConstants.RDF_NAMESPACE,
                                                     namespaceMappings,
                                                     reverseNamespaceMappings);
        
            if (JSON_PROPERTY_SUFFIX_LIST.Equals(collectionType.collectionType))
            {
                JsonObject listObject = new JsonObject();
            
                listObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
                               OslcConstants.RDF_NAMESPACE + JSON_PROPERTY_SUFFIX_NIL);
           
                for (int i = jsonArray.Count - 1; i >= 0; i --)
                {
                    JsonValue o = jsonArray[i];
               
                    JsonObject newListObject = new JsonObject();
                    newListObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST, o);
                    newListObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_REST, listObject);
               
                    listObject = newListObject;
                }
            
                return listObject;
            }
            
            JsonObject container = new JsonObject();
        
            container.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + collectionType.collectionType,
                          jsonArray);
       
            return container;
        }

        private static void BuildResource(IDictionary<string, string>       namespaceMappings,
                                          IDictionary<string, string>       reverseNamespaceMappings,
                                          object                            obj,
                                          Type                              objectType,
                                          JsonObject                        jsonObject,
                                          IDictionary<string, object>       properties,
                                          IDictionary<object,JsonObject>    visitedObjects)
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
            string rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
                                                     OslcConstants.RDF_NAMESPACE,
                                                     namespaceMappings,
                                                     reverseNamespaceMappings);

            if (rdfPrefix != null)
            {
                JsonArray rdfTypesJsonArray = new JsonArray();

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

                if (obj is IExtendedResource)
                {
            	    IExtendedResource extendedResource = (IExtendedResource) obj;
            	    foreach (Uri type in extendedResource.GetTypes())
            	    {
            		    string typeString = type.ToString();
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

        private static void AddType(string    rdfPrefix,
    		                        JsonArray rdfTypesJsonArray,
    		                        string    typeURI)
        {
    	      JsonObject rdfTypeJsonObject = new JsonObject();
              rdfTypeJsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
        		                    typeURI);
              rdfTypesJsonArray.Add(rdfTypeJsonObject);
        }
    
	    private static void BuildResourceAttributes(IDictionary<string, string>     namespaceMappings,
			                                        IDictionary<string, string>     reverseNamespaceMappings,
			                                        object                          obj,
			                                        Type                            objectType,
			                                        JsonObject                      jsonObject,
			                                        IDictionary<string, object>     properties,
			                                        IDictionary<object,JsonObject>  visitedObjects)
        {
	        if (properties == OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON)
	        {
	            return;
	        }
	    
	        foreach (MethodInfo method in objectType.GetMethods())
            {
                if (method.GetParameters().Length == 0)
                {
                    string methodName = method.Name;
                    if (((methodName.StartsWith(METHOD_NAME_START_GET)) &&
                         (methodName.Length > METHOD_NAME_START_GET_LENGTH)) ||
                        ((methodName.StartsWith(METHOD_NAME_START_IS)) &&
                         (methodName.Length > METHOD_NAME_START_IS_LENGTH)))
                    {
                        OslcPropertyDefinition oslcPropertyDefinitionAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);

                        if (oslcPropertyDefinitionAttribute != null)
                        {
                            object value = method.Invoke(obj, null);

                            if (value != null)
                            {
                                IDictionary<string, object> nestedProperties = null;
                                bool onlyNested = false;
                            
                                if (properties != null)
                                {
                                    IDictionary<String, object> map = (IDictionary<string, object>)properties[oslcPropertyDefinitionAttribute.value];
                                
                                    if (map != null)
                                    {
                                        nestedProperties = map;
                                    }
                                    else if (properties is SingletonWildcardProperties &&
                                             ! (properties is NestedWildcardProperties))
                                    {
                                        nestedProperties = OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                                    }
                                    else if (properties is NestedWildcardProperties)
                                    {
                                        nestedProperties = ((NestedWildcardProperties)properties).CommonNestedProperties();
                                        onlyNested = ! (properties is SingletonWildcardProperties);
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
		
            if (obj is IExtendedResource)
            {
        	    IExtendedResource extendedResource = (IExtendedResource) obj;
        	
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
            IDictionary<QName, object> extendedProperties = extendedResource.GetExtendedProperties();

		    foreach (QName qname in extendedProperties.Keys)
		    {
                string ns = qname.GetNamespaceURI();
                string localName = qname.GetLocalPart();
                IDictionary<string, object> nestedProperties = null;
                bool onlyNested = false;
            
                if (properties != null)
                {
                    IDictionary<String, object> map = (IDictionary<string, object>)properties[ns + localName];
                
                    if (map != null)
                    {
                        nestedProperties = map;
                    }
                    else if (properties is SingletonWildcardProperties &&
                             ! (properties is NestedWildcardProperties))
                    {
                        nestedProperties = OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON;
                    }
                    else if (properties is NestedWildcardProperties)
                    {
                        nestedProperties = ((NestedWildcardProperties)properties).CommonNestedProperties();
                        onlyNested = ! (properties is SingletonWildcardProperties);
                    }                                
                    else
                    {
                        continue;
                    }
                }
            
			    JsonValue value = GetExtendedPropertyJsonValue(namespaceMappings,
			                                                   reverseNamespaceMappings,
			                                                   extendedProperties[qname],
			                                                   nestedProperties,
			                                                   onlyNested,
			                                                   visitedObjects);
			
			    if (value == null && ! onlyNested)
			    {
				    logger.Warn("Could not add extended property " + qname + " for resource " + extendedResource.GetAbout());
			    }
			    else
			    {
                    string prefix = reverseNamespaceMappings[ns];

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

	    private static JsonValue GetExtendedPropertyJsonValue(IDictionary<string, string>       namespaceMappings,
												              IDictionary<string, string>       reverseNamespaceMappings,
												              object                            obj,
												              IDictionary<string, object>       nestedProperties,
												              bool                              onlyNested,
					                                          IDictionary<object,JsonObject>    visitedObjects)
	    {
		    Type resourceType = obj.GetType();

		    if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), resourceType))
		    {
			    JsonArray jsonArray = new JsonArray();
			    ICollection<object> c = (ICollection<object>) obj;

			    foreach (object next in c)
			    {
				    JsonValue nextJson = GetExtendedPropertyJsonValue(namespaceMappings,
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
		    else if ((obj is string)  ||
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
				                               (Uri) obj);
		    }
		    else if (obj is IResource && !visitedObjects.ContainsKey(obj))
		    {
			    return HandleSingleResource(obj,
				                            new JsonObject(),
				                            namespaceMappings,
				                            reverseNamespaceMappings,
				                            nestedProperties,
				                            visitedObjects);
		    } else if (visitedObjects.ContainsKey(obj))
		    {
			    JsonObject returnObject = visitedObjects[obj];
			    if (returnObject.Count == 0)
                {
				    return returnObject;
                }
		    }
		
		    return null;
	    }
	
        private static string GetDefaultPropertyName(MethodInfo method)
        {
            string methodName    = method.Name;
            int    startingIndex = methodName.StartsWith(METHOD_NAME_START_GET) ? METHOD_NAME_START_GET_LENGTH : METHOD_NAME_START_IS_LENGTH;
            int    endingIndex   = startingIndex + 1;

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

        private static JsonValue HandleLocalResource(IDictionary<string, string>   namespaceMappings,
                                                     IDictionary<string, string>   reverseNamespaceMappings,
                                                     Type                          resourceType,
                                                     MethodInfo                    method,
                                                     object                        obj,
                                                     IDictionary<string, object>   nestedProperties,
                                                     bool                          onlyNested)
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

            IDictionary<object, JsonObject> visitedObjects = new Dictionary<object,JsonObject>();
            return HandleSingleResource(obj,
                                        new JsonObject(),
                                        namespaceMappings,
                                        reverseNamespaceMappings,
                                        nestedProperties,
                                        visitedObjects);
        }

	    private static JsonValue HandleReifiedResource(IDictionary<string, string>  namespaceMappings,
			                                           IDictionary<string, string>  reverseNamespaceMappings,
			                                           Type                         resourceType,
			                                           MethodInfo                   method,
			                                           object                       reifiedResource,
	                                                   IDictionary<string, object>  properties)
	    {
            object value = reifiedResource.GetType().GetMethod("GetValue", Type.EmptyTypes).Invoke(reifiedResource, null);
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
		    JsonObject jsonObject = HandleResourceReference(namespaceMappings,
		                                                    reverseNamespaceMappings,
		                                                    resourceType,
		                                                    method,
		                                                    (Uri) value);
		
		    // Add any reified statements.
		    IDictionary<object, JsonObject> visitedObjects = new Dictionary<object,JsonObject>();
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
													        Type                        resourceType,
													        MethodInfo                  method,
													        Uri                         uri)
	    {
		    if (! uri.IsAbsoluteUri)
		    {
		        throw new OslcCoreRelativeURIException(resourceType,
		                                               (method == null) ? "<none>" : method.Name,
		                                               uri);
		    }

		    // Special nested JsonObject for Uri
		    JsonObject jsonObject = new JsonObject();

		    // Ensure we have an rdf prefix
		    string rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
		                                             OslcConstants.RDF_NAMESPACE,
		                                             namespaceMappings,
		                                             reverseNamespaceMappings);

		    jsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE,
		                   uri.ToString());
		
		    return jsonObject;
	    }

        private static JsonObject HandleSingleResource(object                        obj,
                                                       JsonObject                       jsonObject,
                                                       IDictionary<string, string>      namespaceMappings,
                                                       IDictionary<string, string>      reverseNamespaceMappings,
                                                       IDictionary<string, object>      properties,
                                                       IDictionary<object,JsonObject>   visitedObjects)
        {
            Type objectType = obj.GetType();

            // Collect the namespace prefix -> namespace mappings
            RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                reverseNamespaceMappings,
                                                objectType);

            if (obj is IResource)
            {
                Uri aboutURI = ((IResource) obj).GetAbout();

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
		        if (! aboutURI.IsAbsoluteUri)
		        {
		            throw new OslcCoreRelativeURIException(objectType,
		                                                   "getAbout",
		                                                   aboutURI);
		        }

		        // Ensure we have an rdf prefix
		        string rdfPrefix = EnsureNamespacePrefix(OslcConstants.RDF_NAMESPACE_PREFIX,
		                                                 OslcConstants.RDF_NAMESPACE,
		                                                 namespaceMappings,
		                                                 reverseNamespaceMappings);

		        jsonObject.Add(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT,
		                       aboutURI.ToString());
		    }
	    }

        private static string EnsureNamespacePrefix(string              prefix,
                                                    string              ns,
                                                    IDictionary<string, string> namespaceMappings,
                                                    IDictionary<string, string> reverseNamespaceMappings)
        {
            string existingPrefix = reverseNamespaceMappings[ns];

            if (existingPrefix != null)
            {
                return existingPrefix;
            }

            string existingNamespace = namespaceMappings[prefix];

            if (existingNamespace == null)
            {
                namespaceMappings.Add(prefix,
                                      ns);

                reverseNamespaceMappings.Add(ns,
                                             prefix);

                return prefix;
            }

            // There is already a namespace for this prefix.  We need to generate a new unique prefix.
            int index = 1;

            while (true)
            {
                string newPrefix = prefix +
                                         index;

                if (! namespaceMappings.ContainsKey(newPrefix))
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

        private static void RecursivelyCollectNamespaceMappings(IDictionary<string, string>     namespaceMappings,
                                                                IDictionary<string, string>     reverseNamespaceMappings,
                                                                Type objectType)
        {
            OslcSchema[] oslcSchemaAttribute = (OslcSchema[])objectType.Assembly.GetCustomAttributes(typeof(OslcSchema), false);

            if (oslcSchemaAttribute.Length > 0)
            {
                OslcNamespaceDefinition[] oslcNamespaceDefinitionAnnotations =
                    (OslcNamespaceDefinition[])oslcSchemaAttribute[0].namespaceType.GetMethod("GetNamespaces", Type.EmptyTypes).Invoke(null, null);

                foreach (OslcNamespaceDefinition oslcNamespaceDefinitionAnnotation in oslcNamespaceDefinitionAnnotations)
                {
                    string prefix       = oslcNamespaceDefinitionAnnotation.prefix;

                    if (namespaceMappings.ContainsKey(prefix))
                    {
                        continue;
                    }

                    string namespaceURI = oslcNamespaceDefinitionAnnotation.namespaceURI;

                    namespaceMappings.Add(prefix,
                                          namespaceURI);

                    reverseNamespaceMappings.Add(namespaceURI,
                                                 prefix);
                }
            }

            Type superType = objectType.BaseType;
            if (superType != null)
            {
                RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                    reverseNamespaceMappings,
                                                    superType);
            }

            Type[] interfaces = objectType.GetInterfaces();
            if (interfaces != null)
            {
                foreach (Type interfac in interfaces)
                {
                    RecursivelyCollectNamespaceMappings(namespaceMappings,
                                                        reverseNamespaceMappings,
                                                        interfac);
                }
            }
        }

        private static void FromJSON(string                                             rdfPrefix,
                                     IDictionary<string, string>                        jsonNamespaceMappings,
                                     IDictionary<Type, IDictionary<string, MethodInfo>> classPropertyDefinitionsToSetMethods,
                                     JsonObject                                         jsonObject,
                                     Type                                               beanType,
                                     object                                             bean)
        {
            IDictionary<string, MethodInfo> setMethodMap;

            if (! classPropertyDefinitionsToSetMethods.ContainsKey(beanType))
            {
                setMethodMap = CreatePropertyDefinitionToSetMethods(beanType);

                classPropertyDefinitionsToSetMethods.Add(beanType,
                                                         setMethodMap);
            }
            else
            {
                setMethodMap = classPropertyDefinitionsToSetMethods[beanType];
            }

            bool isIReifiedResource = false;

            if (bean is IResource)
            {
                string aboutURIObject = jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT) ?
                    (string)jsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_ABOUT] : null;

                if (aboutURIObject != null)
                {
                    Uri aboutURI = new Uri(aboutURIObject);

                    if (!aboutURI.IsAbsoluteUri)
                    {
                        throw new OslcCoreRelativeURIException(beanType,
                                                               "setAbout",
                                                               aboutURI);
                    }

                    ((IResource) bean).SetAbout(aboutURI);
                }
            }
            else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), beanType))
    	    {
                isIReifiedResource = true;

                string resourceReference = (string)jsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE];

    		    beanType.GetMethod("SetValue",  new Type[] { typeof(Uri) }).Invoke(bean, new object[] { new Uri(resourceReference) });
    	    }	
    	
            IExtendedResource extendedResource;
            IDictionary<QName, object> extendedProperties;

            if (bean is IExtendedResource)
            {
        	    extendedResource = (IExtendedResource) bean;
        	    extendedProperties = new DictionaryWithReplacement<QName, object>();
        	    extendedResource.SetExtendedProperties(extendedProperties);
            }
            else
            {
        	    extendedResource = null;
        	    extendedProperties = null;
            }
        
            foreach (string prefixedName in jsonObject.Keys)
            {
                object jsonValue    = jsonObject[prefixedName];
                string[] split      = prefixedName.Split(JSON_PROPERTY_DELIMITER_ARRAY);

                if (split.Length != 2)
                {
                    if (!JSON_PROPERTY_PREFIXES.Equals(prefixedName))
                    {
                        logger.Warn("Ignored JSON property '" + prefixedName + "'.");
                    }
                }
                else
                {
                    string namespacePrefix = split[0];
                    string name            = split[1];
                    string ns              = jsonNamespaceMappings[namespacePrefix];

                    if (ns == null)
                    {
                        throw new OslcCoreMissingNamespacePrefixException(namespacePrefix);
                    }

                    string propertyDefinition = ns + name;                
                    MethodInfo setMethod;

                    if (! setMethodMap.ContainsKey(propertyDefinition))
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
                                object value = FromExtendedJSONValue(jsonValue,
                                                                     rdfPrefix,
                                                                     jsonNamespaceMappings,
                                                                     beanType);
                                QName qName = new QName(ns,
                                                              name,
                                                              namespacePrefix);

                                extendedProperties.Add(qName, value);
                            }
                        }
                    }
                    else
                    {
                        setMethod = setMethodMap[propertyDefinition];

                        Type setMethodParameterType = setMethod.GetParameters()[0].ParameterType;
                        Type setMethodComponentParameterType = setMethodParameterType;

                        if (setMethodComponentParameterType.IsArray)
                        {
                            setMethodComponentParameterType = setMethodComponentParameterType.GetElementType();
                        }
                        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), setMethodComponentParameterType))
                        {
                            setMethodComponentParameterType = setMethodComponentParameterType.GetGenericArguments()[0];
                        }
                    
                        object parameter = FromJSONValue(rdfPrefix,
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
                                             new object[] {parameter});
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
	    private static object FromExtendedJSONValue(object                      jsonValue,
			                                        string                      rdfPrefix,
			                                        IDictionary<string, string> jsonNamespaceMappings,
			                                        Type                        beanType)
	    {
		    if (jsonValue is JsonArray)
		    {
			    JsonArray jsonArray = (JsonArray) jsonValue;
			    IList<object> collection = new List<object>();
			    
			    foreach (JsonValue value in jsonArray)
			    {
				    collection.Add(FromExtendedJSONValue(value, rdfPrefix, jsonNamespaceMappings, beanType));
			    }

			    return collection;
		    }
		    else if (jsonValue is JsonObject)
		    {
			    JsonObject o = (JsonObject) jsonValue;
			
			    // Is it a resource reference?
			    object resourceURIValue = o.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                    o[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null;

			    if (resourceURIValue != null)
			    {
				    Uri uri = new Uri((string) resourceURIValue); 

				    if (!uri.IsAbsoluteUri)
	                {
	                    throw new OslcCoreRelativeURIException(beanType,
	                                                           "<none>",
	                                                           uri);
	                }
				
				    return new Uri((string) resourceURIValue);
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
		    else if (jsonValue is string)
		    {
			    // Check if it's in the OSLC date format.
			    try
			    {
				    return DateTime.Parse((string)jsonValue);
			    }
			    catch (FormatException e)
			    {
				    // It's not a date. Treat it as a string.
				    return jsonValue;
			    }
		    }

		    return jsonValue;
	    }

	    protected static void FillInRdfType(string              rdfPrefix,
			                                JsonObject          jsonObject,
			                                IExtendedResource   resource)
	    {
		    string typeProperty = rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_TYPE;

		    if (jsonObject.ContainsKey(typeProperty))
		    {
                JsonArray types = (JsonArray)jsonObject[typeProperty];

				foreach (JsonValue typeObj in types)
				{
					resource.AddType(new Uri((string)typeObj[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE]));
				}
		    }
	    }

        private static bool IsRdfListNode(string               rdfPrefix,
                                          Type                 beanType,
                                          MethodInfo           setMethod,
                                          object               jsonValue)
        {
            if (! (jsonValue is JsonObject))
            {
                return false;
            }
       
            JsonObject jsonObject = (JsonObject)jsonValue;
       
            bool isListNode = 
                    jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST)
                    && jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_REST);

            if (isListNode)
            {
                return true;
            }
       
            bool isNilResource = RDF_NIL_URI.Equals(
                    jsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                    jsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null);

            if (!isNilResource)
            {
                return false;
            }
       
            string setMethodName = setMethod.Name;

            if (setMethodName.StartsWith(METHOD_NAME_START_SET))
            {
                string getMethodName = METHOD_NAME_START_GET + setMethodName.Substring(METHOD_NAME_START_GET_LENGTH);
                MethodInfo getMethod = beanType.GetMethod(getMethodName, Type.EmptyTypes);

                if (getMethod == null)
                {
                    string isMethodName = METHOD_NAME_START_IS + setMethodName.Substring(METHOD_NAME_START_GET_LENGTH);

                    getMethod = beanType.GetMethod(isMethodName, Type.EmptyTypes);
                    
                    if (getMethod == null)
                    {
                        return false;
                    }
                }
           
                OslcRdfCollectionType collectionType =
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
   
        private static object FromJSONValue(string                                              rdfPrefix,
                                            IDictionary<string, string>                         jsonNamespaceMappings,
                                            IDictionary<Type, IDictionary<string, MethodInfo>>  classPropertyDefinitionsToSetMethods,
                                            Type                                                beanType,
                                            MethodInfo                                          setMethod,
                                            Type                                                setMethodParameterType,
                                            Type                                                setMethodComponentParameterType,
                                            object                                              jsonValue)
        {
            bool isRdfContainerNode = IsRdfListNode(rdfPrefix, beanType, setMethod, jsonValue);
            JsonArray container = null;
        
            if (! isRdfContainerNode && jsonValue is JsonObject)
            {            
                JsonObject parent = (JsonObject)jsonValue;
            
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
       
            if (! isRdfContainerNode && jsonValue is JsonObject)
            {
                JsonObject nestedJsonObject = (JsonObject) jsonValue;

                if (! InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), setMethodComponentParameterType))
                {
            	    // If this is the special case for an rdf:resource?
            	    string uriObject = nestedJsonObject.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                        (string) nestedJsonObject[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null;

            	    if (uriObject != null)
            	    {
            		    Uri uri = new Uri(uriObject);

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
               
                    JsonObject listNode = (JsonObject) jsonValue;

                    while (listNode != null 
                            && ! RDF_NIL_URI.Equals(
                                     listNode.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE) ?
                                        listNode[rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_RESOURCE] : null))
                    {                   
                        JsonValue o = listNode.ContainsKey(rdfPrefix + JSON_PROPERTY_DELIMITER + JSON_PROPERTY_SUFFIX_FIRST) ?
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

                foreach (JsonValue jsonArrayEntryObject in jsonArray)
                {
                    object parameterArrayObject = FromJSONValue(rdfPrefix,
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
                    Array array = Array.CreateInstance(setMethodComponentParameterType,
                                                       jsonArray.Count());
                    int index = 0;

                    foreach (object parameterArrayObject in tempList)
                    {
                        array.SetValue(parameterArrayObject, index);

                        index++;
                    }

                    return array;
                }

                // This has to be a Collection

                object collection = Activator.CreateInstance(setMethodComponentParameterType);
                ICollection<object> values = (ICollection<object>)jsonValue;

                if (values.Count > 0)
                {
                    MethodInfo add = collection.GetType().GetMethod("Add", new Type[] { values.First().GetType() });

                    foreach (object value in values)
                    {
                        add.Invoke(collection, new object[] { value });
                    }
                }

                return collection;
            }
            else
            {
                string stringValue = (string)(JsonValue)jsonValue;

                if (typeof(string) == setMethodComponentParameterType)
                {
                    return stringValue;
                }
                else if (typeof(bool) == setMethodComponentParameterType || typeof(bool?) == setMethodComponentParameterType)
                {
                    // Cannot use Boolean.parseBoolean since it supports case-insensitive TRUE.
                    if (bool.TrueString.ToUpper().Equals(stringValue.ToUpper()))
                    {
                        return true;
                    }
                    else if (bool.FalseString.ToUpper().Equals(stringValue.ToUpper()))
                    {
                        return false;
                    }
                    else
                    {
                        throw new InvalidOperationException("'" + stringValue + "' has wrong format for Boolean.");
                    }
                }
                else if (typeof(byte) == setMethodComponentParameterType || typeof(byte?) == setMethodComponentParameterType)
                {
                    return byte.Parse(stringValue);
                }
                else if (typeof(short) == setMethodComponentParameterType || typeof(short?) == setMethodComponentParameterType)
                {
                    return short.Parse(stringValue);
                }
                else if (typeof(int) == setMethodComponentParameterType || typeof(int?) == setMethodComponentParameterType)
                {
                    return int.Parse(stringValue);
                }
                else if (typeof(long) == setMethodComponentParameterType || typeof(long?) == setMethodComponentParameterType)
                {
                    return long.Parse(stringValue);
                }
                else if (typeof(float) == setMethodComponentParameterType || typeof(float?) == setMethodComponentParameterType)
                {
                    return float.Parse(stringValue);
                }
                else if (typeof(decimal) == setMethodComponentParameterType || typeof(decimal?) == setMethodComponentParameterType)
                {
                    return decimal.Parse(stringValue);
                }
                else if (typeof(double) == setMethodComponentParameterType || typeof(double?) == setMethodComponentParameterType)
                {
                    return double.Parse(stringValue);
                }
                else if (typeof(DateTime) == setMethodComponentParameterType || typeof(DateTime?) == setMethodComponentParameterType)
                {
                    return DateTime.Parse(stringValue);
                }
            }

            return null;
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
                        OslcPropertyDefinition oslcPropertyDefinitionAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);

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

                            Type getMethodReturnType = method.ReturnType;

                            MethodInfo setMethod = beanType.GetMethod(setMethodName,
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
}
