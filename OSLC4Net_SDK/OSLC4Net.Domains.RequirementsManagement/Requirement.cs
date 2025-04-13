using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Domains.RequirementsManagement;

[OslcNamespace(Constants.Domains.RM.NS)]
[OslcResourceShape(title = "Requirement Resource Shape",
    describes = [Constants.Domains.RM.Requirement])]
public record Requirement : IExtendedResource
{
    public Uri About { get; set; }

    public readonly List<Uri> Types = new();

    public IDictionary<QName, object> ExtendedProperties { get; private set; } =
        new Dictionary<QName, object>();

    [OslcDescription(
        "Title (reference: Dublin Core) or often a single line summary of the resource represented as rich text in XHTML content.")]
    // REVISIT: consider [OslcRequired] annotation that will result in 1..1 or 1..* depending on the backing type (@berezovskyi 2025-04)
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "title")]
    [OslcTitle("Title")]
    // REVISIT: consider a default value types map (@berezovskyi 2025-04)
    [OslcValueType(ValueType.XMLLiteral)]
    public string Title { get; set; }

    [OslcDescription(
        "A unique identifier for a resource. Assigned by the service provider when a resource is created. Not intended for end-user display.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.DCTERMS_NAMESPACE + "identifier")]
    [OslcReadOnly]
    [OslcTitle("Identifier")]
    public string Identifier { get; set; }


    public Uri GetAbout()
    {
        return About;
    }

    public void SetAbout(Uri about)
    {
        About = about;
    }

    public ICollection<Uri> GetTypes()
    {
        return Types;
    }

    public void SetTypes(ICollection<Uri> types)
    {
        Types.Clear();
        Types.AddAll(types);
    }

    public void AddType(Uri type)
    {
        Types.Add(type);
    }

    public void SetExtendedProperties(IDictionary<QName, object> properties)
    {
        // must be implemented this way due to how DotNetRdfHelper works - do not Clear+AddRange
        ExtendedProperties = properties;
    }

    public IDictionary<QName, object> GetExtendedProperties()
    {
        return ExtendedProperties;
    }
}
