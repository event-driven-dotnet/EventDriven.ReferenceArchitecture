namespace OrderService.Domain.OrderAggregate.CommandHandlers {

    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrder, CommandResult<Order>> {

        private readonly ILogger<UpdateOrderCommandHandler> logger;

        private readonly IOrderRepository repository;

        public UpdateOrderCommandHandler(IOrderRepository repository,
                                         ILogger<UpdateOrderCommandHandler> logger) {
            this.repository = repository;
            this.logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(UpdateOrder request, CancellationToken cancellationToken) {
            logger.LogInformation("Handling command: {CommandName}", nameof(UpdateOrder));

            try {
                // Persist entity
                var entity = await repository.UpdateOrder(request.Order);

                if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);

                return new CommandResult<Order>(CommandOutcome.Accepted, entity);
            } catch (ConcurrencyException) {
                return new CommandResult<Order>(CommandOutcome.Conflict);
            }
        }

    }

}