using Common.Behaviors;
using EventDriven.CQRS.Abstractions.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using MediatR;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Integration.EventHandlers;
using OrderService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));

// Add command and query handlers
builder.Services.AddHandlers(typeof(Program));

// Add behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Add database settings
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(builder.Configuration);

// Add event handlers
builder.Services.AddSingleton<CustomerAddressUpdatedEventHandler>();

// Add Dapr event bus
builder.Services.AddDaprEventBus(builder.Configuration);
builder.Services.AddMongoEventCache(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

// Use Cloud Events (needed by Dapr)
app.UseCloudEvents();

app.MapControllers();

// Map Dapr subscriber (needed by Dapr)
app.MapSubscribeHandler();

// Map Dapr Event Bus subscribers
app.MapDaprEventBus(eventBus => // used by event bus
{
    var customerAddressUpdatedEventHandler = app.Services.GetRequiredService<CustomerAddressUpdatedEventHandler>();
    eventBus?.Subscribe(customerAddressUpdatedEventHandler, null, "v1");
});

app.Run();