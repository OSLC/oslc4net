using Aspire.Hosting;
using Aspire.Hosting.Testing;
using OSLC4Net.ChangeManagementTest;
using Projects;
using Xunit;

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
            await DistributedApplication.StopAsync().ConfigureAwait(false);
            await DistributedApplication.DisposeAsync().ConfigureAwait(false);
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
            .CreateAsync<OSLC4Net_Test_AspireHost>().ConfigureAwait(true);

        //builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        //{
        //    clientBuilder.AddStandardResilienceHandler();
        //});

        // To output logs to the xUnit.net ITestOutputHelper,
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        // builder.
        var app = await builder.BuildAsync().ConfigureAwait(false);

        await app.StartAsync().WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(true);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("refimpl-cm")
            .WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(true);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("refimpl-rm")
            .WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(true);

        var endpoint = app.GetEndpoint("refimpl-cm", "http");
        ServiceProviderCatalogUriCM =
            endpoint.AbsoluteUri + "services/catalog/singleton";
        ServiceProviderCatalogUriRM =
            app.GetEndpoint("refimpl-rm", "http").AbsoluteUri + "services/catalog/singleton";

        return app;
    }

    public string ServiceProviderCatalogUriRM { get; private set; }

    public string ServiceProviderCatalogUriCM { get; private set; }
}
