using FluentAssertions;
using OSLC4NetExamples.Server.NetCoreApi.Models;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace OSLC4NetExamples.Server.Tests.Models;

public class RootServicesDtoTests
{
    private readonly XmlSerializer _serializer;

    public RootServicesDtoTests()
    {
        _serializer = new XmlSerializer(typeof(RootServicesDto));
    }[Fact]
    public void DeserializeFromFile_ShouldCorrectlyParseRootServices1()
    {        // Arrange
        var xmlContent = File.ReadAllText("TestData/rootservices.rdf");
        Console.WriteLine($"XML Content: {xmlContent}");
        
        // Act
        var result = RootServicesDto.FromXml(xmlContent);

        // Debug output
        Console.WriteLine($"About: '{result.About}'");
        Console.WriteLine($"Title: '{result.Title}'");
        Console.WriteLine($"OAuth Realm: '{result.OAuthRealmName}'");
        Console.WriteLine($"OAuth Domain: '{result.OAuthDomain}'");
        Console.WriteLine($"AM Service Providers: '{result.AmServiceProviders.Resource}'");

        // Assert
        result.Should().NotBeNull();
        result.About.Should().Be("http://localhost:8800/services/rootservices");
        result.Title.Should().Be("Root Services");
        result.AmServiceProviders.Resource.Should().Be("http://localhost:8800/services/catalog/singleton");
        result.RmServiceProviders.Resource.Should().Be("http://localhost:8800/services/catalog/singleton");
        result.CmServiceProviders.Resource.Should().Be("http://localhost:8800/services/catalog/singleton");
        result.OAuthRealmName.Should().Be("RM");
        result.OAuthDomain.Should().Be("http://localhost:8800/");
        result.OAuthRequestConsumerKeyUrl.Resource.Should().Be("http://localhost:8800/services/oauth/requestKey");
        result.OAuthApprovalModuleUrl.Resource.Should().Be("http://localhost:8800/services/oauth/approveKey");
        result.OAuthRequestTokenUrl.Resource.Should().Be("http://localhost:8800/services/oauth/requestToken");
        result.OAuthUserAuthorizationUrl.Resource.Should().Be("http://localhost:8800/services/oauth/authorize");
        result.OAuthAccessTokenUrl.Resource.Should().Be("http://localhost:8800/services/oauth/accessToken");
    }    [Fact]
    public void DeserializeFromFile_ShouldCorrectlyParseRootServices2()
    {
        // Arrange
        var xmlContent = File.ReadAllText("TestData/rootservices2.rdf");

        // Act
        var result = RootServicesDto.FromXml(xmlContent);

        // Assert
        result.Should().NotBeNull();
        result.About.Should().Be("https://server-am.sandbox.lynxwork.com/services/rootservices");
        result.Title.Should().Be("Root Services");
        result.AmServiceProviders.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/catalog/singleton");
        result.RmServiceProviders.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/catalog/singleton");
        result.CmServiceProviders.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/catalog/singleton");
        result.OAuthRealmName.Should().Be("AM");
        result.OAuthDomain.Should().Be("https://server-am.sandbox.lynxwork.com/");
        result.OAuthRequestConsumerKeyUrl.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/oauth/requestKey");
        result.OAuthApprovalModuleUrl.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/oauth/approveKey");
        result.OAuthRequestTokenUrl.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/oauth/requestToken");
        result.OAuthUserAuthorizationUrl.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/oauth/authorize");
        result.OAuthAccessTokenUrl.Resource.Should().Be("https://server-am.sandbox.lynxwork.com/services/oauth/accessToken");
    }

