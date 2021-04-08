using System.Threading.Tasks;

namespace EventDriven.CQRS.Abstractions.Events
{
    /// <summary>
    /// Handles a domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event</typeparam>
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : class, IDomainEvent
    {
        public abstract Task<bool> Handle(TEvent domainEvent);
    }
}
