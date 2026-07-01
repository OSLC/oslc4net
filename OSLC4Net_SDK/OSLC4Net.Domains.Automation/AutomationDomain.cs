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

namespace OSLC4Net.Domains.Automation;

[OslcVocabulary("http://open-services.net/ns/auto#")]
public static partial class Auto;

[OslcShape("http://open-services.net/ns/auto/shapes/2.1#AutomationPlanShape")]
public partial record AutomationPlan;

[OslcShape("http://open-services.net/ns/auto/shapes/2.1#AutomationRequestShape")]
public partial record AutomationRequest;

[OslcShape("http://open-services.net/ns/auto/shapes/2.1#AutomationResultShape")]
public partial record AutomationResult;

[OslcShape("http://open-services.net/ns/auto/shapes/2.1#ParameterInstanceShape")]
public partial record ParameterInstance;
