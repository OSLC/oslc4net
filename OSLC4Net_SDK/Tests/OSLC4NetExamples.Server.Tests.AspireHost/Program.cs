using System.Net;

var builder = DistributedApplication.CreateBuilder(args);

var oslcNetCoreApi = builder
    .AddProject<Projects.OSLC4NetExamples_Server_NetCoreApi>("oslc-netcore-api")
    .WithEndpoint(7270, 7270, isExternal: true, isProxied: false,
        scheme: "https", name: "api-https");

await builder.Build().RunAsync();
