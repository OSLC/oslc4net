using OSLC4Net.Core.DotNetRdfProvider;
using VDS.RDF;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

/// <summary>
/// Regression tests for the ResponseInfo container: a missing next page must not
/// produce an oslc:nextPage triple. OslcRdfOutputFormatter used to coerce a missing
/// next page to string.Empty, which made CreateDotNetRdfGraph call new Uri("") and
/// throw for every non-paged query.
/// </summary>
public class ResponseInfoNextPageTests
{
    private const string Description = "http://localhost/oslc/requirements";
    private const string ResponseInfo = "http://localhost/oslc/requirements?oslc.where=x";
    private const string NextPagePredicate = "http://open-services.net/ns/core#nextPage";
    private const string TotalCountPredicate = "http://open-services.net/ns/core#totalCount";

    [Test]
    public async Task EmptyNextPage_DoesNotThrowAndOmitsNextPageTriple()
    {
        var graph = DotNetRdfHelper.CreateDotNetRdfGraph(
            Description,
            ResponseInfo,
            nextPageAbout: string.Empty,
            totalCount: 3,
            objects: [],
            properties: new Dictionary<string, object>(StringComparer.Ordinal));

        await Assert.That(HasPredicate(graph, NextPagePredicate)).IsFalse();
        await Assert.That(HasPredicate(graph, TotalCountPredicate)).IsTrue();
    }

    [Test]
    public async Task NullNextPage_DoesNotThrowAndOmitsNextPageTriple()
    {
        var graph = DotNetRdfHelper.CreateDotNetRdfGraph(
            Description,
            ResponseInfo,
            nextPageAbout: null,
            totalCount: 3,
            objects: [],
            properties: new Dictionary<string, object>(StringComparer.Ordinal));

        await Assert.That(HasPredicate(graph, NextPagePredicate)).IsFalse();
    }

    [Test]
    public async Task NextPagePresent_EmitsNextPageTriple()
    {
        var nextPage = ResponseInfo + "&oslc.pageSize=10&page=2";

        var graph = DotNetRdfHelper.CreateDotNetRdfGraph(
            Description,
            ResponseInfo,
            nextPageAbout: nextPage,
            totalCount: 20,
            objects: [],
            properties: new Dictionary<string, object>(StringComparer.Ordinal));

        await Assert.That(HasPredicate(graph, NextPagePredicate)).IsTrue();
    }

    private static bool HasPredicate(IGraph graph, string predicate) =>
        graph.GetTriplesWithPredicate(graph.CreateUriNode(new Uri(predicate))).Any();
}
