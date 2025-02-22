// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client;
using OSLC4Net.Client.Oslc;
using OSLC4Net.Client.Oslc.Resources;

const string jazzCmBase = "https://jazz.net/sandbox01-ccm";
const string workItemId = "1300";

var host = Host.CreateDefaultBuilder()
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole(); // Add console logging
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("OSLC4Net client started");

    string username = "%USERNAME%";
    string password = "%PASSWORD%";
    var oslcClient = OslcClient.ForBasicAuth(username, password);

    var resourceUri =
        $"{jazzCmBase}/resource/itemName/com.ibm.team.workitem.WorkItem/{workItemId}";
    OslcResponse<ChangeRequest> response = await oslcClient.GetResourceAsync<ChangeRequest>(resourceUri);
    if (response.Resource is not null)
    {
        var changeRequestResource = response.Resource;
        logger.LogInformation(
            "{shortTitle} {title}", changeRequestResource.GetShortTitle(),
            changeRequestResource.GetTitle());
    }
    else
    {
        logger.LogError("Something went wrong: {status} {reason}",
            (int?)response.StatusCode ?? -1,
            response.ResponseMessage?.ReasonPhrase);
    }

    logger.LogDebug("END");
}
