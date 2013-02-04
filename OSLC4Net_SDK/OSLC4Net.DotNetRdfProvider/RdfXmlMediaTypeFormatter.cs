/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

using log4net;

using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace OSLC4Net.Core.DotNetRdfProvider
{
    /// <summary>
    /// A class to 
    ///     - read RDF/XML from an input stream and create .NET objects.
    ///     - write .NET objects to an output stream as RDF/XML
    /// </summary>
    public class RdfXmlMediaTypeFormatter : MediaTypeFormatter
    {

        public IGraph Graph { get; set; }

        public RdfXmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(OslcMediaType.APPLICATION_RDF_XML_TYPE);
            SupportedMediaTypes.Add(OslcMediaType.APPLICATION_XML_TYPE);
            SupportedMediaTypes.Add(OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML_TYPE);
        }

        public RdfXmlMediaTypeFormatter(IGraph graph)
            : this()
        {
            this.Graph = graph;
        }

        /// <summary>
        /// Test the write-ability of a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanWriteType(Type type)
        {
            if (IsSinglton(type))
            {
                return true;
            }
            
            return GetMemberType(type) != null;
        }

        /// <summary>
        /// Write a .NET object to an output stream
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="writeStream"></param>
        /// <param name="content"></param>
        /// <param name="transportContext"></param>
        /// <returns></returns>
        public override Task  WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext
        )
        {
            return Task.Factory.StartNew(() =>
                {

                    if ((Graph == null) || (Graph.IsEmpty))
                    {

                        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IEnumerable<>), value.GetType()))
                        {
                            Graph = DotNetRdfHelper.CreateDotNetRdfGraph(value as IEnumerable<object>);
                        }
                        else if (type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
                        {
                            Graph = DotNetRdfHelper.CreateDotNetRdfGraph(new object[] { value });
                        }
                        else
                        {
                            Graph = DotNetRdfHelper.CreateDotNetRdfGraph(new EnumerableWrapper(value));
                        }
                    }

                    IRdfWriter xmlWriter;
                    
                    if (content == null || content.Headers == null || content.Headers.ContentType.MediaType.Equals(OslcMediaType.APPLICATION_RDF_XML)) 
                    {
                        RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();

                        rdfXmlWriter.UseDtd = false;
                        rdfXmlWriter.PrettyPrintMode = false;
                        rdfXmlWriter.CompressionLevel = 20;
                        //rdfXmlWriter.UseTypedNodes = false;

                        xmlWriter = rdfXmlWriter;
                    }
                     
                    else
                    {
                        //For now, use the dotNetRDF RdfXmlWriter for application/xml
                        //OslcXmlWriter oslcXmlWriter = new OslcXmlWriter();
                        RdfXmlWriter oslcXmlWriter = new RdfXmlWriter();

                        oslcXmlWriter.PrettyPrintMode = false;
                        oslcXmlWriter.CompressionLevel = 20;

                        xmlWriter = oslcXmlWriter;
                    }
                    
                    StreamWriter streamWriter = new NonClosingStreamWriter(writeStream);

                    xmlWriter.Save(Graph, streamWriter);
                });
        }

        /// <summary>
        /// Test the readability of a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanReadType(Type type)
        {
            if (IsSinglton(type))
            {
                return true;
            }

            return GetMemberType(type) != null;
        }

        /// <summary>
        /// Read RDF/XML from an HTTP input stream and convert to .NET objects
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

            if (content != null && content.Headers != null && content.Headers.ContentLength == 0) return null;

            try
            {
                IRdfReader parser;
                    
                if (content == null || content.Headers == null || content.Headers.ContentType.MediaType.Equals(OslcMediaType.APPLICATION_RDF_XML)) 
                {
                    parser = new RdfXmlParser();
                }
                else
                {
                    //For now, use the dotNetRDF RdfXmlParser() for application/xml.  This could change
                    //parser = new OslcXmlParser();
                    parser = new RdfXmlParser();
                }

                IGraph graph = new Graph();
                StreamReader streamReader = new StreamReader(readStream);

                using (streamReader)
                {
                    parser.Load(graph, streamReader);

                    bool isSingleton = IsSinglton(type);
                    object output = DotNetRdfHelper.FromDotNetRdfGraph(graph, isSingleton ? type : GetMemberType(type));

                    if (isSingleton)
                    {
                        bool haveOne = (int)output.GetType().GetProperty("Count").GetValue(output, null) > 0;

                        tcs.SetResult(haveOne ? output.GetType().GetProperty("Item").GetValue(output, new object[] { 0 }): null);
                    }
                    else if (type.IsArray)
                    {
                        tcs.SetResult(output.GetType().GetMethod("ToArray", Type.EmptyTypes).Invoke(output, null));
                    }
                    else
                    {
                        tcs.SetResult(output);
                    }
                }
            }
            catch (Exception e)
            {
                if (formatterLogger == null) throw;

                formatterLogger.LogError(String.Empty, e.Message);

                tcs.SetResult(GetDefaultValueForType(type));
            }

            return tcs.Task;
        }

        private bool IsSinglton(Type type)
        {
            return type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0;
        }

        private Type GetMemberType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IEnumerable<>), type))
            {
                Type[] interfaces = type.GetInterfaces();

                foreach (Type interfac in interfaces)
                {
                    if (interfac.IsGenericType && interfac.GetGenericTypeDefinition() == typeof(IEnumerable<object>).GetGenericTypeDefinition())
                    {
                        Type memberType = interfac.GetGenericArguments()[0];

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

        private class NonClosingStreamWriter : StreamWriter
        {
            public NonClosingStreamWriter(Stream stream)
                : base(stream)
            {
            }

            public override void Close()
            {
               // Don't let dotNetRDF writer close the file.
            }
        }
    }
}
