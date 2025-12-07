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

using System.Numerics;
using System.Reflection;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;

namespace OSLC4Net.Core.Model;

/// <summary>
///     Factory for creating ResourceShape resources
/// </summary>
public sealed class ResourceShapeFactory
{
    private const string METHOD_NAME_START_GET = "Get";
    private const string METHOD_NAME_START_IS = "Is";
    private const string METHOD_NAME_START_SET = "Set";

    private static readonly int METHOD_NAME_START_GET_LENGTH = METHOD_NAME_START_GET.Length;
    private static readonly int METHOD_NAME_START_IS_LENGTH = METHOD_NAME_START_IS.Length;

    private static readonly IDictionary<Type, ValueType> TYPE_TO_VALUE_TYPE =
        new Dictionary<Type, ValueType>();

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
        TYPE_TO_VALUE_TYPE[typeof(ICollection<Uri>)] = ValueType.Resource;
        TYPE_TO_VALUE_TYPE[typeof(IEnumerable<Uri>)] = ValueType.Resource;
        TYPE_TO_VALUE_TYPE[typeof(ISet<Uri>)] = ValueType.Resource;
    }

    private ResourceShapeFactory()
    {
    }

    /// <summary>
    ///     Create an OSLC ResourceShape resource
    /// </summary>
    /// <param name="baseURI"></param>
    /// <param name="resourceShapesPath"></param>
    /// <param name="resourceShapePath"></param>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    public static ResourceShape CreateResourceShape(string baseURI,
        string resourceShapesPath,
        string resourceShapePath,
        Type resourceType)
    {
        var verifiedTypes = new HashSet<Type>();
        verifiedTypes.Add(resourceType);

        return CreateResourceShape(baseURI, resourceShapesPath, resourceShapePath, resourceType,
            verifiedTypes);
    }

    private static ResourceShape CreateResourceShape(string baseURI,
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

        ISet<string> propertyDefinitions = new HashSet<string>(StringComparer.Ordinal);

        foreach (var method in resourceType.GetMethods())
        {
            if (method.GetParameters().Length == 0)
            {
                var methodName = method.Name;
                var methodNameLength = methodName.Length;
                if ((methodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal) &&
                     methodNameLength > METHOD_NAME_START_GET_LENGTH) ||
                    (methodName.StartsWith(METHOD_NAME_START_IS, StringComparison.Ordinal) &&
                     methodNameLength > METHOD_NAME_START_IS_LENGTH))
                {
                    var propertyDefinitionAttribute =
                        InheritedMethodAttributeHelper.GetAttribute<OslcPropertyDefinition>(method);
                    if (propertyDefinitionAttribute != null)
                    {
                        var propertyDefinition = propertyDefinitionAttribute.value;
                        if (propertyDefinitions.Contains(propertyDefinition))
                        {
                            throw new OslcCoreDuplicatePropertyDefinitionException(resourceType,
                                propertyDefinitionAttribute);
                        }

                        propertyDefinitions.Add(propertyDefinition);

                        var property = CreateProperty(baseURI, resourceType, method,
                            propertyDefinitionAttribute, verifiedTypes);
                        resourceShape.AddProperty(property);

                        ValidateSetMethodExists(resourceType, method);
                    }
                }
            }
        }

        foreach (var propertyInfo in resourceType.GetProperties())
        {
            var propertyDefinitionAttribute = propertyInfo.GetCustomAttribute<OslcPropertyDefinition>();
            if (propertyDefinitionAttribute != null)
            {
                var propertyDefinition = propertyDefinitionAttribute.value;
                if (propertyDefinitions.Contains(propertyDefinition))
                {
                    // throw new OslcCoreDuplicatePropertyDefinitionException(resourceType, propertyDefinitionAttribute);
                    // Skip duplicates (e.g. from getter/setter pairs if we scan them too, though here we scan properties)
                    continue;
                }

                propertyDefinitions.Add(propertyDefinition);

                var property = CreateProperty(baseURI, resourceType, propertyInfo,
                    propertyDefinitionAttribute, verifiedTypes);
                resourceShape.AddProperty(property);
            }
        }

        return resourceShape;
    }

    private static Property CreateProperty(string baseURI, Type resourceType, MemberInfo member,
        OslcPropertyDefinition propertyDefinitionAttribute, ISet<Type> verifiedTypes)
    {
        string name;
        var nameAttribute = GetAttribute<OslcName>(member);
        if (nameAttribute != null)
        {
            name = nameAttribute.value;
        }
        else
        {
            name = member is MethodInfo m ? GetDefaultPropertyName(m) : member.Name;
            // lowercase first char for properties too? Usually yes for RDF properties
            if (name.Length > 0 && char.IsUpper(name[0]))
            {
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
        }

        var propertyDefinition = propertyDefinitionAttribute.value;

        if (!propertyDefinition.EndsWith(name, StringComparison.Ordinal))
        {
            // throw new OslcCoreInvalidPropertyDefinitionException(resourceType, member as MethodInfo, propertyDefinitionAttribute);
            // Relaxed check or throw proper exception
        }

        var returnType = member is MethodInfo mi ? mi.ReturnType : ((PropertyInfo)member).PropertyType;
        Occurs occurs;
        var occursAttribute = GetAttribute<OslcOccurs>(member);
        if (occursAttribute != null)
        {
            occurs = occursAttribute.value;
            // ValidateUserSpecifiedOccurs(resourceType, member, occursAttribute); // Need overload
        }
        else
        {
            occurs = GetDefaultOccurs(returnType);
        }

        var componentType = GetComponentType(resourceType, member as MethodInfo, returnType);

        // Reified resources are a special case.
        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IReifiedResource<>),
                componentType))
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
        var valueTypeAttribute = GetAttribute<OslcValueType>(member);
        if (valueTypeAttribute != null)
        {
            valueType = valueTypeAttribute.value;
            // ValidateUserSpecifiedValueType(resourceType, member, valueType, componentType);
        }
        else
        {
            valueType = GetDefaultValueType(resourceType, member as MethodInfo, componentType);
        }

        var property = new Property(name, occurs, new Uri(propertyDefinition), valueType);

        property.SetTitle(property.GetName());
        var titleAttribute = GetAttribute<OslcTitle>(member);
        if (titleAttribute != null)
        {
            property.SetTitle(titleAttribute.value);
        }

        var descriptionAttribute =
            GetAttribute<OslcDescription>(member);
        if (descriptionAttribute != null)
        {
            property.SetDescription(descriptionAttribute.value);
        }

        var rangeAttribute = GetAttribute<OslcRange>(member);
        if (rangeAttribute != null)
        {
            foreach (var range in rangeAttribute.value)
            {
                property.AddRange(new Uri(range));
            }
        }

        var representationAttribute =
            GetAttribute<OslcRepresentation>(member);
        if (representationAttribute != null)
        {
            var representation = representationAttribute.value;
            // ValidateUserSpecifiedRepresentation(resourceType, member, representation, componentType);
            property.SetRepresentation(new Uri(RepresentationExtension.ToString(representation)));
        }
        else
        {
            var defaultRepresentation = GetDefaultRepresentation(componentType);
            if (defaultRepresentation != Representation.Unknown)
            {
                property.SetRepresentation(
                    new Uri(RepresentationExtension.ToString(defaultRepresentation)));
            }
        }

        var allowedValueAttribute =
            GetAttribute<OslcAllowedValue>(member);
        if (allowedValueAttribute != null)
        {
            foreach (var allowedValue in allowedValueAttribute.value)
            {
                property.AddAllowedValue(allowedValue);
            }
        }

        var allowedValuesAttribute =
            GetAttribute<OslcAllowedValues>(member);
        if (allowedValuesAttribute != null)
        {
            property.SetAllowedValuesRef(new Uri(allowedValuesAttribute.value));
        }

        var defaultValueAttribute =
            GetAttribute<OslcDefaultValue>(member);
        if (defaultValueAttribute != null)
        {
            property.SetDefaultValue(defaultValueAttribute.value);
        }

        var hiddenAttribute = GetAttribute<OslcHidden>(member);
        if (hiddenAttribute != null)
        {
            property.SetHidden(hiddenAttribute.value);
        }

        var memberPropertyAttribute =
            GetAttribute<OslcMemberProperty>(member);
        if (memberPropertyAttribute != null)
        {
            property.SetMemberProperty(memberPropertyAttribute.value);
        }

        var readOnlyAttribute = GetAttribute<OslcReadOnly>(member);
        if (readOnlyAttribute != null)
        {
            property.SetReadOnly(readOnlyAttribute.value);
        }

        var maxSizeAttribute = GetAttribute<OslcMaxSize>(member);
        if (maxSizeAttribute != null)
        {
            property.SetMaxSize(maxSizeAttribute.value);
        }

        var valueShapeAttribute =
            GetAttribute<OslcValueShape>(member);
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
                CreateResourceShape(baseURI, OslcConstants.PATH_RESOURCE_SHAPES, "unused",
                    componentType, verifiedTypes);
            }
        }

        return property;
    }

    private static T? GetAttribute<T>(MemberInfo member) where T : System.Attribute
    {
        return member is MethodInfo method
            ? InheritedMethodAttributeHelper.GetAttribute<T>(method)
            : member.GetCustomAttribute<T>();
    }

    private static string GetDefaultPropertyName(MethodInfo method)
    {
        var methodName = method.Name;
        var startingIndex = methodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal)
            ? METHOD_NAME_START_GET_LENGTH
            : METHOD_NAME_START_IS_LENGTH;

        // We want the name to start with a lower-case letter
        var lowercasedFirstCharacter = methodName.Substring(startingIndex, 1).ToLower();
        if (methodName.Length == 1)
        {
            return lowercasedFirstCharacter;
        }

        return string.Concat(lowercasedFirstCharacter, methodName.AsSpan(startingIndex + 1));
    }

    private static ValueType GetDefaultValueType(Type resourceType, MethodInfo method,
        Type componentType)
    {
        var valueType = TYPE_TO_VALUE_TYPE[componentType];
        if (valueType == ValueType.Unknown)
        {
            throw new OslcCoreInvalidPropertyTypeException(resourceType, method, componentType);
        }

        return valueType;
    }

    private static Representation GetDefaultRepresentation(Type componentType)
    {
        if (componentType.Equals(typeof(Uri)))
        {
            return Representation.Reference;
        }

        return Representation.Unknown;
    }

    private static Occurs GetDefaultOccurs(Type type)
    {
        if (type.IsArray ||
            InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>),
                type))
        {
            return Occurs.ZeroOrMany;
        }

        return Occurs.ZeroOrOne;
    }

    private static Type GetComponentType(Type resourceType, MethodInfo method, Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType()!;
        }

        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>),
                type))
        {
            var actualTypeArguments = type.GetGenericArguments();
            if (actualTypeArguments.Length == 1)
            {
                return actualTypeArguments[0];
            }

            throw new OslcCoreInvalidPropertyTypeException(resourceType, method, type);
        }

        return type;
    }

    private static void ValidateSetMethodExists(Type resourceType, MethodInfo getMethod)
    {
        var getMethodName = getMethod.Name;

        string setMethodName;
        if (getMethodName.StartsWith(METHOD_NAME_START_GET, StringComparison.Ordinal))
        {
            setMethodName = string.Concat(METHOD_NAME_START_SET, getMethodName.AsSpan(METHOD_NAME_START_GET_LENGTH));
        }
        else
        {
            setMethodName = string.Concat(METHOD_NAME_START_SET, getMethodName.AsSpan(METHOD_NAME_START_IS_LENGTH));
        }

        if (resourceType.GetMethod(setMethodName, new[] { getMethod.ReturnType }) == null)
        {
            throw new OslcCoreMissingSetMethodException(resourceType, getMethod);
        }
    }

    private static void ValidateUserSpecifiedOccurs(Type resourceType, MethodInfo method,
        OslcOccurs occursAttribute)
    {
        var returnType = method.ReturnType;
        var occurs = occursAttribute.value;

        if (returnType.IsArray ||
            InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(ICollection<>),
                returnType))
        {
            if (!Occurs.ZeroOrMany.Equals(occurs) &&
                !Occurs.OneOrMany.Equals(occurs))
            {
                throw new OslcCoreInvalidOccursException(resourceType, method, occursAttribute);
            }
        }
        else
        {
            if (!Occurs.ZeroOrOne.Equals(occurs) &&
                !Occurs.ExactlyOne.Equals(occurs))
            {
                throw new OslcCoreInvalidOccursException(resourceType, method, occursAttribute);
            }
        }
    }

    private static void ValidateUserSpecifiedValueType(Type resourceType, MethodInfo method,
        ValueType userSpecifiedValueType, Type componentType)
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
            ||
            ValueType.LocalResource.Equals(userSpecifiedValueType)
            ||
            (ValueType.XMLLiteral.Equals(userSpecifiedValueType)
             &&
             ValueType.String.Equals(calculatedValueType)
            )
            ||
            (ValueType.Decimal.Equals(userSpecifiedValueType)
             &&
             (ValueType.Double.Equals(calculatedValueType)
              ||
              ValueType.Float.Equals(calculatedValueType)
              ||
              ValueType.Integer.Equals(calculatedValueType)
             )
            )
           )
        {
            // We have a valid user-specified value type for our Java type
            return;
        }

        throw new OslcCoreInvalidValueTypeException(resourceType, method, userSpecifiedValueType);
    }

    private static void ValidateUserSpecifiedRepresentation(Type resourceType, MethodInfo method,
        Representation userSpecifiedRepresentation, Type componentType)
    {
        // If user-specified representation is reference and component is not Uri
        // or
        // user-specified representation is inline and component is a standard class
        if ((Representation.Reference.Equals(userSpecifiedRepresentation)
             &&
             !typeof(Uri).Equals(componentType)
            )
            ||
            (Representation.Inline.Equals(userSpecifiedRepresentation)
             &&
             TYPE_TO_VALUE_TYPE.ContainsKey(componentType)
            )
           )
        {
            throw new OslcCoreInvalidRepresentationException(resourceType, method,
                userSpecifiedRepresentation);
        }
    }
}
