using System.Threading;
using System.Threading.Tasks;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers
{
    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrder, CommandResult<Order>>
    {
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        private readonly IOrderRepository _repository;

        public UpdateOrderCommandHandler(IOrderRepository repository,
                                         ILogger<UpdateOrderCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(UpdateOrder request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateOrder));

            try
            {
                // Persist entity
                var entity = await _repository.UpdateOrder(request.Order);

                if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);

                return new CommandResult<Order>(CommandOutcome.Accepted, entity);
            } catch (ConcurrencyException) { return new CommandResult<Order>(CommandOutcome.Conflict); }
        }
    }
}