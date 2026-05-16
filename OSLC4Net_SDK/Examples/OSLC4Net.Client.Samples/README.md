# OSLC4Net Client Samples

This project contains practical examples of using OSLC4Net to connect to and interact with Jazz-based servers (Rational Team Concert - RTC, Change/Requirements/Quality Management services).

## Samples

### EWMSample - Enterprise Workflow Management (Change Management)
Demonstrates OSLC Change Management operations on RTC/EWM servers.

**Features:**
- Authentication to Jazz servers using form-based login
- Discovery of OSLC services via rootservices
- Querying change requests (work items)
- Creating new change requests
- Handling pagination in query results

**Run:**
```bash
dotnet run -- ewm \
  --url "https://your-server/sandbox01-ccm" \
  --user "username" \
  --password "password" \
  --project "Your Project Name"
```

### ERMSample - Enterprise Requirements Management
Demonstrates OSLC Requirements Management operations on RRC/ERM servers.

**Features:**
- Authentication and discovery (same as EWM)
- Querying requirements
- Creating new requirements
- Discovering requirement instance shapes for creation templates

**Run:**
```bash
dotnet run -- erm \
  --url "https://your-server/sandbox01-rm" \
  --user "username" \
  --password "password" \
  --project "Your Project Name"
```

### ETMSample - Enterprise Test Management
Demonstrates OSLC Quality Management operations on RQM/ETM servers.

**Features:**
- Authentication and discovery
- Querying test cases and results
- Creating test cases
- Managing test results

**Run:**
```bash
dotnet run -- etm \
  --url "https://your-server/sandbox01-qm" \
  --user "username" \
  --password "password" \
  --project "Your Project Name"
```

## Quick Start

### Prerequisites
- .NET 10 SDK or later
- Access to a Jazz server (RTC/EWM, RRC/ERM, or RQM/ETM)
- Valid credentials for the server

### Building
```bash
dotnet build .
```

### Running
```bash
# Build and run
dotnet run -- [sample-name] [arguments]

# Build then run (faster for repeated runs)
dotnet build . -c Release
dotnet run -c Release --no-build -- [sample-name] [arguments]
```

## Architecture

The samples use a dispatcher pattern in `Program.cs` that routes commands to the appropriate sample class based on the first argument. Each sample:

1. Initializes a `JazzFormAuthClient` for authentication
2. Authenticates using form-based login
3. Creates a `RootServicesHelper` with the authenticated client to discover OSLC services
4. Executes sample operations (query, create, etc.)
5. Logs results and any errors

### Key Classes

- **JazzFormAuthClient**: Handles form-based authentication with Jazz servers
- **RootServicesHelper**: Discovers OSLC service providers via rootservices document
- **OslcQueryBuilder**: Constructs OSLC query strings
- **OslcClient**: Makes OSLC requests and handles responses

## Important Notes

### Authentication
- All samples use form-based authentication (standard for Jazz servers)
- The authenticated `HttpClient` from `JazzFormAuthClient.GetHttpClient()` must be passed to discovery methods
- Authentication happens BEFORE discovery to ensure the rootservices document is fetched with proper credentials

### Jazz-Specific Configuration
Jazz servers use domain-specific catalog properties in their rootservices document:

| Server | Namespace | Property |
|--------|-----------|----------|
| EWM (CCM) | `http://open-services.net/xmlns/cm/1.0/` | `cmServiceProviders` |
| ERM (RM) | `http://open-services.net/xmlns/rm/1.0/` | `rmServiceProviders` |
| ETM (QM) | `http://open-services.net/xmlns/qm/1.0/` | `qmServiceProviders` |

### Known Limitations

See [TESTING_RESULTS.md](../TESTING_RESULTS.md) for real-world test results against live instances.

- **ERMSample**: May fail on some server configurations when attempting to lookup InstanceShapes
- **ETMSample**: May encounter RDF parsing issues on certain query results

## Testing

Automated test scripts are provided in the `scripts/` directory:

```bash
# Test all samples against jazz.net
./scripts/test-jazz_net.ps1

# Test against other instances
./scripts/test-jazz_nordic.ps1
./scripts/test-jazz_kth.ps1
```

See `scripts/README.md` for configuration details.

## Extending the Samples

To create a new sample:

1. Create a new class (e.g., `MyCustomSample.cs`)
2. Add a public static `Run()` method that accepts command-line arguments
3. Update `Program.cs` to register the sample in the dispatcher
4. The dispatcher will automatically make it available as: `dotnet run -- mycustom [args]`

Example:
```csharp
public class MyCustomSample
{
    public static async Task Run(string[] args)
    {
        var parser = new ArgumentParser(args);
        var url = parser.GetValue("--url");
        // Your code here
    }
}
```

Then update `Program.cs`:
```csharp
var samples = new Dictionary<string, Type>
{
    ["ewm"] = typeof(EWMSample),
    ["erm"] = typeof(ERMSample),
    ["etm"] = typeof(ETMSample),
    ["mycustom"] = typeof(MyCustomSample),  // Add this line
};
```

## Troubleshooting

### Authentication Fails
- Verify credentials are correct
- Check that the server is accessible from your network
- Ensure form-based authentication is enabled on the server

### Rootservices Not Found
- Make sure authentication happens BEFORE discovery
- Verify the URL is correct (should be the base server URL, not including /rootservices)
- Check that the authenticated HttpClient is passed to DiscoverAsync()

### Query Returns No Results
- Verify the project name/ID is correct
- Check server permissions - ensure your user can access the project
- Try a simpler query first before adding filters

### RDF Parsing Errors
- Some server configurations may return malformed RDF
- Try running against a different server or project
- Check server logs for errors

## Documentation

- [OSLC4Net Documentation](../../docs/index.md)
- [OSLC4Net Architecture Overview](../../docs/guides/oslc4net-architecture-overview.md)
- [Minimal OSLC Client Guide](../../docs/guides/minimal-oslc-client.md)
- [Real-World Testing Results](../TESTING_RESULTS.md)

## License

See repository LICENSE file.
