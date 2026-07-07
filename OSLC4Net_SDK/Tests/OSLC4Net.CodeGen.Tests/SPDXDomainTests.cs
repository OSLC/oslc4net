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
using OSLC4Net.Domains.SPDX.Core;
using OSLC4Net.Domains.SPDX.FunctionalSafety;
using OSLC4Net.Domains.SPDX.Software;

namespace OSLC4Net.CodeGen.Tests;

public sealed class SPDXDomainTests
{
    [Test]
    public async Task CoreVocabularyConstantsAreGeneratedFromSpdxOwlTerms()
    {
        await Assert.That(SpdxCore.NS).IsEqualTo("https://spdx.org/rdf/3/terms/Core/");
        await Assert.That(SpdxCore.Prefix).IsEqualTo("spdx");
        await Assert
            .That(SpdxCore.CreationInfo)
            .IsEqualTo("https://spdx.org/rdf/3/terms/Core/CreationInfo");
        await Assert
            .That(SpdxCore.P.CreatedBy)
            .IsEqualTo("https://spdx.org/rdf/3/terms/Core/createdBy");
        await Assert
            .That(SpdxCore.Q.CreatedBy)
            .IsEqualTo(new QName(SpdxCore.NS, "createdBy", SpdxCore.Prefix));
    }

    [Test]
    public async Task ModuleVocabularyConstantsUseModulePrefixes()
    {
        await Assert.That(SpdxSoftware.NS).IsEqualTo("https://spdx.org/rdf/3/terms/Software/");
        await Assert.That(SpdxSoftware.Prefix).IsEqualTo("spdx_software");
        await Assert
            .That(SpdxSoftware.Package)
            .IsEqualTo("https://spdx.org/rdf/3/terms/Software/Package");
        await Assert
            .That(SpdxSoftware.Q.PackageUrl)
            .IsEqualTo(new QName(SpdxSoftware.NS, "packageUrl", SpdxSoftware.Prefix));
        await Assert
            .That(SpdxFunctionalSafety.EvidenceRelationship)
            .IsEqualTo("https://spdx.org/rdf/3/terms/FunctionalSafety/EvidenceRelationship");
        await Assert
            .That(SpdxFunctionalSafety.Q.EvidenceUID)
            .IsEqualTo(
                new QName(SpdxFunctionalSafety.NS, "evidenceUID", SpdxFunctionalSafety.Prefix)
            );
    }

    [Test]
    public async Task SpdxShaclNodeShapeGeneratesOslcResourceShape()
    {
        OslcResourceShape? shapeAttribute =
            Attribute.GetCustomAttribute(typeof(EvidenceRelationship), typeof(OslcResourceShape))
            as OslcResourceShape;

        OslcNamespace? namespaceAttribute =
            Attribute.GetCustomAttribute(typeof(EvidenceRelationship), typeof(OslcNamespace))
            as OslcNamespace;

        await Assert.That(namespaceAttribute?.value).IsEqualTo(SpdxFunctionalSafety.NS);
        await Assert
            .That(shapeAttribute?.describes)
            .IsEquivalentTo([SpdxFunctionalSafety.EvidenceRelationship]);
        await Assert.That(typeof(EvidenceRelationship).IsSubclassOf(typeof(Relationship))).IsTrue();
    }

    [Test]
    public async Task SpdxShaclPropertyConstraintsMapToOslcPropertyShape()
    {
        ResourceShape shape = ResourceShapeFactory.CreateResourceShape(
            "https://example.test",
            "resourceShapes",
            "evidenceRelationship",
            typeof(EvidenceRelationship)
        );

        Property evidenceUid = shape
            .GetProperties()
            .Single(property =>
                property.GetPropertyDefinition() == new Uri(SpdxFunctionalSafety.P.EvidenceUID)
            );

        await Assert
            .That(evidenceUid.GetOccurs())
            .IsEqualTo(new Uri("http://open-services.net/ns/core#Zero-or-one"));
        await Assert
            .That(evidenceUid.GetValueType())
            .IsEqualTo(new Uri("http://open-services.net/ns/core#AnyResource"));
        await Assert
            .That(evidenceUid.GetRepresentation())
            .IsEqualTo(new Uri("http://open-services.net/ns/core#Either"));
        await Assert
            .That(evidenceUid.GetRange())
            .IsEquivalentTo([new Uri(SpdxCore.ExternalIdentifier)]);
    }
}
