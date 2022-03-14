using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderUpdated(Order? Entity) : DomainEvent<Order>(Entity);