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

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

/// <summary>
///     A resource that can hold unknown properties and content. If a setter is not
///     found for a property when reading a resource, it is added as an extended
///     property. These extended properties are preserved when writing the resource
///     back out, for instance on a PUT request. In OSLC, clients MUST preserve
///     unknown content when performing updates of resources.
///     see
///     <a
///         href="https://docs.oasis-open-projects.org/oslc-op/core/v3.0/os/oslc-core.html">
///         OSLC
///         Core 3.0: Unknown properties and content
///     </a>
/// </summary>
public interface IExtendedResource : IResource
{
    /// <summary>
    ///     Gets the RDF types of this resource. These types will be added to the
    ///     serialization of the resource in addition to the
    ///     OslcResourceShape#describes() annotation
    /// </summary>
    /// <returns></returns>
    [OslcDescription("The resource type URIs.")]
    [OslcName("type")]
    [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
    [OslcTitle("Types")]
    [Obsolete("Use .Types property instead")]
    ICollection<Uri> GetTypes();

    /// <summary>
    ///     Sets the RDF types of this resource. These types will be added to the
    ///     serialization of the resource in addition to the
    ///     OslcResourceShape#describes() annotation.
    /// </summary>
    /// <param name="types"></param>
    [Obsolete("Use .Types property instead")]
    void SetTypes(ICollection<Uri> types);

    /// <summary>
    ///     Gets or sets the RDF types of this resource. These types will be added to the
    ///     serialization of the resource in addition to the
    ///     OslcResourceShape#describes() annotation.
    /// </summary>
    ICollection<Uri> Types { get; set; }

    /// <summary>
    ///     Adds an RDF type to this resource. These types will be added to the
    ///     serialization of the resource in addition to the
    ///     OslcResourceShape#describes() annotation.
    /// </summary>
    /// <param name="type"></param>
    void AddType(Uri type);

    /// <summary>
    ///     Sets extended properties not defined in the bean.
    /// </summary>
    /// <param name="properties"></param>
    void SetExtendedProperties(IDictionary<QName, object> properties);

    /// <summary>
    ///     Gets back the list of extended properties not defined in this bean.
    /// </summary>
    /// <returns></returns>
    IDictionary<QName, object> GetExtendedProperties();
}
