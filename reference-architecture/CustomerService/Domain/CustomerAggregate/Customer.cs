using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate;

public class Customer : 
    Entity,
    ICommandProcessor<CreateCustomer, Customer, CustomerCreated>,
    IEventApplier<CustomerCreated>,
    ICommandProcessor<UpdateCustomer, Customer, CustomerUpdated>,
    IEventApplier<CustomerUpdated>,
    ICommandProcessor<RemoveCustomer, CustomerRemoved>,
    IEventApplier<CustomerRemoved>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Address ShippingAddress { get; set; } = null!;

    public CustomerCreated Process(CreateCustomer command)
        // To process command, return one or more domain events
        => new(command.Entity);

    public void Apply(CustomerCreated domainEvent) =>
        // Set Id
        Id = domainEvent.EntityId != default ? domainEvent.EntityId : Guid.NewGuid();
        
    public CustomerUpdated Process(UpdateCustomer command) =>
        // To process command, return a domain event
        new(command.Entity);

    public void Apply(CustomerUpdated domainEvent)
    {
        // Set ETag
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public CustomerRemoved Process(RemoveCustomer command) =>
        // To process command, return a domain event
        new(command.EntityId);

    public void Apply(CustomerRemoved domainEvent)
    {
        // Could mutate state here to implement a soft delete
    }
}