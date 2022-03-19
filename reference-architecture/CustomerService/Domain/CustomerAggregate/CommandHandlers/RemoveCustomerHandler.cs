using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class RemoveCustomerHandler : ICommandHandler<RemoveCustomer>
{
    private readonly ICustomerRepository _repository;

    public RemoveCustomerHandler(
        ICustomerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CommandResult> Handle(RemoveCustomer command, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(command.EntityId);
        if (entity != null)
        {
            // Process command, apply events
            var domainEvent = entity.Process(command);
            entity.Apply(domainEvent);
        }
            
        // Persist entity
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult<Customer>(CommandOutcome.Accepted);
    }
}