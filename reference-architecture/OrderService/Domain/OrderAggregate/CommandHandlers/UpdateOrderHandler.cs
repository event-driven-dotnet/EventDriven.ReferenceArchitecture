using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

public class UpdateOrderHandler : ICommandHandler<Order, UpdateOrder>
{
    private readonly IOrderRepository _repository;

    public UpdateOrderHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult<Order>> Handle(UpdateOrder command, CancellationToken cancellationToken)
    {
        if (command.Entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);

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