using System.Collections;
using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using OSLC4Net.Core;
using OSLC4Net.Core.Attribute;
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
            _ => throw new NotSupportedException($"Unknown RDF format: {requestedType}"),
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
            graph = await DeserializeRdfQuadsAsync(reader, format, baseUri)
                .ConfigureAwait(false);
        }
        else
        {
            graph =
                await DeserializeRdfTriplesAsync(reader, format, baseUri)
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

        if (graph is null)
        {
            return null;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(OslcRequest<>))
        {
            var resourceType = type.GetGenericArguments()[0];
            var resources = GraphToPolymorphicResources(graph, resourceType);
            return Activator.CreateInstance(type, resources, graph);
        }

        if (RequiresPolymorphicResolution(type))
        {
            var resources = GraphToPolymorphicResources(graph, type);
            return resources is IList { Count: > 0 } list ? list[0] : null;
        }

        var isSingleton = type.IsOslcSingleton();
        Type? polymorphicMemberType = GetEnumerableMemberType(type);
        if (!isSingleton && polymorphicMemberType is not null && RequiresPolymorphicResolution(polymorphicMemberType))
        {
            var resources = GraphToPolymorphicResources(graph, polymorphicMemberType);
            if (type.IsArray)
            {
                return resources.GetType().GetMethod("ToArray", Type.EmptyTypes)?
                    .Invoke(resources, null);
            }

            return resources;
        }

        var memberType = type.GetMemberType();
        if (!isSingleton && memberType is not null && RequiresPolymorphicResolution(memberType))
        {
            var resources = GraphToPolymorphicResources(graph, memberType);
            if (type.IsArray)
            {
                return resources.GetType().GetMethod("ToArray", Type.EmptyTypes)?
                    .Invoke(resources, null);
            }

            return resources;
        }

        var output =
            _rdfHelper.FromDotNetRdfGraph(graph,
                isSingleton ? type : memberType);

        if (isSingleton)
        {
            if (output is IList { Count: > 0 } list)
            {
                return list[0];
            }
            return null;
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

    private static Type? GetEnumerableMemberType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        return type.GetInterfaces()
            .Where(static interfaceType => interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(static interfaceType => interfaceType.GetGenericArguments()[0])
            .FirstOrDefault();
    }

    private object GraphToPolymorphicResources(IGraph graph, Type requestedType)
    {
        var listType = typeof(List<>).MakeGenericType(requestedType);
        var resources = (IList)Activator.CreateInstance(listType)!;
        var add = listType.GetMethod("Add", [requestedType])!;
        foreach ((IUriNode subject, Type concreteType) in ResolveResourceTypes(graph, requestedType))
        {
            var resource = _rdfHelper.FromDotNetRdfNode(subject, graph, concreteType);
            add.Invoke(resources, [resource]);
        }

        return resources;
    }

    private static bool RequiresPolymorphicResolution(Type type)
    {
        return type.IsInterface || type.IsAbstract;
    }

    private static IEnumerable<(IUriNode Subject, Type ConcreteType)> ResolveResourceTypes(
        IGraph graph,
        Type requestedType)
    {
        IUriNode rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        Dictionary<string, Type> candidateTypes = GetCandidateResourceTypes(requestedType);
        var candidatesBySubject = new Dictionary<IUriNode, List<Type>>();
        foreach (Triple triple in graph.GetTriplesWithPredicate(rdfType))
        {
            if (triple.Subject is not IUriNode subject || triple.Object is not IUriNode objectNode)
            {
                continue;
            }

            if (!candidateTypes.TryGetValue(objectNode.Uri.AbsoluteUri, out Type? candidateType))
            {
                continue;
            }

            if (!candidatesBySubject.TryGetValue(subject, out List<Type>? subjectCandidates))
            {
                subjectCandidates = new List<Type>();
                candidatesBySubject.Add(subject, subjectCandidates);
            }

            if (!subjectCandidates.Contains(candidateType))
            {
                subjectCandidates.Add(candidateType);
            }
        }

        return candidatesBySubject
            .OrderBy(entry => entry.Key.Uri.AbsoluteUri, StringComparer.Ordinal)
            .Select(entry => (entry.Key, GetMostConcreteType(entry.Key, entry.Value)));
    }

    private static Dictionary<string, Type> GetCandidateResourceTypes(Type requestedType)
    {
        var candidates = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
            .Where(static assembly => !assembly.IsDynamic)
            .SelectMany(GetLoadableTypes)
            .Where(type => !type.IsAbstract &&
                !type.IsInterface &&
                requestedType.IsAssignableFrom(type)))
        {
            foreach (OslcResourceShape resourceShape in type.GetCustomAttributes(typeof(OslcResourceShape), false))
            {
                foreach (string describedType in resourceShape.describes)
                {
                    candidates[describedType] = type;
                }
            }
        }

        return candidates;
    }

    private static IEnumerable<Type> GetLoadableTypes(System.Reflection.Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (System.Reflection.ReflectionTypeLoadException ex)
        {
            return ex.Types.OfType<Type>();
        }
    }

    private static Type GetMostConcreteType(IUriNode subject, List<Type> candidates)
    {
        for (int i = candidates.Count - 1; i >= 0; i--)
        {
            Type current = candidates[i];
            if (candidates.Any(candidate => current != candidate && current.IsAssignableFrom(candidate)))
            {
                candidates.RemoveAt(i);
            }
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        throw new InvalidOperationException(
            $"Multiple unrelated CLR resource types match RDF resource '{subject.Uri}': {string.Join(", ", candidates.Select(static type => type.FullName))}.");
    }

    private static async Task<IGraph> DeserializeRdfTriplesAsync(StreamReader streamReader,
        RdfFormat format, Uri baseUri)
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

    private static async Task<IGraph> DeserializeRdfQuadsAsync(StreamReader streamReader,
        RdfFormat format, Uri baseUri)
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
