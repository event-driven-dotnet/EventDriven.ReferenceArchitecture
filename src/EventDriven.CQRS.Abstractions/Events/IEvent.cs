using System;

namespace EventDriven.CQRS.Abstractions.Events
{
    public interface IEvent
    {
        /// <summary>
        /// Unique ID of the Event
        /// </summary>
        /// <value></value>
        Guid Id { get; }

        /// <summary>
        /// Time at which the event was created
        /// </summary>
        /// <value></value>
        DateTime CreatedAt { get; }

        /// <summary>
        /// The name of the service that produced it and acts as a namespace
        /// </summary>
        /// <value></value>
        string Source { get; set; }
    }
}
