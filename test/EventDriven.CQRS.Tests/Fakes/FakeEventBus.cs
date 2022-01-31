using System.Threading.Tasks;
using EventDriven.EventBus.Abstractions;

namespace EventDriven.CQRS.Tests.Fakes
{
    public class FakeEventBus : EventBus.Abstractions.EventBus
    {
        public FakeEventBus(EventBusOptions eventBusOptions) : base(eventBusOptions)
        {
        }

        public override Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic = null,
            string prefix = null)
        {
            var topicName = topic ?? @event.GetType().Name;
            if (!Topics.TryGetValue(topicName, out var handlers))
                return Task.CompletedTask;
            foreach (var handler in handlers)
                handler.HandleAsync(@event);
            return Task.CompletedTask;
        }
    }
}