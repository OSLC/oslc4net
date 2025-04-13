# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Legend: 🔒️ security fixes; ⚡️ major features/updates; ❗️ breaking changes; 👉 important notes.


## [0.5.0] - 2024-11-15

### Security

This release does not contain security updates.

### Added

- ⚡️ CI integration tests against OSLC RefImpl based on NET Aspire
- Redirect loop protection for `OslcClient` (max 20 redirects as in Firefox/Blink/WebKit)

### Changed

- dotNetRDF dependency was updated to v3.3.1
- ❗️ `OslcClient` was updated to consistently support async operations
- ❗️ `OslcResponse` was updated to expose potentially multiple resources, expose the `Graph` and, in case of error, the `oslc:Error` resource.
- Follow redirects on more responses statuses (was: 301, became: 301, 302, 307, 308, and in case of GET requests, 303 too)


### Deprecated

- Most non-async methods in client classes.
- Further OSLC JSON deprecations. Clients should rely on RDF instead (RDF/XML, Turtle)

### Removed

- ❗️ Multiple non-async methods in `OslcClient`
- ❗️ A property exposing `OslcRestClient` in `ServiceProviderRegistryClient`

### Fixed

- Minor bug fixes to pass acceptance tests against OSLC RefImpl Change Management server.


## [0.4.6] - 2024-11-15

### Security

This release does not contain security updates.

### Added

This release does not contain new features.

### Changed

- Dependency updates in connection with .NET 9 release.

### Deprecated

This release does not introduce deprecations.

### Removed

This release does not remove any features.

### Fixed

- `Requirement.SetDecomposes()` cleared `affectedBy` by mistake.

## [0.4.5] - 2024-10-19

### Security

-  🔒️  Transitive dependency System.Net.Http was set to version 4.3.4 to avoid failing the build when NU1903 is treated as error.
-  🔒️  Transitive dependency System.Text.RegularExpressions was set to version 4.3.1 to avoid failing the build when NU1903 is treated as error.

### Added

This release does not contain new features.

### Changed

