using Aspire.Hosting;
using Aspire.Hosting.Testing;
using OSLC4Net.ChangeManagementTest;
using Projects;

namespace OSLC4Net.ChangeManagementTest;

public class RefimplAspireFixture : System.IAsyncDisposable{
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async ValueTask DisposeAsync()
    {
        if (DistributedApplication is null)
        {
            return;
        }

        try
        {
            await DistributedApplication.StopAsync().ConfigureAwait(false);
        }
        finally
        {
            await DistributedApplication.DisposeAsync().ConfigureAwait(false);
            DistributedApplication = null;
            _initLock.Dispose();
        }
    }

    public async Task EnsureInitializedAsync()
    {
        if (DistributedApplication != null) return;

        await _initLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (DistributedApplication == null)
            {
                 DistributedApplication = await SetupAspireAsync().ConfigureAwait(true);
            }
        }
        finally
        {
            _initLock.Release();
        }
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