namespace CustomerService.Domain.CustomerAggregate.CommandHandlers {

    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Commands;
    using Common.Integration.Events;
    using Common.Integration.Models;
    using EventDriven.CQRS.Abstractions.Commands;
    using EventDriven.EventBus.Abstractions;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomer, CommandResult<Customer>> {

        private readonly IEventBus eventBus;

        private readonly ILogger<UpdateCustomerCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly ICustomerRepository repository;

        public UpdateCustomerCommandHandler(ILogger<UpdateCustomerCommandHandler> logger,
                                            ICustomerRepository repository,
                                            IMapper mapper,
                                            IEventBus eventBus) {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.eventBus = eventBus;
        }

        public async Task<CommandResult<Customer>> Handle(UpdateCustomer request, CancellationToken cancellationToken) {
            // Compare shipping addresses
            logger.LogInformation($"Handling command: {nameof(UpdateCustomer)}");
            var existing = await repository.Get(request.CustomerId);
            var addressChanged = request.Customer.ShippingAddress != existing.ShippingAddress;

            try {
                // Persist entity
                var entity = await repository.Update(request.Customer);

                if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);

                // Publish events
                if (addressChanged) {
                    var shippingAddress = mapper.Map<Address>(entity.ShippingAddress);
                    logger.LogInformation($"Publishing event: v1.{nameof(CustomerAddressUpdated)}");
                    await eventBus.PublishAsync(new CustomerAddressUpdated(entity.Id, shippingAddress),
                                                null,
                                                "v1");
                }

                return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
            } catch (ConcurrencyException) {
                return new CommandResult<Customer>(CommandOutcome.Conflict);
            }
        }

    }

}