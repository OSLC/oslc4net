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

namespace OSLC4Net.Core.Model;

/// <summary>
///     This class represents and abstract OSLC resource.  It is normally the parent class for concrete
///     OSLC resource types.
///     See the ChangeMangement.cs objec in the ChangeManagementCommon project
/// </summary>
public abstract class AbstractResource : IExtendedResource
{
    private IDictionary<QName, object> extendedProperties = new Dictionary<QName, object>();
    private ICollection<Uri> types = new List<Uri>();

    protected AbstractResource(Uri about)
    {
        About = about;
    }

    protected AbstractResource()
    {
    }

    public Uri About { get; set; }

    /// <summary>
    ///     Get the subject URI
    /// </summary>
    /// <returns></returns>
    public Uri GetAbout()
    {
        return About;
    }

    /// <summary>
    ///     Set the subject URI
    /// </summary>
    /// <param name="about"></param>
    public void SetAbout(Uri about)
    {
        About = about;
    }

    /// <param name="properties"></param>
    public void SetExtendedProperties(IDictionary<QName, object> properties)
    {
        extendedProperties = properties;
    }

    /// <summary>
    ///     Get all extended properties
    /// </summary>
    /// <returns></returns>
    public IDictionary<QName, object> GetExtendedProperties()
    {
        return extendedProperties;
    }

    /// <summary>
    ///     Get the RDF types
    /// </summary>
    /// <returns></returns>
    public ICollection<Uri> GetTypes()
    {
        return types;
    }

    /// <summary>
    ///     Set the RDF types
    /// </summary>
    /// <param name="types"></param>
    public void SetTypes(ICollection<Uri> types)
    {
        this.types = types;
    }

    /// <summary>
    ///     Add an additional RDF type
    /// </summary>
    /// <param name="type"></param>
    public void AddType(Uri type)
    {
        types.Add(type);
    }
}
