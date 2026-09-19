#
# Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
#
# All rights reserved. This program and the accompanying materials
# are made available under the terms of the Eclipse Public License v1.0
# which accompanies this distribution.
#
# The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
#

[CmdletBinding()]
param(
    [string]$Project = "OSLC4Net_SDK/Tests/OSLC4Net.Fuzzing/OSLC4Net.Fuzzing.csproj",
    [string]$Corpus = "OSLC4Net_SDK/Tests/OSLC4Net.Fuzzing/Testcases",
    [string]$Dictionary = "OSLC4Net_SDK/Tests/OSLC4Net.Fuzzing/Dictionaries/oslc-query.dict",
    [string]$Command = "sharpfuzz",
    [string]$Fuzzer = "afl-fuzz",
    [int]$DurationSeconds = 300
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Project))
$corpusPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Corpus))
$dictionaryPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Dictionary))
$projectDirectory = Split-Path -Parent $projectPath
$outputDirectory = Join-Path $projectDirectory "fuzz-bin"
$findingsDirectory = Join-Path $projectDirectory "findings"
$projectName = [System.IO.Path]::GetFileNameWithoutExtension($projectPath)
$projectDll = Join-Path $outputDirectory "$projectName.dll"

if (Test-Path -LiteralPath $outputDirectory) {
    Remove-Item -LiteralPath $outputDirectory -Recurse -Force
}

if (Test-Path -LiteralPath $findingsDirectory) {
    Remove-Item -LiteralPath $findingsDirectory -Recurse -Force
}

dotnet publish $projectPath --configuration Release --output $outputDirectory

$excludedAssemblies = @(
    "dnlib.dll",
    "SharpFuzz.dll",
    "SharpFuzz.Common.dll",
    "$projectName.dll"
)

$fuzzingTargets = Get-ChildItem -LiteralPath $outputDirectory -Filter "*.dll" |
    Where-Object { $_.Name -notin $excludedAssemblies } |
    Where-Object { $_.Name -notlike "System.*.dll" }

if (($fuzzingTargets | Measure-Object).Count -eq 0) {
    throw "No fuzzing targets were found in $outputDirectory."
}

foreach ($fuzzingTarget in $fuzzingTargets) {
    Write-Host "Instrumenting $($fuzzingTarget.Name)"
    & $Command $fuzzingTarget.FullName
    if ($LASTEXITCODE -ne 0) {
        throw "SharpFuzz instrumentation failed for $($fuzzingTarget.FullName)."
    }
}

$env:AFL_I_DONT_CARE_ABOUT_MISSING_CRASHES = "1"
$env:AFL_SKIP_BIN_CHECK = "1"

& $Fuzzer -i $corpusPath -o $findingsDirectory -m none -t 10000 -V $DurationSeconds -x $dictionaryPath dotnet $projectDll
exit $LASTEXITCODE
