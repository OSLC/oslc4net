using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Domains.RequirementsManagement;

[OslcNamespace(Constants.Domains.RM.NS)]
[OslcResourceShape(title = "Requirement Collection resource shape",
    describes = [Constants.Domains.RM.RequirementCollection])]
public record RequirementCollection : AbstractResourceRecord
{
    public RequirementCollection(Uri about) : base(about)
    {
    }

    public RequirementCollection()
    {
    }

    //
    //
    // [OslcDescription("The resource type URIs.")] // Escape quotes in description
    // [OslcOccurs(Occurs.ZeroOrMany)]
    // [OslcPropertyDefinition("http://www.w3.org/1999/02/22-rdf-syntax-ns#type")]
    // [OslcName("type")]
    // [OslcRepresentation(Representation.Reference)]
    // [OslcReadOnly(false)] // Assuming read_only property exists
    // [OslcTitle("type")] // Use prop name as fallback title
    [Obsolete("Use Types instead")] public HashSet<Uri> Type => new(Types);

    [OslcDescription(
        "An identifier for a resource. This identifier may be unique with a scope that is defined by the RM provider. Assigned by the service provider when a resource is created. Not intended for end-user display.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrOne)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/identifier")]
    [OslcName("identifier")]
    [OslcValueType(ValueType.String)]
    [OslcReadOnly(true)] // Assuming read_only property exists
    [OslcTitle("identifier")] // Use prop name as fallback title
    public string Identifier { get; set; }

    [OslcDescription(
        "Title (reference: Dublin Core) of the resource represented as rich text in XHTML content. It SHOULD include only content that is valid inside an XHTML <span> element.")] // Escape quotes in description
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/title")]
    [OslcName("title")]
    [OslcValueType(ValueType.XMLLiteral)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("title")] // Use prop name as fallback title
    public string Title { get; set; }

    [OslcDescription(
        "Short name identifying a resource, often used as an abbreviated identifier for presentation to end-users. It SHOULD include only content that is valid inside an XHTML <span> element.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrOne)]
    [OslcPropertyDefinition("http://open-services.net/ns/core#shortTitle")]
    [OslcName("shortTitle")]
    [OslcValueType(ValueType.XMLLiteral)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("shortTitle")] // Use prop name as fallback title
    public string ShortTitle { get; set; }

    [OslcDescription(
        "Descriptive text (reference: Dublin Core) about resource represented as rich text in XHTML content. It SHOULD include only content that is valid and suitable inside an XHTML <div> element.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrOne)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/description")]
    [OslcName("description")]
    [OslcValueType(ValueType.XMLLiteral)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("description")] // Use prop name as fallback title
    public string Description { get; set; }

    [OslcDescription(
        "Tag or keyword for a resource. Each occurrence of a dcterms:subject property denotes an additional tag for the resource.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/subject")]
    [OslcName("subject")]
    [OslcValueType(ValueType.String)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("subject")] // Use prop name as fallback title
    public HashSet<string> Subject { get; set; }

    [OslcDescription(
        "Creator(s) of resource (reference: Dublin Core). It is likely that the target resource will be an <code>foaf:Person</code> but that is not necessarily the case.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/creator")]
    [OslcName("creator")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Either)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("creator")] // Use prop name as fallback title
    public HashSet<Uri> Creator { get; set; }

    [OslcDescription(
        "Contributor(s) to resource (reference: Dublin Core). It is likely that the target resource will be a <code>foaf:Person</code> but that is not necessarily the case.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/contributor")]
    [OslcName("contributor")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Either)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("contributor")] // Use prop name as fallback title
    public HashSet<Uri> Contributor { get; set; }

    [OslcDescription(
        "Timestamp of resource creation (reference: Dublin Core).")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrOne)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/created")]
    [OslcName("created")]
    [OslcValueType(ValueType.DateTime)]
    [OslcReadOnly(true)] // Assuming read_only property exists
    [OslcTitle("created")] // Use prop name as fallback title
    public DateTimeOffset? Created { get; set; }

