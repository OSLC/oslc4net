/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using OSLC4Net.Domains.KerML;

namespace OSLC4Net.CodeGen.Tests;

public sealed class KerMLDomainTests
{
    [Test]
    public async Task GeneratedRecordsImplementInterfacesForAllDirectKerMLSuperclasses()
    {
        await Assert.That(typeof(IExtendedResource).IsAssignableFrom(typeof(IElement))).IsTrue();
        await Assert.That(typeof(IExtendedResource).IsAssignableFrom(typeof(IAssociation))).IsTrue();
        await Assert.That(typeof(IAssociation).IsAssignableFrom(typeof(Association))).IsTrue();
        await Assert.That(typeof(IClassifier).IsAssignableFrom(typeof(Association))).IsTrue();
        await Assert.That(typeof(IRelationship).IsAssignableFrom(typeof(Association))).IsTrue();

        await Assert.That(typeof(IFlow).IsAssignableFrom(typeof(Flow))).IsTrue();
        await Assert.That(typeof(IConnector).IsAssignableFrom(typeof(Flow))).IsTrue();
        await Assert.That(typeof(IStep).IsAssignableFrom(typeof(Flow))).IsTrue();
    }

    [Test]
    public async Task IdenticalInheritedRdfPropertiesAreDeclaredOnce()
    {
        const string sourceProperty = "https://www.omg.org/spec/kerml/vocabulary#source";
        var sourceProperties = typeof(AssociationStructure)
            .GetProperties()
            .Where(property =>
                (Attribute.GetCustomAttribute(property, typeof(OslcPropertyDefinition))
                    as OslcPropertyDefinition)
                    ?.value == sourceProperty
            )
            .ToArray();

        await Assert.That(sourceProperties.Length).IsEqualTo(1);
        await Assert.That(sourceProperties[0].DeclaringType).IsEqualTo(typeof(Association));
        await Assert.That(sourceProperties[0].Name).IsEqualTo(nameof(Association.SourceKerml));
        await Assert
            .That(typeof(Association).GetProperty(nameof(Element.SourceDcterms))?.DeclaringType)
            .IsEqualTo(typeof(Element));
    }
}
