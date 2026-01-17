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

public abstract record AbstractResourceRecord : IExtendedResource
{
    public Uri About { get; set; }

    public List<Uri> Types { get; private set; } = new();

    public IDictionary<QName, object> ExtendedProperties { get; private set; } =
        new Dictionary<QName, object>();

    protected AbstractResourceRecord(Uri about)
    {
        About = about;
    }

    protected AbstractResourceRecord()
    {
    }

    /// <inheritdoc cref="IResource.GetAbout" />
    public Uri GetAbout()
    {
        return About;
    }

    /// <inheritdoc cref="IResource.SetAbout" />
    public void SetAbout(Uri about)
    {
        About = about;
    }

    /// <inheritdoc cref="IExtendedResource.GetTypes" />
    [OslcDescription("The resource type URIs.")]
    [OslcName("type")]
    [OslcPropertyDefinition(OslcConstants.RDF_NAMESPACE + "type")]
    [OslcTitle("Types")]
    public ICollection<Uri> GetTypes()
    {
        return Types;
    }

    /// <inheritdoc cref="IExtendedResource.SetTypes" />
    public void SetTypes(ICollection<Uri> types)
    {
        Types = new List<Uri>(types);
    }

    /// <inheritdoc cref="IExtendedResource.AddType" />
    public void AddType(Uri type)
    {
        Types.Add(type);
    }

    /// <inheritdoc cref="IExtendedResource.SetExtendedProperties" />
    public void SetExtendedProperties(IDictionary<QName, object> properties)
    {
        // must be implemented this way due to how DotNetRdfHelper works - do not Clear+AddRange
        ExtendedProperties = properties;
    }

    /// <inheritdoc cref="IExtendedResource.GetExtendedProperties" />
    public IDictionary<QName, object> GetExtendedProperties()
    {
        return ExtendedProperties;
    }
}
