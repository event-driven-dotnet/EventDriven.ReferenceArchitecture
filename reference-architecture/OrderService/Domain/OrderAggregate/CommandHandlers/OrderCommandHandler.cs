// using System.Linq;
// using System.Threading.Tasks;
// using EventDriven.CQRS.Abstractions.Commands;
// using Microsoft.Extensions.Logging;
// using OrderService.Domain.OrderAggregate.Commands;
// using OrderService.Domain.OrderAggregate.Events;
// using OrderService.Repositories;
//
// namespace OrderService.Domain.OrderAggregate.CommandHandlers
// {
//     public class OrderCommandHandler :
//         ICommandHandler<Order, CreateOrder>,
//         ICommandHandler<Order, UpdateOrder>,
//         ICommandHandler<Order, RemoveOrder>,
//         ICommandHandler<Order, ShipOrder>,
//         ICommandHandler<Order, CancelOrder>
//     {
//         private readonly IOrderRepository _repository;
//         private readonly ILogger<OrderCommandHandler> _logger;
//
//         public OrderCommandHandler(
//             IOrderRepository repository,
//             ILogger<OrderCommandHandler> logger)
//         {
//             _repository = repository;
//             _logger = logger;
//         }
//
//         public async Task<CommandResult<Order>> Handle(CreateOrder command)
//         {
//             // Process command
//             _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
//             var events = command.Order.Process(command);
//             
//             // Apply events
//             var domainEvent = events.OfType<OrderCreated>().SingleOrDefault();
//             if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);
//             command.Order.Apply(domainEvent);
//             
//             // Persist entity
//             var entity = await _repository.AddOrder(command.Order);
//             if (entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);
//             return new CommandResult<Order>(CommandOutcome.Accepted, entity);
//         }
//
//         public async Task<CommandResult<Order>> Handle(UpdateOrder command)
//         {
//             _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateOrder));
//
//             try
//             {
//                 // Persist entity
//                 var entity = await _repository.UpdateOrder(command.Order);
//                 if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
//                 return new CommandResult<Order>(CommandOutcome.Accepted, entity);
//             }
//             catch (ConcurrencyException)
//             {
//                 return new CommandResult<Order>(CommandOutcome.Conflict);
//             }
//         }
//
//         public async Task<CommandResult<Order>> Handle(RemoveOrder command)
//         {
//             // Persist entity
//             _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveOrder));
//             await _repository.RemoveOrder(command.EntityId);
//             return new CommandResult<Order>(CommandOutcome.Accepted);
//         }
//
//         public async Task<CommandResult<Order>> Handle(ShipOrder command)
//         {
//             // Process command
//             _logger.LogInformation("Handling command: {CommandName}", nameof(ShipOrder));
//             var entity = await _repository.GetOrder(command.EntityId);
//             if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
//             var events = entity.Process(command);
//             
//             // Apply events
//             var domainEvent = events.OfType<OrderShipped>().SingleOrDefault();
//             if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);
//             entity.Apply(domainEvent);
//             
//             try
//             {
//                 // Persist entity
//                 var order = await _repository.UpdateOrderState(entity, OrderState.Shipped);
//                 if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
//                 return new CommandResult<Order>(CommandOutcome.Accepted, order);
//             }
//             catch (ConcurrencyException)
//             {
//                 return new CommandResult<Order>(CommandOutcome.Conflict);
//             }
//         }
//
//         public async Task<CommandResult<Order>> Handle(CancelOrder command)
//         {
//             // Process command
//             _logger.LogInformation("Handling command: {CommandName}", nameof(CancelOrder));
//             var entity = await _repository.GetOrder(command.EntityId);
//             if (entity == null) return new CommandResult<Order>(CommandOutcome.NotFound);
//             var events = entity.Process(command);
//             
//             // Apply events
//             var domainEvent = events.OfType<OrderCancelled>().SingleOrDefault();
//             if (domainEvent == null) return new CommandResult<Order>(CommandOutcome.NotHandled);
//             entity.Apply(domainEvent);
//             
//             try
//             {
//                 // Persist entity
//                 var order = await _repository.UpdateOrderState(entity, OrderState.Cancelled);
//                 if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
//                 return new CommandResult<Order>(CommandOutcome.Accepted, order);
//             }
//             catch (ConcurrencyException)
//             {
//                 return new CommandResult<Order>(CommandOutcome.Conflict);
//             }
//         }
//     }
// }
