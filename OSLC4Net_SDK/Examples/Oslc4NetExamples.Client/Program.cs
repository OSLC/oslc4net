// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using OSLC4Net.Client.Oslc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OSLC4Net.Client;
using OSLC4Net.Client.Oslc.Resources;
using VDS.RDF;
using VDS.RDF.Parsing;

var host = Host.CreateDefaultBuilder()
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole(); // Add console logging
    })
    // .ConfigureServices((hostContext, services) =>
    // {
    //     services.AddScoped<DatabaseService>();
    // })
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
        "https://jazz.net/sandbox01-ccm/resource/itemName/com.ibm.team.workitem.WorkItem/1300";
    OslcResponse<ChangeRequest> response = await oslcClient.GetResourceAsync<ChangeRequest>(resourceUri);
    if (response.Resource is not null)
    {
        ChangeRequest wi1300 = response.Resource;
        logger.LogInformation($"{wi1300.GetShortTitle()} {wi1300.GetTitle()}");
    }
    else
    {
        logger.LogError("Something went wrong: {} {}", (response.StatusCode as int?) ?? -1,
            response.ResponseMessage?.ReasonPhrase);
    }

    logger.LogDebug("END");
}
