# AGENTS.md

## Docs

- If a change is significant, add a line to CHANGELOG.md
- If a change breaks anything or requires a non-trivial migration step, add a section to MIGRATION.md
- If a change is noteworthy, add docs to the `docs/articles`. Use snippets where possible. Strive to apply https://diataxis.fr/. Prefer spelling out type names over 'var' except for literals and obvious types. Skip ceremony like setting up loggers or ConfigureAwait. Refrain from numbering sections/subsections to make it easier to restructure content.

Overall, follow `misc/instructions/gov-uk-technical-content.md` and `misc/instructions/iso-house-guide.md` to guide the language style.

## Build Output Configuration

When running builds or tests in this project, set the `AGENT_BUILD` environment variable to minimize build output:

```bash
export AGENT_BUILD=true
dotnet build
```

Or inline:

```bash
AGENT_BUILD=true dotnet build
AGENT_BUILD=true dotnet test
```

This reduces noise from warnings and analyzer messages, showing primarily errors and critical information.

## Git Hooks: Pre-Commit Formatting (Cross-Platform)

To keep the codebase consistently formatted, a pre-commit hook can run `dotnet format` scoped to the SDK.

- Hook behavior:
	- Runs `dotnet format` inside `OSLC4Net_SDK`.
	- Stages any whitespace or formatting changes automatically.
	- Fails the commit if formatting fails or cannot be applied.

- One-time setup:
	- Use the provided POSIX shell hook script: `.githooks/pre-commit`.
	- Point Git to use the repository hooks folder. Quick setup scripts:
		- macOS/Linux: `scripts/setup-hooks.sh`
		- Windows (PowerShell): `scripts/setup-hooks.ps1`

```zsh
# macOS/Linux
./scripts/setup-hooks.sh
```

```powershell
# Windows (PowerShell)
./scripts/setup-hooks.ps1
```

- Manual run (if needed):

```zsh
cd OSLC4Net_SDK
dotnet format
```

Notes:
- Requires `dotnet` to be in PATH.
- If the hook blocks your commit due to formatting changes, review them and commit again.

## Build Commands

Build the solution:
```bash
AGENT_BUILD=true dotnet build OSLC4Net_SDK/OSLC4Net.Core.slnx
```

Run tests (must be run from OSLC4Net_SDK directory or below):
```bash
cd OSLC4Net_SDK

# Quick test run (no coverage)
AGENT_BUILD=true dotnet test --solution OSLC4Net.Core.slnx --configuration Release

# Filter tests using treenode-filter
AGENT_BUILD=true dotnet test --solution OSLC4Net.Core.slnx --configuration Release --treenode-filter '/*/*/*/*[TestCategory!=RunningOslcServerRequired]'

Note: TUnit uses Microsoft.Testing.Platform.

## Project Structure

- `OSLC4Net_SDK/` - Main SDK projects (.NET 6+)
- `OSLC4Net_SDK/Tests/` - Test projects using TUnit and Verify frameworks
- `OSLC4Net_NETFramework/` - Legacy .NET Framework samples - do not touch unless instructed.

## Code Style

- C# 14 with nullable reference types enabled
- Implicit usings enabled
- Follow .NET code analysis and Meziantou.Analyzer rules
- NuGet vulnerabilities treated as errors
- Nullability warnings (CS8600, CS8602, CS8603) treated as errors
