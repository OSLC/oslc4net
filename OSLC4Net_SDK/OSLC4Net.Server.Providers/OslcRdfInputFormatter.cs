using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace OSLC4Net.Server.Providers;

public class OslcRdfInputFormatter : TextInputFormatter
{
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        throw new NotImplementedException();
    }
}
