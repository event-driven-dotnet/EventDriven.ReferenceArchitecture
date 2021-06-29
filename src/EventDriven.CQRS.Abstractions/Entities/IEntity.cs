using System;

namespace EventDriven.CQRS.Abstractions.Entities
{
    /// <summary>
    ///     A type that has an identity with behavior and state that can change over time.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        ///     The ID of the Entity.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        ///     The sequence number (equal to the highest event sequence number applied).
        /// </summary>
        long SequenceNumber { get; set; }

        /// <summary>
        ///     Represents a unique ID that must change atomically with each store of the entity
        ///     to its underlying storage medium.
        /// </summary>
        string ETag { get; set; }

        /// <summary>
        ///     Gets the entity state.
        /// </summary>
        EntityState State { get; set; }
    }
}