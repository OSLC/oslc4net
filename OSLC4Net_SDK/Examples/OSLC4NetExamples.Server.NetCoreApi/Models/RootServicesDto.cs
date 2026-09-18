using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OSLC4NetExamples.Server.NetCoreApi.Models;

[XmlRoot("Description", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
public record RootServicesDto
{
    [XmlNamespaceDeclarations]
    public XmlSerializerNamespaces Namespaces { get; set; } = new();

    [XmlAttribute("about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public string About { get; init; } = string.Empty;

    [XmlElement("title", Namespace = "http://purl.org/dc/terms/")]
    public string Title { get; init; } = "Root Services";

    [XmlElement("amServiceProviders", Namespace = "http://open-services.net/ns/am#")]
    public ResourceReference AmServiceProviders { get; init; } = new();

    [XmlElement("rmServiceProviders", Namespace = "http://open-services.net/xmlns/rm/1.0/")]
    public ResourceReference RmServiceProviders { get; init; } = new();

    [XmlElement("cmServiceProviders", Namespace = "http://open-services.net/xmlns/cm/1.0/")]
    public ResourceReference CmServiceProviders { get; init; } = new();

    [XmlElement("oauthRealmName", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public string OAuthRealmName { get; init; } = string.Empty;

    [XmlElement("oauthDomain", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public string OAuthDomain { get; init; } = string.Empty;

    [XmlElement("oauthRequestConsumerKeyUrl", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public ResourceReference OAuthRequestConsumerKeyUrl { get; init; } = new();

    [XmlElement("oauthApprovalModuleUrl", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public ResourceReference OAuthApprovalModuleUrl { get; init; } = new();

    [XmlElement("oauthRequestTokenUrl", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public ResourceReference OAuthRequestTokenUrl { get; init; } = new();

    [XmlElement("oauthUserAuthorizationUrl", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public ResourceReference OAuthUserAuthorizationUrl { get; init; } = new();

    [XmlElement("oauthAccessTokenUrl", Namespace = "http://jazz.net/xmlns/prod/jazz/jfs/1.0/")]
    public ResourceReference OAuthAccessTokenUrl { get; init; } = new();

    public RootServicesDto()
    {
        Namespaces.Add("oslc_cm", "http://open-services.net/xmlns/cm/1.0/");
        Namespaces.Add("oslc_am", "http://open-services.net/ns/am#");
        Namespaces.Add("oslc_rm", "http://open-services.net/xmlns/rm/1.0/");
        Namespaces.Add("dc", "http://purl.org/dc/terms/");
        Namespaces.Add("jfs", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
        Namespaces.Add("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
    }

    // Create a static method to deserialize from XML with custom namespace handling
    public static RootServicesDto FromXml(string xml)
    {
        try
        {
            // Create an XmlDocument to parse the RDF
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            // Set up namespace manager
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            nsManager.AddNamespace("dc", "http://purl.org/dc/terms/");
            nsManager.AddNamespace("oslc_am", "http://open-services.net/ns/am#");
            nsManager.AddNamespace("oslc_rm", "http://open-services.net/xmlns/rm/1.0/");
            nsManager.AddNamespace("oslc_cm", "http://open-services.net/xmlns/cm/1.0/");
            nsManager.AddNamespace("jfs", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");

            var root = doc.DocumentElement;
            if (root == null)
            {
                return new RootServicesDto();
            }

            return ParseDtoFromXml(root, nsManager);
        }
        catch
        {
            return new RootServicesDto();
        }
    }

    private static RootServicesDto ParseDtoFromXml(XmlElement root, XmlNamespaceManager nsManager)
    {
        return new RootServicesDto
        {
            About = root.GetAttribute("about", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
            Title = root.SelectSingleNode("dc:title", nsManager)?.InnerText ?? "Root Services",
            OAuthRealmName = root.SelectSingleNode("jfs:oauthRealmName", nsManager)?.InnerText ?? "",
            OAuthDomain = root.SelectSingleNode("jfs:oauthDomain", nsManager)?.InnerText ?? "",
            AmServiceProviders = GetResourceReference(root, "oslc_am:amServiceProviders", nsManager),
            RmServiceProviders = GetResourceReference(root, "oslc_rm:rmServiceProviders", nsManager),
            CmServiceProviders = GetResourceReference(root, "oslc_cm:cmServiceProviders", nsManager),
            OAuthRequestConsumerKeyUrl = GetResourceReference(root, "jfs:oauthRequestConsumerKeyUrl", nsManager),
            OAuthApprovalModuleUrl = GetResourceReference(root, "jfs:oauthApprovalModuleUrl", nsManager),
            OAuthRequestTokenUrl = GetResourceReference(root, "jfs:oauthRequestTokenUrl", nsManager),
            OAuthUserAuthorizationUrl = GetResourceReference(root, "jfs:oauthUserAuthorizationUrl", nsManager),
            OAuthAccessTokenUrl = GetResourceReference(root, "jfs:oauthAccessTokenUrl", nsManager)
        };
    }

    private static ResourceReference GetResourceReference(XmlElement root, string xpath, XmlNamespaceManager nsManager)
    {
        return new ResourceReference
        {
            Resource = (root.SelectSingleNode(xpath, nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
        };
    }

    // Create a method to serialize to XML with proper RDF formatting
    public string ToXml()
    {
        try
        {
            var doc = new XmlDocument();
            var root = CreateRootElement(doc);
            doc.AppendChild(root);

            AddBasicElements(doc, root);
            AddServiceProviderElements(doc, root);
            AddOAuthElements(doc, root);

            return FormatXmlDocument(doc);
        }
        catch
        {
            return string.Empty;
        }
    }

    private XmlElement CreateRootElement(XmlDocument doc)
    {
        var root = doc.CreateElement("rdf", "Description", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
        root.SetAttribute("xmlns:oslc_cm", "http://open-services.net/xmlns/cm/1.0/");
        root.SetAttribute("xmlns:oslc_am", "http://open-services.net/ns/am#");
        root.SetAttribute("xmlns:oslc_rm", "http://open-services.net/xmlns/rm/1.0/");
        root.SetAttribute("xmlns:dc", "http://purl.org/dc/terms/");
        root.SetAttribute("xmlns:jfs", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
        root.SetAttribute("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
        root.SetAttribute("about", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", About);
        return root;
    }

    private void AddBasicElements(XmlDocument doc, XmlElement root)
    {
        var titleElement = doc.CreateElement("dc", "title", "http://purl.org/dc/terms/");
        titleElement.InnerText = Title;
        root.AppendChild(titleElement);
    }

    private void AddServiceProviderElements(XmlDocument doc, XmlElement root)
    {
        AddResourceElement(doc, root, "oslc_am", "amServiceProviders", "http://open-services.net/ns/am#", AmServiceProviders.Resource);
        AddResourceElement(doc, root, "oslc_rm", "rmServiceProviders", "http://open-services.net/xmlns/rm/1.0/", RmServiceProviders.Resource);
        AddResourceElement(doc, root, "oslc_cm", "cmServiceProviders", "http://open-services.net/xmlns/cm/1.0/", CmServiceProviders.Resource);
    }

    private void AddOAuthElements(XmlDocument doc, XmlElement root)
    {
        var realmElement = doc.CreateElement("jfs", "oauthRealmName", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
        realmElement.InnerText = OAuthRealmName;
        root.AppendChild(realmElement);

        var domainElement = doc.CreateElement("jfs", "oauthDomain", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
        domainElement.InnerText = OAuthDomain;
        root.AppendChild(domainElement);

        AddResourceElement(doc, root, "jfs", "oauthRequestConsumerKeyUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/", OAuthRequestConsumerKeyUrl.Resource);
        AddResourceElement(doc, root, "jfs", "oauthApprovalModuleUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/", OAuthApprovalModuleUrl.Resource);
        AddResourceElement(doc, root, "jfs", "oauthRequestTokenUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/", OAuthRequestTokenUrl.Resource);
        AddResourceElement(doc, root, "jfs", "oauthUserAuthorizationUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/", OAuthUserAuthorizationUrl.Resource);
        AddResourceElement(doc, root, "jfs", "oauthAccessTokenUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/", OAuthAccessTokenUrl.Resource);
    }

    private static void AddResourceElement(XmlDocument doc, XmlElement root, string prefix, string localName, string namespaceUri, string resourceValue)
    {
        var element = doc.CreateElement(prefix, localName, namespaceUri);
        element.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", resourceValue);
        root.AppendChild(element);
    }

    private static string FormatXmlDocument(XmlDocument doc)
    {
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        });

        doc.WriteContentTo(xmlWriter);
        xmlWriter.Flush();

        return stringWriter.ToString();
    }
}
