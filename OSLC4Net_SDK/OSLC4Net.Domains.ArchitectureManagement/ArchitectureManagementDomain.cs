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

namespace OSLC4Net.Domains.ArchitectureManagement;

[OslcVocabulary("http://open-services.net/ns/am#")]
public static partial class AM;

[OslcShape("http://open-services.net/ns/am/shapes/3.0#ResourceShape")]
public partial record ArchitectureResource;

[OslcShape("http://open-services.net/ns/am/shapes/3.0#LinkTypeShape")]
public partial record LinkType;
