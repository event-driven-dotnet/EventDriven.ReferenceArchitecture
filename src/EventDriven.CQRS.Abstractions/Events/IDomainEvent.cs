using System;

namespace EventDriven.CQRS.Abstractions.Events
{
    /// <inheritdoc />
    public interface IDomainEvent : IEvent
    {
        /// <summary>
        ///     The id of the entity that this event is "about".
        /// </summary>
        /// <value></value>
        Guid EntityId { get; }

        /// <summary>
        ///     Indicates this is the nth event related to a specific EntityId.
        /// </summary>
        long EntitySequenceNumber { get; }
    }
}