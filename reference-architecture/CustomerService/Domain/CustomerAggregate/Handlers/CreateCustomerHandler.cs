using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.DDD.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Handlers;

public class CreateCustomerHandler : ICommandHandler<Customer, CreateCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CreateCustomerHandler> _logger;

    public CreateCustomerHandler(
        ICustomerRepository repository,
        ILogger<CreateCustomerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<CommandResult<Customer>> Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateCustomer));
        var domainEvent = command.Entity.Process(command);

        // Apply events
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
}