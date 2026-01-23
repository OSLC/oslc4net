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

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Client.Oslc.Resources;

/// <summary>
/// https://docs.oasis-open-projects.org/oslc-op/qm/v2.1/os/quality-management-spec.html
/// </summary>
public abstract class QmResource : AbstractResource
{

    private DateTime? created;
    private string identifier;
    private Uri instanceShape;
    private DateTime? modified;
    private Uri serviceProvider;
    private string title;

    protected QmResource() : base()
    {
        AddType(GetRdfType());
    }

    protected QmResource(Uri about) : base(about)
    {
        AddType(GetRdfType());
    }

    protected abstract Uri GetRdfType();

    public void AddRdfType(Uri rdfType)
    {
        AddType(rdfType);
    }

    [OslcDescription("Timestamp of resource creation.")]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "created")]
    [OslcReadOnly]
    [OslcTitle("Created")]
    public DateTime? GetCreated()
    {
        return created;
    }

    [OslcDescription(
        "A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "identifier")]
    [OslcReadOnly]
    [OslcTitle("Identifier")]
    public string GetIdentifier()
    {
        return identifier;
    }

    [OslcDescription("Resource Shape that provides hints as to resource property value-types and allowed values. ")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "instanceShape")]
    [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
    [OslcTitle("Instance Shape")]
    public Uri GetInstanceShape()
    {
        return instanceShape;
    }

    [OslcDescription("Timestamp last latest resource modification.")]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "modified")]
    [OslcReadOnly]
    [OslcTitle("Modified")]
    public DateTime? GetModified()
    {
        return modified;
    }

    [Obsolete("User GetTypes() or .Types instead")]
    public Uri[] GetRdfTypes()
    {
        return GetTypes().ToArray();
    }

    [OslcDescription("The scope of a resource is a Uri for the resource's OSLC Service Provider.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "serviceProvider")]
    [OslcRange(OslcConstants.TYPE_SERVICE_PROVIDER)]
    [OslcTitle("Service Provider")]
    public Uri GetServiceProvider()
    {
        return serviceProvider;
    }

    [OslcDescription(
        "Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.Domains.DCTerms.NS + "title")]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle()
    {
        return title;
    }

    public void SetCreated(DateTime? created)
    {
        this.created = created;
    }

    public void SetIdentifier(string identifier)
    {
        this.identifier = identifier;
    }

    public void SetInstanceShape(Uri instanceShape)
    {
        this.instanceShape = instanceShape;
    }

    public void SetModified(DateTime? modified)
    {
        this.modified = modified;
    }

    [Obsolete("User SetTypes() or .Types instead")]
    public void SetRdfTypes(Uri[] rdfTypes)
    {
        SetTypes(rdfTypes);
    }

    public void SetServiceProvider(Uri serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }
}
