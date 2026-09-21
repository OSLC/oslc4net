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
        ["dcterms"] = "http://purl.org/dc/terms/", // NOSONAR: RDF namespace identifier, not a network endpoint.
        ["oslc"] = "http://open-services.net/ns/core#", // NOSONAR: RDF namespace identifier, not a network endpoint.
        ["oslc_config"] = "http://open-services.net/ns/config#", // NOSONAR: RDF namespace identifier, not a network endpoint.
        ["qm"] = "http://qm.example.com/ns/", // NOSONAR: test namespace identifier, not a network endpoint.
        ["rdf"] = "http://www.w3.org/1999/02/22-rdf-syntax-ns#", // NOSONAR: RDF namespace identifier, not a network endpoint.
        ["xs"] = "http://www.w3.org/2001/XMLSchema" // NOSONAR: RDF namespace identifier, not a network endpoint.
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
        TryParse(() => QueryUtils.ParseProperties(expression, Prefixes));
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
