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

namespace OSLC4Net.Client.Oslc.Resources;

[OslcNamespace(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE)]
[OslcResourceShape(title = "Requirement Collection Resource Shape", describes = new string[] { RmConstants.TYPE_REQUIREMENT_COLLECTION })]
public class RequirementCollection : Requirement
{
    // The only extra field is uses
    private readonly ISet<Uri> uses = new HashSet<Uri>(); // XXX - TreeSet<> in Java

    public RequirementCollection() : base()
    {
        AddRdfType(new Uri(RmConstants.TYPE_REQUIREMENT_COLLECTION));
    }

    public RequirementCollection(Uri about) : base(about)
    {

        AddRdfType(new Uri(RmConstants.TYPE_REQUIREMENT_COLLECTION));
    }

    public void AddUses(Uri uses)
    {
        this.uses.Add(uses);
    }

    [OslcDescription("A collection uses a resource - the resource is in the requirement collection.")]
    [OslcName("uses")]
    [OslcPropertyDefinition(RmConstants.REQUIREMENTS_MANAGEMENT_NAMESPACE + "uses")]
    [OslcRange(RmConstants.TYPE_REQUIREMENT)]
    [OslcTitle("Uses")]
    public Uri[] GetUses()
    {
        return uses.ToArray();
    }

    public void SetUses(Uri[] uses)
    {
        this.uses.Clear();

        if (uses != null)
        {
            this.uses.AddAll(uses);
        }
    }
}
