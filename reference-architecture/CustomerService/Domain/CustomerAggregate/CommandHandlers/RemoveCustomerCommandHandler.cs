namespace CustomerService.Domain.CustomerAggregate.CommandHandlers {

    
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class RemoveCustomerCommandHandler : ICommandHandler<RemoveCustomer, CommandResult<Customer>> {

        private readonly ILogger<RemoveCustomerCommandHandler> logger;
        private readonly ICustomerRepository repository;

        public RemoveCustomerCommandHandler(ILogger<RemoveCustomerCommandHandler> logger, 
                                            ICustomerRepository repository) {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task<CommandResult<Customer>> Handle(RemoveCustomer request, CancellationToken cancellationToken) {
            // Persist entity
            logger.LogInformation("Handling command: {CommandName}", nameof(RemoveCustomer));
            await repository.Remove(request.CustomerId);
            return new CommandResult<Customer>(CommandOutcome.Accepted);
        }

    }

}