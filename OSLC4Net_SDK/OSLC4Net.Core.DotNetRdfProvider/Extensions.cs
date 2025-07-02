using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Exceptions;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Core.DotNetRdfProvider;

public static class Extensions
{
    public static async Task<OslcCoreRequestException> ToOslcExceptionAsync(this
        HttpResponseMessage responseMessage, IResource? requestResource)
    {
        Error? errorResource = null;
        try
        {
            var formatter = new RdfXmlMediaTypeFormatter();
            errorResource = await formatter.ReadFromStreamAsync(typeof(Error),
                    await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false),
                    responseMessage.Content,
                    null)
                .ConfigureAwait(false) as Error;
        }
        catch
        {
            // ignored
        }

        return new OslcCoreRequestException(responseMessage.StatusCode,
            responseMessage.ReasonPhrase, requestResource, errorResource);
    }

    public static bool IsOslcSingleton(this Type type)
    {
        return type.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0;
    }

    public static Type? GetMemberType(this Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (InheritedGenericInterfacesHelper.ImplementsGenericInterface(typeof(IEnumerable<>),
                type))
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() ==
                    typeof(IEnumerable<object>).GetGenericTypeDefinition())
                {
                    var memberType = iface.GetGenericArguments()[0];

                    if (memberType.GetCustomAttributes(typeof(OslcResourceShape), false).Length > 0)
                    {
                        return memberType;
                    }

                    return null;
                }
            }
        }

        return null;
    }
}
