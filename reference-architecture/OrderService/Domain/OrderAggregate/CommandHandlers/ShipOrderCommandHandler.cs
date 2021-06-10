namespace OrderService.Domain.OrderAggregate.CommandHandlers {

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Events;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class ShipOrderCommandHandler : ICommandHandler<ShipOrder, CommandResult<Order>> {

        private readonly ILogger<ShipOrderCommandHandler> logger;

        private readonly IOrderRepository repository;

        public ShipOrderCommandHandler(IOrderRepository repository,
                                       ILogger<ShipOrderCommandHandler> logger) {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(ShipOrder request, CancellationToken cancellationToken) {
            // Process command
            logger.LogInformation("Handling command: {CommandName}", nameof(ShipOrder));
            var entity = await repository.GetOrder(request.EntityId);

            if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);

            var events = entity.Process(request);

            // Apply events
            var domainEvent = events.OfType<OrderShipped>().SingleOrDefault();

            if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);

            entity.Apply(domainEvent);

            try {
                // Persist entity
                var order = await repository.UpdateOrderState(entity, OrderState.Shipped);

                if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);

                return new CommandResult<Order>(CommandOutcome.Accepted, order);
            } catch (ConcurrencyException) {
                return new CommandResult<Order>(CommandOutcome.Conflict);
            }
        }

    }

}