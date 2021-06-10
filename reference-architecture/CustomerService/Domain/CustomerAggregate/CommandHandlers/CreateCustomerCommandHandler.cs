namespace CustomerService.Domain.CustomerAggregate.CommandHandlers {

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Events;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomer, CommandResult<Customer>> {

        private readonly ILogger<CreateCustomerCommandHandler> logger;
        private readonly ICustomerRepository repository;

        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger, ICustomerRepository repository) {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task<CommandResult<Customer>> Handle(CreateCustomer request, CancellationToken cancellationToken) {
            // Process command
            logger.LogInformation("Handling command: {CommandName}", nameof(CreateCustomer));
            var events = request.Customer.Process(request);
            
            // Apply events
            var domainEvent = events.OfType<CustomerCreated>().SingleOrDefault();
            if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
            request.Customer.Apply(domainEvent);
            
            // Persist entity
            var entity = await repository.Add(request.Customer);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
        }

    }

}