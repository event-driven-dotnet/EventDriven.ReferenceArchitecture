# Event Driven .NET Development Guide

A development guide for building loosely coupled, event-driven microservices using [Event Driven .NET](https://github.com/event-driven-dotnet/Home) abstractions and reference architecture.

## Steps

The following steps illustrate how to create microservices based on the principles of [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) (DDD) and [Command Query Responsibility Segregation](https://en.wikipedia.org/wiki/Command%E2%80%93query_separation) (CQRS) with services that communicate asynchronously over an Event Bus abstraction that uses [Dapr](https://dapr.io/) for [publish-subscribe](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/) with an underlying message broker.

1. Create a **CustomerService** Web API project.
   - Add the following packages.
     - **AutoMapper.Extensions.Microsoft.DependencyInjection**
     - **EventDriven.CQRS.Abstractions**
     - **EventDriven.CQRS.Extensions**
     - **EventDriven.DependencyInjection.URF.Mongo**
     - **EventDriven.EventBus.Dapr**
     - **EventDriven.EventBus.Dapr.EventCache.Mongo**
     - **MongoDB.Driver**
     - **URF.Core.Mongo**
2. Add **Domain/CustomerAggregate** folders to the project, then add a `Customer` class that extends `Entity`.
   - Add properties representing entity state.
    ```csharp
    public class Customer : Entity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Address ShippingAddress { get; set; } = null!;
    }
    ```
   - Create commands that are C# records and extend a `Command` base class.
    ```csharp
    public record CreateCustomer(Customer? Entity) : Command<Customer>(Entity);
    ```
    ```csharp
    public record UpdateCustomer(Customer? Entity) : Command<Customer>(Entity);
    ```
    ```csharp
    public record RemoveCustomer(Guid EntityId) : Command(EntityId);
    ```
   - Create an **Events** folder in **Domain/CustomerAggregate** and add events that extend `DomainEvent`.
    ```csharp
    public record CustomerCreated(Customer? Entity) : DomainEvent<Customer>(Entity);
    ```
    ```csharp
    public record CustomerUpdated(Customer? Entity) : DomainEvent<Customer>(Entity);
    ```
    ```csharp
    public record CustomerRemoved(Guid EntityId) : DomainEvent(EntityId);
    ```
   - Update the `Customer` entity to implement `ICommandProcessor` and `IEventApplier` interfaces to process commands by emitting domain events and to apply those events to mutate entity state.
     - Implementing entity behavior by means of `Process` and `Apply` methods allows for easier migration to event sourcing in the future.
     - Implement `ICommandProcessor<CreateCustomer, Customer, CustomerCreated>` to add a `Process` method that accepts a `CreateCustomer` command and returns a `CustomerCreated` event.
     - Implement `IEventApplier<CustomerCreated>` to mutate entity state based on a `CustomerCreated` event.
    ```csharp
    public class Customer : 
        Entity,
        ICommandProcessor<CreateCustomer, Customer, CustomerCreated>,
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
3. Add a `CreateCustomerHandler` class to a **CommandHandlers** folder in **Domain/CustomerAggregate**, and implement `ICommandHandler<Customer, CreateCustomer>`.
   - Inject `ICustomerRepository` into the constructor.
   - In the `Handle` method write code to process the command, apply events, and persist the entity.
    ```csharp
    public async Task<CommandResult<Customer>> Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);

        // Apply events
        command.Entity.Apply(domainEvent);

        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
    ```
4. Add a **Common** class library project to the solution.
   - Add the following packages:
     - **EventDriven.CQRS.Abstractions**
     - **EventDriven.EventBus.Abstractions**
     - **Microsoft.Extensions.Logging.Abstractions**
   - Reference the Common project from the CustomerService project.
   - Create a `LoggingBehavior<TRequest, TResponse>` class that implements `IBehavior<TRequest, TResponse>`.
   - In the `Handle` method perform pre and post handler logging.
    ```csharp
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        string requestType = string.Empty;
        if (typeof(TRequest).IsCommandType())
            requestType = "command";
        else if (typeof(TRequest).IsQueryType())
            requestType = "query";
        _logger.LogInformation("----- Handling {RequestType} '{CommandName}'. Request: {@Request}", 
            requestType, request.GetGenericTypeName(), request);
        var response = await next();
        _logger.LogInformation("----- Handled {RequestType} '{CommandName}'. Response: {@Response}", 
            requestType, request.GetGenericTypeName(), response);
        return response;
    }
    ```
   - Create a `CustomerAddressUpdated` record that extends `IntegrationEvent`.
    ```csharp
    public record CustomerAddressUpdated(Guid CustomerId, Address ShippingAddress) : IntegrationEvent;
    ```
5. Create a **Repositories** folder in the **CustomerService** project to contain repositories.
   - Add an `ICustomerRepository` interface.
    ```csharp
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAsync();
        Task<Customer?> GetAsync(Guid id);
        Task<Customer?> AddAsync(Customer entity);
        Task<Customer?> UpdateAsync(Customer entity);
        Task<int> RemoveAsync(Guid id);
    }
    ```
   - Add a `CustomerRepository` class that implements `ICustomerRepository` and extends `DocumentRepository<Customer>`.
    ```csharp
    public class CustomerRepository : DocumentRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoCollection<Customer> collection) : base(collection)
        {
        }

        public async Task<IEnumerable<Customer>> GetAsync() =>
            await FindManyAsync();

        public async Task<Customer?> GetAsync(Guid id) =>
            await FindOneAsync(e => e.Id == id);

        public async Task<Customer?> AddAsync(Customer entity)
        {
            var existing = await FindOneAsync(e => e.Id == entity.Id);
            if (existing != null) return null;
            if (string.IsNullOrWhiteSpace(entity.ETag))
                entity.ETag = Guid.NewGuid().ToString();
            return await InsertOneAsync(entity);
        }

        public async Task<Customer?> UpdateAsync(Customer entity)
        {
            var existing = await GetAsync(entity.Id);
            if (existing == null) return null;
            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
                throw new ConcurrencyException();
            entity.ETag = Guid.NewGuid().ToString();
            return await FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
        }

        public async Task<int> RemoveAsync(Guid id) =>
            await DeleteOneAsync(e => e.Id == id);
    }
    ```
6. Add an `UpdateCustomerHandler` class to the **CommandHandlers** folder.
   - Inject `ICustomerRepository`, `IEventBus` and `IMapper` into the constructor.
   - In the `Handle` method, see if the shipping address has changed, and if so, publish a `CustomerAddressUpdated` integration event, so that the order service can update the shipping address in the customer's orders.
    ```csharp
    public async Task<CommandResult<Customer>> Handle(UpdateCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);

        // Apply events
        command.Entity.Apply(domainEvent);

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
                _logger.LogInformation("----- Publishing event: {EventName}", $"v1.{nameof(CustomerAddressUpdated)}");
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
7. Create queries and query handlers to retrieve entities from the customer repository.
   - Add `GetCustomer` and `GetCustomers` records to a **Queries** folder.
    ```csharp
    public record GetCustomer(Guid Id) : Query<Customer?>;
    ```
    ```csharp
    public record GetCustomers : Query<IEnumerable<Customer>>;
    ```
   - Add `GetCustomerHandler` to a **QueryHandlers** folder.
    ```csharp
    public class GetCustomerHandler : IQueryHandler<GetCustomer, Customer?>
    {
        private readonly ICustomerRepository _repository;

        public GetCustomerHandler(
            ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer?> Handle(GetCustomer query, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync(query.Id);
            return result;
        }
    }
    ```
   - Add `GetCustomersHandler` to the **QueryHandlers** folder.
    ```csharp
    public class GetCustomersHandler : IQueryHandler<GetCustomers, IEnumerable<Customer>>
    {
        private readonly ICustomerRepository _repository;

        public GetCustomersHandler(
            ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> Handle(GetCustomers query, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync();
            return result;
        }
    }
    ```
