using System.Diagnostics.CodeAnalysis;
using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

[Experimental("EXP001", Message = "This class may change or be removed in future releases after [OslcAllowedValues] attribute support is fully implemented and verified.")]
[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Allowed Values Resource Shape",
    describes = new[] { OslcConstants.TYPE_ALLOWED_VALUES })]
public sealed class AllowedValues
{
    private readonly List<string> allowedValues = new();

    public void AddAllowedValue(string allowedValue)
    {
        allowedValues.Add(allowedValue);
    }

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

    public void SetAllowedValues(string[] allowedValues)
    {
        this.allowedValues.Clear();
        if (allowedValues != null)
        {
            this.allowedValues.AddAll(allowedValues.ToList<string>());
        }
    }
}
