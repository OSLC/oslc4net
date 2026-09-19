/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Query;
using SharpFuzz;

namespace OSLC4Net.Fuzzing;

internal static class Program
{
    private static readonly IDictionary<string, string> Prefixes = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["dcterms"] = "http://purl.org/dc/terms/",
        ["oslc"] = "http://open-services.net/ns/core#",
        ["oslc_config"] = "http://open-services.net/ns/config#",
        ["qm"] = "http://qm.example.com/ns/",
        ["rdf"] = "http://www.w3.org/1999/02/22-rdf-syntax-ns#",
        ["xs"] = "http://www.w3.org/2001/XMLSchema"
    };

    private static void Main()
    {
        Fuzzer.Run(FuzzQueryParsers);
    }

    private static void FuzzQueryParsers(Stream input)
    {
        using StreamReader reader = new(input);
        string expression = reader.ReadToEnd();

        TryParse(() => QueryUtils.ParsePrefixes(expression));
        TryParse(() => QueryUtils.ParseWhere(expression, Prefixes));
        TryParse(() => QueryUtils.ParseSelect(expression, Prefixes));
        TryParse(() => QueryUtils.parseProperties(expression, Prefixes));
        TryParse(() => QueryUtils.ParseOrderBy(expression, Prefixes));
        TryParse(() => QueryUtils.ParseSearchTerms(expression));
        TryParse(() => QueryUtils.InvertSelectedProperties(QueryUtils.ParseSelect(expression, Prefixes)));
    }

    private static void TryParse(Action parse)
    {
        try
        {
            parse();
        }
        catch (ParseException)
        {
            // Invalid query syntax is expected during fuzzing.
        }
    }
}
