using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record CancelOrder(Order Entity) : Command<Order>(Entity);