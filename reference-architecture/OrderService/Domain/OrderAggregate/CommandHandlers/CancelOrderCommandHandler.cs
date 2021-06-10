namespace OrderService.Domain.OrderAggregate.CommandHandlers {

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Events;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class CancelOrderCommandHandler : ICommandHandler<CancelOrder, CommandResult<Order>> {

        private readonly ILogger<CancelOrderCommandHandler> logger;

        private readonly IOrderRepository repository;

        public CancelOrderCommandHandler(IOrderRepository repository,
                                         ILogger<CancelOrderCommandHandler> logger) {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(CancelOrder request, CancellationToken cancellationToken) {
            // Process command
            logger.LogInformation("Handling command: {CommandName}", nameof(CancelOrder));
            var entity = await repository.GetOrder(request.EntityId);

            if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);

            var events = entity.Process(request);

            // Apply events
            var domainEvent = events.OfType<OrderCancelled>().SingleOrDefault();

            if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);

            entity.Apply(domainEvent);

            try {
                // Persist entity
                var order = await repository.UpdateOrderState(entity, OrderState.Cancelled);

                if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);

                return new CommandResult<Order>(CommandOutcome.Accepted, order);
            } catch (ConcurrencyException) {
                return new CommandResult<Order>(CommandOutcome.Conflict);
            }
        }

    }

}