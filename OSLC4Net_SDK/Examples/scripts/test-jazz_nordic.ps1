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
& dotnet build . -c Release

$baseUrl = "https://nordic.clm.ibmcloud.com"

# Test EWM
Write-Host "`n=== Testing EWM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- ewm `
    --url "$baseUrl/ccm" `
    --user $env:JAZZ_NORDIC_USERNAME `
    --password $env:JAZZ_NORDIC_PASSWORD `
    --project "OSLC Open Environment (EWM)"

# Test ERM
Write-Host "`n=== Testing ERM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- erm `
    --url "$baseUrl/rm" `
    --user $env:JAZZ_NORDIC_USERNAME `
    --password $env:JAZZ_NORDIC_PASSWORD `
    --project "OSLC Open Environment (DNG)"

Pop-Location

Write-Host "`nTest completed!" -ForegroundColor Green
