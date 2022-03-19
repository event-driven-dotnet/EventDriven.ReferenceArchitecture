using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

public class ShipOrderHandler : ICommandHandler<Order, ShipOrder>
{
    private readonly IOrderRepository _repository;

    public ShipOrderHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult<Order>> Handle(ShipOrder command, CancellationToken cancellationToken)
    {
        // Process command
        var entity = await _repository.GetAsync(command.EntityId);
        if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
        var domainEvent = entity.Process(command);
            
        // Apply events
        entity.Apply(domainEvent);
            
        try
        {
            // Persist entity
            var order = await _repository.UpdateOrderStateAsync(entity, OrderState.Shipped);
            if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
            return new CommandResult<Order>(CommandOutcome.Accepted, order);
        }
        catch (ConcurrencyException)
        {
            return new CommandResult<Order>(CommandOutcome.Conflict);
        }
    }
}