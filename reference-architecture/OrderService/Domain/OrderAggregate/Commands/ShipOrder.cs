using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record ShipOrder(Guid EntityId) : Command<Order>(null, EntityId);