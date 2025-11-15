using Aspire.Hosting.Testing;
using TUnit.Core;

namespace OSLC4Net.ChangeManagementTest;

[AssemblySetUp]
public class AspireAppLifecycle
{
    public static DistributedApplication? DistributedApplication { get; private set; }
    public static string? ServiceProviderCatalogUriCM { get; private set; }
    public static string? ServiceProviderCatalogUriRM { get; private set; }

    [SetUp]
    public async Task SetUp()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<OSLC4Net_Test_AspireHost>();
        DistributedApplication = await builder.BuildAsync();
        await DistributedApplication.StartAsync();
        ServiceProviderCatalogUriCM = DistributedApplication.GetEndpoint("oslc-cm-refimpl", "http").AbsoluteUri + "services/catalog";
        ServiceProviderCatalogUriRM = DistributedApplication.GetEndpoint("oslc-rm-refimpl", "http").AbsoluteUri + "services/catalog";
    }

    [AssemblyCleanUp]
    public async Task CleanUp()
    {
        if (DistributedApplication is not null)
        {
            await DistributedApplication.StopAsync();
            await DistributedApplication.DisposeAsync();
        }
    }
}
