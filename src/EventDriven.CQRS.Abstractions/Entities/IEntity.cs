using System;

namespace EventDriven.CQRS.Abstractions.Entities
{
    public interface IEntity
    {
        /// <summary>
        /// The ID of the Entity.
        /// </summary>
        /// <value></value>
        Guid Id { get; set; }

        /// <summary>
        /// The sequence number (equal to the highest event sequence number applied).
        /// </summary>
        /// <value></value>
        long SequenceNumber { get; set; }

        /// <summary>
        /// Represents a unique ID that must change atomically with each store of the entity
        /// to its underlying storage medium.
        /// </summary>
        /// <value></value>
        string ETag { get; set; }

        /// <summary>
        /// Gets the entity state.
        /// </summary>
        EntityState State { get; set; }
    }
}
