/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Model;
using OSLC4Net.Domains.SPDX;

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
    }
}
