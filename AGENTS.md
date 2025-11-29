# AGENTS.md

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

## Build Commands

Build the solution:
```bash
AGENT_BUILD=true dotnet build OSLC4Net_SDK/OSLC4Net.Core.slnx
```

Run tests (must be run from OSLC4Net_SDK directory or below):
```bash
cd OSLC4Net_SDK

# Run all tests with coverage and TRX reports (MTP flags after --)
AGENT_BUILD=true dotnet test --solution OSLC4Net.Core.slnx --configuration Release -- --coverage --report-trx

# Filter tests using treenode-filter (before --), MTP flags after --
AGENT_BUILD=true dotnet test --solution OSLC4Net.Core.slnx --configuration Release --treenode-filter '/*/*/*/*[TestCategory!=RunningOslcServerRequired]' -- --coverage --report-trx
```

Note: TUnit uses Microsoft.Testing.Platform. When using `dotnet test`, pass MTP flags after `--`.

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
