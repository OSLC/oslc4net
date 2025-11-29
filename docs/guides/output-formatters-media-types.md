---
title: Output Formatters and Media Types
description: Configure OSLC RDF output and content negotiation strategies in OSLC4Net.
---

# Output Formatters & Media Types

OSLC relies on RDF (RDF/XML, Turtle, JSON-LD) plus auxiliary JSON formats (compact). Proper negotiation enables broad client interoperability.

## Register the RDF Output Formatter

Insert OSLC formatter at the start to ensure precedence:

```csharp
builder.Services.AddControllers(o => o.OutputFormatters.Insert(0, new OslcRdfOutputFormatter()));
```

## Controller-Level Produces

Annotate endpoints with supported types:

```csharp
[Produces("application/rdf+xml", "text/turtle", "application/ld+json")]
```

Add `application/json` when returning Compact JSON or selection results.

## Accept Header Handling

Pattern:
1. Inspect `Request.Headers.Accept`.
2. Choose specialized JSON compact shape if explicitly prefers JSON and no RDF types.
3. Return 406 if request demands an unsupported representation (e.g. `text/html` for Compact).

## Vary Header

Set `Vary: Accept` when multiple representations exist for the same URI to prevent cache confusion.

```csharp
Response.Headers.Vary = "Accept";
```

## Link Header for Resource Preview

Expose preview equivalence:
```csharp
Response.Headers.Append("Link", $"<{compactUri}>; rel=\"{OslcConstants.OSLC_CORE_NAMESPACE}Compact\"");
```

## Choosing Default Format

If no Accept header provided, return RDF/XML or JSON-LD consistently. Prefer a single deterministic default. Note that legacy OSLC systems generally prefer RDF/XML.
