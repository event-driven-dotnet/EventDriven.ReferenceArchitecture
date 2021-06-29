using System;

namespace EventDriven.CQRS.Abstractions.Events
{
    /// <summary>
    ///     A statement of fact about what change has been made to the domain state.
    /// </summary>
    /// <param name="EntityId">The id of the entity that this event is "about".</param>
    /// <param name="EntityETag">A unique ID that must change atomically with each store of the entity.</param>
    /// <param name="EntitySequenceNumber">Indicates this is the nth event related to a specific EntityId.</param>
    public abstract record DomainEvent(Guid EntityId,
                                       string EntityETag = null,
                                       long EntitySequenceNumber = 0)
        : Event, IDomainEvent;
}