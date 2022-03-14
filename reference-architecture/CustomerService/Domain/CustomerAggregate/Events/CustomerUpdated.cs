using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CustomerUpdated(Customer? Entity) : DomainEvent<Customer>(Entity);