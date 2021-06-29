using System.Threading;
using System.Threading.Tasks;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers
{
    public class RemoveOrderCommandHandler : ICommandHandler<RemoveOrder, CommandResult>
    {
        private readonly ILogger<RemoveOrderCommandHandler> _logger;

        private readonly IOrderRepository _repository;

        public RemoveOrderCommandHandler(IOrderRepository repository,
                                         ILogger<RemoveOrderCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CommandResult> Handle(RemoveOrder request, CancellationToken cancellationToken)
        {
            // Persist entity
            _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
            await _repository.RemoveOrder(request.OrderId);

            return new CommandResult(CommandOutcome.Accepted);
        }
    }
}