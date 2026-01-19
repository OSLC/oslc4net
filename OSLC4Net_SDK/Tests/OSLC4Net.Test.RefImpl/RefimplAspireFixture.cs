using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Projects;

namespace OSLC4Net.ChangeManagementTest;

public class RefimplAspireFixture : IAsyncDisposable
{
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
                DistributedApplication = await SetupAspireAsync().ConfigureAwait(false);
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
            .CreateAsync<OSLC4Net_Test_AspireHost>().ConfigureAwait(false);

        //builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        //{
        //    clientBuilder.AddStandardResilienceHandler();
        //});

        // To output logs to the xUnit.net ITestOutputHelper,
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        // builder.
        var app = await builder.BuildAsync().ConfigureAwait(false);

        await app.StartAsync().WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(false);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("refimpl-cm")
            .WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(false);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("refimpl-rm")
            .WaitAsync(TimeSpan.FromSeconds(300)).ConfigureAwait(false);

        var endpoint = app.GetEndpoint("refimpl-cm", "http");
        ServiceProviderCatalogUriCM =
            endpoint.AbsoluteUri + "services/catalog/singleton";
        ServiceProviderCatalogUriRM =
            app.GetEndpoint("refimpl-rm", "http").AbsoluteUri + "services/catalog/singleton";

        // Wait for the catalog endpoints to be fully ready
        // The health check only verifies /services/rootservices, but the catalog
        // may take additional time to initialize, especially on slower CI runners
        await WaitForCatalogReadyAsync(ServiceProviderCatalogUriCM).ConfigureAwait(false);
        await WaitForCatalogReadyAsync(ServiceProviderCatalogUriRM).ConfigureAwait(false);

        return app;
    }

    /// <summary>
    /// Polls the catalog endpoint until it returns 200 OK.
    /// This ensures the service is fully initialized before tests run.
    /// </summary>
    private static async Task WaitForCatalogReadyAsync(string catalogUri, int maxRetries = 30, int delayMs = 1000)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin")));

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var response = await httpClient.GetAsync(catalogUri).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch (HttpRequestException ex)
            {
                // Connection refused or other network error - service not ready yet
                System.Diagnostics.Trace.TraceInformation(
                    "Attempt {0}/{1} to reach catalog '{2}' failed with HttpRequestException: {3}",
                    i + 1,
                    maxRetries,
                    catalogUri,
                    ex.Message);
            }

            await Task.Delay(delayMs).ConfigureAwait(false);
        }

        throw new TimeoutException($"Catalog endpoint {catalogUri} did not become ready within {maxRetries * delayMs / 1000} seconds");
    }

    public string ServiceProviderCatalogUriRM { get; private set; } = null!;

    public string ServiceProviderCatalogUriCM { get; private set; } = null!;
}
