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
using System.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.JsonProvider;
using OSLC4Net.Core.Model;

using log4net;

namespace JsonProviderTests
{
    [TestClass]
    public class JsonMediaTypeFormatterTests
    {
        [TestMethod]
        public void TestJsonSerialization()
        {
            ChangeRequest changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

            changeRequest1.SetFixed(true);
            changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

            OSLC4Net.Core.JsonProvider.JsonMediaTypeFormatter formatter = new OSLC4Net.Core.JsonProvider.JsonMediaTypeFormatter();

            string json = Serialize<ChangeRequest>(formatter, changeRequest1, OslcMediaType.APPLICATION_JSON_TYPE);

            Debug.WriteLine(json);

            ChangeRequest changeRequest2 = Deserialize<ChangeRequest>(formatter, json, OslcMediaType.APPLICATION_JSON_TYPE);

            Assert.AreEqual(changeRequest1.GetAbout(), changeRequest2.GetAbout());
            Assert.AreEqual(changeRequest1.IsFixed(), changeRequest2.IsFixed());
            Assert.AreEqual(changeRequest1.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
            Assert.AreEqual(changeRequest1.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
        }

        [TestMethod]
        public void TestJsonCollectionSerialization()
        {
            List<ChangeRequest> crListOut = new List<ChangeRequest>();
            ChangeRequest changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeRequest1"));
            changeRequest1.SetFixed(true);
            changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

            crListOut.Add(changeRequest1);

            ChangeRequest changeRequest2 = new ChangeRequest(new Uri("http://com/somewhere/changeRequest2"));
            changeRequest2.SetFixed(false);
            changeRequest2.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest1"), "Test of links"));

            crListOut.Add(changeRequest2);

            JsonObject jsonObject = JsonHelper.CreateJson("http://com/somewhere/changerequests",
                                                          "http://com/somewhere/changerequests?page=20",
                                                          "http://com/somewhere/changerequests?page=21",
                                                          null,
                                                          crListOut,
                                                          null);
            OSLC4Net.Core.JsonProvider.JsonMediaTypeFormatter formatter = new OSLC4Net.Core.JsonProvider.JsonMediaTypeFormatter(jsonObject);

            string rdfXml = SerializeCollection<ChangeRequest>(formatter, crListOut, OslcMediaType.APPLICATION_JSON_TYPE);

            Debug.WriteLine(rdfXml);

            List<ChangeRequest> crListIn = DeserializeCollection<ChangeRequest>(formatter, rdfXml, OslcMediaType.APPLICATION_JSON_TYPE).ToList();
            Assert.AreEqual(crListOut.Count, crListIn.Count);

            //No guarantees of order in a collection, use the "about" attribute to identify individual ChangeRequests
            foreach (ChangeRequest cr in crListIn)
            {
                string crAboutUri = cr.GetAbout().AbsoluteUri;

                if (crAboutUri.Equals("http://com/somewhere/changeRequest1"))
                {
                    Assert.AreEqual(cr.IsFixed(), changeRequest1.IsFixed());
                    Assert.AreEqual(cr.GetAffectedByDefects()[0].GetValue(), changeRequest1.GetAffectedByDefects()[0].GetValue());
                    Assert.AreEqual(cr.GetAffectedByDefects()[0].GetLabel(), changeRequest1.GetAffectedByDefects()[0].GetLabel());
                }
                else if (crAboutUri.Equals("http://com/somewhere/changeRequest2"))
                {
                    Assert.AreEqual(cr.IsFixed(), changeRequest2.IsFixed());
                    Assert.AreEqual(cr.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
                    Assert.AreEqual(cr.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
                }
                else
                {
                    Assert.Fail("Deserialized ChangeRequest about attribute not recognized: " + crAboutUri);
                }
            }

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

        private string SerializeCollection<T>(MediaTypeFormatter formatter, IEnumerable<T> value, MediaTypeHeaderValue mediaType)
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

            return formatter.ReadFromStreamAsync(typeof(T), stream, content, logFormatter).Result as T;
        }

        private IEnumerable<T> DeserializeCollection<T>(MediaTypeFormatter formatter, string str, MediaTypeHeaderValue mediaType) where T : class
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            HttpContent content = new StreamContent(stream);

            content.Headers.ContentType = mediaType;

            writer.Write(str);
            writer.Flush();

            stream.Position = 0;

            return formatter.ReadFromStreamAsync(typeof(List<T>), stream, content, logFormatter).Result as IEnumerable<T>;
        }

        private class LogFormatter : IFormatterLogger
        {
            public LogFormatter(ILog logger)
            {
                this.logger = logger;
            }

            public void LogError(string errorPath, Exception exception)
            {
                logger.Error(errorPath, exception);
            }

            public void LogError(string errorPath, string errorMessage)
            {
                logger.Error(errorPath + ": " + errorMessage);
            }

            private ILog logger;
        }

        private static LogFormatter logFormatter = new LogFormatter(LogManager.GetLogger(typeof(JsonMediaTypeFormatterTests)));
    }
}
