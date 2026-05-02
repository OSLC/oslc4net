using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSLC4Net.Core.Query;
using OSLC4Net.Core.Model;
using OSLC4Net.Core.Attribute;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace OSLC4Net.Core.QueryTests;

[OslcNamespace("http://example.com/ns#")]
public class MockResource : AbstractResource
{
    [OslcName("name")]
    [OslcPropertyDefinition("http://example.com/ns#name")]
    public string Name { get; set; }

    [OslcName("id")]
    [OslcPropertyDefinition("http://example.com/ns#id")]
    public int Id { get; set; }
}

public class SortTermsResourceTests
{
    [Test]
    public async Task SortTerms_SortsResources()
    {
        var prefixMap = new Dictionary<string, string>
        {
            { "ex", "http://example.com/ns#" }
        };

        var orderByStr = "+ex:id,-ex:name";
        var orderBy = QueryUtils.ParseOrderBy(orderByStr, prefixMap);

        var resources = new List<MockResource>
        {
            new MockResource { Id = 2, Name = "B" },
            new MockResource { Id = 1, Name = "Z" },
            new MockResource { Id = 2, Name = "A" },
        };

        IOrderedEnumerable<MockResource> ordered = null;
        bool isFirst = true;

        foreach (var term in orderBy.Children)
        {
            if (term is SimpleSortTerm simpleTerm)
            {
                var identifier = simpleTerm.Identifier.ToString(); // e.g. "http://example.com/ns#id" or "ex:id"?

                Func<MockResource, object> keySelector = null;
                if (identifier.Contains("id"))
                {
                    keySelector = r => r.Id;
                }
                else if (identifier.Contains("name"))
                {
                    keySelector = r => r.Name;
                }

                if (keySelector != null)
                {
                    if (isFirst)
                    {
                        ordered = simpleTerm.Ascending ? resources.OrderBy(keySelector) : resources.OrderByDescending(keySelector);
                        isFirst = false;
                    }
                    else
                    {
                        ordered = simpleTerm.Ascending ? ordered.ThenBy(keySelector) : ordered.ThenByDescending(keySelector);
                    }
                }
            }
        }

        var result = ordered?.ToList();

        await Assert.That(result).IsNotNull();
        await Assert.That(result[0].Id).IsEqualTo(1);
        await Assert.That(result[0].Name).IsEqualTo("Z");

        // id 2, desc name: "B" > "A"
        await Assert.That(result[1].Id).IsEqualTo(2);
        await Assert.That(result[1].Name).IsEqualTo("B");

        await Assert.That(result[2].Id).IsEqualTo(2);
        await Assert.That(result[2].Name).IsEqualTo("A");
    }
}
