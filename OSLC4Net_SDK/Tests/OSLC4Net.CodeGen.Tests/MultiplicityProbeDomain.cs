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

namespace OSLC4Net.CodeGen.Tests;

internal static class MultiplicityUris
{
    public const string Vocabulary = "https://example.test/oslc/multiplicity#";
    public const string Shape =
        "https://example.test/oslc/multiplicity/shapes#MultiplicityProbeShape";
    public const string ShapeTitle = "Constant title";
}

[OslcVocabulary(MultiplicityUris.Vocabulary)]
public static partial class MultiplicityVocabulary;

[OslcShape(MultiplicityUris.Shape)]
public partial record MultiplicityProbe;

[OslcShape(MultiplicityUris.Shape, Title = MultiplicityUris.ShapeTitle)]
public partial record ConstantAttributeProbe;
