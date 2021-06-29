using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.CQRS.Abstractions.Events;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;

namespace OrderService.Domain.OrderAggregate
{
    public interface IOrderAggregate : ICommandProcessor<CreateOrder, CommandResult<Order>>,
                                       IEventApplier<OrderCreated>,
                                       ICommandProcessor<ShipOrder, CommandResult<Order>>,
                                       IEventApplier<OrderShipped>,
                                       ICommandProcessor<CancelOrder, CommandResult<Order>>,
                                       IEventApplier<OrderCancelled> { }
}