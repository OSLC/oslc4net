# Contributing to OSLC4Net

First, thank you for considering contributing to OSLC4Net. It is contributors like you who make OSLC4Net a great tool.

## How can I contribute?

### Reporting bugs
- Ensure the bug was not already reported by searching on GitHub under [Issues](https://github.com/oslc/oslc4net/issues).
- If you're unable to find an open issue addressing the problem, [open a new one](https://github.com/oslc/oslc4net/issues/new). Be sure to include a title and clear description, as much relevant information as possible, and a code sample or an executable test case demonstrating the expected behavior that is not occurring.

### Suggesting enhancements
- Open a new issue to discuss your suggested enhancements. Please provide as much detail as possible, including the problem you're trying to solve and how your enhancement would help.

### Setting up your development environment

For `OSLC4Net_SDK`, use regular .NET 8+ SDK. `OSCL4Net_Framework` uses .NET Framework and is unmaintained.

### Submitting pull requests
- Fork the repository on GitHub.
- Create a new branch for your changes.
- Make your changes, including appropriate test cases.
- Ensure all tests pass.
- Create a pull request on GitHub.
- Ensure your pull request has a clear title and description.
- Link your pull request to any relevant issues.

## Commit messages

OSLC4Net uses [scoped commits](https://scopedcommits.com/) — a lightweight alternative to Conventional Commits. See [Stop using Conventional Commits](https://sumnerevans.com/posts/software-engineering/stop-using-conventional-commits/) for the motivation.

The subject line is:

```
<scope>: <imperative summary in lowercase>
```

No `feat:` / `fix:` / `chore:` / `refactor:` type prefixes. The scope tells you where in the codebase the change lives; the summary tells you what changed.

PR titles follow the same convention — the squash-merge subject becomes the commit subject.

### Top-level scopes

| Scope | What it covers |
|---|---|
| `core:` | `OSLC4Net.Core` — model, attributes, constants, wildcard properties |
| `query:` | `OSLC4Net.Core.Query` |
| `client:` | `OSLC4Net.Client`; sub-scope `client: restsharp:` for `OSLC4Net.Client.RestSharp` |
| `server:` | `OSLC4Net.Server.Providers`; sub-scope `server: provider:` for `OSLC4Net.Core.DotNetRdfProvider` and `OSLC4Net.Core.JsonProvider` |
| `domains:` | Domain libraries — always sub-scoped: `domains: rm:`, `domains: cm:`, future `domains: am:`, `domains: qm:`, `domains: config:` |
| `trs:` | Tracked Resource Set (reserved; use once the module lands) |
| `tests:` | Cross-cutting test infrastructure only (Aspire host, `Test.RefImpl`). Per-project test changes attribute to the code's own scope |
| `docs:` | `docs/`, `README.md`, `CHANGELOG.md`, `MIGRATION.md`, `OSLC4Net_SDK/Examples/`, `AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md`, `misc/instructions/` |
| `build:` | MSBuild plumbing, `Directory.*.props`, `global.json`, csproj files, `.github/workflows/`, dependency bumps, `.pre-commit-config.yaml`, `scripts/`, `LICENSE`, `.gitignore` |

`docs:`, `build:`, and `server: provider:` are intentionally flat — the subject line says which file or package changed, so further sub-scoping adds noise without information. Sub-scopes only exist for `client:` and `domains:`.

### Examples

- `core: add property support to ResourceShape`
- `domains: cm: migrate ChangeRequest to property-based shape`
- `query: fix SortTermsImpl collection parsing`
- `client: restsharp: drop sync overloads`
- `server: provider: update dotNetRDF usage to avoid deprecations`
- `server: sanitize user id before logging (CWE-117)`
- `build: bump Meziantou.Analyzer to 3.0.84`
- `build: upload coverage to GitHub PR Code Coverage`
- `docs: document copyright-header policy for AGENTS.md`

### Multiple scopes

If a change genuinely spans top-level scopes, split it into separate commits. For a multi-scope PR, the squash-merge title picks the dominant scope and the PR description lists the others.

### Dependabot

Dependabot is configured (via `.github/dependabot.yml`) to emit `build:` prefixes for both NuGet and GitHub Actions bumps, so no retitling is needed on squash.

## Coding conventions

Follow _Framework Design Guidelines_ where possible.

## Code of Conduct
This project and everyone participating in it are governed by the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/about/policies/code-of-conduct). By participating, you are expected to uphold this code.
