namespace OrderService.Domain.OrderAggregate.CommandHandlers {

    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class RemoveOrderCommandHandler : ICommandHandler<RemoveOrder, CommandResult<Order>> {

        private readonly ILogger<RemoveOrderCommandHandler> logger;

        private readonly IOrderRepository repository;

        public RemoveOrderCommandHandler(IOrderRepository repository,
                                         ILogger<RemoveOrderCommandHandler> logger) {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(RemoveOrder request, CancellationToken cancellationToken) {
            // Persist entity
            logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
            await repository.RemoveOrder(request.OrderId);

            return new CommandResult<Order>(CommandOutcome.Accepted);
        }

    }

}