    [Theory]
    [InlineData("TestData/rootservices.rdf")]
    [InlineData("TestData/rootservices2.rdf")]
    public void RoundTrip_DeserializeAndSerialize_ShouldMaintainDataIntegrity(string testFilePath)
    {
        // Arrange
        var originalXmlContent = File.ReadAllText(testFilePath);

        // Act - Deserialize
        RootServicesDto deserializedDto;
        using (var stringReader = new StringReader(originalXmlContent))
        {
            deserializedDto = (RootServicesDto)_serializer.Deserialize(stringReader)!;
        }

        // Act - Serialize back to XML
        string serializedXmlContent;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings 
        { 
            Indent = true, 
            IndentChars = "    ",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        }))
        {
            _serializer.Serialize(xmlWriter, deserializedDto);
            serializedXmlContent = stringWriter.ToString();
        }

        // Act - Deserialize again to verify integrity
        RootServicesDto reDeserializedDto;
        using (var stringReader = new StringReader(serializedXmlContent))
        {
            reDeserializedDto = (RootServicesDto)_serializer.Deserialize(stringReader)!;
        }

        // Assert - All key properties should match between original and final deserialization
        reDeserializedDto.About.Should().Be(deserializedDto.About);
        reDeserializedDto.Title.Should().Be(deserializedDto.Title);
        reDeserializedDto.AmServiceProviders.Resource.Should().Be(deserializedDto.AmServiceProviders.Resource);
        reDeserializedDto.RmServiceProviders.Resource.Should().Be(deserializedDto.RmServiceProviders.Resource);
        reDeserializedDto.CmServiceProviders.Resource.Should().Be(deserializedDto.CmServiceProviders.Resource);
        reDeserializedDto.OAuthRealmName.Should().Be(deserializedDto.OAuthRealmName);
        reDeserializedDto.OAuthDomain.Should().Be(deserializedDto.OAuthDomain);
        reDeserializedDto.OAuthRequestConsumerKeyUrl.Resource.Should().Be(deserializedDto.OAuthRequestConsumerKeyUrl.Resource);
        reDeserializedDto.OAuthApprovalModuleUrl.Resource.Should().Be(deserializedDto.OAuthApprovalModuleUrl.Resource);
        reDeserializedDto.OAuthRequestTokenUrl.Resource.Should().Be(deserializedDto.OAuthRequestTokenUrl.Resource);
        reDeserializedDto.OAuthUserAuthorizationUrl.Resource.Should().Be(deserializedDto.OAuthUserAuthorizationUrl.Resource);
        reDeserializedDto.OAuthAccessTokenUrl.Resource.Should().Be(deserializedDto.OAuthAccessTokenUrl.Resource);
    }

    [Fact]
    public void Serialize_ShouldProduceValidRdfXml()
    {
        // Arrange
        var dto = new RootServicesDto
        {
            About = "https://example.com/services/rootservices",
            Title = "Root Services",
            AmServiceProviders = new ResourceReference { Resource = "https://example.com/services/catalog/singleton" },
            RmServiceProviders = new ResourceReference { Resource = "https://example.com/services/catalog/singleton" },
            CmServiceProviders = new ResourceReference { Resource = "https://example.com/services/catalog/singleton" },
            OAuthRealmName = "TEST",
            OAuthDomain = "https://example.com/",
            OAuthRequestConsumerKeyUrl = new ResourceReference { Resource = "https://example.com/services/oauth/requestKey" },
            OAuthApprovalModuleUrl = new ResourceReference { Resource = "https://example.com/services/oauth/approveKey" },
            OAuthRequestTokenUrl = new ResourceReference { Resource = "https://example.com/services/oauth/requestToken" },
            OAuthUserAuthorizationUrl = new ResourceReference { Resource = "https://example.com/services/oauth/authorize" },
            OAuthAccessTokenUrl = new ResourceReference { Resource = "https://example.com/services/oauth/accessToken" }
        };        // Act
        var xmlContent = dto.ToXml();

        // Assert
        xmlContent.Should().NotBeNullOrEmpty();
        xmlContent.Should().Contain("xmlns:oslc_cm=\"http://open-services.net/xmlns/cm/1.0/\"");
        xmlContent.Should().Contain("xmlns:oslc_am=\"http://open-services.net/ns/am#\"");
        xmlContent.Should().Contain("xmlns:oslc_rm=\"http://open-services.net/xmlns/rm/1.0/\"");
        xmlContent.Should().Contain("xmlns:dc=\"http://purl.org/dc/terms/\"");
        xmlContent.Should().Contain("xmlns:jfs=\"http://jazz.net/xmlns/prod/jazz/jfs/1.0/\"");
        xmlContent.Should().Contain("xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"");
        xmlContent.Should().Contain("rdf:about=\"https://example.com/services/rootservices\"");
        xmlContent.Should().Contain("<dc:title>Root Services</dc:title>");
        xmlContent.Should().Contain("rdf:resource=\"https://example.com/services/catalog/singleton\"");
        xmlContent.Should().Contain("<jfs:oauthRealmName>TEST</jfs:oauthRealmName>");
        xmlContent.Should().Contain("<jfs:oauthDomain>https://example.com/</jfs:oauthDomain>");
    }

    [Fact]
    public void Serialize_ShouldProduceValidXmlDocument()
    {
        // Arrange
        var dto = new RootServicesDto
        {
            About = "https://test.example.com/services/rootservices",
            AmServiceProviders = new ResourceReference { Resource = "https://test.example.com/services/catalog/singleton" },
            RmServiceProviders = new ResourceReference { Resource = "https://test.example.com/services/catalog/singleton" },
            CmServiceProviders = new ResourceReference { Resource = "https://test.example.com/services/catalog/singleton" },
            OAuthRealmName = "TEST_REALM",
            OAuthDomain = "https://test.example.com/",
            OAuthRequestConsumerKeyUrl = new ResourceReference { Resource = "https://test.example.com/services/oauth/requestKey" },
            OAuthApprovalModuleUrl = new ResourceReference { Resource = "https://test.example.com/services/oauth/approveKey" },
            OAuthRequestTokenUrl = new ResourceReference { Resource = "https://test.example.com/services/oauth/requestToken" },
            OAuthUserAuthorizationUrl = new ResourceReference { Resource = "https://test.example.com/services/oauth/authorize" },
            OAuthAccessTokenUrl = new ResourceReference { Resource = "https://test.example.com/services/oauth/accessToken" }
        };

        // Act
        string xmlContent;
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings 
        { 
            Indent = true, 
            IndentChars = "    ",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        }))
        {
            _serializer.Serialize(xmlWriter, dto);
            xmlContent = stringWriter.ToString();
        }

        // Assert - The XML should be valid and parseable
        var xmlDoc = new XmlDocument();
        var parseAction = () => xmlDoc.LoadXml(xmlContent);
        parseAction.Should().NotThrow("because the serialized XML should be well-formed");

        // Verify the XML structure is correct
        xmlDoc.DocumentElement.Should().NotBeNull();
        xmlDoc.DocumentElement!.LocalName.Should().Be("Description");
        xmlDoc.DocumentElement.NamespaceURI.Should().Be("http://www.w3.org/1999/02/22-rdf-syntax-ns#");
    }

    [Fact]
    public void Constructor_ShouldInitializeNamespacesCorrectly()
    {
        // Act
        var dto = new RootServicesDto();

        // Assert
        dto.Namespaces.Should().NotBeNull();
        
        // Check that all required namespaces are present
        var namespaces = new Dictionary<string, string>();
        dto.Namespaces.ToArray().ToList().ForEach(ns => namespaces.Add(ns.Name, ns.Namespace));

        namespaces.Should().ContainKey("oslc_cm").WhoseValue.Should().Be("http://open-services.net/xmlns/cm/1.0/");
        namespaces.Should().ContainKey("oslc_am").WhoseValue.Should().Be("http://open-services.net/ns/am#");
        namespaces.Should().ContainKey("oslc_rm").WhoseValue.Should().Be("http://open-services.net/xmlns/rm/1.0/");
        namespaces.Should().ContainKey("dc").WhoseValue.Should().Be("http://purl.org/dc/terms/");
        namespaces.Should().ContainKey("jfs").WhoseValue.Should().Be("http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
        namespaces.Should().ContainKey("rdf").WhoseValue.Should().Be("http://www.w3.org/1999/02/22-rdf-syntax-ns#");
    }
}
