/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.JsonLd.Syntax;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;
using VDS.RDF.Writing;

namespace OSLC4Net.Core.DotNetRdfProvider;

/// <summary>
///     A class to
///     - read RDF/XML from an input stream and create .NET objects.
///     - write .NET objects to an output stream as RDF/XML
/// </summary>
public class RdfXmlMediaTypeFormatter : MediaTypeFormatter
{
    const int STREAM_BUFFER_SIZE = 8192;
    private HttpRequestMessage? httpRequest;
    private readonly DotNetRdfHelper _rdfHelper;

    /// <summary>
    ///     Defauld RdfXml formatter
    /// </summary>
    /// <param name="graph"></param>
    public RdfXmlMediaTypeFormatter(DotNetRdfHelper? rdfHelper = null, bool rebuildgraph = true)
    {
        _rdfHelper = rdfHelper ?? Activator.CreateInstance<DotNetRdfHelper>();
        RebuildGraph = rebuildgraph;

        SupportedMediaTypes.Add(OslcMediaType.APPLICATION_RDF_XML_TYPE);
        SupportedMediaTypes.Add(OslcMediaType.APPLICATION_XML_TYPE);
        SupportedMediaTypes.Add(OslcMediaType.TEXT_XML_TYPE);
        SupportedMediaTypes.Add(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML_TYPE);
        SupportedMediaTypes.Add(OslcMediaType.TEXT_TURTLE_TYPE);
    }

    /// <summary>
    ///     RdfXml formatter which accepts a pre-built RDF Graph
    /// </summary>
    /// <param name="graph"></param>
    public RdfXmlMediaTypeFormatter(
        IGraph graph,
        DotNetRdfHelper? rdfHelper,
        bool rebuildgraph = true
    ) : this(rdfHelper, rebuildgraph)
    {
        Graph = graph;
    }

    public IGraph? Graph { get; set; }
    public bool RebuildGraph { get; set; }

