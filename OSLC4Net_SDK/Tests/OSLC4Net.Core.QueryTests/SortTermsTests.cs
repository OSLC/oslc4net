using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSLC4Net.Core.Query;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace OSLC4Net.Core.QueryTests;

public class SortTermsTests
{
    [Test]
    public async Task ParseOrderBy_ReturnsValidSortTermsImpl()
    {
        var prefixMap = new Dictionary<string, string>
        {
            { "oslc", "http://open-services.net/ns/core#" },
            { "dcterms", "http://purl.org/dc/terms/" },
            { "foaf", "http://xmlns.com/foaf/0.1/" }
        };
        var orderByStr = "+oslc:name,-oslc:shortId,dcterms:creator{+foaf:name}";
        var sortTerms = QueryUtils.ParseOrderBy(orderByStr, prefixMap);

        var children = sortTerms.Children;
        await Assert.That(children).IsNotNull();
        await Assert.That(children.Count).IsEqualTo(3);

        // Ensure it's read only!
        await Assert.That(() => children.Add(null)).Throws<NotSupportedException>();
    }
}
