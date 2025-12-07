using Microsoft.Extensions.Logging;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Resources;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Client.Samples;

public abstract class SampleBase<TResource> where TResource : IExtendedResource
{
    protected readonly ILogger Logger;

    protected SampleBase(ILogger logger)
    {
        Logger = logger;
    }

    protected async Task ProcessPagedQueryResultsAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects)
    {
        int page = 1;
        do
        {
            Logger.LogInformation("Page {Page}:", page);
            await ProcessCurrentPageAsync(result, client, asDotNetObjects).ConfigureAwait(false);
            if (result.MoveNext())
            {
                result = result.Current;
                page++;
            }
            else
            {
                break;
            }
        } while (true);
    }

    protected async Task ProcessCurrentPageAsync(OslcQueryResult result, OslcClient client, bool asDotNetObjects)
    {
        if (asDotNetObjects)
        {
            foreach (var resource in result.GetMembers<TResource>())
            {
                PrintResourceInfo(resource);
            }
            return;
        }

        foreach (string resultsUrl in result.GetMembersUrls())
        {
            Logger.LogInformation(resultsUrl);

            try
            {
                //Get a single artifact by its URL 
                HttpResponseMessage response = await client.GetResourceRawAsync(resultsUrl, OSLCConstants.CT_RDF).ConfigureAwait(false);

                if (response != null)
                {
                    //Just print the raw RDF/XML (or process the XML as desired)
                    await ProcessRawResponseAsync(response).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to process artifact at url: {Url}", resultsUrl);
                throw;
            }
        }
    }

    protected async Task ProcessRawResponseAsync(HttpResponseMessage response)
    {
        if (!Logger.IsEnabled(LogLevel.Trace))
        {
            response.ConsumeContent();
            return;
        }

        using Stream inStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using StreamReader streamReader = new StreamReader(new BufferedStream(inStream), System.Text.Encoding.UTF8);

        string line = null;
        while ((line = await streamReader.ReadLineAsync().ConfigureAwait(false)) != null)
        {
            Logger.LogTrace(line);
        }
        response.ConsumeContent();
    }

    protected abstract void PrintResourceInfo(TResource resource);
}
