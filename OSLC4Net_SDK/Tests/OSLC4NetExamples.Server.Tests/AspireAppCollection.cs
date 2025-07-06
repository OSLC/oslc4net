/*******************************************************************************
 * Copyright (c) 2025 OSLC4Net contributors.
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
 *     GitHub Copilot - initial API and implementation
 *******************************************************************************/

using Xunit;

namespace OSLC4NetExamples.Server.Tests;

[CollectionDefinition("AspireApp")]
public class AspireAppCollection : ICollectionFixture<RefimplAspireFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
