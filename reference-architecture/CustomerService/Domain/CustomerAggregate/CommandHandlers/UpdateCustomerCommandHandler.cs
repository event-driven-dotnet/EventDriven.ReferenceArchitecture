using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers
{

    public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomer, CommandResult<Customer>>
    {

        private readonly IEventBus _eventBus;

        private readonly ILogger<UpdateCustomerCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _repository;

        public UpdateCustomerCommandHandler(ILogger<UpdateCustomerCommandHandler> logger,
                                            ICustomerRepository repository,
                                            IMapper mapper,
                                            IEventBus eventBus)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<CommandResult<Customer>> Handle(UpdateCustomer request, CancellationToken cancellationToken)
        {
            // Compare shipping addresses
            _logger.LogInformation($"Handling command: {nameof(UpdateCustomer)}");
            var existing = await _repository.Get(request.CustomerId);
            var addressChanged = request.Customer.ShippingAddress != existing.ShippingAddress;

            try
            {
                // Persist entity
                var entity = await _repository.Update(request.Customer);

                if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);

                // Publish events
                if (addressChanged)
                {
                    var shippingAddress = _mapper.Map<Common.Integration.Models.Address>(entity.ShippingAddress);
                    _logger.LogInformation($"Publishing event: v1.{nameof(CustomerAddressUpdated)}");
                    await _eventBus.PublishAsync(new CustomerAddressUpdated(entity.Id, shippingAddress),
                        null,
                        "v1");
                }

                return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
            } catch (ConcurrencyException) { return new CommandResult<Customer>(CommandOutcome.Conflict); }
        }

    }

}