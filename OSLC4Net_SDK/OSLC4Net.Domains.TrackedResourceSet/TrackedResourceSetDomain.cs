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

namespace OSLC4Net.Domains.TrackedResourceSet;

[OslcVocabulary("http://open-services.net/ns/core/trs#")]
public static partial class TRS;

[OslcShape("http://open-services.net/ns/trs/shapes/3.0#BaseShape")]
public partial record Base;

[OslcShape("http://open-services.net/ns/trs/shapes/3.0#ChangeLogShape")]
public partial record ChangeLog;

[OslcShape("http://open-services.net/ns/trs/shapes/3.0#CreationEventShape")]
public partial record CreationEvent;

[OslcShape("http://open-services.net/ns/trs/shapes/3.0#DeletionEventShape")]
public partial record DeletionEvent;

[OslcShape("http://open-services.net/ns/trs/shapes/3.0#ModificationEventShape")]
public partial record ModificationEvent;

[OslcShape("http://open-services.net/ns/trs/shapes/3.0#TrackedResourceSetShape")]
public partial record TrackedResourceSetResource;
