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
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace OSLC4Net.Core.DotNetRdfProvider;

/// <summary>
///     A class to
///     - read RDF/XML from an input stream and create .NET objects.
///     - write .NET objects to an output stream as RDF/XML
/// </summary>
public class RdfXmlMediaTypeFormatter : MediaTypeFormatter
{
    private HttpRequestMessage httpRequest;

    /// <summary>
    ///     Defauld RdfXml formatter
    /// </summary>
    /// <param name="graph"></param>
    public RdfXmlMediaTypeFormatter(bool rebuildgraph = true)
    {
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
        bool rebuildgraph = true
    ) : this(rebuildgraph)
    {
        Graph = graph;
    }

    public IGraph Graph { get; set; }
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

            if (actualTypeArguments.Count() != 1)
            {
                return false;
            }

            if (ImplementsICollection(actualTypeArguments[0]))
            {
                actualTypeArguments = actualTypeArguments[0].GetGenericArguments();

                if (actualTypeArguments.Count() != 1)
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

        if (IsSingleton(actualType))
        {
            return true;
        }

        var memberType = GetMemberType(type);

        if (memberType == null)
        {
            return false;
        }

        return memberType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0;
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
    public override Task WriteToStreamAsync(
        Type type,
        object value,
        Stream writeStream,
        HttpContent content,
        TransportContext transportContext
    )
    {
        return Task.Factory.StartNew(() =>
        {
            if (Graph == null || Graph.IsEmpty || RebuildGraph)
            {
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
                        //TODO:  should this be set by the app based on service provider info
                        var portNum = httpRequest.RequestUri.Port;
                        string portString = null;
                        if (portNum == 80 || portNum == 443)
                        {
                            portString = "";
                        }
                        else
                        {
                            portString = ":" + portNum;
                        }

                        var descriptionAbout = httpRequest.RequestUri.Scheme + "://" +
                                               httpRequest.RequestUri.Host +
                                               portString +
                                               httpRequest.RequestUri.LocalPath;

                        //Subject URI for the responseInfo is the full request URI
                        var responseInfoAbout = httpRequest.RequestUri.ToString();

                        var totalCountProp = value.GetType().GetProperty("TotalCount");
                        var nextPageProp = value.GetType().GetProperty("NextPage");

                        Graph = DotNetRdfHelper.CreateDotNetRdfGraph(descriptionAbout,
                            responseInfoAbout,
                            (string)nextPageProp.GetValue(value, null),
                            (int)totalCountProp.GetValue(value, null),
                            objects as IEnumerable<object>,
                            (IDictionary<string, object>)propertiesProp.GetValue(value, null));
                    }
                    else
                    {
                        Graph = DotNetRdfHelper.CreateDotNetRdfGraph(null, null, null, null,
                            objects as IEnumerable<object>,
                            (IDictionary<string, object>)propertiesProp.GetValue(value, null));
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

            IRdfWriter rdfWriter;

            if (content == null || content.Headers == null ||
                content.Headers.ContentType.MediaType.Equals(OslcMediaType.APPLICATION_RDF_XML))
            {
                var rdfXmlWriter = new RdfXmlWriter
                {
                    UseDtd = false, PrettyPrintMode = false, CompressionLevel = 20
                };
                //turtlelWriter.UseTypedNodes = false;

                rdfWriter = rdfXmlWriter;
            }
            else if (content.Headers.ContentType.MediaType.Equals(OslcMediaType.TEXT_TURTLE))
            {
                var turtlelWriter = new CompressingTurtleWriter(TurtleSyntax.W3C)
                {
                    PrettyPrintMode = false
                };

                rdfWriter = turtlelWriter;
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

            StreamWriter streamWriter = new NonClosingStreamWriter(writeStream);

            rdfWriter.Save(Graph, streamWriter);
        });
    }

    /// <summary>
    ///     Test the readability of a type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public override bool CanReadType(Type type)
    {
        if (IsSingleton(type))
        {
            return true;
        }

        var memberType = GetMemberType(type);

        if (memberType == null)
        {
            return false;
        }

        return memberType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0;
    }

    /// <summary>
    ///     Read RDF/XML from an HTTP input stream and convert to .NET objects
    /// </summary>
    /// <param name="type"></param>
    /// <param name="readStream"></param>
    /// <param name="content"></param>
    /// <param name="formatterLogger"></param>
    /// <returns></returns>
    public override Task<object> ReadFromStreamAsync(
        Type type,
        Stream readStream,
        HttpContent content,
        IFormatterLogger formatterLogger
    )
    {
        var tcs = new TaskCompletionSource<object>();

        if (content == null || content.Headers == null || content.Headers.ContentLength == 0)
        {
            return null;
        }

        try
        {
            IRdfReader rdfParser;

            // TODO: one class per RDF content type
            var mediaType = content.Headers.ContentType.MediaType;
            if (mediaType.Equals(OslcMediaType.APPLICATION_RDF_XML))
            {
                rdfParser = new RdfXmlParser();
            }
            else if (mediaType.Equals(OslcMediaType.TEXT_TURTLE))
            {
                // TODO: make IRI validation configurable
                rdfParser = new TurtleParser(TurtleSyntax.Rdf11Star, false);
            }
            else if (mediaType.Equals(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML)
                     || mediaType.Equals(OslcMediaType.APPLICATION_XML))
            {
                //For now, use the dotNetRDF RdfXmlParser() for application/xml.  This could change
                rdfParser = new RdfXmlParser();
            }
            else
            {
                throw new UnsupportedMediaTypeException(
                    $"Given type is not supported or is not valid RDF: ${content.Headers.ContentType.MediaType}",
                    content.Headers.ContentType);
            }

            IGraph? graph = new Graph();
            var streamReader = new StreamReader(readStream);

            using (streamReader)
            {
                // var rdfString = streamReader.ReadToEnd();
                // Debug.Write(rdfString);
                // readStream.Position = 0; // reset stream
                // streamReader.DiscardBufferedData();

                rdfParser.Load(graph, streamReader);

                var isSingleton = IsSingleton(type);
                var output =
                    DotNetRdfHelper.FromDotNetRdfGraph(graph,
                        isSingleton ? type : GetMemberType(type));

                if (isSingleton)
                {
                    var haveOne =
                        (int)output.GetType().GetProperty("Count").GetValue(output, null) > 0;

                    tcs.SetResult(haveOne
                        ? output.GetType().GetProperty("Item").GetValue(output, new object[] { 0 })
                        : null);
                }
                else if (type.IsArray)
                {
                    tcs.SetResult(output.GetType().GetMethod("ToArray", Type.EmptyTypes)
                        .Invoke(output, null));
                }
                else
                {
                    tcs.SetResult(output);
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

            tcs.SetResult(GetDefaultValueForType(type));
        }

        return tcs.Task;
    }

    private bool IsSingleton(Type type)
    {
        return type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0;
    }

    private Type GetMemberType(Type type)
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

    private static bool ImplementsGenericType(Type genericType, Type typeToTest)
    {
        var isParentGeneric = genericType.IsGenericType;

        return ImplementsGenericType(genericType, typeToTest, isParentGeneric);
    }

    private static bool ImplementsGenericType(Type genericType, Type typeToTest,
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

    private static Type[] GetChildClassParameterArguments(Type genericType, Type typeToTest)
    {
        var isParentGeneric = genericType.IsGenericType;

        while (true)
        {
            var parentType = typeToTest.BaseType;
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

    private static bool ImplementsICollection(Type type)
    {
        return type.IsGenericType && typeof(ICollection<>) == type.GetGenericTypeDefinition();
    }

    private class NonClosingStreamWriter : StreamWriter
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
