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
        if (ImplementsGenericType(typeof(FilteredResource<>), type))
        {
            var resourceProp = value.GetType().GetProperty("Resource");
            var actualTypeArguments =
                GetChildClassParameterArguments(typeof(FilteredResource<>), type);
            var objects = resourceProp.GetValue(value, null);
            var propertiesProp = value.GetType().GetProperty("Properties");

            if (!ImplementsICollection(actualTypeArguments[0]))
            {
                objects = new EnumerableWrapper(objects);
            }

            if (ImplementsGenericType(typeof(ResponseInfo<>), type))
            {
                //Subject URI for the collection is the query capability
                // FIXME: should this be set by the app based on service provider info
                var portNum = httpContext.Request.Host.Port;
                string portString = null;
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

                graph = DotNetRdfHelper.CreateDotNetRdfGraph(descriptionAbout,
                    responseInfoAbout,
                    (string)nextPageProp.GetValue(value, null),
                    (int)totalCountProp.GetValue(value, null),
                    objects as IEnumerable<object>,
                    (IDictionary<string, object>)propertiesProp.GetValue(value, null));
            }
            else
            {
                graph = DotNetRdfHelper.CreateDotNetRdfGraph(null, null, null, null,
                    objects as IEnumerable<object>,
                    (IDictionary<string, object>)propertiesProp.GetValue(value, null));
            }
        }
        else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                     typeof(IEnumerable<>), value.GetType()))
        {
            graph = DotNetRdfHelper.CreateDotNetRdfGraph(value as IEnumerable<object>);
        }
        else if (type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
        {
            graph = DotNetRdfHelper.CreateDotNetRdfGraph(new[] { value });
        }
        else
        {
            graph = DotNetRdfHelper.CreateDotNetRdfGraph(new EnumerableWrapper(value));
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
