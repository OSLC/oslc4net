/*******************************************************************************
 * Copyright (c) 2023 Andrii Berezovskyi and OSLC4Net contributors.
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
///     OSLC Resource Shape resource
/// </summary>
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Creation Factory Resource Shape",
    describes = new[] { OslcConstants.TYPE_CREATION_FACTORY })]
public class CreationFactory : AbstractResource
{
    private readonly SortedSet<Uri> resourceShapes = new SortedUriSet();
    private readonly SortedSet<Uri> resourceTypes = new SortedUriSet();
    private readonly SortedSet<Uri> usages = new SortedUriSet();

    private Uri creation;
    private string label;
    private string title;

    public CreationFactory()
    {
    }

    public CreationFactory(string title, Uri creation) : this()
    {
        this.title = title;
        this.creation = creation;
    }

    public void AddResourceShape(Uri resourceShape)
    {
        resourceShapes.Add(resourceShape);
    }

    public void AddResourceType(Uri resourceType)
    {
        resourceTypes.Add(resourceType);
    }

    public void AddUsage(Uri usage)
    {
        usages.Add(usage);
    }

    [OslcDescription("To create a new resource via the factory, post it to this Uri")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "creation")]
    [OslcReadOnly]
    [OslcTitle("Creation")]
    public Uri GetCreation()
    {
        return creation;
    }

    [OslcDescription("Very short label for use in menu items")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "label")]
    [OslcReadOnly]
    [OslcTitle("Label")]
    public string GetLabel()
    {
        return label;
    }

    [OslcDescription(
        "A creation factory may provide resource shapes that describe shapes of resources that may be created")]
    [OslcName("resourceShape")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "resourceShape")]
    [OslcRange(OslcConstants.TYPE_RESOURCE_SHAPE)]
    [OslcReadOnly]
    [OslcTitle("Resource Shapes")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_RESOURCE_SHAPE)]
    public Uri[] GetResourceShapes()
    {
        return resourceShapes.ToArray();
    }

    [OslcDescription(
        "The expected resource type Uri of the resource that will be created using this creation factory. These would be " +
        "the URIs found in the result resource's rdf:type property")]
    [OslcName("resourceType")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "resourceType")]
    [OslcReadOnly]
    [OslcTitle("Resource Types")]
    public Uri[] GetResourceTypes()
    {
        return resourceTypes.ToArray();
    }

    [OslcDescription("Title string that could be used for display")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
    [OslcReadOnly]
    [OslcTitle("Title")]
    [OslcValueType(ValueType.XMLLiteral)]
    public string GetTitle()
    {
        return title;
    }

    [OslcDescription(
        "An identifier Uri for the domain specified usage of this creation factory. If a service provides multiple " +
        "creation factories, it may designate the primary or default one that should be used with a property value of " +
        "https://open-services.net/ns/core#default")]
    [OslcName("usage")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "usage")]
    [OslcReadOnly]
    [OslcTitle("Usages")]
    public Uri[] GetUsages()
    {
        return usages.ToArray();
    }

    public void SetCreation(Uri creation)
    {
        this.creation = creation;
    }

    public void SetLabel(string label)
    {
        this.label = label;
    }

    public void SetResourceShapes(Uri[] resourceShapes)
    {
        this.resourceShapes.Clear();
        if (resourceShapes != null)
        {
            this.resourceShapes.AddAll(resourceShapes);
        }
    }

    public void SetResourceTypes(Uri[] resourceTypes)
    {
        this.resourceTypes.Clear();
        if (resourceTypes != null)
        {
            this.resourceTypes.AddAll(resourceTypes);
        }
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetUsages(Uri[] usages)
    {
        this.usages.Clear();
        if (usages != null)
        {
            this.usages.AddAll(usages);
        }
    }
}
