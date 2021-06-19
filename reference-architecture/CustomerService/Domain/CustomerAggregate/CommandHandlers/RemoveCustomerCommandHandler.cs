using System.Threading;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers
{

    public class RemoveCustomerCommandHandler : ICommandHandler<RemoveCustomer, CommandResult>
    {

        private readonly ILogger<RemoveCustomerCommandHandler> _logger;
        private readonly ICustomerRepository _repository;

        public RemoveCustomerCommandHandler(ILogger<RemoveCustomerCommandHandler> logger,
                                            ICustomerRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<CommandResult> Handle(RemoveCustomer request, CancellationToken cancellationToken)
        {
            // Persist entity
            _logger.LogInformation($"Handling command: {nameof(RemoveCustomer)}");
            await _repository.Remove(request.CustomerId);
            return new CommandResult(CommandOutcome.Accepted);
        }

    }

}