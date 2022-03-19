using AutoMapper;
using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using Integration = Common.Integration;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class UpdateCustomerHandler : ICommandHandler<Customer, UpdateCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCustomerHandler> _logger;

    public UpdateCustomerHandler(
        ICustomerRepository repository,
        IEventBus eventBus,
        IMapper mapper,
        ILogger<UpdateCustomerHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<CommandResult<Customer>> Handle(UpdateCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);

        // Apply events
        command.Entity.Apply(domainEvent);

        // Compare shipping addresses
        var existing = await _repository.GetAsync(command.EntityId);
        if (existing == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        var addressChanged = command.Entity.ShippingAddress != existing.ShippingAddress;

        try
        {
            // Persist entity
            var entity = await _repository.UpdateAsync(command.Entity);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
                
            // Publish events
            if (addressChanged)
            {
                var shippingAddress = _mapper.Map<Integration.Models.Address>(entity.ShippingAddress);
                _logger.LogInformation("----- Publishing event: {EventName}", $"v1.{nameof(CustomerAddressUpdated)}");
                await _eventBus.PublishAsync(
                    new CustomerAddressUpdated(entity.Id, shippingAddress),
                    null, "v1");
            }
            return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
        }
        catch (ConcurrencyException)
        {
            return new CommandResult<Customer>(CommandOutcome.Conflict);
        }
    }
}