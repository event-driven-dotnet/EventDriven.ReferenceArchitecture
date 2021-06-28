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
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrder, CommandResult<Order>>
    {
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        private readonly IOrderRepository _repository;

        public CancelOrderCommandHandler(IOrderRepository repository,
                                         ILogger<CancelOrderCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(CancelOrder request, CancellationToken cancellationToken)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(CancelOrder));
            var entity = await _repository.GetOrder(request.EntityId);

            if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);

            var events = entity.Process(request);

            // Apply events
            var domainEvent = events.OfType<OrderCancelled>()
                                    .SingleOrDefault();

            if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);

            entity.Apply(domainEvent);

            try
            {
                // Persist entity
                var order = await _repository.UpdateOrderState(entity, OrderState.Cancelled);

                if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);

                return new CommandResult<Order>(CommandOutcome.Accepted, order);
            } catch (ConcurrencyException) { return new CommandResult<Order>(CommandOutcome.Conflict); }
        }
    }
}