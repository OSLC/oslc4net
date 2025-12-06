#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test EWM and ERM samples against the Nordic (IBM Cloud) instance
.DESCRIPTION
    This script runs the OSLC4Net client samples against the nordic.clm.ibmcloud.com Rational instance.
    Requires JAZZ_NORDIC_USERNAME and JAZZ_NORDIC_PASSWORD environment variables.
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

Push-Location $samplesDir

# Build the samples
Write-Host "Building samples..." -ForegroundColor Cyan
$env:AGENT_BUILD = $true
if (-not (& dotnet build . -c Release)) {
    Write-Host "Build failed!" -ForegroundColor Red
    Pop-Location
    exit 1
}

$baseUrl = "https://nordic.clm.ibmcloud.com"
$results = @{}

# Test EWM
Write-Host "`n=== Testing EWM Sample ===" -ForegroundColor Green
try {
    & dotnet run -c Release --no-build -- ewm `
        --url "$baseUrl/ccm" `
        --user $env:JAZZ_NORDIC_USERNAME `
        --password $env:JAZZ_NORDIC_PASSWORD `
        --project "OSLC Open Environment (EWM)"
    $results['EWM'] = 'SUCCESS'
    Write-Host "EWM test passed" -ForegroundColor Green
} catch {
    $results['EWM'] = "FAILED: $_"
    Write-Host "EWM test failed: $_" -ForegroundColor Red
}

# Test ERM
Write-Host "`n=== Testing ERM Sample ===" -ForegroundColor Green
try {
    & dotnet run -c Release --no-build -- erm `
        --url "$baseUrl/rm" `
        --user $env:JAZZ_NORDIC_USERNAME `
        --password $env:JAZZ_NORDIC_PASSWORD `
        --project "OSLC Open Environment (DNG)"
    $results['ERM'] = 'SUCCESS'
    Write-Host "ERM test passed" -ForegroundColor Green
} catch {
    $results['ERM'] = "FAILED: $_"
    Write-Host "ERM test failed: $_" -ForegroundColor Red
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
