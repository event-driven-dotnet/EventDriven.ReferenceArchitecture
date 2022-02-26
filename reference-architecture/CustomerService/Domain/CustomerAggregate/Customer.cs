using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate
{
    public class Customer : 
        Entity,
        ICommandProcessor<CreateCustomer, CustomerCreated>,
        IEventApplier<CustomerCreated>
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
    }
}