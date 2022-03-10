using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events;

public record OrderShipped(Order Entity) : DomainEvent<Order>(Entity);