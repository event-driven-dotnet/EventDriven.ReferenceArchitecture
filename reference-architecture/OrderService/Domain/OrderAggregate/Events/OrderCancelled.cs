using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderCancelled(Guid EntityId) : DomainEvent<Order>(null, EntityId);