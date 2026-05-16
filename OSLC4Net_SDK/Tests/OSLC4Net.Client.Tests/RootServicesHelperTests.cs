using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OSLC4Net.Client.Oslc.Helpers;
using TUnit.Core;

namespace OSLC4Net.Client.Tests
{
    public class RootServicesHelperTests
    {
        private static string SampleRootServicesRdf(string catalogUri, string ns, string prop)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
                 + "<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">"
                 + "<rdf:Description rdf:about=\"http://example.com/rootservices\">"
                 + $"<ns:{prop} xmlns:ns=\"{ns}\" rdf:resource=\"{catalogUri}\" />"
                 + "</rdf:Description>"
                 + "</rdf:RDF>";
        }

        [Test]
        public async Task DiscoverCatalog_UsesWellKnown_First_When_Legacy_Missing()
        {
            var baseUrl = "https://example.com/oslc";
            var ns = "http://open-services.net/ns/cm#";
            var prop = "cmServiceProviders";
            var expectedCatalog = "https://example.com/cm/catalog";

            var handler = new FakeHttpMessageHandler((req, ct) =>
            {
                var url = req.RequestUri!.ToString();
                if (url == "https://example.com/.well-known/oslc/rootservices.xml")
                {
                    var xml = SampleRootServicesRdf(expectedCatalog, ns, prop);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(xml, Encoding.UTF8, "application/rdf+xml"),
                        RequestMessage = req
                    });
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = req });
            });
            var httpClient = new HttpClient(handler);

            var helper = new RootServicesHelper(baseUrl, ns, prop);
            var doc = await helper.DiscoverAsync(httpClient);

            await Assert.That(doc.ServiceProviderCatalog).IsEqualTo(expectedCatalog);
        }

        [Test]
        public async Task DiscoverCatalog_FallsBack_To_BaseUrl_Plus_Rootservices()
        {
            var baseUrl = "https://example.com/services";
            var ns = "http://open-services.net/ns/rm#";
            var prop = "rmServiceProviders";
            var expectedCatalog = "https://example.com/rm/sp";

            var handler = new FakeHttpMessageHandler((req, ct) =>
            {
                var url = req.RequestUri!.ToString();
                if (url == "https://example.com/.well-known/oslc/rootservices.xml")
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = req });
                }
                if (url == "https://example.com/services/rootservices")
                {
                    var xml = SampleRootServicesRdf(expectedCatalog, ns, prop);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(xml, Encoding.UTF8, "application/rdf+xml"),
                        RequestMessage = req
                    });
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = req });
            });
            var httpClient = new HttpClient(handler);

            var helper = new RootServicesHelper(baseUrl, ns, prop);
            var doc = await helper.DiscoverAsync(httpClient);

            await Assert.That(doc.ServiceProviderCatalog).IsEqualTo(expectedCatalog);
        }
    }
}
