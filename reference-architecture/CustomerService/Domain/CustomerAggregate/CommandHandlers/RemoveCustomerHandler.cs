using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class RemoveCustomerHandler : ICommandHandler<RemoveCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<RemoveCustomerHandler> _logger;

    public RemoveCustomerHandler(
        ICustomerRepository repository,
        ILogger<RemoveCustomerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<CommandResult> Handle(RemoveCustomer command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveCustomer));
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