# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- CI configuration based on Github Actions (MSBuild, NuGet, VStest).

### Changed

- Since 2017-04-15, the project is hosted on Github under the OSLC community org.
- The project now targets .NET 4.8 instead of .NET 4.5.
- Nuget project configuration was migrated from `packages.config` to PackageReference.
- The StockQuoteSample was updated to ASP.NET MVC 5.

### Deprecated

- Service Provider Catalog autodetection on the local host, port 8080, should not be relied upon, as the corresponding logic is deprecated.

### Removed

- Support for .NET 4.5 (and thus, 4.6 and 4.7) was dropped.

### Fixed

- `System.Net.Http.Formatting` issues were resolved by removing references to related SDK-provider assemblies, assembly extensions, and replacing them with a uniform reference to `Microsoft.AspNet.WebApi.Client` 5.2.9.

### Security

- **Updated `log4net` in response to CVE-2018-1285.**
- **Updated `Newtonsoft.Json` in response to [CWE-755](https://cwe.mitre.org/data/definitions/755.html).**

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



[unreleased]: https://github.com/OSLC/oslc4net/compare/v0.2.3...HEAD
[0.2.3]: https://github.com/OSLC/oslc4net/releases/tag/v0.2.3
