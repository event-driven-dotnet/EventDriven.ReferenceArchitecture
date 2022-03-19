using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CustomerCreated(Customer? Entity) : DomainEvent<Customer>(Entity);