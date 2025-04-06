using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Error Resource Shape",
    describes = new[] { OslcConstants.TYPE_ERROR })]
public class Error
{
    private ExtendedError extendedError;
    private string message;
    private string statusCode;

    [OslcDescription("Extended error information.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "extendedError")]
    [OslcRange(OslcConstants.TYPE_EXTENDED_ERROR)]
    [OslcReadOnly]
    [OslcRepresentation(Representation.Inline)]
    [OslcTitle("Extended Error")]
    [OslcValueShape(OslcConstants.PATH_RESOURCE_SHAPES + "/" + OslcConstants.PATH_EXTENDED_ERROR)]
    [OslcValueType(ValueType.LocalResource)]
    public ExtendedError GetExtendedError()
    {
        return extendedError;
    }

    [OslcDescription("An informative message describing the error that occurred.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "message")]
    [OslcReadOnly]
    [OslcTitle("Message")]
    public string GetMessage()
    {
        return message;
    }

    [OslcDescription("The HTTP status code reported with the error.")]
    [OslcOccurs(Occurs.ExactlyOne)]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "statusCode")]
    [OslcReadOnly]
    [OslcTitle("Status Code")]
    public string GetStatusCode()
    {
        return statusCode;
    }

    public void SetExtendedError(ExtendedError extendedError)
    {
        this.extendedError = extendedError;
    }

    public void SetMessage(string message)
    {
        this.message = message;
    }

    public void SetStatusCode(string statusCode)
    {
        this.statusCode = statusCode;
    }
}
