using System.Net;
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
    private const int TimeoutHealthcheck = 1; // seconds

    // between retries, seconds
    private const int DelayHealthcheck = 1;

    public async ValueTask DisposeAsync()
    {
        if (DistributedApplication is not null)
        {
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
            .CreateAsync<OSLC4Net_Test_AspireHost>().ConfigureAwait(true);

        var refimplCM = builder
            .AddDockerfile("refimpl-cm", "../../../../refimpl/src/", "server-cm/Dockerfile")
            .WithEndpoint(8801, 8080, isExternal: true, isProxied: false,
                scheme: "http", name: "http");

        var refimplRM = builder
            .AddDockerfile("refimpl-rm", "../../../../refimpl/src/", "server-rm/Dockerfile")
            .WithEndpoint(8800, 8080, isExternal: true, isProxied: false,
                scheme: "http", name: "http");


        //builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        //{
        //    clientBuilder.AddStandardResilienceHandler();
        //});

        // To output logs to the xUnit.net ITestOutputHelper,
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        await using var app = await builder.BuildAsync().ConfigureAwait(true);

        await app.StartAsync().ConfigureAwait(true);

        ServiceProviderCatalogUriCM =
            refimplCM.GetEndpoint("http").Url + "/services/catalog/singleton";
        // ServiceProviderCatalogUriRM = "http://localhost:8800/services/catalog/singleton";
        ServiceProviderCatalogUriRM =
            refimplRM.GetEndpoint("http").Url + "/services/catalog/singleton";
        // Poll for server availability
        await AwaitRefimplStartupAsync().ConfigureAwait(true);

        return app;
    }

    public string ServiceProviderCatalogUriRM { get; set; }

    public string ServiceProviderCatalogUriCM { get; set; }

    private async Task AwaitRefimplStartupAsync()
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(TimeoutHealthcheck);
        var maxRetries = 180;
        var retryDelay = TimeSpan.FromSeconds(DelayHealthcheck);
        var serverUrlCm = new Uri(ServiceProviderCatalogUriCM);
        var serverUrlRm = new Uri(ServiceProviderCatalogUriRM);

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var response = await httpClient.GetAsync(serverUrlCm).ConfigureAwait(true);
                var responseRm = await httpClient.GetAsync(serverUrlRm).ConfigureAwait(true);
                if (response.StatusCode == HttpStatusCode.Unauthorized &&
                    responseRm.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Server is ready
                    break;
                }
            }
            catch
            {
                // Server not ready yet
            }

            if (i < maxRetries - 1)
            {
                await Task.Delay(retryDelay).ConfigureAwait(true);
            }
            else
            {
                throw new TimeoutException(
                    $"Server at {serverUrlCm} or {serverUrlRm} did not start within {maxRetries * retryDelay.TotalSeconds} seconds");
            }
        }
    }
}