    /// <summary>
    ///     Save the HttpRequestMessage locally for use during serialization.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="request"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type,
        HttpRequestMessage request, MediaTypeHeaderValue mediaType)
    {
        httpRequest = request;
        return base.GetPerRequestFormatterInstance(type, request, mediaType);
    }

    /// <summary>
    ///     Test the write-ability of a type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public override bool CanWriteType(Type type)
    {
        Type actualType;

        if (ImplementsGenericType(typeof(FilteredResource<>), type))
        {
            var actualTypeArguments =
                GetChildClassParameterArguments(typeof(FilteredResource<>), type);

            if (actualTypeArguments.Length != 1)
            {
                return false;
            }

            if (ImplementsICollection(actualTypeArguments[0]))
            {
                actualTypeArguments = actualTypeArguments[0].GetGenericArguments();

                if (actualTypeArguments.Length != 1)
                {
                    return false;
                }

                actualType = actualTypeArguments[0];
            }
            else
            {
                actualType = actualTypeArguments[0];
            }
        }
        else
        {
            actualType = type;
        }

        if (IsOslcSingleton(actualType))
        {
            return true;
        }

        var memberType = GetMemberType(type);

        if (memberType == null)
        {
            return false;
        }

        return memberType.GetCustomAttributes(typeof(OslcResourceShape), true).Length > 0;
    }

    /// <summary>
    ///     Write a .NET object to an output stream
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="writeStream"></param>
    /// <param name="content"></param>
    /// <param name="transportContext"></param>
    /// <returns></returns>
    public override async Task WriteToStreamAsync(
        Type type,
        object value,
        Stream writeStream,
        HttpContent content,
        TransportContext transportContext
    )
    {

        if (Graph == null || Graph.IsEmpty || RebuildGraph)
        {
            if (ImplementsGenericType(typeof(FilteredResource<>), type))
            {
                var resourceProp = value.GetType().GetProperty("Resource")!;
                var actualTypeArguments =
                    GetChildClassParameterArguments(typeof(FilteredResource<>), type);
                var objects = resourceProp.GetValue(value, null);
                var propertiesProp = value.GetType().GetProperty("Properties")!;

                if (!ImplementsICollection(actualTypeArguments[0]))
                {
                    objects = new EnumerableWrapper(objects!);
                }

                if (ImplementsGenericType(typeof(ResponseInfo<>), type))
                {
                    //Subject URI for the collection is the query capability
                    // FIXME: should this be set by the app based on service provider info
                    var portNum = httpRequest!.RequestUri!.Port;
                    string portString;
                    if (portNum == 80 || portNum == 443)
                    {
                        portString = string.Empty;
                    }
                    else
                    {
                        portString = $":{portNum}";
                    }

                    var descriptionAbout = httpRequest.RequestUri.Scheme + "://" +
                                           httpRequest.RequestUri.Host +
                                           portString +
                                           httpRequest.RequestUri.LocalPath;

                    //Subject URI for the responseInfo is the full request URI
                    var responseInfoAbout = httpRequest.RequestUri.ToString();

                    var totalCountProp = value.GetType().GetProperty("TotalCount")!;
                    var nextPageProp = value.GetType().GetProperty("NextPage")!;

                    Graph = DotNetRdfHelper.CreateDotNetRdfGraph(descriptionAbout,
                        responseInfoAbout,
                        (string?)nextPageProp.GetValue(value, null),
                        (long?)totalCountProp.GetValue(value, null),
                        objects as IEnumerable<object>,
                        (IDictionary<string, object>?)propertiesProp.GetValue(value, null));
                }
                else
                {
                    Graph = DotNetRdfHelper.CreateDotNetRdfGraph(null, null, null, null,
                        objects as IEnumerable<object>,
                        (IDictionary<string, object>?)propertiesProp.GetValue(value, null));
                }
            }
            else if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(
                         typeof(IEnumerable<>), value.GetType()))
            {
                Graph = DotNetRdfHelper.CreateDotNetRdfGraph(value as IEnumerable<object>);
            }
            else if (type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
            {
                Graph = DotNetRdfHelper.CreateDotNetRdfGraph(new[] { value });
            }
            else
            {
                Graph = DotNetRdfHelper.CreateDotNetRdfGraph(new EnumerableWrapper(value));
            }
        }

        IRdfWriter? tripleWriter = null;
        IStoreWriter? quadWriter = null;

        if (content?.Headers?.ContentType?.MediaType == null ||
            content.Headers.ContentType.MediaType.Equals(OslcMediaType.APPLICATION_RDF_XML,
                StringComparison.Ordinal))
        {
            // REVISIT: make Turtle the default (@berezovskyi 2025-05)
            var rdfXmlWriter = new RdfXmlWriter
            {
                UseDtd = false,
                PrettyPrintMode = true,
                CompressionLevel = 20
            };

            tripleWriter = rdfXmlWriter;
        }
        else if (content.Headers.ContentType.MediaType.Equals(OslcMediaType.TEXT_TURTLE,
                     StringComparison.Ordinal))
        {
            var turtlelWriter = new CompressingTurtleWriter(TurtleSyntax.W3C)
            {
                PrettyPrintMode = true,
                CompressionLevel = 20
            };

            tripleWriter = turtlelWriter;
        }
        else if (content.Headers.ContentType.MediaType.Equals(OslcMediaType.APPLICATION_JSON_LD,
                     StringComparison.Ordinal))
        {
            // REVISIT: should be configurable to lower the overhead (@berezovskyi 2025-05)
            var writer = new JsonLdWriter(new JsonLdWriterOptions
            {
                JsonFormatting = Formatting.Indented,
                Ordered = true,
                ProcessingMode = JsonLdProcessingMode.JsonLd11
            });
            quadWriter = writer;
        }
        else
        {
            //For now, use the dotNetRDF RdfXmlWriter for application/xml
            //OslcXmlWriter oslcXmlWriter = new OslcXmlWriter();
            var oslcXmlWriter = new RdfXmlWriter
            {
                UseDtd = false,
                PrettyPrintMode = false,
                CompressionLevel = 20
            };

            tripleWriter = oslcXmlWriter;
        }

        StreamWriter streamWriter = new NonClosingStreamWriter(writeStream);

        if (tripleWriter is not null)
        {
            tripleWriter.Save(Graph, streamWriter);
        }
        else if (quadWriter is not null)
        {
            var graphCollection = new GraphCollection();
            graphCollection.Add(Graph, true);
            var quadStore = new TripleStore(graphCollection);
            quadWriter.Save(quadStore, streamWriter);
        }

    }

    /// <summary>
    ///     Test the readability of a type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public override bool CanReadType(Type type)
    {
        if (type == typeof(Graph) || type == typeof(BaseGraph) || type == typeof(IGraph))
        {
            return true;
        }

        if (IsOslcSingleton(type))
        {
            return true;
        }

        var memberType = GetMemberType(type);

        if (memberType == null)
        {
            return false;
        }

        var canReadType =
            memberType.GetCustomAttributes(typeof(OslcResourceShape), true).Length > 0;
        return canReadType;
    }

    /// <summary>
    ///     Read RDF/XML from an HTTP input stream and convert to .NET objects
    /// </summary>
    /// <param name="type"></param>
    /// <param name="readStream"></param>
    /// <param name="content"></param>
    /// <param name="formatterLogger"></param>
    /// <returns></returns>
    public override async Task<object?> ReadFromStreamAsync(
        Type type,
        Stream readStream,
        HttpContent content,
        IFormatterLogger formatterLogger
    )
    {
        //var tcs = new TaskCompletionSource<object>();

        if (content == null || content.Headers == null || content.Headers.ContentLength == 0)
        {
            return null;
        }

        try
        {
            IRdfReader? tripleReader = null;
            IStoreReader? quadReader = null;

            // TODO: one class per RDF content type
            var mediaType = content.Headers.ContentType?.MediaType;
            if (OslcMediaType.APPLICATION_RDF_XML.Equals(mediaType))
            {
                tripleReader = new RdfXmlParser();
            }
            else if (OslcMediaType.TEXT_TURTLE.Equals(mediaType))
            {
                // TODO: make IRI validation configurable
                tripleReader = new TurtleParser(TurtleSyntax.Rdf11Star, false);
            }
            else if (OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML.Equals(mediaType)
                     || OslcMediaType.APPLICATION_XML.Equals(mediaType))
            {
                //For now, use the dotNetRDF RdfXmlParser() for application/xml.  This could change
                tripleReader = new RdfXmlParser();
            }
            else if (OslcMediaType.APPLICATION_JSON_LD.Equals(mediaType))
            {
                //For now, use the dotNetRDF RdfXmlParser() for application/xml.  This could change
                quadReader = new JsonLdParser(new JsonLdProcessorOptions
                {
                    ProcessingMode = JsonLdProcessingMode.JsonLd11
                });
            }
            else
            {
                throw new UnsupportedMediaTypeException(
                    $"Given type is not supported or is not valid RDF: ${content.Headers.ContentType?.MediaType}",
                    content.Headers.ContentType!);
            }

            IGraph graph = new Graph();
            // REVISIT: we need a more robust way to obtain request URI
            content.Headers.TryGetValues(OSLC4NetConstants.INNER_URI_HEADER, out var shuttleRequestUri);
            graph.BaseUri = shuttleRequestUri?.SingleOrDefault()?.ToSafeUri() ?? httpRequest?.RequestUri;
            var encodingHeader = content.Headers.ContentEncoding.SingleOrDefault();
            var streamEncoding = Encoding.UTF8;
            if (encodingHeader is not null)
            {
                try
                {
                    streamEncoding = Encoding.GetEncoding(encodingHeader);
                }
                catch
                {
                    streamEncoding = Encoding.UTF8;
                }
            }
            // do not close the stream after reading
            var streamReader = new StreamReader(readStream, streamEncoding, false, STREAM_BUFFER_SIZE, true);

            using (streamReader)
            {
#if DEBUG
                var rdfString = streamReader.ReadToEnd();
                //Debug.Write(rdfString);
                readStream.Position = 0; // reset stream
                                         //streamReader.DiscardBufferedData();
#endif

                if (tripleReader is not null)
                {
                    tripleReader.Load(graph, streamReader);
                }
                else if (quadReader is not null)
                {
                    var quadStore = new TripleStore();
                    quadReader.Load(quadStore, streamReader);
                    // REVISIT: for now we support single graph in JSON-LD payloads (@berezovskyi 2025-05)
                    graph = quadStore.Graphs.Single();
                }
                else
                {
                    ThrowHelper.ThrowInvalidOperationException(
                        "Either a quad or triple reader is required.");
                }

                // REVISIT: make RDFS reasoning configurable (@berezovskyi 2025-05)
                // TODO: make schema loads configurable (@berezovskyi 2025-05)
                var reasoner = new StaticRdfsReasoner();
                // reasoner.Initialise(schema);
                reasoner.Apply(graph);

                // REVISIT: better handling of assignable types (@berezovskyi 2025-04)
                if (type == typeof(Graph) || type == typeof(BaseGraph) || type == typeof(IGraph))
                {
                    return graph;
                }

                var isSingleton = IsOslcSingleton(type);
                var output =
                    _rdfHelper.FromDotNetRdfGraph(graph,
                        isSingleton ? type : GetMemberType(type)!);

                if (isSingleton)
                {
                    var haveOne =
                        (int)output.GetType().GetProperty("Count")!.GetValue(output, null)! > 0;

                    return haveOne
                        ? output.GetType().GetProperty("Item")!.GetValue(output, new object[] { 0 })
                        : null;
                }
                else if (type.IsArray)
                {
                    return output.GetType().GetMethod("ToArray", Type.EmptyTypes)!
                        .Invoke(output, null);
                }
                else
                {
                    return output;
                }
            }
        }
        catch (Exception e)
        {
            if (formatterLogger == null)
            {
                throw;
            }

            formatterLogger.LogError(string.Empty, e.Message);

            return GetDefaultValueForType(type);
        }
        finally
        {
            readStream.Seek(0, SeekOrigin.Begin);
        }
    }

    private static bool IsOslcSingleton(Type type)
    {
        return type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0;
    }

    private static Type? GetMemberType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IEnumerable<>),
                type))
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() ==
                    typeof(IEnumerable<object>).GetGenericTypeDefinition())
                {
                    var memberType = iface.GetGenericArguments()[0];

                    if (memberType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
                    {
                        return memberType;
                    }

                    return null;
                }
            }
        }

        return null;
    }

    public static bool ImplementsGenericType(Type genericType, Type? typeToTest)
    {
        var isParentGeneric = genericType.IsGenericType;

        return ImplementsGenericType(genericType, typeToTest, isParentGeneric);
    }

    public static bool ImplementsGenericType(Type genericType, Type? typeToTest,
        bool isParentGeneric)
    {
        if (typeToTest == null)
        {
            return false;
        }

        typeToTest = isParentGeneric && typeToTest.IsGenericType
            ? typeToTest.GetGenericTypeDefinition()
            : typeToTest;

        if (typeToTest == genericType)
        {
            return true;
        }

        return ImplementsGenericType(genericType, typeToTest.BaseType, isParentGeneric);
    }

    public static Type[] GetChildClassParameterArguments(Type genericType, Type typeToTest)
    {
        var isParentGeneric = genericType.IsGenericType;

        while (true)
        {
            var parentType = typeToTest.BaseType!;
            var parentToTest = isParentGeneric && parentType.IsGenericType
                ? parentType.GetGenericTypeDefinition()
                : parentType;

            if (parentToTest == genericType)
            {
                return typeToTest.GetGenericArguments();
            }

            typeToTest = parentType;
        }
    }

    public static bool ImplementsICollection(Type type)
    {
        return type.IsGenericType && typeof(ICollection<>) == type.GetGenericTypeDefinition();
    }

    public class NonClosingStreamWriter : StreamWriter
    {
        public NonClosingStreamWriter(Stream stream)
            : base(stream)
        {
        }

        public override void Close()
        {
            // Don't let dotNetRDF writer close the file, but need to flush output.
            Flush();
        }
    }
}
