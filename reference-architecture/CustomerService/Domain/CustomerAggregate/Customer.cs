using System;
using System.Collections.Generic;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate
{
    public class Customer : 
        Entity,
        ICommandProcessor<CreateCustomer>,
        IEventApplier<CustomerCreated>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address ShippingAddress { get; set; }

        public IEnumerable<IDomainEvent> Process(CreateCustomer command)
            // To process command, return one or more domain events
            => new List<IDomainEvent>
            {
                new CustomerCreated(command.Customer)
            };

        public void Apply(CustomerCreated domainEvent) =>
            // Set Id
            Id = domainEvent.EntityId != default(Guid) ? domainEvent.EntityId : Guid.NewGuid();
    }
}