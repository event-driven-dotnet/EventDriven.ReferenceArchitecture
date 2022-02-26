using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record ShipOrder(Guid EntityId, string ETag) : Command(EntityId);
}