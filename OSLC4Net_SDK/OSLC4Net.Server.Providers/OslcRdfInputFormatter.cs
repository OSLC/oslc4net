using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using OSLC4Net.Core;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.JsonLd.Syntax;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;

namespace OSLC4Net.Server.Providers;

public class OslcRdfInputFormatter : TextInputFormatter
{
    private readonly DotNetRdfHelper _rdfHelper;

    public OslcRdfInputFormatter(DotNetRdfHelper? rdfHelper = null)
    {
        _rdfHelper = rdfHelper ?? Activator.CreateInstance<DotNetRdfHelper>();

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.APPLICATION_RDF_XML));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.TEXT_TURTLE));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.APPLICATION_JSON_LD));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.APPLICATION_NTRIPLES));

        SupportedEncodings.Add(Encoding.UTF8);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(
        InputFormatterContext context,
        Encoding encoding)
    {
        Guard.IsNotNull(context, nameof(context));
        Guard.IsNotNull(context.HttpContext.Request.ContentType);
        var contentType = context.HttpContext.Request.ContentType!;

        var requestedMediaType = new MediaType(contentType);
        var requestedType = $"{requestedMediaType.Type}/{requestedMediaType.SubType}";

        var format = requestedType switch
        {
            OslcMediaType.APPLICATION_RDF_XML => RdfFormat.RdfXml,
            OslcMediaType.TEXT_TURTLE => RdfFormat.Turtle,
            OslcMediaType.APPLICATION_NTRIPLES => RdfFormat.NTriples,
            OslcMediaType.APPLICATION_JSON_LD => RdfFormat.JsonLd,
            _ => throw new ArgumentOutOfRangeException(nameof(requestedType),
                "Unknown RDF format"),
        };
        using var reader = new StreamReader(context.HttpContext.Request.Body, encoding);

        // REVISIT: we need a more robust way to obtain request URI
        context.HttpContext.Request.Headers.TryGetValue(OSLC4NetConstants.INNER_URI_HEADER,
            out var shuttleRequestUri);
        var baseUri = shuttleRequestUri.SingleOrDefault()?.ToSafeUri() ??
                      context.HttpContext.Request.GetEncodedUrl().ToSafeUri();

        IGraph graph;
        if (format == RdfFormat.JsonLd)
        {
            graph = await DeserializeRdfQuadsAsync(reader, format, context.ModelType, baseUri)
                .ConfigureAwait(false);
        }
        else
        {
            graph =
                await DeserializeRdfTriplesAsync(reader, format, context.ModelType, baseUri)
                    .ConfigureAwait(false);
        }

        var obj = GraphToObjects(graph, context.ModelType);

        return await InputFormatterResult.SuccessAsync(obj).ConfigureAwait(false);
    }

    private object? GraphToObjects(IGraph? graph, Type type)
    {
        if (type == typeof(Graph) || type == typeof(BaseGraph) || type == typeof(IGraph))
        {
            return graph;
        }

        var isSingleton = type.IsOslcSingleton();
        var output =
            _rdfHelper.FromDotNetRdfGraph(graph,
                isSingleton ? type : type.GetMemberType());

        if (isSingleton)
        {
            var haveOne =
                (int)output.GetType().GetProperty("Count")?.GetValue(output, null) > 0;

            return haveOne
                ? output.GetType().GetProperty("Item")?.GetValue(output, new object[] { 0 })
                : null;
        }
        else if (type.IsArray)
        {
            return output.GetType().GetMethod("ToArray", Type.EmptyTypes)?
                .Invoke(output, null);
        }
        else
        {
            return output;
        }
    }

    private async Task<IGraph> DeserializeRdfTriplesAsync(StreamReader streamReader,
        RdfFormat format,
        Type contextModelType, Uri baseUri)
    {
        IRdfReader? tripleReader = format switch
        {
            RdfFormat.RdfXml => new RdfXmlParser(),
            RdfFormat.Turtle => new TurtleParser(TurtleSyntax.Rdf11Star, false),
            RdfFormat.NTriples => new NTriplesParser(),
            RdfFormat.JsonLd => throw new NotSupportedException(
                "This method supports only triple-based formats, use quad-based method"),
            _ => throw new ArgumentOutOfRangeException(nameof(format), "Unknown RDF format"),
        };

        IGraph graph = new Graph();
        graph.BaseUri = baseUri;

        using (streamReader)
        {
            tripleReader.Load(graph, streamReader);

            // REVISIT: make RDFS reasoning configurable (@berezovskyi 2025-05)
            // TODO: make schema loads configurable (@berezovskyi 2025-05)
            var reasoner = new StaticRdfsReasoner();
            // reasoner.Initialise(schema);
            reasoner.Apply(graph);

            return graph;
        }
    }

    private async Task<IGraph> DeserializeRdfQuadsAsync(StreamReader streamReader,
        RdfFormat format,
        Type type, Uri baseUri)
    {
        IStoreReader? quadReader = format switch
        {
            RdfFormat.JsonLd => new JsonLdParser(new JsonLdProcessorOptions
            {
                ProcessingMode = JsonLdProcessingMode.JsonLd11
            }),
            RdfFormat.RdfXml or
                RdfFormat.Turtle or RdfFormat.NTriples => throw new NotSupportedException(
                    "This method supports only quad-based formats, use triple-based method"),
            _ => throw new ArgumentOutOfRangeException(nameof(format), "Unknown RDF format"),
        };

        IGraph graph = new Graph();
        graph.BaseUri = baseUri;

        using (streamReader)
        {
            var quadStore = new TripleStore();
            quadReader.Load(quadStore, streamReader);
            // REVISIT: for now we support single graph in JSON-LD payloads (@berezovskyi 2025-05)
            graph = quadStore.Graphs.Single();

            // REVISIT: make RDFS reasoning configurable (@berezovskyi 2025-05)
            // TODO: make schema loads configurable (@berezovskyi 2025-05)
            var reasoner = new StaticRdfsReasoner();
            // reasoner.Initialise(schema);
            reasoner.Apply(graph);

            return graph;
        }
    }
}
