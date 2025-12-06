# OSLC4Net Client Samples Testing

This directory contains test scripts for the OSLC4Net client samples targeting Enterprise Workflow Management (EWM), Enterprise Requirements Management (ERM), and Enterprise Test Management (ETM) systems.

## Overview

The OSLC4Net samples have been renamed from legacy IBM product names to current product names:
- **RTC** (Rational Team Concert) → **EWM** (Enterprise Workflow Management)
- **RRC** (Rational Requirements Composer) → **ERM** (Enterprise Requirements Management)  
- **RQM** (Rational Quality Manager) → **ETM** (Enterprise Test Management)

## Setup

1. Copy `jazz.env.example` to `jazz.env`:
   ```powershell
   Copy-Item jazz.env.example jazz.env
   ```

2. Update `jazz.env` with your credentials for the Jazz instance(s) you want to test:
   ```
   JAZZ_NET_USERNAME=your_username
   JAZZ_NET_PASSWORD=your_password
   JAZZ_NORDIC_USERNAME=your_username
   JAZZ_NORDIC_PASSWORD=your_password
   JAZZ_ITM_USERNAME=your_username
   JAZZ_ITM_PASSWORD=your_password
   ```

## Running Tests

The test scripts will:
1. Load credentials from `jazz.env`
2. Build the samples in Release configuration
3. Run each sample against the target Jazz instance
4. Report results

### Test against jazz.net (IBM Rational Cloud - public instance)
```powershell
./test-jazz_net.ps1
```

This tests:
- **EWM** (Enterprise Workflow Management)  
  - Queries for open change requests
  - Creates, updates, and retrieves change requests
- **ERM** (Enterprise Requirements Management)
  - Creates requirements with different types
  - Builds requirement collections
  - Queries requirements by various criteria
- **ETM** (Enterprise Test Management)
  - Queries for passed test results  
  - Creates and updates test cases

### Test against Nordic (IBM Cloud instance)
```powershell
./test-jazz_nordic.ps1
```

Tests:
- EWM
- ERM

### Test against KTH ITM instance
```powershell
./test-jazz_kth.ps1
```

Tests:
- EWM
- ERM

## Sample Usage

You can run individual samples directly:

```powershell
# Run EWM sample
dotnet run --project ../OSLC4Net.Client.Samples/ -- ewm `
    --url "https://jazz.net/sandbox01-ccm/" `
    --user "your_username" `
    --password "your_password" `
    --project "Your Project Name"

# Run ERM sample
dotnet run --project ../OSLC4Net.Client.Samples/ -- erm `
    --url "https://jazz.net/sandbox01-rm/" `
    --user "your_username" `
    --password "your_password" `
    --project "Your Project Name"

# Run ETM sample
dotnet run --project ../OSLC4Net.Client.Samples/ -- etm `
    --url "https://jazz.net/sandbox01-qm/" `
    --user "your_username" `
    --password "your_password" `
    --project "Your Project Name"
```

## Project Structure

### Sample Classes
Located in `../OSLC4Net.Client.Samples/`:
- `EWMSample.cs` - Demonstrates OSLC 2.0 Change Management operations
- `ERMSample.cs` - Demonstrates OSLC 2.0 Requirements Management operations  
- `ETMSample.cs` - Demonstrates OSLC 2.0 Quality Management operations

### Program Architecture
- `Program.cs` - Dispatcher that routes to the appropriate sample
- `*Sample.cs` - Individual sample implementations with `Run()` entry points

## Product Name Mappings

| Legacy Name | New Name | Context Path | Notes |
|-------------|----------|--------------|-------|
| RTC (Rational Team Concert) | EWM (Enterprise Workflow Management) | `-ccm/` | Change & Configuration Management |
| RRC (Rational Requirements Composer) | ERM (Enterprise Requirements Management) | `-rm/` | Also called DOORS Next Generation (DNG) |
| RQM (Rational Quality Manager) | ETM (Enterprise Test Management) | `-qm/` | Quality Management |

## Troubleshooting

### Connection Issues
- **Connection refused**: Check that the server URL is correct and accessible
- **SSL certificate errors**: Some internal instances may have self-signed certificates. The samples use basic authentication which should work.

### Authentication Issues
- **Authentication failed**: Verify username/password are correct for the instance
- **401 Unauthorized**: Check that the user account has access to the specified project area

### Project/Resource Issues
- **Project not found**: Ensure the project name matches exactly what appears in the server
- **Creation failed**: Some instances may have restrictions on creating test data. Check server logs.
- **Resource not found**: Verify the resource URLs and types are available in the target instance

## Related Documentation

- See `lyo-samples` repository for similar Java samples
- [OSLC Specifications](http://open-services.net/)
- [Eclipse Lyo Documentation](https://projects.eclipse.org/projects/technology.lyo)

