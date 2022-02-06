using System.Threading.Tasks;
using AutoMapper;
using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Integration = Common.Integration;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers
{
    public class CustomerCommandHandler :
        ICommandHandler<Customer, CreateCustomer>,
        ICommandHandler<Customer, UpdateCustomer>,
        ICommandHandler<Customer, RemoveCustomer>
    {
        private readonly ICustomerRepository _repository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerCommandHandler> _logger;

        public CustomerCommandHandler(
            ICustomerRepository repository,
            IEventBus eventBus,
            IMapper mapper,
            ILogger<CustomerCommandHandler> logger)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CommandResult<Customer>> Handle(CreateCustomer command)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(CreateCustomer));
            var domainEvent = command.Entity.Process(command);
            
            // Apply events
            command.Entity.Apply(domainEvent);
            
            // Persist entity
            var entity = await _repository.Add(command.Entity);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
        }

        public async Task<CommandResult<Customer>> Handle(UpdateCustomer command)
        {
            // Compare shipping addresses
            _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateCustomer));
            var existing = await _repository.Get(command.EntityId);
            if (existing == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
            var addressChanged = command.Entity.ShippingAddress != existing.ShippingAddress;
            
            try
            {
                // Persist entity
                var entity = await _repository.Update(command.Entity);
                if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
                
                // Publish events
                if (addressChanged)
                {
                    var shippingAddress = _mapper.Map<Integration.Models.Address>(entity.ShippingAddress);
                    _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(CustomerAddressUpdated)}");
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

        public async Task<CommandResult<Customer>> Handle(RemoveCustomer command)
        {
            // Persist entity
            _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveCustomer));
            await _repository.Remove(command.EntityId);
            return new CommandResult<Customer>(CommandOutcome.Accepted);
        }
    }
}