---
name: add-oslc-domain
description: Add or update an OSLC4Net generated domain project from RDF vocabulary and optional OSLC ResourceShape inputs. Use for OSLC4Net domain additions, generated domain seed files, vocab.nt/shapes.nt resources, converting Turtle/RDF/XML/JSON-LD RDF inputs to N-Triples, wiring the domain into OSLC4Net.Core.slnx and docfx, and validating generated vocabulary or shape code.
---

# Add OSLC Domain

Use this skill inside the OSLC4Net repository when adding a generated domain under `OSLC4Net_SDK/OSLC4Net.Domains.*`.

## Workflow

1. Inspect existing generated domains such as `OSLC4Net.Domains.KerML`, `OSLC4Net.Domains.SysMLV2`, and `OSLC4Net.Domains.QualityManagement`.
2. Validate every RDF input before conversion:

```bash
riot --validate INPUT.ttl
riot --count INPUT.ttl
```

3. Convert non-N-Triples inputs to repo-local N-Triples resources because the source generator consumes `.nt` `AdditionalFiles`:

```bash
mkdir -p OSLC4Net_SDK/OSLC4Net.Domains.NAME/Resources
riot --output=ntriples INPUT.ttl > OSLC4Net_SDK/OSLC4Net.Domains.NAME/Resources/vocab.nt
```

Use `riot --formatted=turtle` only for human inspection. Commit the `.nt` file used by the analyzer, not an extra `.ttl` copy.

4. If shapes are available, validate and convert them separately to `Resources/shapes.nt`. If the input has no `oslc:ResourceShape` triples or the user says to skip shapes, create a vocabulary-only domain and omit `shapes.nt`.

5. Generate the seed declaration:

```bash
scripts/oslc_domain_seed_gen.py \
  --namespace OSLC4Net.Domains.NAME \
  --vocabulary-class NAMEVocabulary \
  --vocabulary-uri VOCABULARY_URI \
  --vocabulary-prefix PREFIX \
  --output OSLC4Net_SDK/OSLC4Net.Domains.NAME/NAMEDomain.cs
```

Add `--vocabulary-prefix` when the RDF does not provide `vann:preferredNamespacePrefix`, or when the preferred prefix should differ from the generated class name. Add `--shapes OSLC4Net_SDK/OSLC4Net.Domains.NAME/Resources/shapes.nt` only when `shapes.nt` exists.

For ontologies with top-level module paths, prefer multiple vocabulary classes with module-scoped namespace URIs instead of one root vocabulary. For example, SPDX uses terms under `https://spdx.org/rdf/3/terms/Core/` and `https://spdx.org/rdf/3/terms/Software/`, so declare separate classes:

```csharp
[OslcVocabulary("https://spdx.org/rdf/3/terms/Core/", "spdx")]
public static partial class SpdxCore;

[OslcVocabulary("https://spdx.org/rdf/3/terms/Software/", "spdx_software")]
public static partial class SpdxSoftware;
```

Keep the trailing slash when constants are generated as `NS + "localName"`.

6. Add `OSLC4Net_SDK/OSLC4Net.Domains.NAME/OSLC4Net.Domains.NAME.csproj` with:
   - `TargetFramework` `net10.0`
   - `AdditionalFiles` for `Resources\vocab.nt`
   - `AdditionalFiles` for `Resources\shapes.nt` only when shapes exist
   - project references to `OSLC4Net.Core` and `OSLC4Net.CodeGen` as an analyzer.

7. Wire the project into `OSLC4Net_SDK/OSLC4Net.Core.slnx` and `docs/docfx.json`.
8. Add focused TUnit coverage in `Tests/OSLC4Net.CodeGen.Tests` when generator behavior or a real vocabulary constant needs protection. Prefer assertions on generated constants, `QName`s, shape metadata, or round-tripping behavior.
9. Run formatting and validation from the required directories:

```bash
dotnet format whitespace ./OSLC4Net_SDK
dotnet format style ./OSLC4Net_SDK --no-restore
cd OSLC4Net_SDK
export AGENT_BUILD=true
dotnet test --solution OSLC4Net.Core.slnx --configuration Release --treenode-filter '/*/OSLC4Net.CodeGen.Tests/*/*'
dotnet build OSLC4Net.Core.slnx
cd ../docs
docfx metadata docfx.json
docfx build docfx.json
```

## RDF Handling

Use Apache Jena CLI tools directly:

- `riot --validate FILE` proves RDF syntax, not ontology consistency.
- `riot --count FILE` confirms parseable triple count.
- `riot --output=ntriples INPUT > output.nt` produces analyzer-ready N-Triples.
- `arq --data=FILE --query=query.rq` inspects classes, properties, ontology URI, preferred prefix, and whether OSLC shapes exist.
- `rdfcompare left.nt right.ttl` compares graphs structurally when checking a conversion.

Useful discovery queries:

```sparql
PREFIX owl: <http://www.w3.org/2002/07/owl#>
PREFIX vann: <http://purl.org/vocab/vann/>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
SELECT ?ontology ?prefix ?label WHERE {
  ?ontology a owl:Ontology .
  OPTIONAL { ?ontology vann:preferredNamespacePrefix ?prefix }
  OPTIONAL { ?ontology rdfs:label ?label }
}
```

```sparql
PREFIX oslc: <http://open-services.net/ns/core#>
SELECT ?shape ?describes WHERE {
  ?shape a oslc:ResourceShape ;
         oslc:describes ?describes .
}
ORDER BY ?shape
```

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX owl: <http://www.w3.org/2002/07/owl#>
SELECT (COUNT(DISTINCT ?term) AS ?terms) WHERE {
  { ?term a rdfs:Class } UNION { ?term a owl:Class }
  UNION { ?term a rdf:Property } UNION { ?term a owl:ObjectProperty }
  UNION { ?term a owl:DatatypeProperty } UNION { ?term a owl:AnnotationProperty }
}
```

```sparql
PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX owl: <http://www.w3.org/2002/07/owl#>
SELECT ?term WHERE {
  { ?class a rdfs:Class } UNION { ?class a owl:Class }
  BIND(?class AS ?term)
}
ORDER BY ?term
LIMIT 40
```

## Repository Rules

- Do not patch upstream vocabulary or shape triples for spelling, labels, ranges, or modeling corrections. Prefer code-side handling and document source defects upstream.
- Keep generated domain resources under `OSLC4Net_SDK/OSLC4Net.Domains.NAME/Resources`.
- Remove temporary source RDF files after converting them to the committed `.nt` resources unless the source file is intentionally curated repo documentation.
- Do not inline `AGENT_BUILD`; export it after changing into `OSLC4Net_SDK`.
- For noteworthy new domains, add documentation under `docs/articles` only when there is user-facing guidance beyond API metadata.