8. Add read and write models to a **DTO** folder. Note that read and write models may differ from one another.
   - Add an `AutoMapperProfile` class that extends `Profile` and maps DTO's to entities.
9. Add a `CustomerCommandController` to the project that injects `ICommandBroker` and `IMapper` into the ctor.
   - Add Post, Put and Delete actions which accept a `Customer` DTO, map it to a `Customer` entity and call `SendAsync` on the command broker, passing the appropriate command.
   - Map input and output entities to corresponding DTO's.
    ```csharp
    // POST api/customer
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DTO.Write.Customer customerDto)
    {
        var customerIn = _mapper.Map<Customer>(customerDto);
        var result = await _commandBroker.SendAsync(new CreateCustomer(customerIn));

        if (result.Outcome != CommandOutcome.Accepted)
            return result.ToActionResult();
        var customerOut = _mapper.Map<DTO.Write.Customer>(result.Entity);
        return new CreatedResult($"api/customer/{customerOut.Id}", customerOut);
    }

    // PUT api/customer
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] DTO.Write.Customer customerDto)
    {
        var customerIn = _mapper.Map<Customer>(customerDto);
        var result = await _commandBroker.SendAsync(new UpdateCustomer(customerIn));

        if (result.Outcome != CommandOutcome.Accepted)
            return result.ToActionResult();
        var customerOut = _mapper.Map<DTO.Write.Customer>(result.Entity);
        return result.ToActionResult(customerOut);
    }

    // DELETE api/customer/id
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Remove([FromRoute] Guid id)
    {
        var result = await _commandBroker.SendAsync(new RemoveCustomer(id));
        return result.Outcome != CommandOutcome.Accepted
            ? result.ToActionResult() 
            : new NoContentResult();
    }
    ```
