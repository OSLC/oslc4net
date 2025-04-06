var builder = DistributedApplication.CreateBuilder(args);

// NOTE: we need to grab the endpoint, thus this container is added in the DistributedApplicationTestingBuilder
//builder.AddContainer("refimpl-cm", "server-cm")
//    .WithEndpoint(port: 8801, targetPort: 8080, scheme: "http", name: "http");

builder.Build().Run();
