using EventDriven.DDD.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Handlers;

public class RemoveOrderHandler : ICommandHandler<RemoveOrder>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<RemoveOrderHandler> _logger;

    public RemoveOrderHandler(
        IOrderRepository repository,
        ILogger<RemoveOrderHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(RemoveOrder command, CancellationToken cancellationToken)
    {
        // Persist entity
        _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult<Order>(CommandOutcome.Accepted);
    }
}