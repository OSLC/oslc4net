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
using OSLC4Net.Domains.ChangeManagement;
using ChangeManagementTask = OSLC4Net.Domains.ChangeManagement.Task;

namespace OSLC4Net.CodeGen.Tests;

public sealed class ChangeManagementDomainTests
{
    [Test]
    public async System.Threading.Tasks.Task GeneratedRecordsUseVocabularySuperclassWhenAvailable()
    {
        await Assert.That(typeof(ChangeRequest).BaseType).IsEqualTo(typeof(AbstractResourceRecord));
        await Assert.That(typeof(ChangeNotice).BaseType).IsEqualTo(typeof(ChangeRequest));
        await Assert.That(typeof(Defect).BaseType).IsEqualTo(typeof(ChangeRequest));
        await Assert.That(typeof(Enhancement).BaseType).IsEqualTo(typeof(ChangeRequest));
        await Assert.That(typeof(ChangeManagementTask).BaseType).IsEqualTo(typeof(ChangeRequest));
        await Assert.That(typeof(ReviewTask).BaseType).IsEqualTo(typeof(ChangeManagementTask));
    }
}
