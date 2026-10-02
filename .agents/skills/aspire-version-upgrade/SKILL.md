---
name: aspire-version-upgrade
description: Update Aspire AppHost SDK and package versions consistently in OSLC4Net, regenerate every affected NuGet lock file, and verify locked restore and builds. Use whenever upgrading, downgrading, or aligning Aspire dependencies in this repository.
---

<!--
Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
which accompanies this distribution.

The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
-->

# Aspire version upgrades

Update Aspire as a coordinated SDK, package, and lock-file change. Do not change only `global.json`.

## Version sources

- `OSLC4Net_SDK/global.json` selects `Aspire.AppHost.Sdk` for AppHost projects.
- `OSLC4Net_SDK/Directory.Packages.props` defines `AspireVersion`. It pins `Aspire.Hosting.Testing` and supplies versions for platform-specific Aspire SDK packages in `Directory.Build.targets`.
- `OSLC4Net_SDK/Directory.Build.targets` lists the supported AppHost package RIDs: `linux-x64`, `linux-arm64`, `osx-arm64`, and `win-x64`.

Keep `Aspire.AppHost.Sdk` and `AspireVersion` on the same target version unless repository configuration or release requirements explicitly establish otherwise. The central property is required even when the dependency bot changes only the SDK in `global.json`.

## Upgrade workflow

1. Start at the repository root. Check the working tree and identify the requested Aspire version. Find all relevant SDK and package references:

   ```bash
   rg -n 'Aspire\.AppHost\.Sdk|AspireVersion|Aspire\.Hosting\.Testing|Aspire\.(Dashboard\.Sdk|Hosting\.Orchestration)' \
     OSLC4Net_SDK -g 'global.json' -g '*.props' -g '*.targets' -g '*.csproj' -g 'packages.lock.json'
   ```

2. Set `Aspire.AppHost.Sdk` in `OSLC4Net_SDK/global.json` and `AspireVersion` in `OSLC4Net_SDK/Directory.Packages.props` to the requested version. Inspect any other Aspire version declarations found by the search and keep them aligned where they belong to the same release train.

3. Regenerate lock files; do not edit `packages.lock.json` by hand. Change to `OSLC4Net_SDK` and stay there through the build checks:

   ```bash
   cd OSLC4Net_SDK
   export AGENT_BUILD=true
   dotnet restore OSLC4Net.Core.slnx --force-evaluate
   ```

   Find Aspire-consuming projects with:

   ```bash
   rg -l 'Aspire\.Hosting\.Testing|<Project Sdk="Aspire\.AppHost\.Sdk"' \
     . -g '*.csproj'
   ```

   Restore every such project that is not included in `OSLC4Net.Core.slnx`, also with `--force-evaluate`. Some projects have independent lock files even though the main solution does not include them. For example, `Tests/OSLC4NetExamples.Server.Tests` and `Tests/OSLC4NetExamples.Server.Tests.AspireHost` have separate lock files and must be refreshed when their dependency graph changes.

4. Review the diff. Confirm every affected lock file is included, the central version and SDK version agree, and the four platform-specific Aspire package entries in each AppHost lock file resolve to the requested version. Keep unrelated lock-file churn out of the change.

5. Check locked restore as CI does. `OSLC4Net_SDK/Directory.Build.targets` enables `RestoreLockedMode` when `GITHUB_ACTIONS` is `true`:

   ```bash
   export GITHUB_ACTIONS=true
   dotnet restore OSLC4Net.Core.slnx
   ```

   Also run locked restore for each Aspire-consuming project outside the solution. Do not pass `--force-evaluate` to these checks. CI covers the supported Windows, Linux x64, and Linux ARM64 environments, so retain their platform-specific entries in the lock files.

6. Build the solution from `OSLC4Net_SDK`:

   ```bash
   dotnet build OSLC4Net.Core.slnx --configuration Release --no-restore
   ```

   Also build every Aspire-consuming project outside the solution with `--configuration Release --no-restore`. Locked restore verifies the dependency graph; building checks compatibility with Aspire APIs. For the example-server projects:

   ```bash
   dotnet build Tests/OSLC4NetExamples.Server.Tests.AspireHost/OSLC4NetExamples.Server.Tests.AspireHost.csproj --configuration Release --no-restore
   dotnet build Tests/OSLC4NetExamples.Server.Tests/OSLC4NetExamples.Server.Tests.csproj --configuration Release --no-restore
   ```

7. Return to the repository root and check the diff:

   ```bash
   cd ..
   git diff --check
   ```

   Before committing, follow the formatting commands and commit-message convention in the repository `AGENTS.md`.

## Diagnose restore failures

- `NU1004` saying an Aspire package version changed usually means a lock file was not regenerated or `AspireVersion` still disagrees with `global.json`.
- If CI names a project rather than the solution, check whether it has its own `packages.lock.json` and whether it is included in `OSLC4Net.Core.slnx`.
- If an AppHost restore fails on only one operating system, compare all RID-specific Aspire package entries in its lock file. A restore on one developer machine can leave another platform entry stale when the versions were not coordinated.
- Fix the version source or regenerate the affected lock file, then rerun locked restore. Do not disable locked mode to make CI pass.
