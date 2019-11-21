/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
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

namespace OSLC4Net.Core.Model
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Reflection;

    using OSLC4Net.Core.Attribute;
    using OSLC4Net.Core.Exceptions;

    #endregion

    /// <summary>
    /// Factory for creating ResourceShape resources
    /// </summary>
    public sealed class ResourceShapeFactory
    {
        private static readonly string METHOD_NAME_START_GET = "Get";

        private static readonly string METHOD_NAME_START_IS = "Is";

        private static readonly string METHOD_NAME_START_SET = "Set";

        private static int METHOD_NAME_START_GET_LENGTH = METHOD_NAME_START_GET.Length;

        private static int METHOD_NAME_START_IS_LENGTH = METHOD_NAME_START_IS.Length;

        private static IDictionary<Type, ValueType> TYPE_TO_VALUE_TYPE = new Dictionary<Type, ValueType>();

        static ResourceShapeFactory()
        {
            // Primitive types, which are actually just aliases for objects in the
            // System namespace
            TYPE_TO_VALUE_TYPE[typeof(bool)] = ValueType.Boolean;
            TYPE_TO_VALUE_TYPE[typeof(byte)] = ValueType.Integer;
            TYPE_TO_VALUE_TYPE[typeof(short)] = ValueType.Integer;
            TYPE_TO_VALUE_TYPE[typeof(int)] = ValueType.Integer;
            TYPE_TO_VALUE_TYPE[typeof(long)] = ValueType.Integer;
            TYPE_TO_VALUE_TYPE[typeof(float)] = ValueType.Float;
            TYPE_TO_VALUE_TYPE[typeof(decimal)] = ValueType.Float;
            TYPE_TO_VALUE_TYPE[typeof(double)] = ValueType.Double;
            TYPE_TO_VALUE_TYPE[typeof(string)] = ValueType.String;

            // Object types
            TYPE_TO_VALUE_TYPE[typeof(BigInteger)] = ValueType.Integer;
            TYPE_TO_VALUE_TYPE[typeof(DateTime)] = ValueType.DateTime;
            TYPE_TO_VALUE_TYPE[typeof(Uri)] = ValueType.Resource;
        }

        private ResourceShapeFactory()
            : base()
        {
        }

        /// <summary>
        /// Create an OSLC ResourceShape resource
        /// </summary>
        /// <param name="baseURI"></param>
        /// <param name="resourceShapesPath"></param>
        /// <param name="resourceShapePath"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static ResourceShape CreateResourceShape(
            string baseURI,
            string resourceShapesPath,
            string resourceShapePath,
            Type resourceType)
        {
            var verifiedTypes = new HashSet<Type>();
            verifiedTypes.Add(resourceType);

            return CreateResourceShape(baseURI, resourceShapesPath, resourceShapePath, resourceType, verifiedTypes);
        }

        private static Property CreateProperty(
            string baseURI,
            Type resourceType,
            MethodInfo method,
            OslcPropertyDefinition propertyDefinitionAttribute,
            ISet<Type> verifiedTypes)
        {
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

            var propertyDefinition = propertyDefinitionAttribute.value;

            if (!propertyDefinition.EndsWith(name))
            {
                throw new OslcCoreInvalidPropertyDefinitionException(resourceType, method, propertyDefinitionAttribute);
            }

            var returnType = method.ReturnType;
            Occurs occurs;
            var occursAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcOccurs>(method);
            if (occursAttribute != null)
            {
                occurs = occursAttribute.value;
                ValidateUserSpecifiedOccurs(resourceType, method, occursAttribute);
            }
            else
            {
                occurs = GetDefaultOccurs(returnType);
            }

            var componentType = GetComponentType(resourceType, method, returnType);

            // Reified resources are a special case.
            if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>), componentType))
            {
                var genericType = typeof(IReifiedResource<object>).GetGenericTypeDefinition();

                var interfaces = componentType.GetInterfaces();

                foreach (var interfac in interfaces)
                {
                    if (interfac.IsGenericType && genericType == interfac.GetGenericTypeDefinition())
                    {
                        componentType = interfac.GetGenericArguments()[0];
                        break;
                    }
                }
            }

            ValueType valueType;
            var valueTypeAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcValueType>(method);
            if (valueTypeAttribute != null)
            {
                valueType = valueTypeAttribute.value;
                ValidateUserSpecifiedValueType(resourceType, method, valueType, componentType);
            }
            else
            {
                valueType = GetDefaultValueType(resourceType, method, componentType);
            }

            var property = new Property(name, occurs, new Uri(propertyDefinition), valueType);

            property.SetTitle(property.GetName());
            var titleAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcTitle>(method);
            if (titleAttribute != null)
            {
                property.SetTitle(titleAttribute.value);
            }

            var descriptionAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcDescription>(method);
            if (descriptionAttribute != null)
            {
                property.SetDescription(descriptionAttribute.value);
            }

            var rangeAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcRange>(method);
            if (rangeAttribute != null)
            {
                foreach (var range in rangeAttribute.value)
                {
                    property.AddRange(new Uri(range));
                }
            }

            var representationAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcRepresentation>(method);
            if (representationAttribute != null)
            {
                var representation = representationAttribute.value;
                ValidateUserSpecifiedRepresentation(resourceType, method, representation, componentType);
                property.SetRepresentation(new Uri(RepresentationExtension.ToString(representation)));
            }
            else
            {
                var defaultRepresentation = GetDefaultRepresentation(componentType);
                if (defaultRepresentation != Representation.Unknown)
                {
                    property.SetRepresentation(new Uri(RepresentationExtension.ToString(defaultRepresentation)));
                }
            }

            var allowedValueAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcAllowedValue>(method);
            if (allowedValueAttribute != null)
            {
                foreach (var allowedValue in allowedValueAttribute.value)
                {
                    property.AddAllowedValue(allowedValue);
                }
            }

            var allowedValuesAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcAllowedValues>(method);
            if (allowedValuesAttribute != null)
            {
                property.SetAllowedValuesRef(new Uri(allowedValuesAttribute.value));
            }

            var defaultValueAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcDefaultValue>(method);
            if (defaultValueAttribute != null)
            {
                property.SetDefaultValue(defaultValueAttribute.value);
            }

            var hiddenAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcHidden>(method);
            if (hiddenAttribute != null)
            {
                property.SetHidden(hiddenAttribute.value);
            }

            var memberPropertyAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcMemberProperty>(method);
            if (memberPropertyAttribute != null)
            {
                property.SetMemberProperty(memberPropertyAttribute.value);
            }

            var readOnlyAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcReadOnly>(method);
            if (readOnlyAttribute != null)
            {
                property.SetReadOnly(readOnlyAttribute.value);
            }

            var maxSizeAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcMaxSize>(method);
            if (maxSizeAttribute != null)
            {
                property.SetMaxSize(maxSizeAttribute.value);
            }

            var valueShapeAttribute = InheritedMethodAttributeHelper.GetAttribute<OslcValueShape>(method);
            if (valueShapeAttribute != null)
            {
                property.SetValueShape(new Uri(baseURI + "/" + valueShapeAttribute.value));
            }

            if (ValueType.LocalResource.Equals(valueType))
            {
                // If this is a nested class we potentially have not yet verified
                if (verifiedTypes.Add(componentType))
                {
                    // Validate nested resource ignoring return value, but throwing any exceptions
                    CreateResourceShape(
                        baseURI,
                        OslcConstants.PATH_RESOURCE_SHAPES,
                        "unused",
                        componentType,
                        verifiedTypes);
                }
            }

            return property;
        }

        private static ResourceShape CreateResourceShape(
            string baseURI,
            string resourceShapesPath,
            string resourceShapePath,
            Type resourceType,
            ISet<Type> verifiedTypes)
        {
            var resourceShapeAttribute =
                (OslcResourceShape[])resourceType.GetCustomAttributes(typeof(OslcResourceShape), false);
            if (resourceShapeAttribute == null || resourceShapeAttribute.Length == 0)
            {
                throw new OslcCoreMissingAttributeException(resourceType, typeof(OslcResourceShape));
            }

            var about = new Uri(baseURI + "/" + resourceShapesPath + "/" + resourceShapePath);
            var resourceShape = new ResourceShape(about);

            var title = resourceShapeAttribute[0].title;
            if (title != null && title.Length > 0)
            {
                resourceShape.SetTitle(title);
            }

            foreach (var describesItem in resourceShapeAttribute[0].describes)
            {
                resourceShape.AddDescribeItem(new Uri(describesItem));
            }

            ISet<string> propertyDefinitions = new HashSet<string>();

            foreach (var method in resourceType.GetMethods())
            {
                if (method.GetParameters().Length == 0)
                {
                    var methodName = method.Name;
                    var methodNameLength = methodName.Length;
                    if (methodName.StartsWith(METHOD_NAME_START_GET) && methodNameLength > METHOD_NAME_START_GET_LENGTH
                        || methodName.StartsWith(METHOD_NAME_START_IS)
                        && methodNameLength > METHOD_NAME_START_IS_LENGTH)
                    {
                        var propertyDefinitionAttribute =
                            InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);
                        if (propertyDefinitionAttribute != null)
                        {
                            var propertyDefinition = propertyDefinitionAttribute.value;
                            if (propertyDefinitions.Contains(propertyDefinition))
                            {
                                throw new OslcCoreDuplicatePropertyDefinitionException(
                                    resourceType,
                                    propertyDefinitionAttribute);
                            }

                            propertyDefinitions.Add(propertyDefinition);

                            var property = CreateProperty(
                                baseURI,
                                resourceType,
                                method,
                                propertyDefinitionAttribute,
                                verifiedTypes);
                            resourceShape.AddProperty(property);

                            ValidateSetMethodExists(resourceType, method);
                        }
                    }
                }
            }

            return resourceShape;
        }

        private static Type GetComponentType(Type resourceType, MethodInfo method, Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), type))
            {
                var actualTypeArguments = type.GetGenericArguments();
                if (actualTypeArguments.Length == 1)
                {
                    return actualTypeArguments[0];
                }

                throw new OslcCoreInvalidPropertyTypeException(resourceType, method, type);
            }
            else
            {
                return type;
            }
        }

        private static Occurs GetDefaultOccurs(Type type)
        {
            if (type.IsArray
                || InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), type))
            {
                return Occurs.ZeroOrMany;
            }

            return Occurs.ZeroOrOne;
        }

        private static string GetDefaultPropertyName(MethodInfo method)
        {
            var methodName = method.Name;
            var startingIndex = methodName.StartsWith(METHOD_NAME_START_GET)
                                    ? METHOD_NAME_START_GET_LENGTH
                                    : METHOD_NAME_START_IS_LENGTH;

            // We want the name to start with a lower-case letter
            var lowercasedFirstCharacter = methodName.Substring(startingIndex, 1).ToLower();
            if (methodName.Length == 1)
            {
                return lowercasedFirstCharacter;
            }

            return lowercasedFirstCharacter + methodName.Substring(startingIndex + 1);
        }

        private static Representation GetDefaultRepresentation(Type componentType)
        {
            if (componentType.Equals(typeof(Uri)))
            {
                return Representation.Reference;
            }

            return Representation.Unknown;
        }

        private static ValueType GetDefaultValueType(Type resourceType, MethodInfo method, Type componentType)
        {
            var valueType = TYPE_TO_VALUE_TYPE[componentType];
            if (valueType == ValueType.Unknown)
            {
                throw new OslcCoreInvalidPropertyTypeException(resourceType, method, componentType);
            }

            return valueType;
        }

        private static void ValidateSetMethodExists(Type resourceType, MethodInfo getMethod)
        {
            var getMethodName = getMethod.Name;

            string setMethodName;
            if (getMethodName.StartsWith(METHOD_NAME_START_GET))
            {
                setMethodName = METHOD_NAME_START_SET + getMethodName.Substring(METHOD_NAME_START_GET_LENGTH);
            }
            else
            {
                setMethodName = METHOD_NAME_START_SET + getMethodName.Substring(METHOD_NAME_START_IS_LENGTH);
            }

            if (resourceType.GetMethod(setMethodName, new Type[] { getMethod.ReturnType }) == null)
            {
                throw new OslcCoreMissingSetMethodException(resourceType, getMethod);
            }
        }

        private static void ValidateUserSpecifiedOccurs(
            Type resourceType,
            MethodInfo method,
            OslcOccurs occursAttribute)
        {
            var returnType = method.ReturnType;
            var occurs = occursAttribute.value;

            if (returnType.IsArray
                || InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>), returnType))
            {
                if (!Occurs.ZeroOrMany.Equals(occurs) && !Occurs.OneOrMany.Equals(occurs))
                {
                    throw new OslcCoreInvalidOccursException(resourceType, method, occursAttribute);
                }
            }
            else
            {
                if (!Occurs.ZeroOrOne.Equals(occurs) && !Occurs.ExactlyOne.Equals(occurs))
                {
                    throw new OslcCoreInvalidOccursException(resourceType, method, occursAttribute);
                }
            }
        }

        private static void ValidateUserSpecifiedRepresentation(
            Type resourceType,
            MethodInfo method,
            Representation userSpecifiedRepresentation,
            Type componentType)
        {
            // If user-specified representation is reference and component is not Uri
            // or
            // user-specified representation is inline and component is a standard class
            if (Representation.Reference.Equals(userSpecifiedRepresentation) && !typeof(Uri).Equals(componentType)
                || Representation.Inline.Equals(userSpecifiedRepresentation)
                && TYPE_TO_VALUE_TYPE.ContainsKey(componentType))
            {
                throw new OslcCoreInvalidRepresentationException(resourceType, method, userSpecifiedRepresentation);
            }
        }

        private static void ValidateUserSpecifiedValueType(
            Type resourceType,
            MethodInfo method,
            ValueType userSpecifiedValueType,
            Type componentType)
        {
            var calculatedValueType = TYPE_TO_VALUE_TYPE[componentType];

            // If user-specified value type matches calculated value type
            // or
            // user-specified value type is local resource (we will validate the local resource later)
            // or
            // user-specified value type is xml literal and calculated value type is string
            // or
            // user-specified value type is decimal and calculated value type is numeric
            if (userSpecifiedValueType.Equals(calculatedValueType)
                || ValueType.LocalResource.Equals(userSpecifiedValueType)
                || ValueType.XMLLiteral.Equals(userSpecifiedValueType) && ValueType.String.Equals(calculatedValueType)
                || ValueType.Decimal.Equals(userSpecifiedValueType)
                && (ValueType.Double.Equals(calculatedValueType) || ValueType.Float.Equals(calculatedValueType)
                                                                 || ValueType.Integer.Equals(calculatedValueType)))
            {
                // We have a valid user-specified value type for our Java type
                return;
            }

            throw new OslcCoreInvalidValueTypeException(resourceType, method, userSpecifiedValueType);
        }
    }
}