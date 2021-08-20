using System;
using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record CancelOrder(Guid EntityId, string ETag) : Command.Update(EntityId, ETag);
}