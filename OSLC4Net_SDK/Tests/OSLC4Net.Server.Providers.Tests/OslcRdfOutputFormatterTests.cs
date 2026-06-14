using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using OSLC4Net.Core.Model;
using OSLC4Net.Server.Providers;

namespace OSLC4Net.Server.Providers.Tests;

/// <summary>
/// Tests for serializing an oslc:ResponseInfo container through the output formatter.
/// </summary>
public class OslcRdfOutputFormatterTests
{
    private const string Path = "/oslc/service_provider/abc/requirements";

    private static async Task<string> SerializeAsync(HostString host, string scheme,
        ResponseInfoArray<ServiceProvider> responseInfo)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = scheme;
        httpContext.Request.Host = host;
        httpContext.Request.Path = Path;
        httpContext.Request.QueryString = new QueryString("?oslc.where=dcterms:identifier=\"X\"");
        var body = new MemoryStream();
        httpContext.Response.Body = body;

        var context = new OutputFormatterWriteContext(
            httpContext,
            (stream, encoding) => new StreamWriter(stream, encoding),
            typeof(ResponseInfoArray<ServiceProvider>),
            responseInfo)
        {
            ContentType = OslcMediaType.TEXT_TURTLE,
        };

        await new OslcRdfOutputFormatter().WriteResponseBodyAsync(context, Encoding.UTF8);

        body.Position = 0;
        return await new StreamReader(body).ReadToEndAsync();
    }

    private static ResponseInfoArray<ServiceProvider> EmptyContainer() =>
        new([], new Dictionary<string, object>(StringComparer.Ordinal), 0, (Uri?)null!);

    [Test]
    public async Task NonDefaultPort_DoesNotDuplicatePortInSubjectUri()
    {
        // Regression: the formatter used to append the port to a HostString that already
        // carried it, producing "localhost:7000:7000" and throwing UriFormatException.
        var turtle = await SerializeAsync(new HostString("localhost", 7000), "https",
            EmptyContainer());

        await Assert.That(turtle).Contains("https://localhost:7000" + Path);
        await Assert.That(turtle).DoesNotContain("localhost:7000:7000");
    }

    [Test]
    public async Task EmptyContainer_SerializesTotalCountWithoutMembers()
    {
        // Regression: an empty ResponseInfo (a query that matched nothing) used to throw
        // because the oslc namespace prefix was never registered.
        var turtle = await SerializeAsync(new HostString("localhost", 7000), "https",
            EmptyContainer());

        await Assert.That(turtle).Contains("totalCount");
        await Assert.That(turtle).DoesNotContain("nextPage");
    }
}
