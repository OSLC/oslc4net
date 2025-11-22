using System.Runtime.CompilerServices;
using EmptyFiles;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Writing;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public static class VerifyInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // or UseSourceFileRelativeDirectory
        Verifier.UseProjectRelativeDirectory("Snapshots");

        FileExtensions.AddTextExtension("ttl");

        VerifierSettings.RegisterFileConverter<IGraph>(
            (graph, _) =>
            {
                return CanonicalizeAndSerializeGraph(graph);
            });
        VerifierSettings.RegisterFileConverter<IResource>(
            (resource, _) =>
            {
                var graph = DotNetRdfHelper.CreateDotNetRdfGraph([resource]);
                return CanonicalizeAndSerializeGraph(graph);
            });
    }

    private static ConversionResult CanonicalizeAndSerializeGraph(IGraph graph)
    {
        using var writer = new System.IO.StringWriter();
        var rdfc = new RdfCanonicalizer();
        using var graphCollection = new GraphCollection
        {
            { graph, false }
        };
        using var tripleStore = new TripleStore(graphCollection);
        var canonicalizedRdfDataset = rdfc.Canonicalize(tripleStore);
        var newGraph = canonicalizedRdfDataset.OutputDataset.Graphs.Single();
        // https://www.w3.org/TR/rdf-canon/ prescribes N-Quads serialization, but let's try Turtle for better readability
        var ttlWriter = new CompressingTurtleWriter();
        ttlWriter.Save(newGraph, writer);
        return new ConversionResult(null, "ttl", writer.ToString());
    }

}
