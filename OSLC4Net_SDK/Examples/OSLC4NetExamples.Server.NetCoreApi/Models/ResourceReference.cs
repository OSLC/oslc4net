using System.Xml.Serialization;

namespace OSLC4NetExamples.Server.NetCoreApi.Models;

public record ResourceReference
{
    [XmlAttribute("resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public string Resource { get; init; } = string.Empty;
}
