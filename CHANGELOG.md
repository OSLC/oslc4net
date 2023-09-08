# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## UNRELEASED

### Security

- (breaking) `OSLC4Net.Client` now defaults to strong TLS certificate checking. Skipping TLS checks now requires explicit configuration.

### Added

- NA

### Changed

- `OSLC4Net.Core` now targets `netstandard2.0`, which allows it to  be used under .NET Framework 4.7.2 or higher as well as .NET Core/.NET 5+.
- dotNetRDF was upgraded from v1 to v3 to enable targeting `netstandard2.0`. One of the key breaking changes is that `ITriple` and `INode` no longer have the `.Graph` property. This caused breaking changes to some of the method signatures in `OSLC4Net.DotNetRdfProvider` to allow the `IGraph` instance to be passed.
- `OSLC4Net.Client` now targets `netstandard2.0`.
  - The main breaking change is the replacement of the legacy `WebRequestHandler` with `HttpClientHandler`. This caused some method/constructor signatures to change. 
  - `RemoteCertificateValidationCallback` was replaced with a lambda function.
- `OSLC4Net.Query` now targets `netstandard2.0`. The Antlr3 runtime package targeting PCL was replaced with a package targeting NETStandard1.0 (Antlr 4 targets NETStandard2.0).

### Deprecated

- NA

### Removed

- NA

### Fixed

- NA

## [0.3.0-alpha] - 2023-04-29

### Security

- **Updated `log4net` in response to CVE-2018-1285.**
- **Updated `Newtonsoft.Json` in response to [CWE-755](https://cwe.mitre.org/data/definitions/755.html).**

### Added

- CI configuration based on Github Actions (MSBuild, NuGet, VStest).

### Changed

- Since 2017-04-15, the project is hosted on Github under the OSLC community org.
- The project now targets .NET 4.8 instead of .NET 4.5.
- Nuget project configuration was migrated from `packages.config` to PackageReference.
- The StockQuoteSample was updated to ASP.NET MVC 5.
- Migrated from MSBuild-based NuGet package restore to NuGet 2.7+ Automatic Package Restore.
- Migrated from MSTestV1 (`Microsoft.VisualStudio.QualityTools.UnitTestFramework`) to MSTestV2 (`MSTest.Test*`).
- **`AssemblyVersion` was set to `0.3.0.0`** (from 1.0.0.0 in the v0.2.3). This could be a breaking change in some cases, but given that the project was used by a small number of people, the version being obviously wrong (given being present in the 0.2.3 release) and not having a Nuget release before, we decided to go ahead. Furthermore, switching from a direct assembly reference or a project reference to a Nuget package would require changes to the project references anyway.

### Deprecated

- Service Provider Catalog autodetection on the local host, port 8080, should not be relied upon, as the corresponding logic is deprecated.

### Removed

- Support for .NET 4.5 (and thus, 4.6 and 4.7) was dropped.

### Fixed

- `System.Net.Http.Formatting` issues were resolved by removing references to related SDK-provider assemblies, assembly extensions, and replacing them with a uniform reference to `Microsoft.AspNet.WebApi.Client` 5.2.9.

## [0.2.3] - 2013-07-26

### Added

- Turtle support (marshalling/unmarshalling)
- OSLC Query support
- OAuth support

### Changed

- Improved the client

### Deprecated 

m/a

### Removed

m/a

### Fixed

m/a

### Security

m/a

## Template

### Security

- NA

### Added

- NA

### Changed

- NA

### Deprecated

- NA

### Removed

- NA

### Fixed

- NA


[unreleased]: https://github.com/OSLC/oslc4net/compare/v0.2.3...HEAD
[0.2.3]: https://github.com/OSLC/oslc4net/releases/tag/v0.2.3
