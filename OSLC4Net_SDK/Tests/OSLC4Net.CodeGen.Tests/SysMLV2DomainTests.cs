// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Domains.SysMLV2;

namespace OSLC4Net.CodeGen.Tests;

public sealed class SysMLV2DomainTests
{
    [Test]
    public async Task GeneratedRecordsUseSysMLSuperclass()
    {
        await Assert.That(typeof(AcceptActionUsage).BaseType).IsEqualTo(typeof(ActionUsage));
    }
}