- Significant build changes to manage package versions centrally.
- NuGet/assembly versions are now set based on the git tag name.
- 👉 "snapshot" builds how have the version similar to 999.9.9-ts.202410192025, where 202410192025 is a timestamp; please note that such snapshot builds are only available via [Github Packages](https://github.com/orgs/OSLC/packages?repo_name=oslc4net).

### Deprecated

This release does not introduce deprecations.

### Removed

- Dependencies on "bridge" packages added during the migration from .NET Framework to .NET 6:
  - System.Configuration.ConfigurationManager
  - System.Data.DataSetExtensions
  - Microsoft.CSharp

### Fixed

- Example and test project had `<IsPackable>false</IsPackable>` property set to prevent pushing their packages to NuGet.

## [0.4.4] - 2024-10-19

YANKED - deploy to NuGet.org did not succeed.

## [0.4.3] - 2024-10-19

### Security

This release does not contain security updates.

### Added

- ️⚡️ An example project using `OslcClient` and basic auth to retrieve a WorkItem (OSLC ChangeRequest) from Jazz.
- ``OslcClient::ForBasicAuth()`` factory method.
- ️️️⚡️ ``OslcClient.GetResourceAsync()`` strongly typed async method that returns `OslcResponse<T>` with either a typed resource or an error.
- Support for complex MIME type strings for content negotiation. Current `Accept` string is set to ``text/turtle;q=1.0, application/rdf+xml;q=0.9, application/n-triples;q=0.8, text/n3;q=0.7`` by default.
- OSLC Query results now expose a `.TotalCount` property.

### Changed

- ❗️ `OSLC4Net.Client` now requires `netstandard2.1` (was: `netstandard2.0`)
- Upgraded dotNetRDF to [v3.3.0](https://github.com/dotnetrdf/dotnetrdf/releases/tag/v3.3.0)

### Deprecated

- Some constructors on `OslcClient` were deprecated (around skipping TLS checks).
- 👉 log4net logging will be replaced with the standard Microsoft [ILogger](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger?view=net-8.0) in a future release.
- Direct use of `IEnumerator` properties on `OslcQueryResponse` to iterate over response pages.
- Multiple symbols on OSLC Query related code with Java-like signatures (various `Get*` methods) and string types. Prefer C# props of type `Uri`.

### Removed

This release does not remove any features.

### Fixed

- `OslcClient` no longer overwrites most of the headers (#204). It was a similar issue to #19 (but happening with `OslcRestClient`).
- Ensure OSLC Query responses are processed correctly when they contain multiple `oslc:ResponseInfo` objects ([!203](https://github.com/OSLC/oslc4net/pull/203)).


## [0.4.2] - 2024-10-09

### Security

- 🔒️❗️ `OSLC4Net.Client` now defaults to strong TLS certificate checking. Skipping TLS checks now requires explicit configuration.

### Added

- ⚡️ Support for .NET 6+ was added by migrating most of the projects in the solution to target `netstandard2.0`.

### Changed

- ⚡️ `OSLC4Net.Core` now targets `netstandard2.0`, which allows it to  be used under .NET Framework 4.7.2 or higher as well as .NET 6+.
- ⚡️ dotNetRDF was upgraded from v1 to v3 to enable targeting `netstandard2.0`.
  - One of the key breaking changes is that `ITriple` and `INode` no longer have the `.Graph` property.
  - ❗️ This caused breaking changes to some of the method signatures in `OSLC4Net.DotNetRdfProvider` to allow the `IGraph` instance to be passed.
- `OSLC4Net.Client` now targets `netstandard2.0`.
  - ❗️ The main breaking change is the replacement of the legacy `WebRequestHandler` with `HttpClientHandler`. This caused some method/constructor signatures to change.
  - ❗️ `RemoteCertificateValidationCallback` was replaced with a lambda function.
- `OSLC4Net.Query` now targets `netstandard2.0`. The Antlr3 runtime package targeting PCL was replaced with a package targeting NETStandard1.0 (Antlr 4 targets NETStandard2.0).
- ❗️ `JsonMediaTypeFormatter` was renamed into `OslcJsonMediaTypeFormatter` to better reflect its purpose (support a bespoke OSLC JSON format) and avoid conflict with `System.Net.Http.Formatting.MediaTypeFormatter`.
- Test projects were migrated from MSTestV2 to xUnit (except for integration tests for OSLC CM)

### Deprecated

- .NET 6 support is deprecated as the EOL is approaching soon. Given that all
  non-Framework libraries target `netstandard2.0`, this should have no impact
  on the users.

### Removed

- .NET 7 support was removed since the SDK has reached EOL.
- `Newtonsoft.Json` package was only used in the StockQuoteSample ASP.NET MVC project. Its references were removed from all other projects.

### Fixed

This release does not contain bug fixes.

## [0.4.1] - YANKED

YANKED due to NuGet deployment issues.

## [0.4.0] - YANKED

YANKED due to NuGet deployment issues.

## [0.3.0-alpha] - 2023-04-29

### Security

- 🔒️ **Updated `log4net` in response to CVE-2018-1285 (CVSS 9.8/10).**
- 🔒️ **Updated `Newtonsoft.Json` in response to [CWE-755](https://cwe.mitre.org/data/definitions/755.html).**

### Added

- CI configuration based on Github Actions (MSBuild, NuGet, VStest).

### Changed

- 👉 Since 2017-04-15, the project is hosted on Github under the OSLC community org.
- The project now targets .NET 4.8 instead of .NET 4.5.
- ⚡️ Nuget project configuration was migrated from `packages.config` to PackageReference.
- ⚡️ The StockQuoteSample was updated to ASP.NET MVC 5.
- Migrated from MSBuild-based NuGet package restore to NuGet 2.7+ Automatic Package Restore.
- Migrated from MSTestV1 (`Microsoft.VisualStudio.QualityTools.UnitTestFramework`) to MSTestV2 (`MSTest.Test*`).
- ❗️👉 **`AssemblyVersion` was set to `0.3.0.0`** (from 1.0.0.0 in the v0.2.3). This could be a breaking change in some cases, but given that the project was used by a small number of people, the version being obviously wrong (given being present in the 0.2.3 release) and not having a Nuget release before, we decided to go ahead. Furthermore, switching from a direct assembly reference or a project reference to a Nuget package would require changes to the project references anyway.

### Deprecated

- Service Provider Catalog autodetection on the local host, port 8080, should not be relied upon, as the corresponding logic is deprecated.

### Removed

- ❗️ Support for .NET 4.5, 4.6, and 4.7 was dropped.

### Fixed

- `System.Net.Http.Formatting` issues were resolved by removing references to related SDK-provider assemblies, assembly extensions, and replacing them with a uniform reference to `Microsoft.AspNet.WebApi.Client` 5.2.9.

## [0.2.3] - 2013-07-26

### Security

This release does not contain security updates.

### Added

- ⚡️ Turtle support (marshalling/unmarshalling)
- ⚡️ OSLC Query support
- ⚡️ OAuth 1.0a support

### Changed

- Improved the client

### Deprecated

This release does not introduce deprecations.

### Removed

This release does not remove any features.

### Fixed

This release does not contain bug fixes.

---

**TEMPLATE:**

## [UNRELEASED] - YYYY-MM-DD

### Security

This release does not contain security updates.

### Added

This release does not contain new features.

### Changed

This release does not contain other significant changes.

### Deprecated

This release does not introduce deprecations.

### Removed

This release does not remove any features.

### Fixed

This release does not contain bug fixes.


[UNRELEASED]: https://github.com/OSLC/oslc4net/compare/v0.4.6...HEAD
[0.4.6]: https://github.com/OSLC/oslc4net/releases/tag/v0.4.6
[0.4.5]: https://github.com/OSLC/oslc4net/releases/tag/v0.4.5
[0.4.4]: https://github.com/OSLC/oslc4net/releases/tag/v0.4.4
[0.4.3]: https://github.com/OSLC/oslc4net/releases/tag/v0.4.3
[0.4.2]: https://github.com/OSLC/oslc4net/releases/tag/v0.4.2
[0.3.0-alpha]: https://github.com/OSLC/oslc4net/releases/tag/v0.3.0-alpha
[0.2.3]: https://github.com/OSLC/oslc4net/releases/tag/v0.2.3
