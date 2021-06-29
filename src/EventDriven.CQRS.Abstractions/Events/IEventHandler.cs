using System.Threading.Tasks;

namespace EventDriven.CQRS.Abstractions.Events
{
    /// <summary>
    ///     Event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : class, IDomainEvent
    {
        /// <summary>
        ///     Handles an event.
        /// </summary>
        /// <param name="domainEvent">The event.</param>
        /// <returns>True if handled successfully.</returns>
        Task<bool> Handle(TEvent domainEvent);
    }
}