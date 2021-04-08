using System;
using EventDriven.CQRS.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events
{
    public record OrderShipped(Guid EntityId, string ETag) : DomainEvent(EntityId, ETag);
}
