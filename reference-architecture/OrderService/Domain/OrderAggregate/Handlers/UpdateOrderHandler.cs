using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Handlers;

public class UpdateOrderHandler : ICommandHandler<Order, UpdateOrder>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<UpdateOrderHandler> _logger;

    public UpdateOrderHandler(
        IOrderRepository repository,
        ILogger<UpdateOrderHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CommandResult<Order>> Handle(UpdateOrder command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateOrder));

        try
        {
            // Persist entity
            var entity = await _repository.UpdateAsync(command.Entity);
            if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
            return new CommandResult<Order>(CommandOutcome.Accepted, entity);
        }
        catch (ConcurrencyException)
        {
            return new CommandResult<Order>(CommandOutcome.Conflict);
        }
    }
}