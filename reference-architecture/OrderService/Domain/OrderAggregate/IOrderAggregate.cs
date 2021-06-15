namespace OrderService.Domain.OrderAggregate {

    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using EventDriven.CQRS.Abstractions.Events;
    using Events;

    public interface IOrderAggregate : ICommandProcessor<CreateOrder, CommandResult<Order>>,
                                       IEventApplier<OrderCreated>,
                                       ICommandProcessor<ShipOrder, CommandResult<Order>>,
                                       IEventApplier<OrderShipped>,
                                       ICommandProcessor<CancelOrder, CommandResult<Order>>,
                                       IEventApplier<OrderCancelled> { }

}