    [OslcDescription(
        "Timestamp of last resource modification (reference: Dublin Core).")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrOne)]
    [OslcPropertyDefinition("http://purl.org/dc/terms/modified")]
    [OslcName("modified")]
    [OslcValueType(ValueType.DateTime)]
    [OslcReadOnly(true)] // Assuming read_only property exists
    [OslcTitle("modified")] // Use prop name as fallback title
    public DateTimeOffset? Modified { get; set; }

    [OslcDescription(
        "The scope of a resource is a URI for the resource's OSLC Service Provider.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/core#serviceProvider")]
    [OslcName("serviceProvider")]

    // Range specified: http://open-services.net/ns/core#ServiceProvider - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("serviceProvider")] // Use prop name as fallback title
    public HashSet<Uri> ServiceProvider { get; set; }

    [OslcDescription(
        "Resource Shape that provides hints as to resource property value-types and allowed values.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrOne)]
    [OslcPropertyDefinition("http://open-services.net/ns/core#instanceShape")]
    [OslcName("instanceShape")]

    // Range specified: http://open-services.net/ns/core#ResourceShape - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("instanceShape")] // Use prop name as fallback title
    public Uri InstanceShape { get; set; }

    [OslcDescription(
        "A collection uses a resource - the resource is in the requirement collection.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#uses")]
    [OslcName("uses")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("uses")] // Use prop name as fallback title
    public HashSet<Uri> Uses { get; set; }

    [OslcDescription(
        "The subject is elaborated by the object. For example, a collection of user requirements elaborates a business need, or a model elaborates a collection of system requirements.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#elaboratedBy")]
    [OslcName("elaboratedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("elaboratedBy")] // Use prop name as fallback title
    public HashSet<Uri> ElaboratedBy { get; set; }

    [OslcDescription("The object is elaborated by the subject.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#elaborates")]
    [OslcName("elaborates")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("elaborates")] // Use prop name as fallback title
    public HashSet<Uri> Elaborates { get; set; }

    [OslcDescription(
        "The subject is specified by the object. For example, a model element might make a requirement collection more precise.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#specifiedBy")]
    [OslcName("specifiedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("specifiedBy")] // Use prop name as fallback title
    public HashSet<Uri> SpecifiedBy { get; set; }

    [OslcDescription("The object is specified by the subject.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#specifies")]
    [OslcName("specifies")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("specifies")] // Use prop name as fallback title
    public HashSet<Uri> Specifies { get; set; }

    [OslcDescription(
        "The subject is affected by the object, such as a defect or issue.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#affectedBy")]
    [OslcName("affectedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("affectedBy")] // Use prop name as fallback title
    public HashSet<Uri> AffectedBy { get; set; }

    [OslcDescription(
        "Resource, such as a change request, which manages this requirement collection.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#trackedBy")]
    [OslcName("trackedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("trackedBy")] // Use prop name as fallback title
    public HashSet<Uri> TrackedBy { get; set; }

    [OslcDescription(
        "Resource, such as a change request, which implements this requirement collection.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#implementedBy")]
    [OslcName("implementedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("implementedBy")] // Use prop name as fallback title
    public HashSet<Uri> ImplementedBy { get; set; }

    [OslcDescription(
        "Resource, such as a test plan, which validates this requirement collection.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#validatedBy")]
    [OslcName("validatedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("validatedBy")] // Use prop name as fallback title
    public HashSet<Uri> ValidatedBy { get; set; }

    [OslcDescription(
        "The subject is satisfied by the object. For example, a collection of user requirements is satisfied by a requirement collection of system requirements.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#satisfiedBy")]
    [OslcName("satisfiedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("satisfiedBy")] // Use prop name as fallback title
    public HashSet<Uri> SatisfiedBy { get; set; }

    [OslcDescription("The object is satisfied by the subject.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#satisfies")]
    [OslcName("satisfies")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("satisfies")] // Use prop name as fallback title
    public HashSet<Uri> Satisfies { get; set; }

    [OslcDescription(
        "The subject is decomposed by the object. For example, a collection of business requirements is decomposed by a collection of user requirements.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#decomposedBy")]
    [OslcName("decomposedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("decomposedBy")] // Use prop name as fallback title
    public HashSet<Uri> DecomposedBy { get; set; }

    [OslcDescription("The object is decomposed by the subject.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#decomposes")]
    [OslcName("decomposes")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("decomposes")] // Use prop name as fallback title
    public HashSet<Uri> Decomposes { get; set; }

    [OslcDescription(
        "The subject is constrained by the object. For example, a requirement collection is constrained by a requirement collection.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#constrainedBy")]
    [OslcName("constrainedBy")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("constrainedBy")] // Use prop name as fallback title
    public HashSet<Uri> ConstrainedBy { get; set; }

    [OslcDescription("The object is constrained by the subject.")] // Escape quotes in description
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/rm#constrains")]
    [OslcName("constrains")]

    // Range specified: http://open-services.net/ns/core#AnyResource - Consider adding OslcRange attribute if needed
    [OslcRepresentation(Representation.Reference)]
    [OslcReadOnly(false)] // Assuming read_only property exists
    [OslcTitle("constrains")] // Use prop name as fallback title
    public HashSet<Uri> Constrains { get; set; }
}
