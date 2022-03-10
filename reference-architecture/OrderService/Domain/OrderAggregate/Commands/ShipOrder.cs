using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record ShipOrder(Order Entity) : Command<Order>(Entity);