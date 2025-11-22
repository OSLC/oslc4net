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

        /* Without credentials, our healthcheck can only sense the 500->401 transition. However,
         * after this, there may be some time until the Jakarta REST providers are fully initialized
         * to process authenticated requests. Normally, the client would absorb transient failures.
         */
        // TODO: remove once OSLC4Net client is integrated with Polly (#460)
        await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(true);

        return app;
    }

    public string ServiceProviderCatalogUriRM { get; private set; }

    public string ServiceProviderCatalogUriCM { get; private set; }
}
