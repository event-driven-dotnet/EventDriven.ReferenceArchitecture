using EventDriven.CQRS.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

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
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
        var entity = await _repository.GetAsync(command.EntityId);
        if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
        var domainEvent = entity.Process(command);
            
        // Apply events
        entity.Apply(domainEvent);

        // Persist entity
        _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult<Order>(CommandOutcome.Accepted);
    }
}