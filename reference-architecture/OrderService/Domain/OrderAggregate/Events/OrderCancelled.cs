using System;
using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events
{
    [ExcludeFromCodeCoverage]
    public record OrderCancelled(Guid EntityId, string ETag) : DomainEvent(EntityId, ETag);
}