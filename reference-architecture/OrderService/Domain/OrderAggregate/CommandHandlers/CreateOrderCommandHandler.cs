using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrder, CommandResult<Order>>
    {
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        private readonly IOrderRepository _repository;

        public CreateOrderCommandHandler(IOrderRepository repository,
                                         ILogger<CreateOrderCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(CreateOrder request, CancellationToken cancellationToken)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
            var events = request.Order.Process(request);

            // Apply events
            var domainEvent = events.OfType<OrderCreated>()
                                    .SingleOrDefault();

            if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);

            request.Order.Apply(domainEvent);

            // Persist entity
            var entity = await _repository.AddOrder(request.Order);

            if (entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);

            return new CommandResult<Order>(CommandOutcome.Accepted, entity);
        }
    }
}