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

Push-Location $samplesDir

# Build the samples
Write-Host "Building samples..." -ForegroundColor Cyan
& dotnet build . -c Release

$baseUrl = "https://oslc.itm.kth.se"

# Test EWM
Write-Host "`n=== Testing EWM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- ewm `
    --url "$baseUrl/ccm" `
    --user $env:JAZZ_ITM_USERNAME `
    --password $env:JAZZ_ITM_PASSWORD `
    --project "Lyo Smoke Test Lifecycle Project (Change Management)"

# Test ERM
Write-Host "`n=== Testing ERM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- erm `
    --url "$baseUrl/rm" `
    --user $env:JAZZ_ITM_USERNAME `
    --password $env:JAZZ_ITM_PASSWORD `
    --project "Lyo Smoke Test Lifecycle Project (Requirements)"

Pop-Location

Write-Host "`nTest completed!" -ForegroundColor Green
