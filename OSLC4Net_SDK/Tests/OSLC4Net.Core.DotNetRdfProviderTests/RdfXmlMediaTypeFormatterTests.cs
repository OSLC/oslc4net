/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
 * Copyright (c) 2023 Andrii Berezovskyi and OSLC4Net contributors.
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
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using OSLC4Net.ChangeManagement;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using Xunit;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class RdfXmlMediaTypeFormatterTests
{
    [Fact]
    public async Task TestRdfXmlSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var rdfXml = await SerializeAsync(formatter, changeRequest1,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        Debug.WriteLine(rdfXml);

        var changeRequest2 =
            await DeserializeAsync<ChangeRequest>(formatter, rdfXml,
                OslcMediaType.APPLICATION_RDF_XML_TYPE);

        Assert.NotNull(changeRequest2);
        Assert.Equal(changeRequest1.GetAbout(), changeRequest2.GetAbout());
        Assert.Equal(changeRequest1.IsFixed(), changeRequest2.IsFixed());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
    }

    [Fact]
    public async Task TestRdfXmlCollectionSerializationAsync()
    {
        var crListOut = new List<ChangeRequest>();
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeRequest1"));
        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        crListOut.Add(changeRequest1);

        var changeRequest2 = new ChangeRequest(new Uri("http://com/somewhere/changeRequest2"));
        changeRequest2.SetFixed(false);
        changeRequest2.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest1"), "Test of links"));

        crListOut.Add(changeRequest2);

        var rdfGraph = DotNetRdfHelper.CreateDotNetRdfGraph("http://com/somewhere/changerequests",
                                                               "http://com/somewhere/changerequests?page=20",
                                                               "http://com/somewhere/changerequests?page=21",
                                                               null,
                                                               crListOut,
                                                               null);
        // TODO: fix overload confusion with one arg
        var formatter = new RdfXmlMediaTypeFormatter(rdfGraph, true);

        var rdfXml =
            await SerializeCollectionAsync(formatter, crListOut,
                OslcMediaType.APPLICATION_RDF_XML_TYPE);

        Debug.WriteLine(rdfXml);

        var crListIn =
            (await DeserializeCollectionAsync<ChangeRequest>(formatter, rdfXml,
                OslcMediaType.APPLICATION_RDF_XML_TYPE) ?? throw new InvalidOperationException())
            .ToList();
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

    [Fact]
    public async Task TestXmlSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var rdfXml =
            await SerializeAsync(formatter, changeRequest1, OslcMediaType.APPLICATION_XML_TYPE);

        Debug.WriteLine(rdfXml);

        var changeRequest2 =
            await DeserializeAsync<ChangeRequest>(formatter, rdfXml,
                OslcMediaType.APPLICATION_XML_TYPE);

        Assert.NotNull(changeRequest2);
        Assert.Equal(changeRequest1.GetAbout(), changeRequest2.GetAbout());
        Assert.Equal(changeRequest1.IsFixed(), changeRequest2.IsFixed());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
    }

    [Fact]
    public async Task TestTurtleSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var turtle =
            await SerializeAsync(formatter, changeRequest1, OslcMediaType.TEXT_TURTLE_TYPE);

        Debug.WriteLine(turtle);

        var changeRequest2 =
            await DeserializeAsync<ChangeRequest>(formatter, turtle,
                OslcMediaType.TEXT_TURTLE_TYPE);

        Assert.NotNull(changeRequest2);
        Assert.Equal(changeRequest1.GetAbout(), changeRequest2.GetAbout());
        Assert.Equal(changeRequest1.IsFixed(), changeRequest2.IsFixed());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetValue(), changeRequest2.GetAffectedByDefects()[0].GetValue());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetLabel(), changeRequest2.GetAffectedByDefects()[0].GetLabel());
    }

    [Fact]
    public async Task TestJsonLdSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"),
            "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var jsonLd =
            await SerializeAsync(formatter, changeRequest1, OslcMediaType.APPLICATION_JSON_LD_TYPE);

        Debug.WriteLine(jsonLd);

        var changeRequest2 =
            await DeserializeAsync<ChangeRequest>(formatter, jsonLd,
                OslcMediaType.APPLICATION_JSON_LD_TYPE);

        Assert.NotNull(changeRequest2);
        Assert.Equal(changeRequest1.GetAbout(), changeRequest2.GetAbout());
        Assert.Equal(changeRequest1.IsFixed(), changeRequest2.IsFixed());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetValue(),
            changeRequest2.GetAffectedByDefects()[0].GetValue());
        Assert.Equal(changeRequest1.GetAffectedByDefects()[0].GetLabel(),
            changeRequest2.GetAffectedByDefects()[0].GetLabel());
    }


    private static async Task<string> SerializeAsync<T>(MediaTypeFormatter formatter, T value,
        MediaTypeHeaderValue mediaType)
    {
        await using Stream stream = new MemoryStream();
        using HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        await formatter.WriteToStreamAsync(typeof(T), value, stream, content, null);
        stream.Position = 0;

        return await content.ReadAsStringAsync();
    }

    private static async Task<string> SerializeCollectionAsync<T>(MediaTypeFormatter formatter,
        IEnumerable<T> value, MediaTypeHeaderValue mediaType)
    {
        await using Stream stream = new MemoryStream();
        using HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        await formatter.WriteToStreamAsync(typeof(T), value, stream, content, null);
        stream.Position = 0;

        return await content.ReadAsStringAsync();
    }

    private static async Task<T?> DeserializeAsync<T>(MediaTypeFormatter formatter, string str,
        MediaTypeHeaderValue mediaType) where T : class
    {
        await using Stream stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        using HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        await writer.WriteAsync(str);
        await writer.FlushAsync();

        stream.Position = 0;

        return await formatter.ReadFromStreamAsync(typeof(T), stream, content, null) as T;
    }

    private static async Task<IEnumerable<T>?> DeserializeCollectionAsync<T>(
        MediaTypeFormatter formatter,
        string str, MediaTypeHeaderValue mediaType) where T : class
    {
        await using Stream stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        using HttpContent content = new StreamContent(stream);

        content.Headers.ContentType = mediaType;

        await writer.WriteAsync(str);
        await writer.FlushAsync();

        stream.Position = 0;

        return await formatter.ReadFromStreamAsync(typeof(List<T>), stream, content, null) as
            IEnumerable<T>;
    }
}
