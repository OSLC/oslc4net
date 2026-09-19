# OSLC query parser fuzzing

This project is an opt-in [SharpFuzz](https://github.com/metalnem/sharpfuzz)
harness for the public OSLC query parsers. Invalid query syntax is expected;
exceptions other than `ParseException` are reported as fuzzing failures.

## Prerequisites

- .NET 10 SDK
- PowerShell
- AFL++ (`brew install afl++` on macOS or install the `afl++` package on Linux)
- SharpFuzz's instrumentation tool:

  ```shell
  dotnet tool install --global SharpFuzz.CommandLine --version 2.3.0
  ```

## Run

From the repository root, start the opt-in fuzzing script:

```shell
pwsh scripts/fuzz-oslc-query.ps1
```

The script publishes and instruments this project, then starts AFL++ with the
checked-in corpus under `OSLC4Net_SDK/Tests/OSLC4Net.Fuzzing/Testcases` and the
OSLC query dictionary for five minutes. Findings are written below the fuzzing
project directory and are intentionally ignored by Git. Pass
`-DurationSeconds <seconds>` to change the campaign length.

To smoke-test the harness without AFL++, pipe one seed through the executable:

```shell
cd OSLC4Net_SDK
dotnet run --project Tests/OSLC4Net.Fuzzing/OSLC4Net.Fuzzing.csproj \
  < Tests/OSLC4Net.Fuzzing/Testcases/where
```
