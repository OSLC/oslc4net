using System.Runtime.CompilerServices;
using EmptyFiles;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public static class VerifyInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Verifier.UseProjectRelativeDirectory("Snapshots");
        Verifier.UseSourceFileRelativeDirectory("Snapshots");

        // VerifierSettings.AddExtraSettings(
        //     _ => _.TypeNameHandling = TypeNameHandling.All);
        // VerifierSettings.AddScrubber(_ => _.Replace("String to verify", "new value"));

        FileExtensions.AddTextExtension("ttl");

        VerifierSettings.RegisterFileConverter<IGraph>((graph, context) =>
        {
            using var writer = new StringWriter();

            var rdfc = new RdfCanonicalizer();
            var graphCollection = new GraphCollection
            {
                { graph, false }
            };
            var canonicalizedRdfDataset = rdfc.Canonicalize(new TripleStore(graphCollection));

            var newGraph = canonicalizedRdfDataset.OutputDataset.Graphs.Single();

            // https://www.w3.org/TR/rdf-canon/ prescribes N-Quads serialization, but let's try Turtle for better readability
            var ttlWriter = new VDS.RDF.Writing.CompressingTurtleWriter();
            ttlWriter.Save(newGraph, writer);
            return new(null, "ttl", writer.ToString());
        });

        VerifierSettings.RegisterFileConverter<IResource>((resource, context) =>
        {
            using var writer = new StringWriter();
            var graph = DotNetRdfHelper.CreateDotNetRdfGraph([resource]);

            var rdfc = new RdfCanonicalizer();
            var graphCollection = new GraphCollection
            {
                { graph, false }
            };
            var canonicalizedRdfDataset = rdfc.Canonicalize(new TripleStore(graphCollection));

            var newGraph = canonicalizedRdfDataset.OutputDataset.Graphs.Single();

            // https://www.w3.org/TR/rdf-canon/ prescribes N-Quads serialization, but let's try Turtle for better readability
            var ttlWriter = new VDS.RDF.Writing.CompressingTurtleWriter();
            // ttlWriter.Save(graph, writer);
            ttlWriter.Save(newGraph, writer);
            return new(null, "ttl", writer.ToString());
        });
    }
}
