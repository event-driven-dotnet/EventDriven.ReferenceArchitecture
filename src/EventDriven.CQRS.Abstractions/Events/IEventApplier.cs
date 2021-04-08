namespace EventDriven.CQRS.Abstractions.Events
{
    public interface IEventApplier<in TEvent> where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Applies a change to the given Entity from the specified domain event.
        /// </summary>
        /// <param name="domainEvent">Domain event to apply.</param>
        void Apply(TEvent domainEvent);
    }
}
