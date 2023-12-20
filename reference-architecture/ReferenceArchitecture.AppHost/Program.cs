var builder = DistributedApplication.CreateBuilder(args);

// TODO: Uncomment if Dapr installed on Mac with Homebrew. See https://github.com/dotnet/aspire-samples/issues/67
// builder.AddDapr(configure => configure.DaprPath = "/opt/homebrew/Cellar/dapr-cli/1.12.0/bin/dapr");

// To set Dapr log level pass dapr options to WithDaprSidecar method instead of string app id
// var customerDaprOptions = new DaprSidecarOptions { AppId = "customer-service", LogLevel = "debug", EnableApiLogging = true };
// var orderDaprOptions = new DaprSidecarOptions { AppId = "order-service", LogLevel = "debug", EnableApiLogging = true };

var pubSub = builder.AddDaprPubSub("pubsub");

builder.AddProject<Projects.CustomerService>("customer-service")
    .WithDaprSidecar()
    .WithReference(pubSub);
builder.AddProject<Projects.OrderService>("order-service")
    .WithDaprSidecar()
    .WithReference(pubSub);

builder.Build().Run();
