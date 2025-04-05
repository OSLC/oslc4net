var builder = DistributedApplication.CreateBuilder(args);

//builder.AddContainer("refimpl-cm", "server-cm")
//    .WithEndpoint(port: 8801, targetPort: 8080, scheme: "http", name: "http");

builder.Build().Run();
