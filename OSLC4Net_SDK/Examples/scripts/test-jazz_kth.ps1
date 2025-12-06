#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test EWM and ERM samples against the KTH ITM instance
.DESCRIPTION
    This script runs the OSLC4Net client samples against the oslc.itm.kth.se test instance.
    Requires JAZZ_ITM_USERNAME and JAZZ_ITM_PASSWORD environment variables.
.NOTES
    Based on lyo-samples test scripts
#>

param()

$ErrorActionPreference = "Stop"
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$envFile = Join-Path $scriptPath "jazz.env"
$samplesDir = Join-Path (Split-Path -Parent $scriptPath) "OSLC4Net.Client.Samples"

# Load environment if it exists
if (Test-Path $envFile) {
    Get-Content $envFile | ForEach-Object {
        if ($_ -match '^\s*([^#=]+)=(.*)$') {
            [Environment]::SetEnvironmentVariable($matches[1], $matches[2], "Process")
        }
    }
} else {
    Write-Host ".env file not found at $envFile" -ForegroundColor Yellow
}

$JAZZ_ITM_PROJECT_NAME = if ($env:JAZZ_ITM_PROJECT_NAME) { $env:JAZZ_ITM_PROJECT_NAME } else { "Lyo Smoke Test Lifecycle Project" }

Push-Location $samplesDir

# Build the samples
Write-Host "Building samples..." -ForegroundColor Cyan
$env:AGENT_BUILD = $true
& dotnet build . -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    Pop-Location
    exit 1
}

$baseUrl = "https://oslc.itm.kth.se"
$results = @{}

# Test EWM
Write-Host "`n=== Testing EWM Sample ===" -ForegroundColor Green
& dotnet run -c Release --no-build -- ewm `
    --url "$baseUrl/ccm" `
    --user $env:JAZZ_ITM_USERNAME `
    --password $env:JAZZ_ITM_PASSWORD `
    --project "$JAZZ_ITM_PROJECT_NAME (Change Management)"

if ($LASTEXITCODE -eq 0) {
    $results['EWM'] = 'SUCCESS'
    Write-Host "EWM test passed" -ForegroundColor Green
} else {
    $results['EWM'] = "FAILED: Exit code $LASTEXITCODE"
    Write-Host "EWM test failed: Exit code $LASTEXITCODE" -ForegroundColor Red
}

# Test ERM
Write-Host "`n=== Testing ERM Sample ===" -ForegroundColor Green
& dotnet run -c Release --no-build -- erm `
    --url "$baseUrl/rm" `
    --user $env:JAZZ_ITM_USERNAME `
    --password $env:JAZZ_ITM_PASSWORD `
    --project "$JAZZ_ITM_PROJECT_NAME (Requirements)"

if ($LASTEXITCODE -eq 0) {
    $results['ERM'] = 'SUCCESS'
    Write-Host "ERM test passed" -ForegroundColor Green
} else {
    $results['ERM'] = "FAILED: Exit code $LASTEXITCODE"
    Write-Host "ERM test failed: Exit code $LASTEXITCODE" -ForegroundColor Red
}

Pop-Location

# Report results
Write-Host "`n=== Test Results ===" -ForegroundColor Cyan
$results | Format-Table -AutoSize
$failedCount = @($results.Values | Where-Object { $_ -ne 'SUCCESS' }).Count
if ($failedCount -gt 0) {
    Write-Host "$failedCount test(s) failed!" -ForegroundColor Red
    exit 1
} else {
    Write-Host "All tests passed!" -ForegroundColor Green
    exit 0
}
