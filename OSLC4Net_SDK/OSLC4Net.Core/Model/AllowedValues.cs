using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Allowed Values Resource Shape",
    describes = new[] { OslcConstants.TYPE_ALLOWED_VALUES })]
internal class AllowedValues
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
