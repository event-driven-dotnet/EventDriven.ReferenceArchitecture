using System.Threading.Tasks;

namespace EventDriven.CQRS.Abstractions.Events
{
    /// <inheritdoc />
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : class, IDomainEvent
    {
        /// <inheritdoc />
        public abstract Task<bool> Handle(TEvent domainEvent);
    }
}