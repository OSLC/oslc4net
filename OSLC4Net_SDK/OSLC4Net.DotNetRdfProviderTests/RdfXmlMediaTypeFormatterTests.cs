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
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;

namespace DotNetRdfProviderTests
{
    [TestClass]
    public class RdfXmlMediaTypeFormatterTests
    {
        [TestMethod]
        public void TestRdfXmlSerialization()
        {
            ChangeRequest changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

            changeRequest1.SetFixed(true);
            changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

            RdfXmlMediaTypeFormatter formatter = new RdfXmlMediaTypeFormatter();

            string rdfXml = Serialize<ChangeRequest>(formatter, changeRequest1, OslcMediaType.APPLICATION_RDF_XML_TYPE);

            Debug.WriteLine(rdfXml);

            ChangeRequest changeRequest2 = Deserialize<ChangeRequest>(formatter, rdfXml, OslcMediaType.APPLICATION_RDF_XML_TYPE);

            Assert.AreEqual(changeRequest1.GetAbout(), changeRequest2.GetAbout());
            Assert.AreEqual(changeRequest1.IsFixed(), changeRequest2.IsFixed());
            Assert.AreEqual(changeRequest1.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
            Assert.AreEqual(changeRequest1.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
        }

        [TestMethod]
        public void TestXmlSerialization()
        {
            ChangeRequest changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

            changeRequest1.SetFixed(true);
            changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

            RdfXmlMediaTypeFormatter formatter = new RdfXmlMediaTypeFormatter();

            string rdfXml = Serialize<ChangeRequest>(formatter, changeRequest1, OslcMediaType.APPLICATION_XML_TYPE);

            Debug.WriteLine(rdfXml);

            ChangeRequest changeRequest2 = Deserialize<ChangeRequest>(formatter, rdfXml, OslcMediaType.APPLICATION_XML_TYPE);

            Assert.AreEqual(changeRequest1.GetAbout(), changeRequest2.GetAbout());
            Assert.AreEqual(changeRequest1.IsFixed(), changeRequest2.IsFixed());
            Assert.AreEqual(changeRequest1.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
            Assert.AreEqual(changeRequest1.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
        }

        private string Serialize<T>(MediaTypeFormatter formatter, T value, MediaTypeHeaderValue mediaType)
        {
            Stream stream = new MemoryStream();
            HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
            stream.Position = 0;

            return content.ReadAsStringAsync().Result;
        }

        private T Deserialize<T>(MediaTypeFormatter formatter, string str, MediaTypeHeaderValue mediaType) where T : class
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            writer.Write(str);
            writer.Flush();

            stream.Position = 0;

            return formatter.ReadFromStreamAsync(typeof(T), stream, content, null).Result as T;
        }
    }
}
