using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class CreateCustomerHandler : ICommandHandler<Customer, CreateCustomer>
{
    private readonly ICustomerRepository _repository;

    public CreateCustomerHandler(
        ICustomerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CommandResult<Customer>> Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);

        // Apply events
        command.Entity.Apply(domainEvent);

        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
}