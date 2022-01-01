using System;
using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record CancelOrder(Guid EntityId, string ETag) : Command(EntityId);
}