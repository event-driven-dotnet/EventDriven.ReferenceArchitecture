using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CustomerRemoved(Guid EntityId) : DomainEvent(EntityId);