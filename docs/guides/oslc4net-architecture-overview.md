---
title: OSLC4Net Architecture Overview
description: High-level structure of the OSLC4Net framework for building OSLC servers and clients.
---

# OSLC4Net Architecture Overview

OSLC4Net provides a focused set of building blocks for implementing OSLC providers and consumers on .NET.

## Layers
- Domain Models: `ChangeRequest`, `Requirement`, dialogs, previews – represent OSLC resources.
- Serialization / Formatters: RDF output formatter integrates with ASP.NET Core to emit RDF/XML, Turtle, JSON-LD.
- Provider Assembly: Controllers define REST endpoints mapping application data → OSLC resources.
- Client Library: `OslcClient` handles authenticated retrieval and wraps responses in `OslcResponse<T>`.

## Discovery Flow (Server Side)

1. OSLC Root Services document. It provides authentication and OSLC service provider catalog links.
2. Catalog lists Service Providers.
3. Service Provider defines services, such as query capabilities, dialogs.
4. Individual services expose the capabilities you are after (search, preview, creation, etc.).

## Resource Representation

- Each resource carries an "about" URI (stable identity, not merely a URL link) and core properties (title, description, additional identifiers besides the URI).
- A subset of properties that are commonly used on given resource types are mapped to C# properties. The remaining properties attach are exposed via "extended properties" found on most resources.

## Delegated UI & Preview

- Selection Dialog: iframe-friendly HTML endpoint.
- Compact Resource: lightweight metadata + small/large preview pointers.
- Link header ties full resource to its Compact representation.

## Extensibility
- Add new domain resources by creating mapping classes and endpoints.
- Introduce additional dialogs (creation, selection) following existing patterns.
