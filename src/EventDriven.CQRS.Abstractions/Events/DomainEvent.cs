using System;

namespace EventDriven.CQRS.Abstractions.Events
{
    public abstract record DomainEvent(
            Guid EntityId,
            string EntityETag = null,
            long EntitySequenceNumber = 0)
        : Event, IDomainEvent;
}
