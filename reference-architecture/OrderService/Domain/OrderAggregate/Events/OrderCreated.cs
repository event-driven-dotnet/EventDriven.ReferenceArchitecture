using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderCreated(Order? Entity) : DomainEvent<Order>(Entity);