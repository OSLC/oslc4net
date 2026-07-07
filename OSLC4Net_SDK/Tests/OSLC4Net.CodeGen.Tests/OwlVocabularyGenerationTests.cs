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

namespace OSLC4Net.CodeGen.Tests;

public sealed class OwlVocabularyGenerationTests
{
    [Test]
    public async Task VocabularyConstantsAreGeneratedFromOwlTerms()
    {
        await Assert.That(OwlVocabulary.NS).IsEqualTo("https://example.test/rdf/terms/");
        await Assert.That(OwlVocabulary.Prefix).IsEqualTo("explicit");
        await Assert
            .That(OwlVocabulary.CoreExampleClass)
            .IsEqualTo("https://example.test/rdf/terms/Core/ExampleClass");
        await Assert
            .That(OwlVocabulary.P.CoreRelatedElement)
            .IsEqualTo("https://example.test/rdf/terms/Core/relatedElement");
        await Assert
            .That(OwlVocabulary.Q.CoreName)
            .IsEqualTo(new QName(OwlVocabulary.NS, "Core/name", OwlVocabulary.Prefix));
    }
}
