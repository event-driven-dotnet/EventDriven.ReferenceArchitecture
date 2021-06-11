namespace CustomerService.Domain.CustomerAggregate.CommandHandlers {

    
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Microsoft.Extensions.Logging;
    using Repositories;

    public class RemoveCustomerCommandHandler : ICommandHandler<RemoveCustomer, CommandResult> {

        private readonly ILogger<RemoveCustomerCommandHandler> logger;
        private readonly ICustomerRepository repository;

        public RemoveCustomerCommandHandler(ILogger<RemoveCustomerCommandHandler> logger, 
                                            ICustomerRepository repository) {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task<CommandResult> Handle(RemoveCustomer request, CancellationToken cancellationToken) {
            // Persist entity
            logger.LogInformation($"Handling command: {nameof(RemoveCustomer)}");
            await repository.Remove(request.CustomerId);
            return new CommandResult(CommandOutcome.Accepted);
        }

    }

}