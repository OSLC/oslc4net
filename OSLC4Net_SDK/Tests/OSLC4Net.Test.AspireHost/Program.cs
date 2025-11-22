using System.Net;

var builder = DistributedApplication.CreateBuilder(args);

_ = builder
    .AddDockerfile("refimpl-cm", "../../../../refimpl/src/", "server-cm/Dockerfile")
    .WithEndpoint(8801, 8080, isExternal: true, isProxied: false,
        scheme: "http", name: "http")
    // .WithHttpHealthCheck("/services/catalog/singleton", (int)HttpStatusCode.Unauthorized);
    .WithHttpHealthCheck("/services/rootservices", (int)HttpStatusCode.OK);

_ = builder
    .AddDockerfile("refimpl-rm", "../../../../refimpl/src/", "server-rm/Dockerfile")
    .WithEndpoint(8800, 8080, isExternal: true, isProxied: false,
        scheme: "http", name: "http")
    // .WithHttpHealthCheck("/services/catalog/singleton", (int)HttpStatusCode.Unauthorized);
    .WithHttpHealthCheck("/services/rootservices", (int)HttpStatusCode.OK);

await builder.Build().RunAsync().ConfigureAwait(false);
