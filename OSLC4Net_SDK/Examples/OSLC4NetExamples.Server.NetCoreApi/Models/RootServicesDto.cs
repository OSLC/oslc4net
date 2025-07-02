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
            nsManager.AddNamespace("jfs", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/"); var root = doc.DocumentElement;
            if (root == null)
            {
                return new RootServicesDto();
            }

            var dto = new RootServicesDto
            {
                About = root.GetAttribute("about", "http://www.w3.org/1999/02/22-rdf-syntax-ns#"),
                Title = root.SelectSingleNode("dc:title", nsManager)?.InnerText ?? "Root Services",
                OAuthRealmName = root.SelectSingleNode("jfs:oauthRealmName", nsManager)?.InnerText ?? "",
                OAuthDomain = root.SelectSingleNode("jfs:oauthDomain", nsManager)?.InnerText ?? "",
                AmServiceProviders = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("oslc_am:amServiceProviders", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                RmServiceProviders = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("oslc_rm:rmServiceProviders", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                CmServiceProviders = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("oslc_cm:cmServiceProviders", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                OAuthRequestConsumerKeyUrl = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("jfs:oauthRequestConsumerKeyUrl", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                OAuthApprovalModuleUrl = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("jfs:oauthApprovalModuleUrl", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                OAuthRequestTokenUrl = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("jfs:oauthRequestTokenUrl", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                OAuthUserAuthorizationUrl = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("jfs:oauthUserAuthorizationUrl", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                },
                OAuthAccessTokenUrl = new ResourceReference
                {
                    Resource = (root.SelectSingleNode("jfs:oauthAccessTokenUrl", nsManager) as XmlElement)?.GetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#") ?? ""
                }
            };

            return dto;
        }
        catch
        {
            return new RootServicesDto();
        }
    }

    // Create a method to serialize to XML with proper RDF formatting
    public string ToXml()
    {
        try
        {
            var doc = new XmlDocument();

            // Create the root element with proper namespace
            var root = doc.CreateElement("rdf", "Description", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            // Add namespace declarations
            root.SetAttribute("xmlns:oslc_cm", "http://open-services.net/xmlns/cm/1.0/");
            root.SetAttribute("xmlns:oslc_am", "http://open-services.net/ns/am#");
            root.SetAttribute("xmlns:oslc_rm", "http://open-services.net/xmlns/rm/1.0/");
            root.SetAttribute("xmlns:dc", "http://purl.org/dc/terms/");
            root.SetAttribute("xmlns:jfs", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            root.SetAttribute("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            // Add the rdf:about attribute
            root.SetAttribute("about", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", About);

            doc.AppendChild(root);

            // Add child elements
            var titleElement = doc.CreateElement("dc", "title", "http://purl.org/dc/terms/");
            titleElement.InnerText = Title;
            root.AppendChild(titleElement);

            // Add service provider elements
            var amElement = doc.CreateElement("oslc_am", "amServiceProviders", "http://open-services.net/ns/am#");
            amElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", AmServiceProviders.Resource);
            root.AppendChild(amElement);

            var rmElement = doc.CreateElement("oslc_rm", "rmServiceProviders", "http://open-services.net/xmlns/rm/1.0/");
            rmElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", RmServiceProviders.Resource);
            root.AppendChild(rmElement);

            var cmElement = doc.CreateElement("oslc_cm", "cmServiceProviders", "http://open-services.net/xmlns/cm/1.0/");
            cmElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", CmServiceProviders.Resource);
            root.AppendChild(cmElement);

            // Add OAuth elements
            var realmElement = doc.CreateElement("jfs", "oauthRealmName", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            realmElement.InnerText = OAuthRealmName;
            root.AppendChild(realmElement);

            var domainElement = doc.CreateElement("jfs", "oauthDomain", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            domainElement.InnerText = OAuthDomain;
            root.AppendChild(domainElement);

            var requestKeyElement = doc.CreateElement("jfs", "oauthRequestConsumerKeyUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            requestKeyElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", OAuthRequestConsumerKeyUrl.Resource);
            root.AppendChild(requestKeyElement);

            var approvalElement = doc.CreateElement("jfs", "oauthApprovalModuleUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            approvalElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", OAuthApprovalModuleUrl.Resource);
            root.AppendChild(approvalElement);

            var requestTokenElement = doc.CreateElement("jfs", "oauthRequestTokenUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            requestTokenElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", OAuthRequestTokenUrl.Resource);
            root.AppendChild(requestTokenElement);

            var authElement = doc.CreateElement("jfs", "oauthUserAuthorizationUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            authElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", OAuthUserAuthorizationUrl.Resource);
            root.AppendChild(authElement);

            var accessTokenElement = doc.CreateElement("jfs", "oauthAccessTokenUrl", "http://jazz.net/xmlns/prod/jazz/jfs/1.0/");
            accessTokenElement.SetAttribute("resource", "http://www.w3.org/1999/02/22-rdf-syntax-ns#", OAuthAccessTokenUrl.Resource);
            root.AppendChild(accessTokenElement);

            // Create XML declaration and format the output
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
        catch
        {
            return string.Empty;
        }
    }
}

public record ResourceReference
{
    [XmlAttribute("resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public string Resource { get; init; } = string.Empty;
}
