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
}
