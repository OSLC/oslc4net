#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test EWM, ERM, and ETM samples against the jazz.net instance
.DESCRIPTION
    This script runs the OSLC4Net client samples against the jazz.net Rational Cloud instance.
    Requires JAZZ_NET_USERNAME and JAZZ_NET_PASSWORD environment variables.
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

$JAZZ_NET_PROJECT_ID = if ($env:JAZZ_NET_PROJECT_ID) { $env:JAZZ_NET_PROJECT_ID } else { "sandbox01" }
$JAZZ_NET_PROJECT_NAME = 'smarx721 Project'

Push-Location $samplesDir

# Build the samples
Write-Host "Building samples..." -ForegroundColor Cyan
& dotnet build . -c Release

$baseUrl = "https://jazz.net/${JAZZ_NET_PROJECT_ID}"

# Test EWM
Write-Host "`n=== Testing EWM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- ewm `
    --url "$baseUrl-ccm" `
    --user $env:JAZZ_NET_USERNAME `
    --password $env:JAZZ_NET_PASSWORD `
    --project "$JAZZ_NET_PROJECT_NAME (Change and Architecture Management)"

# Test ERM
Write-Host "`n=== Testing ERM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- erm `
    --url "$baseUrl-rm" `
    --user $env:JAZZ_NET_USERNAME `
    --password $env:JAZZ_NET_PASSWORD `
    --project "$JAZZ_NET_PROJECT_NAME (Requirements Management)"

# Test ETM
Write-Host "`n=== Testing ETM Sample ===" -ForegroundColor Green
& dotnet run -c Release `
    --no-build -- etm `
    --url "$baseUrl-qm" `
    --user $env:JAZZ_NET_USERNAME `
    --password $env:JAZZ_NET_PASSWORD `
    --project "$JAZZ_NET_PROJECT_NAME (Quality Management)"

Pop-Location

Write-Host "`nTest completed!" -ForegroundColor Green
