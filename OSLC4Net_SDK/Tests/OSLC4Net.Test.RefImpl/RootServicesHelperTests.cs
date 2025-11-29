using System;
using System.Threading.Tasks;
using OSLC4Net.Client.Oslc.Helpers;

namespace OSLC4Net.ChangeManagementTest;

[ClassDataSource<RefimplAspireFixture>(Shared = SharedType.PerAssembly)]
[Property("TestCategory", "RunningOslcServerRequired")]
public class RootServicesHelperTests
{
    private readonly RefimplAspireFixture _fixture;

    public RootServicesHelperTests(RefimplAspireFixture fixture)
    {
        _fixture = fixture;
    }

    [Before(Test)]
    public async Task Setup()
    {
        await _fixture.EnsureInitializedAsync().ConfigureAwait(true);
    }

    [Test]
    public async Task DiscoversCatalogUrl_FromRootservicesOrWellKnown()
    {
        // Extract base URL (scheme://host:port) from catalog URI and add /services
        var catalogUri = new Uri(_fixture.ServiceProviderCatalogUriRM);
        var baseUrl = catalogUri.GetLeftPart(UriPartial.Authority) + "/services";
        var helper = new RootServicesHelper(
            baseUrl,
            "http://open-services.net/xmlns/rm/1.0/",
            "rmServiceProviders");
        var doc = await helper.DiscoverAsync().ConfigureAwait(true);
        CommunityToolkit.Diagnostics.Guard.IsNotNull(doc);
        CommunityToolkit.Diagnostics.Guard.IsNotNull(doc.ServiceProviderCatalog);
        CommunityToolkit.Diagnostics.Guard.IsTrue(doc.ServiceProviderCatalog.Contains("catalog"));
    }
}
