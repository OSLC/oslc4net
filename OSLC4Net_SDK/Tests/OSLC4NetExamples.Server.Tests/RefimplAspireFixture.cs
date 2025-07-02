using Aspire.Hosting;
using Aspire.Hosting.Testing;
using OSLC4Net.ChangeManagementTest;
using Xunit;
using Projects;

// TODO: consider a refimpl-aspire collection instead https://xunit.net/docs/shared-context#collection-fixture (@berezovskyi 2025-04)
[assembly: AssemblyFixture(typeof(RefimplAspireFixture))]
[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace OSLC4Net.ChangeManagementTest;

public class RefimplAspireFixture : IAsyncLifetime
{
    public async ValueTask DisposeAsync()
    {
        if (DistributedApplication is not null)
        {
            await DistributedApplication.StopAsync();
            await DistributedApplication.DisposeAsync();
        }
    }

    public async ValueTask InitializeAsync()
    {
        DistributedApplication ??= await SetupAspireAsync().ConfigureAwait(true);
    }

    public DistributedApplication? DistributedApplication { get; set; }

    protected async Task<DistributedApplication> SetupAspireAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<OSLC4NetExamples_Server_Tests_AspireHost>().ConfigureAwait(true);

        var app = await builder.BuildAsync();

        await app.StartAsync().WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(true);

        // Wait a bit for the application to fully start
        await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(true);

        NetCoreApiBaseUri = 
            app.GetEndpoint("oslc-netcore-api", "api-https").AbsoluteUri;

        return app;
    }

    public string NetCoreApiBaseUri { get; private set; } = null!;
}
