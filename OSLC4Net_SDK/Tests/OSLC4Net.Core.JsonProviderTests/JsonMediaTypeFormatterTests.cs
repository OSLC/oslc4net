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

using System.Diagnostics;
using System.Json;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using log4net;
using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.JsonProvider;
using OSLC4Net.Core.Model;
using Xunit;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace OSLC4Net.Core.JsonProviderTests;

[Obsolete]
public class JsonMediaTypeFormatterTests
{
    [Fact]
    public async Task TestJsonSerialization()
    {
        ChangeRequest changeRequest1 = new(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"),
            label: "Test of links"));

        OslcJsonMediaTypeFormatter formatter = new();

        Assert.NotNull(changeRequest1);
        var json = Serialize(formatter, changeRequest1, OslcMediaType.APPLICATION_JSON_TYPE);

        Assert.NotNull(json);
        Debug.WriteLine(json);

        var changeRequest2 = await Deserialize<ChangeRequest>(formatter, json, OslcMediaType.APPLICATION_JSON_TYPE);

        Assert.NotNull(changeRequest2);
        Assert.Equal(changeRequest1.GetAbout(), changeRequest2.GetAbout());
        Assert.Equal(changeRequest1.IsFixed(), changeRequest2.IsFixed());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetValue(),
            changeRequest2.GetAffectedByDefects()[0].GetValue());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetLabel(),
            changeRequest2.GetAffectedByDefects()[0].GetLabel());
    }

    [Fact]
    public void TestJsonCollectionSerialization()
    {
        List<ChangeRequest> crListOut = new();
        ChangeRequest changeRequest1 = new(new Uri("http://com/somewhere/changeRequest1"));
        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        crListOut.Add(changeRequest1);

        ChangeRequest changeRequest2 = new(new Uri("http://com/somewhere/changeRequest2"));
        changeRequest2.SetFixed(false);
        changeRequest2.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest1"), "Test of links"));

        crListOut.Add(changeRequest2);

        JsonValue json = JsonHelper.CreateJson("http://com/somewhere/changerequests",
                                               "http://com/somewhere/changerequests?page=20",
                                               "http://com/somewhere/changerequests?page=21",
                                               null,
                                               crListOut,
                                               null);
        OslcJsonMediaTypeFormatter formatter = new(json, false);

        var jsonString = SerializeCollection(formatter, crListOut, OslcMediaType.APPLICATION_JSON_TYPE);

        var crListIn = DeserializeCollection<ChangeRequest>(formatter, jsonString, OslcMediaType.APPLICATION_JSON_TYPE).ToList();
        Assert.Equal(crListOut.Count, crListIn.Count);

        //No guarantees of order in a collection, use the "about" attribute to identify individual ChangeRequests
        foreach (var cr in crListIn)
        {
            var crAboutUri = cr.GetAbout().AbsoluteUri;

            if (crAboutUri.Equals("http://com/somewhere/changeRequest1"))
            {
                Assert.Equal(cr.IsFixed(), changeRequest1.IsFixed());
                Assert.Equal(cr.GetAffectedByDefects()[0].GetValue(), changeRequest1.GetAffectedByDefects()[0].GetValue());
                Assert.Equal(cr.GetAffectedByDefects()[0].GetLabel(), changeRequest1.GetAffectedByDefects()[0].GetLabel());
            }
            else if (crAboutUri.Equals("http://com/somewhere/changeRequest2"))
            {
                Assert.Equal(cr.IsFixed(), changeRequest2.IsFixed());
                Assert.Equal(cr.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
                Assert.Equal(cr.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
            }
            else
            {
                Assert.Fail("Deserialized ChangeRequest about attribute not recognized: " + crAboutUri);
            }
        }

    }

    private static string Serialize<T>(MediaTypeFormatter formatter, T value, MediaTypeHeaderValue mediaType) where T : IResource
    {
        Stream stream = new MemoryStream();
        HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
        stream.Position = 0;

        return content.ReadAsStringAsync().Result;
    }

    private static string SerializeCollection<T>(MediaTypeFormatter formatter, IEnumerable<T> value, MediaTypeHeaderValue mediaType) where T : IResource
    {
        Stream stream = new MemoryStream();
        HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
        stream.Position = 0;

        return content.ReadAsStringAsync().Result;
    }

    private static async Task<T> Deserialize<T>(MediaTypeFormatter formatter, string str, MediaTypeHeaderValue mediaType) where T : class, IResource
    {
        Stream stream = new MemoryStream();
        StreamWriter writer = new(stream);
        HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        writer.Write(str);
        writer.Flush();

        stream.Position = 0;

        var result = await formatter.ReadFromStreamAsync(typeof(T), stream, content, logFormatter);

        Debug.Write(result.ToString());

        return result as T;
    }

    private static IEnumerable<T> DeserializeCollection<T>(MediaTypeFormatter formatter, string str, MediaTypeHeaderValue mediaType) where T : class, IResource
    {
        Stream stream = new MemoryStream();
        StreamWriter writer = new(stream);
        HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        writer.Write(str);
        writer.Flush();

        stream.Position = 0;

        return formatter.ReadFromStreamAsync(typeof(List<T>), stream, content, logFormatter).Result as IEnumerable<T>;
    }

    private sealed class LogFormatter : IFormatterLogger
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

        private readonly ILog logger;
    }

    private static readonly LogFormatter logFormatter = new(LogManager.GetLogger(typeof(JsonMediaTypeFormatterTests)));
}
