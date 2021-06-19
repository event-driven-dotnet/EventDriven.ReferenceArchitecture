using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers
{

    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomer, CommandResult<Customer>>
    {

        private readonly ILogger<CreateCustomerCommandHandler> _logger;
        private readonly ICustomerRepository _repository;

        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger, ICustomerRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<CommandResult<Customer>> Handle(CreateCustomer request, CancellationToken cancellationToken)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(CreateCustomer));
            var events = request.Customer.Process(request);

            // Apply events
            var domainEvent = events.OfType<CustomerCreated>()
                                    .SingleOrDefault();
            if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);

            request.Customer.Apply(domainEvent);

            // Persist entity
            var entity = await _repository.Add(request.Customer);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);

            return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
        }

    }

}