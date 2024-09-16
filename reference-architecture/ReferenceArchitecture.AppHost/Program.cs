var builder = DistributedApplication.CreateBuilder(args);

var pubSub = builder.AddDaprPubSub("pubsub");

builder.AddProject<Projects.CustomerService>("customer-service")
    .WithDaprSidecar()
    .WithReference(pubSub);
builder.AddProject<Projects.OrderService>("order-service")
    .WithDaprSidecar()
    .WithReference(pubSub);

builder.Build().Run();
