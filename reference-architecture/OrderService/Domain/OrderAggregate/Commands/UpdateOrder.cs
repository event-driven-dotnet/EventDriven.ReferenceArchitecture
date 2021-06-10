namespace OrderService.Domain.OrderAggregate.Commands {

    using EventDriven.CQRS.Abstractions.Commands;

    public record UpdateOrder(Order Order) : ICommand<CommandResult<Order>>;

}