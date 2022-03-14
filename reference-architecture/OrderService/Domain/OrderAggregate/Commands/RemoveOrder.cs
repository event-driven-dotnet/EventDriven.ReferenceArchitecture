using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record RemoveOrder(Guid EntityId) : Command(EntityId);