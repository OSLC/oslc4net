/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Attribute;
using OSLC4Net.Domains.ConfigurationManagement;

namespace OSLC4Net.Server.Providers.Tests;

public sealed class ConfigurationManagementDomainTests
{
    [Test]
    public async Task IdenticalInheritedPropertiesAreDeclaredOnce()
    {
        System.Reflection.PropertyInfo[] selectsProperties = typeof(ChangeSetSelections)
            .GetProperties()
            .Where(property =>
                (
                    Attribute.GetCustomAttribute(property, typeof(OslcPropertyDefinition))
                    as OslcPropertyDefinition
                )?.value == "http://open-services.net/ns/config#selects"
            )
            .ToArray();

        await Assert.That(selectsProperties).HasSingleItem();
        await Assert.That(selectsProperties[0].DeclaringType).IsEqualTo(typeof(Selections));
    }
}
