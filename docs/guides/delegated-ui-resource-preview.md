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

Two paths based on Accept:
- JSON compact shape (Appendix A OSLC 3.0).
- RDF-compatible `Compact` object.

JSON example:
```csharp
var compactDto = new {
  title = requirement.Title,
  icon = iconUri,
  smallPreview = new { document = smallDoc, hintWidth = "320px", hintHeight = "200px" },
  largePreview = new { document = largeDoc, hintWidth = "600px", hintHeight = "400px" }
};
return new JsonResult(compactDto);
```

RDF object example:

```csharp
var compact = new Compact();
// set the URI as per your controller layout
compact.SetAbout(new Uri($"{requirementUri}&compact"));
compact.Title = requirement.Title;
compact.SmallPreview = new Preview { Document = new Uri(smallDoc), HintWidth = "320px", HintHeight = "200px" };
```

## Link Header

Add a `Link` header referencing the Compact representation from the full resource response:

```csharp
Response.Headers.Append("Link", $"<{requirementUri}&compact>; rel=\"{OslcConstants.OSLC_CORE_NAMESPACE}Compact\"");
```
