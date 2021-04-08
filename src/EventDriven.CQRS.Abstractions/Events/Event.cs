using System;

namespace EventDriven.CQRS.Abstractions.Events
{
    public abstract record Event(string Source = null) : IEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string Source { get; set; } = Source;
    }
}
