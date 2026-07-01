// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

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
}
