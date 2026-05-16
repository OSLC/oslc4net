---
title: Extended Properties and Namespaces
description: Add supplemental properties to OSLC resources using stable vocabularies.
---

# Extended Properties & Namespaces

Extended properties enrich resources beyond the base OSLC resource shape. Keep additions purposeful and interoperable by:

- reusing existing vocabularies
- only using existing properties according to their existing semantics

## Adding a Property

```csharp
cr.SetExtendedProperties(new Dictionary<QName, object>{
  [OslcConstants.Domains.RDFS.Q.SeeAlso] = new Uri(issue.HtmlUrl)
});
```

Use `QName` for clarity and to avoid typos. OSLC4Net provides generated vocabs and QName helpers for:

- Dublin Core (both the new DC Terms and the legacy DC Elements)
- RDF(S)
- FOAF
- LDP
- SKOS
- PROV
- QUDT
