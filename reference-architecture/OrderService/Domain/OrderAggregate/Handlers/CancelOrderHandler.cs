using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Handlers;

public class CancelOrderHandler : ICommandHandler<Order, CancelOrder>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<CancelOrderHandler> _logger;

    public CancelOrderHandler(
        IOrderRepository repository,
        ILogger<CancelOrderHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CommandResult<Order>> Handle(CancelOrder command, CancellationToken cancellationToken)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(CancelOrder));
        var entity = await _repository.GetAsync(command.EntityId);
        if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
        var domainEvent = entity.Process(command);
            
        // Apply events
        entity.Apply(domainEvent);
            
        try
        {
            // Persist entity
            var order = await _repository.UpdateOrderStateAsync(entity, OrderState.Cancelled);
            if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
            return new CommandResult<Order>(CommandOutcome.Accepted, order);
        }
        catch (ConcurrencyException)
        {
            return new CommandResult<Order>(CommandOutcome.Conflict);
        }
    }
}