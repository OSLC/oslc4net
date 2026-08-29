---
title: Delegated UI and Resource Preview
description: Implement OSLC selection dialogs and Compact resource previews with OSLC4Net.
---

# Implement Delegated UI (Selection Dialog) and Resource Preview (Compact + HTML)

Delegated UI lets a consumer embed a provider’s selection interface. Resource Preview provides lightweight summaries (Compact) plus HTML previews.

## Selection Dialog Basics

Create a `Dialog` attached to a `Service`:
```csharp
var selectionDialog = new Dialog();
selectionDialog.SetTitle("Requirement Selection Dialog");
selectionDialog.SetLabel("Select Requirement");
selectionDialog.SetDialog(new Uri($"{baseUrl}/oslc/service_provider/{documentMid}/requirements/selector"));
selectionDialog.SetHintWidth("500px");
selectionDialog.SetHintHeight("500px");
selectionDialog.SetResourceTypes([ new Uri("http://open-services.net/ns/rm#Requirement") ]);
service.SetSelectionDialogs([ selectionDialog ]);
```

Hints guide iframe sizing; keep dimensions moderate.

## Compact Resource Construction

When a consumer requests the Compact representation of a resource, you should
serve it based on the client's preferred content format (Content Negotiation).
OSLC Core 3.0 Resource Preview Spec defines two main formats: RDF-native (e.g.,
Turtle, JSON-LD, RDF/XML) and plain JSON.

### OSLC Compact resource (RDF-native)

If the client requests an RDF-friendly representation (like `text/turtle`,
`application/ld+json`, or `application/rdf+xml`), initialize the SDK's
`Compact` model directly via its constructor and assign properties directly:

```csharp
using OSLC4Net.Core.Model;

var compactResource = new Compact(new Uri($"{requirementUri}&compact"))
{
    Title = requirement.Title ?? requirement.Identifier ?? "",
    ShortTitle = requirement.Identifier ?? "",
    Icon = new Uri(iconUri),
    IconTitle = "Requirement",
    IconAltLabel = "Requirement",
    SmallPreview = new Preview
    {
        Document = new Uri(smallDocUri),
        HintWidth = "320px",
        HintHeight = "200px"
    },
    LargePreview = new Preview
    {
        Document = new Uri(largeDocUri),
        HintWidth = "600px",
        HintHeight = "400px"
    }
};

return Ok(compactResource);
```

### Plain JSON
If the client requests `application/json`, return the modern camelCase-mapped `CompactDto` which matches the OSLC 3.0 plain JSON schema shape:

```csharp
using OSLC4Net.Core.Model;

var compactDto = new CompactDto
{
    Title = requirement.Title ?? requirement.Identifier ?? "",
    ShortTitle = requirement.Identifier ?? "",
    Icon = new Uri(iconUri),
    IconTitle = "Requirement",
    IconAltLabel = "Requirement",
    SmallPreview = new PreviewDto
    {
        Document = new Uri(smallDocUri),
        HintWidth = "320px",
        HintHeight = "200px"
    },
    LargePreview = new PreviewDto
    {
        Document = new Uri(largeDocUri),
        HintWidth = "600px",
        HintHeight = "400px"
    }
};

return new JsonResult(compactDto);
```

## Content Negotiation & Link Header

When serving the main resource representation, you must advertise the
availability of the Compact representation by appending a `Link` header. You
should also return a `Vary` header containing `Accept` to allow proper HTTP
caching across different formats.

```csharp
[HttpGet]
[Route("/requirements/{id}")]
public async Task<IActionResult> GetRequirement(string id)
{
    var requirement = await _service.GetRequirementAsync(id);

    if (Request.Query.ContainsKey("compact"))
    {
        // Handle Compact representation request based on Accept headers:
        var accept = Request.Headers.Accept.ToString();
        Response.Headers.Vary = "Accept";

        // poor man's content negotiation
        var wantsJson = string.IsNullOrWhiteSpace(accept) || accept.Contains("application/json");
        var prefersRdf = accept.Contains("text/turtle") || accept.Contains("application/rdf+xml");

        if (wantsJson && !prefersRdf)
        {
            // Serve JSON CompactDto
            return new JsonResult(CreateCompactDto(requirement));
        }
        else
        {
            // Serve RDF Compact model
            return Ok(CreateCompactRdfModel(requirement));
        }
    }

    // Serve full resource and attach the Link header for the compact resource
    var requirementUri = $"{Request.Scheme}://{Request.Host}{Request.Path}";
    Response.Headers.Append("Link", $"<{requirementUri}?compact>; rel=\"{OslcConstants.OSLC_CORE_NAMESPACE}Compact\"");

    return Ok(requirement);
}
```
