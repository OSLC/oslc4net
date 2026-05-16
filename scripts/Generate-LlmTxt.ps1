param (
    [string]$TargetDll = ".\bin\Debug\net8.0\MyAssembly.dll",
    [string]$OutputFile = "llm.txt"
)

# 1. Attempt to resolve ILDASM path automatically (adjust valid paths as needed)
$ildasmPath = Get-Command "ildasm" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
if (-not $ildasmPath) {
    # Common fallback locations for Visual Studio / .NET SDKs
    $possiblePaths = @(
        "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\ildasm.exe",
        "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7.2 Tools\ildasm.exe"
    )
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) { $ildasmPath = $path; break }
    }
}

function Get-AssemblySignature {
    param ([string]$DllPath)

    try {
        # --- ATTEMPT 1: REFLECTION (Preferred, Cleaner Output) ---
        $asm = [System.Reflection.Assembly]::LoadFrom($DllPath)
        
        # Iterate over all exported types
        foreach ($type in $asm.GetExportedTypes()) {
            "Class: $($type.FullName)"
            
            # Get methods and signatures
            foreach ($method in $type.GetMethods([System.Reflection.BindingFlags]"Public,Instance,Static,DeclaredOnly")) {
                "  Method: $($method.ReturnType.Name) $($method.Name)($(($method.GetParameters() | ForEach-Object { $_.ParameterType.Name }) -join ', '))"
            }
            "" # New line between classes
        }
    }
    catch {
        # --- ATTEMPT 2: FALLBACK TO ILDASM (Robust against missing dependencies) ---
        Write-Warning "Reflection failed for $DllPath (likely missing dependencies). Falling back to ILDASM."
        Write-Warning "Error: $($_.Exception.Message)"

        if ($ildasmPath) {
            # # /TEXT sends output to console
            # # /NOBAR removes the progress bar
            # # /PUBONLY shows only public members (similar to javap behavior)
            # & $ildasmPath $DllPath /TEXT /NOBAR /PUBONLY /HEADER /METADATA
        }
        else {
            Write-Error "Could not load assembly via Reflection and ILDASM was not found on the system."
        }
    }
}

# Run the function and pipe to Tee-Object
Get-AssemblySignature -DllPath $TargetDll | Tee-Object -FilePath $OutputFile