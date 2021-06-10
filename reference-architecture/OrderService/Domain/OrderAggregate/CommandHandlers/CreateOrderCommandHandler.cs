namespace OrderService.Domain.OrderAggregate.CommandHandlers {

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Events;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class CreateOrderCommandHandler : ICommandHandler<CreateOrder, CommandResult<Order>> {

        private readonly ILogger<CreateOrderCommandHandler> logger;

        private readonly IOrderRepository repository;

        public CreateOrderCommandHandler(IOrderRepository repository,
                                         ILogger<CreateOrderCommandHandler> logger) {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(CreateOrder request, CancellationToken cancellationToken) {
            // Process command
            logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
            var events = request.Order.Process(request);

            // Apply events
            var domainEvent = events.OfType<OrderCreated>().SingleOrDefault();

            if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);

            request.Order.Apply(domainEvent);

            // Persist entity
            var entity = await repository.AddOrder(request.Order);

            if (entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);

            return new CommandResult<Order>(CommandOutcome.Accepted, entity);
        }

    }

}