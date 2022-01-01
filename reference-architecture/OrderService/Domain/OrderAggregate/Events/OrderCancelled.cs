using System;
using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events
{
    public record OrderCancelled(Guid EntityId, string ETag) : DomainEvent(EntityId, ETag);
}
