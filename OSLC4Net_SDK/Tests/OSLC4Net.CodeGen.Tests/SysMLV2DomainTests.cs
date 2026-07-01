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
using OSLC4Net.Domains.SysMLV2;

namespace OSLC4Net.CodeGen.Tests;

public sealed class SysMLV2DomainTests
{
    [Test]
    public async Task GeneratedRecordsUseSysMLSuperclass()
    {
        await Assert.That(typeof(AcceptActionUsage).BaseType).IsEqualTo(typeof(ActionUsage));
    }

    [Test]
    public async Task GeneratedRecordsImplementInterfacesForAllDirectSysMLSuperclasses()
    {
        await Assert.That(typeof(IExtendedResource).IsAssignableFrom(typeof(IElement))).IsTrue();
        await Assert.That(typeof(IExtendedResource).IsAssignableFrom(typeof(IActionUsage))).IsTrue();
        await Assert.That(typeof(IActionUsage).IsAssignableFrom(typeof(ActionUsage))).IsTrue();
        await Assert.That(typeof(IOccurrenceUsage).IsAssignableFrom(typeof(ActionUsage))).IsTrue();
        await Assert.That(typeof(IStep).IsAssignableFrom(typeof(ActionUsage))).IsTrue();

        await Assert.That(typeof(IFlowUsage).IsAssignableFrom(typeof(FlowUsage))).IsTrue();
        await Assert.That(typeof(IActionUsage).IsAssignableFrom(typeof(FlowUsage))).IsTrue();
        await Assert.That(typeof(IConnectorAsUsage).IsAssignableFrom(typeof(FlowUsage))).IsTrue();
        await Assert.That(typeof(IFlow).IsAssignableFrom(typeof(FlowUsage))).IsTrue();

        await Assert.That(typeof(IPerformActionUsage).IsAssignableFrom(typeof(PerformActionUsage))).IsTrue();
        await Assert.That(typeof(IActionUsage).IsAssignableFrom(typeof(PerformActionUsage))).IsTrue();
        await Assert.That(typeof(IEventOccurrenceUsage).IsAssignableFrom(typeof(PerformActionUsage))).IsTrue();
    }
}
