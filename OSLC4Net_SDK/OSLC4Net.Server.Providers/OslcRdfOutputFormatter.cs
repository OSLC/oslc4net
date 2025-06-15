using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
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
    public OslcRdfOutputFormatter()
    {
        SupportedMediaTypes.Add(OslcMediaType.TEXT_TURTLE_TYPE.AsMsNetType());
        SupportedMediaTypes.Add(OslcMediaType.APPLICATION_RDF_XML_TYPE.AsMsNetType());

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context,
        Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var serviceProvider = httpContext.RequestServices;

        var logger = serviceProvider.GetRequiredService<ILogger<OslcRdfOutputFormatter>>();
        var buffer = new StringBuilder();

        var type = context.ObjectType;
        var value = context.Object;
        var httpRequest = httpContext.Request;
        IGraph graph;
        if (type != null && ImplementsGenericType(typeof(FilteredResource<>), type))
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(context),
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

        // TODO: set the default
        var contentType = context.ContentType;
        await SerializeGraph(contentType, graph, httpContext.Response);

        //await httpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);
    }


    private async Task SerializeGraph(StringSegment contentType, IGraph graph,
        HttpResponse httpContextResponse)
    {
        IRdfWriter rdfWriter;

        var requestedMediaType = new MediaType(contentType);

        var requestedType = $"{requestedMediaType.Type}/{requestedMediaType.SubType}";

        if (requestedType.Equals(OslcMediaType.APPLICATION_RDF_XML))
        {
            var rdfXmlWriter = new RdfXmlWriter
            {
                UseDtd = false, PrettyPrintMode = false, CompressionLevel = 20
            };
            //turtlelWriter.UseTypedNodes = false;

            rdfWriter = rdfXmlWriter;
        }
        else if (requestedType.Equals(OslcMediaType.TEXT_TURTLE))
        {
            var turtleWriter = new CompressingTurtleWriter(TurtleSyntax.W3C)
            {
                PrettyPrintMode = true,
                CompressionLevel = WriterCompressionLevel.Minimal,
                HighSpeedModePermitted = true
            };

            rdfWriter = turtleWriter;
        }
        else
        {
            //For now, use the dotNetRDF RdfXmlWriter for application/xml
            //OslcXmlWriter oslcXmlWriter = new OslcXmlWriter();
            var oslcXmlWriter = new RdfXmlWriter
            {
                UseDtd = false, PrettyPrintMode = false, CompressionLevel = 20
            };

            rdfWriter = oslcXmlWriter;
        }

        await using var writer =
            new HttpResponseStreamWriter(httpContextResponse.BodyWriter.AsStream(),
                Encoding.UTF8);
        rdfWriter.Save(graph, writer);
    }
}

public static class RdfOutputFormatterExtensions
{
    // REVISIT: can we drop one of the two?
    public static MediaTypeHeaderValue AsMsNetType(
        this System.Net.Http.Headers.MediaTypeHeaderValue oslcMediaType)
    {
        return MediaTypeHeaderValue.Parse(oslcMediaType.MediaType);
    }
}
