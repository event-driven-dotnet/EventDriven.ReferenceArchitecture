namespace EventDriven.CQRS.Abstractions.Events
{
    /// <summary>
    ///     Applies an event by muting entity state.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    public interface IEventApplier<in TEvent> where TEvent : class, IDomainEvent
    {
        /// <summary>
        ///     Applies a change to the given entity from the specified domain event.
        /// </summary>
        /// <param name="domainEvent">Domain event to apply.</param>
        void Apply(TEvent domainEvent);
    }
}