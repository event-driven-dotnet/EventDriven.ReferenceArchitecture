using System;
using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record RemoveOrder(Guid EntityId) : Command(EntityId);
}