namespace OrderService.Domain.OrderAggregate.Events {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Events;

    [ExcludeFromCodeCoverage]
    public record OrderShipped(Guid EntityId, string ETag) : DomainEvent(EntityId, ETag);

}