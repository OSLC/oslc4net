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

namespace OSLC4Net.Core.DotNetRdfProviderTests;

public class RdfXmlMediaTypeFormatterTests
{
    [Test]
    public async Task TestRdfXmlSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var rdfXml = await RdfHelpers.SerializeAsync(formatter, changeRequest1,
            OslcMediaType.APPLICATION_RDF_XML_TYPE);

        Debug.WriteLine(rdfXml);

        var changeRequest2 =
            await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, rdfXml,
                OslcMediaType.APPLICATION_RDF_XML_TYPE);

        await Assert.That(changeRequest2).IsNotNull();
        await Assert.That(changeRequest2.GetAbout()).IsEqualTo(changeRequest1.GetAbout());
        await Assert.That(changeRequest2.IsFixed()).IsEqualTo(changeRequest1.IsFixed());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetValue()).IsEqualTo(changeRequest1.GetAffectedByDefects()[0].GetValue());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetLabel()).IsEqualTo(changeRequest1.GetAffectedByDefects()[0].GetLabel());

        await Verify(changeRequest1);
    }

    [Test]
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
        var formatter = new RdfXmlMediaTypeFormatter(rdfGraph, null);

        var rdfXml =
            await RdfHelpers.SerializeCollectionAsync(formatter, crListOut,
                OslcMediaType.APPLICATION_RDF_XML_TYPE);

        Debug.WriteLine(rdfXml);

        var crListIn =
            (await RdfHelpers.DeserializeCollectionAsync<ChangeRequest>(formatter, rdfXml,
                OslcMediaType.APPLICATION_RDF_XML_TYPE) ?? throw new InvalidOperationException())
            .ToList();
        await Assert.That(crListIn.Count).IsEqualTo(crListOut.Count);

        //No guarantees of order in a collection, use the "about" attribute to identify individual ChangeRequests
        foreach (var cr in crListIn)
        {
            var crAboutUri = cr.GetAbout().AbsoluteUri;

            if (crAboutUri.Equals("http://com/somewhere/changeRequest1"))
            {
                await Assert.That(changeRequest1.IsFixed()).IsEqualTo(cr.IsFixed());
                await Assert.That(changeRequest1.GetAffectedByDefects()[0].GetValue()).IsEqualTo(cr.GetAffectedByDefects()[0].GetValue());
                await Assert.That(changeRequest1.GetAffectedByDefects()[0].GetLabel()).IsEqualTo(cr.GetAffectedByDefects()[0].GetLabel());
            }
            else if (crAboutUri.Equals("http://com/somewhere/changeRequest2"))
            {
                await Assert.That(changeRequest2.IsFixed()).IsEqualTo(cr.IsFixed());
                await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetValue()).IsEqualTo(cr.GetAffectedByDefects()[0].GetValue());
                await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetLabel()).IsEqualTo(cr.GetAffectedByDefects()[0].GetLabel());
            }
            else
            {
                // Assert.Fail("Deserialized ChangeRequest about attribute not recognized: " + crAboutUri);
                await Assert.That(true).IsFalse(); // TUnit doesn't support message in IsFalse directly yet?
                throw new Exception("Deserialized ChangeRequest about attribute not recognized: " + crAboutUri);
            }
        }

        await Verify(rdfGraph);
    }

    [Test]
    public async Task TestXmlSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var rdfXml =
            await RdfHelpers.SerializeAsync(formatter, changeRequest1, OslcMediaType.APPLICATION_XML_TYPE);

        Debug.WriteLine(rdfXml);

        var changeRequest2 =
            await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, rdfXml,
                OslcMediaType.APPLICATION_XML_TYPE);

        await Assert.That(changeRequest2).IsNotNull();
        await Assert.That(changeRequest2.GetAbout()).IsEqualTo(changeRequest1.GetAbout());
        await Assert.That(changeRequest2.IsFixed()).IsEqualTo(changeRequest1.IsFixed());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetValue()).IsEqualTo(changeRequest1.GetAffectedByDefects()[0].GetValue());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetLabel()).IsEqualTo(changeRequest1.GetAffectedByDefects()[0].GetLabel());
    }

    [Test]
    public async Task TestTurtleSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"), "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var turtle =
            await RdfHelpers.SerializeAsync(formatter, changeRequest1, OslcMediaType.TEXT_TURTLE_TYPE);

        Debug.WriteLine(turtle);

        var changeRequest2 =
            await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, turtle,
                OslcMediaType.TEXT_TURTLE_TYPE);

        await Assert.That(changeRequest2).IsNotNull();
        await Assert.That(changeRequest2.GetAbout()).IsEqualTo(changeRequest1.GetAbout());
        await Assert.That(changeRequest2.IsFixed()).IsEqualTo(changeRequest1.IsFixed());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetValue()).IsEqualTo(changeRequest1.GetAffectedByDefects()[0].GetValue());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetLabel()).IsEqualTo(changeRequest1.GetAffectedByDefects()[0].GetLabel());
    }

    [Test]
    public async Task TestJsonLdSerializationAsync()
    {
        var changeRequest1 = new ChangeRequest(new Uri("http://com/somewhere/changeReuest"));

        changeRequest1.SetFixed(true);
        changeRequest1.AddAffectedByDefect(new Link(new Uri("http://com/somewhere/changeRequest2"),
            "Test of links"));

        var formatter = new RdfXmlMediaTypeFormatter();

        var jsonLd =
            await RdfHelpers.SerializeAsync(formatter, changeRequest1, OslcMediaType.APPLICATION_JSON_LD_TYPE);

        Debug.WriteLine(jsonLd);

        var changeRequest2 =
            await RdfHelpers.DeserializeAsync<ChangeRequest>(formatter, jsonLd,
                OslcMediaType.APPLICATION_JSON_LD_TYPE);

        await Assert.That(changeRequest2).IsNotNull();
        await Assert.That(changeRequest2.GetAbout()).IsEqualTo(changeRequest1.GetAbout());
        await Assert.That(changeRequest2.IsFixed()).IsEqualTo(changeRequest1.IsFixed());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetValue()).IsEqualTo(
            changeRequest1.GetAffectedByDefects()[0].GetValue());
        await Assert.That(changeRequest2.GetAffectedByDefects()[0].GetLabel()).IsEqualTo(
            changeRequest1.GetAffectedByDefects()[0].GetLabel());
    }




}
