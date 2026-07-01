# AGENTS.md


## Code style

Follow https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/ where possible.

## Tests

Use TUnit for unit tests. Prefer Verify and round-tripping checks when dealing with RDF data. When creating multiline dummy data, prefer `"""` strings.

Follow https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices where relevant.

Follow the key techniques from Working Effectively With Legacy Code by Michael C. Feathers, especially when modifying existing public code. See `misc/instructions/legacy-code-testing.md` for detailed instructions.

Strive to improve coverage but without silly attempts to cover every getter and setter.

Aim for at least 70% branch (not line) coverage in classes you modify and 95%+ branch coverage in classes you add. To obtain coverage JSON information:

```
cd OSLC4Net_SDK
dotnet test --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
dotnet reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:JsonSummary
# outputs to OSLC4Net_SDK/CoverageReport/Summary.json
```

## Docs

- Every breaking change must have at least a one-liner in the CHANGELOG.md. If a change is significant, add a line as well.
- If a change breaks anything or requires a non-trivial migration step, add a section to MIGRATION.md
- If a change is noteworthy, add docs to the `docs/articles`. Use snippets where possible. Strive to apply https://diataxis.fr/. Prefer spelling out type names over 'var' except for literals and obvious types. Skip ceremony like setting up loggers or ConfigureAwait. Refrain from numbering sections/subsections to make it easier to restructure content.

Overall, follow `misc/instructions/gov-uk-technical-content.md` and `misc/instructions/iso-house-guide.md` to guide the language style.

## RDF resources

Domain `Resources/*.nt` files copied from OSLC, OMG, or W3C specifications are canonical upstream files. Do not patch spelling, labels, ranges, or other vocabulary data locally unless the change is generated from an upstream update. For temporary SDK behavior, prefer code-side overrides or extensions with a `REVISIT` comment and report the source defect upstream.

## Running the code and tests

When running builds or tests in this project, set the `AGENT_BUILD` environment variable to minimize build output from warnings and analyzer messages:

```bash
cd OSLC4Net_SDK ; export AGENT_BUILD=true; dotnet build OSLC4Net.Core.slnx
```

Run tests (must be run from OSLC4Net_SDK directory or below):

```bash
# Quick test run (no coverage)
cd OSLC4Net_SDK ; export AGENT_BUILD=true; dotnet test --solution OSLC4Net.Core.slnx --configuration Release

# Filter tests using treenode-filter
cd OSLC4Net_SDK ; export AGENT_BUILD=true; dotnet test --solution OSLC4Net.Core.slnx --configuration Release --treenode-filter '/*/*/*/*[TestCategory!=RunningOslcServerRequired]'
```

> Note: TUnit uses Microsoft.Testing.Platform.

Do not inline the `AGENT_BUILD` variable!

You must be CD'd into `OSLC4Net_SDK` or subfolder before running `dotnet` commands.

### Test Filters

Running TUnit via `dotnet run` supports test filters.

TUnit can select tests by:

- Assembly
- Namespace
- Class name
- Test name

You must use the `--treenode-filter` flag on the command line.

The syntax for the filter value is (without the angled brackets) `/<Assembly>/<Namespace>/<Class name>/<Test name>`

Wildcards are also supported with `*`

**Filter Operators** TUnit supports several operators for building complex filters:

- **Wildcard matching:** Use `*` for pattern matching (e.g., `LoginTests*` matches `LoginTests`, `LoginTestsSuite`, etc.)
- **Equality:** Use `=` for exact match (e.g., `[Category=Unit]`)
- **Negation:** Use `!=` for excluding values (e.g., `[Category!=Performance]`)
- **AND operator:** Use `&` to combine conditions (e.g., `[Category=Unit]&[Priority=High]`)
- **OR operator:** Use `|` to match either condition - requires parentheses (e.g., `(/*/*/Class1/*)|(/*/*/Class2/*)`)

For full information on the treenode filters, see [Microsoft's documentation](https://github.com/microsoft/testfx/blob/main/docs/mstest-runner-graphqueryfiltering/graph-query-filtering.md)

So an example could be:

`dotnet run --treenode-filter /*/*/LoginTests/*` - To run all tests in the class `LoginTests`

or

`dotnet run --treenode-filter /*/*/*/AcceptCookiesTest` - To run all tests with the name `AcceptCookiesTest`

TUnit also supports filtering by your own [properties](https://thomhurst.github.io/TUnit/docs/test-lifecycle/properties). So you could do:

`dotnet run --treenode-filter /*/*/*/*[MyFilterName=*SomeValue*]`

And if your test had a property with the name "MyFilterName" and its value contained "SomeValue", then your test would be executed.

## Commit messages

Use the scoped-commit convention defined in [CONTRIBUTING.md](CONTRIBUTING.md#commit-messages): `scope: imperative summary in lowercase`, no `feat:` / `fix:` / `chore:` / `refactor:` type prefixes. Top-level scopes are `core` / `query` / `client` / `server` / `domains` / `trs` / `tests` / `docs` / `build`. Apply the same convention to PR titles — the squash-merge subject becomes the commit subject.

## Formatting (required before every commit)

Run from the repo root:

```bash
dotnet format whitespace ./OSLC4Net_SDK && dotnet format style ./OSLC4Net_SDK --no-restore
```

- Use `whitespace` + `style` only. Skip `analyzers` — it runs third-party rules (Meziantou etc.) and is slow/noisy.
- `--no-restore` on the second call avoids a redundant restore.
- Also run this **after resolving merge conflicts**. Hand-edited hunks re-introduce style drift even when both parents were clean.
- A pre-commit hook (`.pre-commit-config.yaml`) enforces this; install once with `pre-commit install`.

## Copyright headers

Any file you've **significantly** touched (substantive code, docs, or
non-trivial test changes — not just whitespace, renames, or a one-line
tweak) should carry the copyright attribution:

```
Copyright (c) <YYYY> Andrii Berezovskyi and OSLC4Net contributors.
```

where `<YYYY>` is the current year **at the time you add the line**.

Rules:
- **New files**: include this full header:

  ```text
  /*
   * Copyright (c) <YYYY> Andrii Berezovskyi and OSLC4Net contributors.
   *
   * All rights reserved. This program and the accompanying materials
   * are made available under the terms of the Eclipse Public License v1.0
   * which accompanies this distribution.
   *
   * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
   */
  ```

  Do not add inherited IBM/contributor headers to new files unless the file
  actually derives from that source.
- **Existing files** that already have other copyright attributions
  (e.g. `Copyright (c) 2012 IBM Corporation.`): add the
  `Andrii Berezovskyi and OSLC4Net contributors.` line immediately after
  the last existing copyright line. Preserve all other attributions and
  the `Contributors:` block.
- **Existing files** that already carry an
  `Andrii Berezovskyi and OSLC4Net contributors.` line: leave the year
  alone. Do not "refresh" it to the current year.
- Never remove other copyright attributions.

## Project Structure

- `OSLC4Net_SDK/` - Main SDK projects (.NET 10)
- `OSLC4Net_SDK/Tests/` - Test projects using TUnit and Verify frameworks

## Code Style

- C# 14 with nullable reference types enabled
- Implicit usings enabled
- Follow .NET code analysis and Meziantou.Analyzer rules
- NuGet vulnerabilities treated as errors
- Nullability warnings (CS8600, CS8602, CS8603) treated as errors