10. Add a `CustomerQueryController` to the project that injects `IQueryBroker` and `IMapper` into the constructor.
   - Use the repository to retrieve entities, then map those to `Customer` DTO objects.
      ```csharp
      // GET api/customer
      [HttpGet]
      public async Task<IActionResult> GetCustomers()
      {
          var customers = await _queryBroker.SendAsync(new GetCustomers());
          var result = _mapper.Map<IEnumerable<CustomerView>>(customers);
          return Ok(result);
      }

      // GET api/customer/id
      [HttpGet]
      [Route("{id:guid}")]
      public async Task<IActionResult> GetCustomer([FromRoute] Guid id)
      {
          var customer = await _queryBroker.SendAsync(new GetCustomer(id));
          if (customer == null) return NotFound();
          var result = _mapper.Map<CustomerView>(customer);
          return Ok(result);
      }
      ```
11. Register dependencies for **CustomerService** in `Program`.
    ```csharp
    // Add automapper
    builder.Services.AddAutoMapper(typeof(Program));

    // Add command and query handlers
    builder.Services.AddHandlers(typeof(Program));

    // Add behaviors
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    // Add database settings
    builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
    builder.Services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(builder.Configuration);

    // Add Dapr event bus
    builder.Services.AddDaprEventBus(builder.Configuration, true);
    builder.Services.AddDaprMongoEventCache(builder.Configuration);
    ```
12. Add configuration entries to **appsettings.json**.
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
13. Repeat these steps for the **Order** service.
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
   - Register dependencies for **CustomerService** in `Program`.
   - In `Program` register `CustomerAddressUpdatedEventHandler` and add the Dapr Event Bus.
      ```csharp
      builder.Services.AddSingleton<CustomerAddressUpdatedEventHandler>();
      builder.Services.AddDaprEventBus(builder.Configuration, true);
      builder.Services.AddDaprMongoEventCache(builder.Configuration);
      ```
   - Also in `Program` use Cloud Events, map subscribe handlers, and map Dapr Event Bus endpoints.
      ```csharp
      app.UseCloudEvents();
      app.UseEndpoints(endpoints =>
      {
          endpoints.MapControllers();
          endpoints.MapSubscribeHandler();
          endpoints.MapDaprEventBus(eventBus =>
          {
              var customerAddressUpdatedEventHandler = app.Services.GetRequiredService<CustomerAddressUpdatedEventHandler>();
              eventBus.Subscribe(customerAddressUpdatedEventHandler, null, "v1");
          });
      });
      ```
14. Add configuration entries for `DaprEventCacheOptions` and `DaprStoreDatabaseSettings` to **appsettings.json**.
    ```json
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
    ```
15. Lastly, add a **dapr/components** directory to the **reference-architecture** folder.
    - Add the following dapr component yaml files:
      - **pubsub.yaml**
      - **statestore.yaml**
      - **statestore-mongodb.yaml**
    - Files not in use should be placed in a separate folder.
    - Add a **tye.yaml** file to the **reference-architecture** folder (recommended), including the dapr extension and components path. Optionally specify port bindings which match those in **launchSettings.json**.
    ```yaml
    name: event-driven-ref-arch
    extensions:
      - name: dapr
        log-level: debug
        components-path: "dapr/components"
    services:
      - name: customer-service
        project: CustomerService/CustomerService.csproj
        bindings:
          - port: 5656
      - name: order-service
        project: OrderService/OrderService.csproj
        bindings:
          - port: 5757
    ```
    - Run the Customer and Order services using [Tye](https://github.com/dotnet/tye).
      - Be sure to install the [Tye CLI](https://github.com/dotnet/tye/blob/main/docs/getting_started.md) tool globally.
      - To debug services, set breakpoints and execute `tye run --debug *`. Then attach the debugger to **CustomerService.dll** and/or **OrderService.dll**.

