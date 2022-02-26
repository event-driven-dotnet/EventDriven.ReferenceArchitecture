# EventDriven.ReferenceArchitecture

Reference architecture for using **EventDriven** abstractions and libraries for Domain Driven Design (**DDD**), Command-Query Responsibility Segregation (**CQRS**) and Event Driven Architecture (**EDA**).

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (5.0 or greater)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- MongoDB Docker: `docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo`
- [MongoDB Client](https://robomongo.org/download):
  - Download Robo 3T only.
  - Add connection to localhost on port 27017.
- [Dapr](https://dapr.io/) (Distributed Application Runtime)
  - [Install Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
  - [Initialize Dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)
- [Microsoft Tye](https://github.com/dotnet/tye/blob/main/docs/getting_started.md) (recommended)

## Introduction

This project builds on the principles of [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) (DDD) to provide a set of abstractions and reference architecture for implementing the [Command Query Responsibility Segregation](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs) (CQRS) pattern. By providing an event bus abstraction over [Dapr](https://dapr.io/) (Distributed Application Runtime), the reference architecture demonstrates how to apply principles of [Event Driven Architecture](https://en.wikipedia.org/wiki/Event-driven_architecture) (EDA). Because entities process commands by emitting domain events, adding [event sourcing](https://microservices.io/patterns/data/event-sourcing.html) at a later time will be relatively straightforward.

The **Reference Architecture** projects demonstrate how to apply these concepts to two microservices: `CustomerService` and `OrderService`. In addition, each service has *separate controllers for read and write operations*, thus segregating command and query responsibilities, with different sets of models, or Data Transfer Objects (DTO's).
- **Query Controller**: Uses repository to retrieve entities and converts them to DTO's with AutoMapper.
- **Command Controller**: Converts DTO's to domain entities using AutoMapper. Then hands control over to a command handler for executing business logic.
- **Command Handler**: Uses a domain entity to process commands which generate one or more domain events, then requests entity to apply the domain events in order to mutate entity state. Persists entity state to a state store and optionally publishes an integration event which is handled by another microservice.
- **Repository**: Persists entity state to a database.
- **Event Bus**: Used to publish integration events, as well as subscribe to events using an event handler. Dapr is used to abstract away the underlying pub/sub implementation. The default is Redis (for local development), but Dapr can be configured to use other components, such as AWS SNS+SQS.

> **Note**: This example illustrates a *simple* CQRS implementation with a **shared database** and **single service** for both read and write operations. A more sophisticated implementation might entail **separate services and databases** for read and write operations, using integration events to communicate between them. This simple example only uses integration events to communicate between the customer and order services.

<p align="center">
  <img width="600" src="images/event-driven-cqrs-ref-arch.png">
</p>

### Run services with Tye and Dapr

> **Note**: As an alternative to Tye, you can run services directly usng the Dapr CLI. This may be useful for troubleshooting Dapr issues after setting `Microsoft.AspNetCore` logging level to `Debug`.
> `dapr run --app-id service-name --app-port #### --components-path ../dapr/components -- dotnet run`

1. Open a terminal at the **reference-architecture** directory and run Tye to launch all services simultaneously.
    ```
    tye run
    ```
2. Alternatively, run Tye in debug mode.
    ```
    tye run --debug *
    ```
    - Set breakpoints in **OrderService**, **CustomerService**.
    - Attach the IDE debugger to **OrderService.dll**, **CustomerService.dll**.
3. Open the Tye dashboard at http://localhost:8000 to inspect service endpoints and view logs.
4. Create some customers.
   - Open http://localhost:5656/swagger
   - Execute posts using contents of **customers.json**.
   - Copy post response, modify fields, then execute puts.
     - Make sure to copy `etag` value from last response, or you will get a concurrency error.
   - Copy `id` and `etag` values to execute deletes.
   - Execute gets to retrieve customers.
   - View customers database collections using Robo 3T.
5. Create some orders.
   - Open http://localhost:5757/swagger
   - Execute posts using contents of **orders.json**.
   - Copy post response, modify fields, then execute puts.
     - Make sure to copy `etag` value from last response, or you will get a concurrency error.
   - Copy `id` and `etag` values to execute deletes.
   - Execute gets to retrieve orders.
   - View orders database collections using Robo 3T.
6. Update the address of a customer who has order.
   - Note the address is also updated for the customer's orders.
   - Observe log messages in terminal when integration events are published and handled.

### Development Guide

> This section describes how to build the Customer and Order services from scratch using the **EventDriven.DDD.Abstractions** package. For your own project substitute `Customer` and `Order` for your own aggregate entites and related classes.

1. Add **Domain** and **CustomerAggregate** folders to the project, then add a `Customer` class that extends `Entity`.
   - Add properties representing entity state.
   - Create commands that are C# records and extend a `Command` base class.
    ```csharp
    public record CreateCustomer(Customer Customer) : Command.Create(Customer.Id);
    ```
  - Create domain events that extend `DomainEvent`.
    ```csharp
    public record CustomerCreated(Customer Customer) : DomainEvent(Customer.Id);
    ```
   - Where you need to execute business logic, implement `ICommandProcessor` and `IEventApplier` interfaces to process commands by emitting domain events and to apply those events to mutate entity state.
        ```csharp
        public class Customer : 
            Entity,
            ICommandProcessor<CreateCustomer, CustomerCreated>,
            IEventApplier<CustomerCreated>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Address ShippingAddress { get; set; }

            public CustomerCreated Process(CreateCustomer command)
                // To process command, return one or more domain events
                => new(command.Entity);

            public void Apply(CustomerCreated domainEvent) =>
                // Set Id
                Id = domainEvent.EntityId != default ? domainEvent.EntityId : Guid.NewGuid();
        }
        ```
2. Add a `CustomerCommandHandler` class that implements `ICommandHandler` for create, update and remove commands.
   - Inject `ICustomerRepository`, `IEventBus` and `IMapper` into the ctor.
   - In the handler for `CreateCustomer`, write code to process the command, apply events, and persist the entity.
    ```csharp
    public async Task<CommandResult<Customer>> Handle(CreateCustomer command)
    {
        // Process command
        var domainEvent = command.Entity.Process(command);
        
        // Apply events
        command.Entity.Apply(domainEvent);
        
        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
    ```
   - Create a **Common** class library project and add the package **EventDriven.DDD.Abstractions**.
     - Reference the Common project from the CustomerService project.
     - Create a `CustomerAddressUpdated` record that extends `IntegrationEvent`.
    ```csharp
    public record CustomerAddressUpdated(Guid CustomerId, Address ShippingAddress) : IntegrationEvent;
    ```
   - In the `UpdateCustomer` handler, see if the shipping address has changed, and if so, publish a `CustomerAddressUpdated` integration event, so that the order service can update the shipping address in the customer's orders.
    ```csharp
    public async Task<CommandResult<Customer>> Handle(UpdateCustomer command)
    {
        // Compare shipping addresses
        var existing = await _repository.GetAsync(command.EntityId);
        if (existing == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        var addressChanged = command.Entity.ShippingAddress != existing.ShippingAddress;
        
        try
        {
            // Persist entity
            var entity = await _repository.UpdateAsync(command.Entity);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
            
            // Publish events
            if (addressChanged)
            {
                var shippingAddress = _mapper.Map<Integration.Models.Address>(entity.ShippingAddress);
                await _eventBus.PublishAsync(
                    new CustomerAddressUpdated(entity.Id, shippingAddress),
                    null, "v1");
            }
            return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
        }
        catch (ConcurrencyException)
        {
            return new CommandResult<Customer>(CommandOutcome.Conflict);
        }
    }
    ```
3. Add a `CustomerCommandController` to the project that injects `CustomerCommandHandler` into the ctor.
   - Add Post, Put and Delete actions which accept a `Customer` DTO, map it to a `Customer` entity and invoke the appropriate command handler.
4. Add a `CustomerQueryController` to the project that injects a `ICustomerRepository` into the ctor.
   - Use the repository to retrieve entities, then map those to `Customer` DTO objects.
5. Register dependencies in `Startup.ConfigureServices`.
    ```csharp
    // Registrations
    services.AddAutoMapper(typeof(Startup));
    services.AddSingleton<CustomerCommandHandler>();
    services.AddSingleton<ICustomerRepository, CustomerRepository>();
    services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(Configuration);

    // Add Dapr event bus
    services.AddDaprEventBus(Configuration, true);

    // Add Dapr Mongo event cache
    services.AddDaprMongoEventCache(Configuration);
    ```
6. Add configuration entries to **appsettings.json**.
    ```json
    "CustomerDatabaseSettings": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "CustomersDb",
      "CollectionName": "Customers"
    },
    "DaprEventBusOptions": {
      "PubSubName": "pubsub"
    },
    "DaprEventCacheOptions": {
      "DaprStateStoreOptions": {
        "StateStoreName": "statestore-mongodb"
      }
    },
    "DaprStoreDatabaseSettings": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "daprStore",
      "CollectionName": "daprCollection"
    },
    "DaprEventBusSchemaOptions": {
      "UseSchemaRegistry": true,
      "SchemaValidatorType": "Json",
      "SchemaRegistryType": "Mongo",
      "AddSchemaOnPublish": true,
      "MongoStateStoreOptions": {
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "schema-registry",
        "SchemasCollectionName": "schemas"
      }
    }
    ```
7. Repeat these steps for the **Order** service.
   - Reference the Common project.
   - Add **Integration/EventHandlers** folders with a `CustomerAddressUpdatedEventHandler` class that extends `IntegrationEventHandler<CustomerAddressUpdated>`.
   - Override `HandleAsync` to update the order addresses for the customer.
    ```csharp
    public override async Task HandleAsync(CustomerAddressUpdated @event)
    {
        var orders = await _orderRepository.GetCustomerOrders(@event.CustomerId);
        foreach (var order in orders)
        {
            var shippingAddress = _mapper.Map<Address>(@event.ShippingAddress);
            await _orderRepository.UpdateOrderAddress(order.Id, shippingAddress);
        }
    }
    ```
   - In `Startup.ConfigureServices` register `CustomerAddressUpdatedEventHandler` then add the Dapr Event Bus.
    ```csharp
    services.AddSingleton<CustomerAddressUpdatedEventHandler>();
    services.AddDaprEventBus(Configuration, true);
    services.AddDaprMongoEventCache(Configuration);
    ```
   - In `Startup.Configure` use Cloud Events, map subscribe handlers, and map Dapr Event Bus endpoints, subscribing with the event handler.
    ```csharp
    // Use cloud events
    app.UseCloudEvents();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        
        // Map subscribe handlers
        endpoints.MapSubscribeHandler();
        endpoints.MapDaprEventBus(eventBus =>
        {
            // Subscribe with event handler
            eventBus.Subscribe(customerAddressUpdatedEventHandler, null, "v1");
        });
    });
    ```
