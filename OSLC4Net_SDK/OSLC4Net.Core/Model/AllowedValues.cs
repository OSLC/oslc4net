using System.Diagnostics.CodeAnalysis;
using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

[Experimental("OSLCEXP001", Message = "This class may change or be removed in future releases after [OslcAllowedValues] attribute support is fully implemented and verified.")]
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Allowed Values Resource Shape",
    describes = new[] { OslcConstants.TYPE_ALLOWED_VALUES })]
public sealed class AllowedValues : IResource
{
    private readonly List<string> allowedValues = [];

    public Uri About { get; set; } = null!;

    public void AddAllowedValue(string allowedValue)
    {
        allowedValues.Add(allowedValue);
    }

    public Uri GetAbout() => About;

    [OslcDescription("Value allowed for a property")]
    [OslcName("allowedValue")]
    [OslcOccurs(Occurs.OneOrMany)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "allowedValue")]
    [OslcReadOnly]
    [OslcTitle("Allowed Values")]
    public string[] GetAllowedValues()
    {
        return allowedValues.ToArray<string>();
    }

    public void SetAbout(Uri about)
    {
        About = about;
    }

    public void SetAllowedValues(string[] allowedValues)
    {
        this.allowedValues.Clear();
        if (allowedValues != null)
        {
            this.allowedValues.AddAll(allowedValues.ToList<string>());
        }
    }
}
