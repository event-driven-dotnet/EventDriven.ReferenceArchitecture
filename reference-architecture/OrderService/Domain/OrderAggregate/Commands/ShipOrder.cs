using System;
using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record ShipOrder(Guid EntityId, string ETag) : Command.Update(EntityId, ETag);
}