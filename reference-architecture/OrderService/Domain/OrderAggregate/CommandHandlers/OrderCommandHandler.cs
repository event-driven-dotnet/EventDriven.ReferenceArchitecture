using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.CommandHandlers
{
    public class OrderCommandHandler :
        ICommandHandler<Order, CreateOrder>,
        ICommandHandler<Order, UpdateOrder>,
        ICommandHandler<Order, RemoveOrder>,
        ICommandHandler<Order, ShipOrder>,
        ICommandHandler<Order, CancelOrder>
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger<OrderCommandHandler> _logger;

        public OrderCommandHandler(
            IOrderRepository repository,
            ILogger<OrderCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CommandResult<Order>> Handle(CreateOrder command)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
            var domainEvent = command.Entity.Process(command);
            
            // Apply events
            command.Entity.Apply(domainEvent);
            
            // Persist entity
            var entity = await _repository.AddAsync(command.Entity);
            if (entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);
            return new CommandResult<Order>(CommandOutcome.Accepted, entity);
        }

        public async Task<CommandResult<Order>> Handle(UpdateOrder command)
        {
            _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateOrder));

            try
            {
                // Persist entity
                var entity = await _repository.UpdateAsync(command.Entity);
                if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
                return new CommandResult<Order>(CommandOutcome.Accepted, entity);
            }
            catch (ConcurrencyException)
            {
                return new CommandResult<Order>(CommandOutcome.Conflict);
            }
        }

        public async Task<CommandResult<Order>> Handle(RemoveOrder command)
        {
            // Persist entity
            _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
            await _repository.RemoveAsync(command.EntityId);
            return new CommandResult<Order>(CommandOutcome.Accepted);
        }

        public async Task<CommandResult<Order>> Handle(ShipOrder command)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(ShipOrder));
            var entity = await _repository.GetAsync(command.EntityId);
            if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
            var domainEvent = entity.Process(command);
            
            // Apply events
            entity.Apply(domainEvent);
            
            try
            {
                // Persist entity
                var order = await _repository.UpdateOrderStateAsync(entity, OrderState.Shipped);
                if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
                return new CommandResult<Order>(CommandOutcome.Accepted, order);
            }
            catch (ConcurrencyException)
            {
                return new CommandResult<Order>(CommandOutcome.Conflict);
            }
        }

        public async Task<CommandResult<Order>> Handle(CancelOrder command)
        {
            // Process command
            _logger.LogInformation("Handling command: {CommandName}", nameof(CancelOrder));
            var entity = await _repository.GetAsync(command.EntityId);
            if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
            var domainEvent = entity.Process(command);
            
            // Apply events
            entity.Apply(domainEvent);
            
            try
            {
                // Persist entity
                var order = await _repository.UpdateOrderStateAsync(entity, OrderState.Cancelled);
                if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
                return new CommandResult<Order>(CommandOutcome.Accepted, order);
            }
            catch (ConcurrencyException)
            {
                return new CommandResult<Order>(CommandOutcome.Conflict);
            }
        }
    }
}