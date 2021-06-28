using System;

namespace EventDriven.CQRS.Abstractions.Events
{
    /// <summary>
    ///     A statement of fact about what change has been made to the domain state.
    /// </summary>
    /// <param name="Source">The source of the event.</param>
    public abstract record Event(string Source = null) : IEvent
    {
        /// <inheritdoc />
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <inheritdoc />
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        /// <inheritdoc />
        public string Source { get; set; } = Source;
    }
}