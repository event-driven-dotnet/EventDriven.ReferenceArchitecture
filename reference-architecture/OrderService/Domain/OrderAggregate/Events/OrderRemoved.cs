using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderRemoved(Guid EntityId) : DomainEvent(EntityId);