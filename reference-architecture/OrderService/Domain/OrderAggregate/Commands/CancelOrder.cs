using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record CancelOrder(Guid EntityId) : Command<Order>(null, EntityId);