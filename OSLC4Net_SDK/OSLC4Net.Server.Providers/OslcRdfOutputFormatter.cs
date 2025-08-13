using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using static OSLC4Net.Core.DotNetRdfProvider.RdfXmlMediaTypeFormatter;

namespace OSLC4Net.Server.Providers;

public class OslcRdfOutputFormatter : TextOutputFormatter
{
    private readonly OslcOutputFormatConfig _config;

    // From https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/custom-formatters?view=aspnetcore-9.0#specify-supported-media-types-and-encodings
    // A formatter class can not use constructor injection for its dependencies. For example,
    // ILogger<VcardOutputFormatter> can't be added as a parameter to the constructor. To access
    // services, use the context object that gets passed in to the methods. A code example in this
    // article and the sample show how to do this.
    public OslcRdfOutputFormatter(OslcOutputFormatConfig? config = null)
    {
        _config = config ?? new OslcOutputFormatConfig();

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.APPLICATION_RDF_XML));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.TEXT_TURTLE));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.APPLICATION_JSON_LD));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(OslcMediaType.APPLICATION_NTRIPLES));

        SupportedEncodings.Add(Encoding.UTF8);
        // SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context,
        Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;

        // var logger = httpContext.RequestServices.GetRequiredService<ILogger<OslcRdfOutputFormatter>>();

        var type = context.ObjectType;
        var value = context.Object;
        var httpRequest = httpContext.Request;
        var graph = ConvertOslcObjectsToGraph(type, value, httpContext, httpRequest);

        var contentType = context.ContentType.ToString();
        var requestedMediaType = new MediaType(contentType);
        var requestedType = $"{requestedMediaType.Type}/{requestedMediaType.SubType}";

        var ctx = new SerializationContext
        {
            Format = requestedType switch
            {
                OslcMediaType.APPLICATION_RDF_XML => RdfFormat.RdfXml,
                OslcMediaType.TEXT_TURTLE => RdfFormat.Turtle,
                OslcMediaType.APPLICATION_NTRIPLES => RdfFormat.NTriples,
                OslcMediaType.APPLICATION_JSON_LD => RdfFormat.JsonLd,
                _ => throw new ArgumentOutOfRangeException(nameof(requestedType),
                    "Unknown RDF format"),
            },
            Graph = graph
        };

        await SerializeToRdfAsync(ctx, httpContext.Response).ConfigureAwait(false);
    }

    private static IGraph ConvertOslcObjectsToGraph(Type? type, object? value,
        HttpContext httpContext,
        HttpRequest httpRequest)
    {
        IGraph graph;
        if (type != null && ImplementsGenericType(typeof(FilteredResource<>), type))
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value),
                    "Value cannot be null for FilteredResource");
            }

            var resourceProp = value.GetType().GetProperty("Resource");
            var actualTypeArguments =
                GetChildClassParameterArguments(typeof(FilteredResource<>), type);
            var objects = resourceProp?.GetValue(value, null);
            var propertiesProp = value.GetType().GetProperty("Properties");
            if (!ImplementsICollection(actualTypeArguments[0]))
            {
                if (objects != null)
                {
                    objects = new EnumerableWrapper(objects);
                }
            }

            if (ImplementsGenericType(typeof(ResponseInfo<>), type))
            {
                //Subject URI for the collection is the query capability
                // FIXME: should this be set by the app based on service provider info
                var portNum = httpContext.Request.Host.Port;
                string? portString;
                if (portNum == 80 || portNum == 443)
                {
                    portString = "";
                }
                else
                {
                    portString = ":" + portNum;
                }

                var descriptionAbout = httpRequest.Scheme + "://" +
                                       httpRequest.Host +
                                       portString +
                                       httpRequest.Path;

                //Subject URI for the responseInfo is the full request URI
                var responseInfoAbout = httpRequest.GetEncodedUrl();
                var totalCountProp = value.GetType().GetProperty("TotalCount");
                var nextPageProp = value.GetType().GetProperty("NextPage");

                var nextPageValue = nextPageProp?.GetValue(value, null) as string ?? string.Empty;
                var totalCountValue = totalCountProp?.GetValue(value, null);
                var totalCount = totalCountValue as int? ?? 0;
                var propertiesValue =
                    propertiesProp?.GetValue(value, null) as IDictionary<string, object>;

                graph = DotNetRdfHelper.CreateDotNetRdfGraph(descriptionAbout,
                    responseInfoAbout,
                    nextPageValue,
                    totalCount,
                    objects as IEnumerable<object> ?? Enumerable.Empty<object>(),
                    propertiesValue ?? new Dictionary<string, object>(StringComparer.Ordinal));
            }
            else
            {
                var propertiesValue =
                    propertiesProp?.GetValue(value, null) as IDictionary<string, object>;

                graph = DotNetRdfHelper.CreateDotNetRdfGraph(string.Empty, string.Empty,
                    string.Empty, null,
                    objects as IEnumerable<object> ?? Enumerable.Empty<object>(),
                    propertiesValue ?? new Dictionary<string, object>(StringComparer.Ordinal));
            }
        }
        else if (value != null && InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                     typeof(IEnumerable<>), value.GetType()))
        {
            graph = DotNetRdfHelper.CreateDotNetRdfGraph(value as IEnumerable<object> ??
                                                         Enumerable.Empty<object>());
        }
        else if (type != null &&
                 type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
        {
            graph = DotNetRdfHelper.CreateDotNetRdfGraph(value != null
                ? new[] { value }
                : Enumerable.Empty<object>());
        }
        else
        {
            graph = DotNetRdfHelper.CreateDotNetRdfGraph(value != null
                ? new EnumerableWrapper(value)
                : Enumerable.Empty<object>());
        }

        return graph;
    }

    private async Task SerializeToRdfAsync(SerializationContext ctx,
        HttpResponse httpContextResponse)
    {
        // TODO: deal with namespaces
        var responseStreamWriter = new HttpResponseStreamWriter(
            httpContextResponse.BodyWriter.AsStream(),
            Encoding.UTF8);
        if (ctx.Format is RdfFormat.Turtle or RdfFormat.NTriples or RdfFormat.RdfXml)
        {
            await SerializeTriplesAsync(ctx, responseStreamWriter).ConfigureAwait(false);
        }
        else if (ctx.Format is RdfFormat.JsonLd)
        {
            await SerializeQuadsAsync(ctx, responseStreamWriter).ConfigureAwait(false);
        }
    }

    private async Task SerializeTriplesAsync(SerializationContext ctx,
        HttpResponseStreamWriter textWriter)
    {
        IRdfWriter triplesWriter = ctx.Format switch
        {
            RdfFormat.RdfXml => new PrettyRdfXmlWriter
            {
                UseDtd = _config.UseDtd,
                PrettyPrintMode = _config.PrettyPrint,
                CompressionLevel = _config.CompressionLevel
            },
            RdfFormat.Turtle => new CompressingTurtleWriter(TurtleSyntax.W3C)
            {
                PrettyPrintMode = _config.PrettyPrint,
                CompressionLevel = _config.CompressionLevel,
                HighSpeedModePermitted = !_config.PrettyPrint
            },
            RdfFormat.NTriples => new NTriplesWriter(NTriplesSyntax.Rdf11)
            {
                SortTriples = _config.PrettyPrint
            },
            RdfFormat.JsonLd => throw new NotSupportedException(
                "This method supports only triple-based formats, use quad-based method"),
            _ => throw new ArgumentOutOfRangeException(nameof(ctx), "Unknown RDF format"),
        };

        await using (textWriter.ConfigureAwait(false))
        {
            triplesWriter.Save(ctx.Graph, textWriter);
        }
    }

    private async Task SerializeQuadsAsync(SerializationContext ctx,
        HttpResponseStreamWriter textWriter)
    {
        IStoreWriter quadsWriter = ctx.Format switch
        {
            RdfFormat.JsonLd => new JsonLdWriter(new JsonLdWriterOptions
            {
                JsonFormatting = _config.PrettyPrint ? Formatting.Indented : Formatting.None,
                Ordered = _config.PrettyPrint,
                ProcessingMode = _config.JsonLdMode
            }),
            RdfFormat.NTriples or RdfFormat.RdfXml or RdfFormat.Turtle => throw
                new NotSupportedException(
                    "This method supports only quad-based formats, use triple-based method"),
            _ => throw new ArgumentOutOfRangeException(nameof(ctx), "Unknown RDF format"),
        };

        await using (textWriter.ConfigureAwait(false))
        {
            var graphCollection = new GraphCollection();
            graphCollection.Add(ctx.Graph, true);
            var quadStore = new TripleStore(graphCollection);
            quadsWriter.Save(quadStore, textWriter);
        }
    }
}
