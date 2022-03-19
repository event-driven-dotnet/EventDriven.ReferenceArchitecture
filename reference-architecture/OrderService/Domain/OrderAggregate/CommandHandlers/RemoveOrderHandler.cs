using EventDriven.CQRS.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

public class RemoveOrderHandler : ICommandHandler<RemoveOrder>
{
    private readonly IOrderRepository _repository;

    public RemoveOrderHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(RemoveOrder command, CancellationToken cancellationToken)
    {
        // Process command
        var entity = await _repository.GetAsync(command.EntityId);
        if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
        var domainEvent = entity.Process(command);
            
        // Apply events
        entity.Apply(domainEvent);

        // Persist entity
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult<Order>(CommandOutcome.Accepted);
    }
}